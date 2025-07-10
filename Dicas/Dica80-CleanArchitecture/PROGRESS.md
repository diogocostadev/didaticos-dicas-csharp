# 🏗️ **Dica 80: Clean Architecture com ASP.NET Core**

## 🎯 **Objetivo**

Demonstrar como implementar **Clean Architecture** em .NET 9.0, seguindo os princípios de **Robert C. Martin** com:
- **Separação de responsabilidades** em camadas bem definidas
- **Inversão de dependências** entre camadas
- **CQRS** (Command Query Responsibility Segregation) 
- **Domain-Driven Design** (DDD)
- **Domain Events** para comunicação assíncrona
- **Repository Pattern** com Unit of Work

---

## 📁 **Estrutura do Projeto**

```
Dica80-CleanArchitecture/
├── src/
│   ├── Dica80.CleanArchitecture.Domain/     ✅ COMPLETO
│   │   ├── Common/BaseEntity.cs             # Base para entidades
│   │   ├── Entities/                        # Entidades de domínio  
│   │   ├── ValueObjects/                    # Value Objects
│   │   ├── Events/                          # Domain Events
│   │   ├── Repositories/                    # Interfaces dos repositórios
│   │   └── Exceptions/                      # Exceções específicas
│   │
│   ├── Dica80.CleanArchitecture.Application/ ✅ COMPLETO
│   │   ├── Common/                          # Classes base CQRS
│   │   ├── DTOs/                            # Data Transfer Objects
│   │   ├── Users/Commands/                  # Commands de usuário
│   │   ├── Users/Queries/                   # Queries de usuário
│   │   ├── Projects/Commands/               # Commands de projeto
│   │   ├── Mappings/                        # AutoMapper profiles
│   │   ├── EventHandlers/                   # Domain event handlers
│   │   └── DependencyInjection.cs          # Configuração DI
│   │
│   ├── Dica80.CleanArchitecture.Infrastructure/ 🔄 PRÓXIMO
│   │   ├── Data/                            # Entity Framework
│   │   ├── Repositories/                    # Implementações
│   │   ├── Services/                        # Serviços externos
│   │   └── DependencyInjection.cs          
│   │
│   └── Dica80.CleanArchitecture.WebAPI/    🔄 PRÓXIMO
│       ├── Controllers/                     # API Controllers
│       ├── Middleware/                      # Custom middleware
│       ├── Extensions/                      # Extensions methods
│       └── Program.cs                       # Configuração da aplicação
│
└── README.md                                # Documentação principal
```

---

## 🎨 **Princípios da Clean Architecture**

### **1. Dependency Inversion** 🔄
```
WebAPI → Application → Domain
   ↓         ↓
Infrastructure ←←←←←←←←
```

### **2. Separation of Concerns** 📋
- **Domain**: Regras de negócio e entidades
- **Application**: Casos de uso e orquestração  
- **Infrastructure**: Persistência e serviços externos
- **WebAPI**: Controllers e configuração

### **3. Testability** 🧪
- **Domain**: Sem dependências externas
- **Application**: Testável com mocks
- **Infrastructure**: Testável com containers
- **WebAPI**: Testável com TestServer

---

## ⚡ **Tecnologias Utilizadas**

| Camada | Tecnologias |
|--------|-------------|
| **Domain** | `.NET 9.0` (sem dependências) |
| **Application** | `MediatR`, `FluentValidation`, `AutoMapper` |
| **Infrastructure** | `Entity Framework Core`, `SQL Server` |
| **WebAPI** | `ASP.NET Core`, `Swagger`, `JWT` |

---

## 🔥 **Principais Características**

### **Domain Layer (✅ Implementado)**
```csharp
// Entidades com comportamento rico
public class User : BaseEntity
{
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateLastLogin() => LastLoginAt = DateTime.UtcNow;
}

// Value Objects validados
public class Email : ValueObject
{
    public static Email Create(string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");
        return new Email(email.ToLowerInvariant());
    }
}

// Domain Events
public class UserCreatedEvent : BaseDomainEvent
{
    public User User { get; }
    public UserCreatedEvent(User user) => User = user;
}
```

### **Application Layer (✅ Implementado)**
```csharp
// CQRS Commands
public record CreateUserCommand : BaseCommand<Result<UserDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}

// CQRS Queries  
public record GetUserByIdQuery : BaseQuery<Result<UserDto>>
{
    public Guid Id { get; init; }
}

// Validation com FluentValidation
public class CreateUserCommandValidator : BaseValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        ValidateRequiredEmail(x => x.Email);
        ValidateRequiredString(nameof(CreateUserCommand.Name), x => x.Name);
    }
}

// Handlers com Result Pattern
public class CreateUserCommandHandler : BaseCommandHandler<CreateUserCommand, Result<UserDto>>
{
    public override async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Lógica de criação...
        return Result<UserDto>.Success(userDto);
    }
}
```

### **MediatR Pipeline (✅ Implementado)**
```csharp
Request → ValidationBehavior → LoggingBehavior → PerformanceBehavior → Handler
```

---

## 🎯 **Casos de Uso Demonstrados**

### **Gestão de Usuários**
- ✅ Criar usuário com validação de email
- ✅ Buscar usuário por ID ou email
- ✅ Listar usuários com paginação e filtros
- ✅ Ativar/desativar usuários
- ✅ Estatísticas de usuário

### **Gestão de Projetos**  
- ✅ Criar projeto com orçamento
- ✅ Definir datas e responsáveis
- ✅ Completar projeto com validações
- 🔄 Buscar projetos por critérios
- 🔄 Acompanhar progresso

### **Gestão de Tarefas**
- 🔄 Criar tarefas com prioridades
- 🔄 Atribuir tarefas a usuários  
- 🔄 Marcar tarefas como concluídas
- 🔄 Comentar em tarefas
- 🔄 Relatórios de produtividade

---

## 📊 **Status da Implementação**

| Componente | Status | Descrição |
|------------|--------|-----------|
| **Domain Entities** | ✅ | User, Project, TaskItem, Comment |
| **Value Objects** | ✅ | Email, Money, Priority |
| **Domain Events** | ✅ | Created, Updated, Completed |
| **Repository Interfaces** | ✅ | IUserRepository, IProjectRepository, etc. |
| **CQRS Commands** | ✅ | Create, Update, Delete operations |
| **CQRS Queries** | ✅ | Get by ID, Get with pagination |
| **Validation** | ✅ | FluentValidation + Pipeline |
| **AutoMapper** | ✅ | Entity ↔ DTO mappings |
| **Event Handlers** | ✅ | Domain event processing |
| **EF Core Setup** | ✅ | Infrastructure layer complete |
| **API Controllers** | ✅ | WebAPI layer complete |
| **Authentication** | 🔄 | JWT implementation |
| **Unit Tests** | 🔄 | Testing examples |

---

## 🚀 **Próximos Passos**

1. **JWT Authentication** - Implementar autenticação e autorização
2. **Testing** - Unit, Integration e E2E tests
3. **Docker** - Containerização completa
4. **CI/CD** - Pipeline de deploy
5. **Performance** - Otimizações e monitoramento

---

## 💡 **Conceitos Demonstrados**

- ✅ **Clean Architecture** - Separação em camadas
- ✅ **CQRS** - Separação Command/Query  
- ✅ **Domain-Driven Design** - Rich domain models
- ✅ **Repository Pattern** - Abstração de dados
- ✅ **Unit of Work** - Transações consistentes
- ✅ **Domain Events** - Comunicação assíncrona
- ✅ **Value Objects** - Encapsulamento de conceitos
- ✅ **Result Pattern** - Tratamento de erros
- ✅ **Validation Pipeline** - Validação centralizada
- ✅ **Dependency Injection** - Inversão de controle

---

## 🎯 **Benefícios Alcançados**

✅ **Manutenibilidade** - Código organizado e legível  
✅ **Testabilidade** - Camadas isoladas e testáveis  
✅ **Escalabilidade** - Arquitetura flexível  
✅ **Flexibilidade** - Fácil mudança de tecnologias  
✅ **Qualidade** - Validações e tratamento de erros  
✅ **Performance** - Monitoramento e otimização  

---

🏆 **Esta implementação demonstra como construir aplicações enterprise robustas e escaláveis seguindo as melhores práticas da indústria!**
