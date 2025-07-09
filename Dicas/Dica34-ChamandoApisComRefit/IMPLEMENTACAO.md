# Implementação da Dica 34: Chamando APIs com Refit

## 📝 Estrutura do Projeto

```
Dica34-ChamandoApisComRefit/
├── Dica34.ChamandoApisComRefit/           # Projeto principal
│   ├── Models/
│   │   └── ApiModels.cs                   # Modelos de dados da API
│   ├── Apis/
│   │   └── ApiInterfaces.cs               # Interfaces Refit
│   ├── Services/
│   │   └── ApiServices.cs                 # Serviços de aplicação
│   ├── Program.cs                         # Configuração e endpoints
│   └── *.csproj                          # Configuração do projeto
├── Dica34.ChamandoApisComRefit.Tests/     # Projeto de testes
│   ├── UnitTests.cs                      # Testes unitários com mocks
│   ├── IntegrationTests.cs               # Testes de integração com WireMock
│   └── *.csproj                          # Configuração dos testes
└── README.md                             # Documentação completa
```

## 🎯 Objetivos da Implementação

Esta implementação demonstra como usar **Refit** para criar clientes de API tipados e elegantes em C#, incluindo:

### 1. **Interfaces Refit Declarativas**
- `IUserApi` - CRUD completo de usuários
- `IProductApi` - Consulta de produtos (API externa)
- `ISecureApi` - Operações com autenticação
- `IResponseTypesApi` - Diferentes tipos de resposta

### 2. **Recursos Avançados do Refit**
- Query parameters dinâmicos
- Headers customizados (Authorization, API Keys)
- Multipart form data para uploads
- Response wrapping com `ApiResponse<T>`
- Tratamento de diferentes tipos de conteúdo

### 3. **Integração com HttpClientFactory**
- Configuração centralizada de clientes HTTP
- Políticas de retry com Polly
- Circuit breaker para resiliência
- Timeouts e configurações de rede

### 4. **Modelos de Dados Robustos**
- Records imutáveis para DTOs
- Suporte a dados aninhados complexos
- Paginação de resultados
- Tratamento de erros estruturado

## 🔧 Configuração e Uso

### 1. **Instalação de Pacotes**
```xml
<PackageReference Include="Refit" Version="7.0.0" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.0" />
<PackageReference Include="Polly.Extensions.Http" Version="3.0.0" />
```

### 2. **Configuração no Program.cs**
```csharp
// Configuração do Refit com políticas de resiliência
builder.Services.AddRefitClient<IUserApi>(refitSettings)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.example.com"))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());
```

### 3. **Exemplos de Uso Prático**

#### Operações CRUD Básicas:
```csharp
// Buscar usuário
var user = await userApi.GetUserAsync(1);

// Criar usuário
var newUser = await userApi.CreateUserAsync(new CreateUserRequest("João", "joao@test.com"));

// Atualizar usuário
var updated = await userApi.UpdateUserAsync(1, new UpdateUserRequest("João Silva", "joao@test.com", true));

// Deletar usuário
await userApi.DeleteUserAsync(1);
```

#### Busca com Filtros:
```csharp
var users = await userApi.GetUsersAsync(
    page: 1, 
    size: 10, 
    search: "João",
    filter: new UserFilter(IsActive: true, City: "São Paulo")
);
```

#### Upload de Arquivos:
```csharp
using var fileStream = File.OpenRead("documento.pdf");
var result = await secureApi.UploadFileAsync(
    new StreamPart(fileStream, "documento.pdf"),
    "Documento importante",
    "Bearer token-aqui"
);
```

## 🧪 Estratégias de Teste

### 1. **Testes Unitários com Mocks**
```csharp
[Fact]
public async Task GetUserAsync_WhenUserExists_ShouldReturnUser()
{
    // Arrange
    var mockUserApi = new Mock<IUserApi>();
    mockUserApi.Setup(x => x.GetUserAsync(1))
              .ReturnsAsync(expectedUser);
    
    // Act & Assert
    var result = await userService.GetUserAsync(1);
    Assert.Equal(expectedUser, result);
}
```

### 2. **Testes de Integração com WireMock**
```csharp
[Fact]
public async Task Integration_CreateUser_ShouldCallCorrectEndpoint()
{
    // Configurar mock server
    _mockServer
        .Given(Request.Create().WithPath("/users").UsingPost())
        .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(createdUser));
    
    // Testar chamada real
    var result = await _userApi.CreateUserAsync(request);
    Assert.Equal(createdUser.Id, result.Id);
}
```

## 📊 APIs Demonstradas

### 1. **JSONPlaceholder** (Usuarios)
- GET /users - Lista usuários
- GET /users/{id} - Busca usuário específico
- POST /users - Cria usuário
- PUT /users/{id} - Atualiza usuário
- DELETE /users/{id} - Remove usuário

### 2. **FakeStore API** (Produtos)
- GET /products - Lista produtos
- GET /products/categories - Lista categorias
- GET /products/category/{category} - Produtos por categoria

### 3. **HttpBin** (Testes)
- Endpoints para teste de headers
- Simulação de autenticação
- Upload de arquivos

## 🎮 Como Executar

### 1. **Executar a Aplicação**
```bash
cd Dica34.ChamandoApisComRefit
dotnet run
```

### 2. **Testar Endpoints**
```bash
# Buscar usuários
curl https://localhost:7000/demo/users

# Buscar usuário específico
curl https://localhost:7000/demo/user/1

# Buscar produtos
curl https://localhost:7000/demo/products

# Buscar por categoria
curl https://localhost:7000/demo/products/category/electronics
```

### 3. **Executar Testes**
```bash
cd Dica34.ChamandoApisComRefit.Tests
dotnet test --verbosity normal
```

## 🔍 Recursos Demonstrados

### ✅ **Implementados**
- [x] Interfaces Refit declarativas
- [x] Integração com HttpClientFactory
- [x] Políticas de retry e circuit breaker
- [x] Headers customizados e autenticação
- [x] Query parameters dinâmicos
- [x] Multipart form data
- [x] Response wrapping
- [x] Tratamento de erros tipado
- [x] Testes unitários com mocks
- [x] Testes de integração com WireMock
- [x] Logging estruturado
- [x] Configuração JSON personalizada
- [x] Cancelation tokens
- [x] APIs externas reais

### 🎯 **Benefícios Demonstrados**
- **Type Safety**: Contratos tipados previnem erros
- **Code Generation**: Refit gera implementação automaticamente
- **Testabilidade**: Fácil de mockar e testar
- **Resiliência**: Políticas de retry e circuit breaker
- **Flexibilidade**: Suporte a diversos cenários de API
- **Maintainability**: Código limpo e declarativo

## 📝 Lições Aprendidas

### 1. **Vantagens do Refit**
- Reduz drasticamente o código boilerplate
- Oferece type safety completo
- Integra perfeitamente com DI container
- Facilita muito os testes

### 2. **Configuração Importante**
- Sempre configurar timeouts
- Implementar políticas de retry
- Usar logging para debugging
- Configurar serialização JSON adequadamente

### 3. **Boas Práticas**
- Separar interfaces por domínio
- Usar CancellationTokens
- Tratar erros específicos (404, 401, etc.)
- Implementar circuit breaker para APIs externas

Esta implementação fornece uma base sólida para usar Refit em projetos reais, demonstrando desde conceitos básicos até configurações avançadas de produção.
