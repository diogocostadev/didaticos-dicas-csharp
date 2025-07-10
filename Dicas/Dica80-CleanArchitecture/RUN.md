# 🚀 Como Executar o Dica80 - Clean Architecture

## 📋 Pré-requisitos

- **.NET 9.0 SDK** ou superior
- **Visual Studio Code** ou Visual Studio
- **Git** para clonar o repositório

## 🛠️ Configuração e Execução

### 1. **Navegue até o projeto**
```bash
cd Dicas/Dica80-CleanArchitecture
```

### 2. **Compile a solução**
```bash
dotnet build
```

### 3. **Execute a aplicação**
```bash
# Opção 1: Executar direto da pasta do projeto
cd src/Dica80.CleanArchitecture.WebAPI
dotnet run

# Opção 2: Executar da raiz do projeto
dotnet run --project src/Dica80.CleanArchitecture.WebAPI
```

### 4. **Acesse a aplicação**

A aplicação estará disponível em:
- **Swagger UI**: `https://localhost:5001` (ou HTTP na porta mostrada no console)
- **Health Check**: `https://localhost:5001/health`

## 🎯 Funcionalidades Disponíveis

### **API Endpoints**

#### **Usuários**
- `GET /api/users` - Listar usuários com paginação
- `GET /api/users/{id}` - Buscar usuário por ID
- `POST /api/users` - Criar novo usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Deletar usuário

#### **Projetos** 
- `GET /api/projects` - Listar projetos
- `GET /api/projects/{id}` - Buscar projeto por ID
- `POST /api/projects` - Criar novo projeto

### **Dados de Teste**

A aplicação é iniciada com dados iniciais:

**Usuários:**
- admin@cleanarch.com (Administrador)
- manager@cleanarch.com (Gerente)
- member@cleanarch.com (Membro)

**Projeto:**
- "Sample Clean Architecture Project" com tarefas de exemplo

## 🏗️ Arquitetura

```
src/
├── Dica80.CleanArchitecture.Domain/      # ✅ Entidades, Value Objects, Domain Events
├── Dica80.CleanArchitecture.Application/ # ✅ CQRS, Handlers, DTOs, Validation
├── Dica80.CleanArchitecture.Infrastructure/ # ✅ EF Core, Repositories, Data Access
└── Dica80.CleanArchitecture.WebAPI/      # ✅ Controllers, Middleware, Configuration
```

## 🎨 Padrões Implementados

- ✅ **Clean Architecture** - Separação em camadas
- ✅ **CQRS** - Command Query Responsibility Segregation
- ✅ **Domain-Driven Design** - Rich domain models
- ✅ **Repository Pattern** - Abstração de dados
- ✅ **Unit of Work** - Controle de transações
- ✅ **Domain Events** - Comunicação assíncrona
- ✅ **Value Objects** - Encapsulamento de conceitos
- ✅ **Result Pattern** - Tratamento de erros
- ✅ **MediatR Pipeline** - Validation, Logging, Performance
- ✅ **Dependency Injection** - Inversão de controle

## 🧪 Testando a API

### **Exemplo: Criar Usuário**
```bash
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teste@cleanarch.com",
    "firstName": "João",
    "lastName": "Silva",
    "role": 1
  }'
```

### **Exemplo: Listar Usuários**
```bash
curl https://localhost:5001/api/users?pageNumber=1&pageSize=10
```

### **Exemplo: Criar Projeto**
```bash
curl -X POST https://localhost:5001/api/projects \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Novo Projeto",
    "description": "Descrição do projeto",
    "ownerId": "guid-do-usuario",
    "budget": {
      "amount": 100000,
      "currency": "BRL"
    }
  }'
```

## 📊 Logs e Monitoramento

- **Logs**: Arquivos gerados em `logs/cleanarchitecture-YYYY-MM-DD.txt`
- **Request ID**: Cada requisição recebe um ID único nos headers
- **Global Exception Handling**: Tratamento centralizado de erros
- **Validation Pipeline**: Validação automática com FluentValidation

## 🔧 Configurações

### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "" // Deixe vazio para usar In-Memory Database
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### **Banco de Dados**
- **Desenvolvimento**: In-Memory Database (padrão)
- **Produção**: SQL Server (configure a connection string)

## 🚀 Próximas Implementações

- 🔄 **JWT Authentication** - Sistema de autenticação
- 🔄 **Unit Tests** - Testes automatizados
- 🔄 **Docker** - Containerização
- 🔄 **Swagger Documentation** - Documentação completa da API

---

## 💡 Dicas

1. **Explore o Swagger UI** para uma interface visual da API
2. **Verifique os logs** para entender o fluxo de execução
3. **Use o Health Check** para monitorar a aplicação
4. **Analise a estrutura** para entender Clean Architecture na prática

🏆 **Este projeto demonstra uma implementação completa de Clean Architecture com .NET 9.0!**
