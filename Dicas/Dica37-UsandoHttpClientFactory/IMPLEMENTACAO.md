# Implementação da Dica 37: Usando HttpClientFactory

## 📋 Visão Geral

Esta implementação demonstra o uso correto do `HttpClientFactory` para gerenciar requisições HTTP de forma eficiente, evitando problemas como socket exhaustion e fornecendo recursos avançados de resilência.

## 🏗️ Estrutura do Projeto

```
Dica37-UsandoHttpClientFactory/
├── Dica37.HttpClientFactory/           # Console app principal
│   ├── Program.cs                      # Demonstração completa
│   └── Dica37.HttpClientFactory.csproj # Configuração do projeto
├── README.md                           # Documentação principal
└── IMPLEMENTACAO.md                    # Este arquivo
```

## 🔧 Dependências Utilizadas

- **Microsoft.Extensions.Hosting** (8.0.0): Host builder e DI container
- **Microsoft.Extensions.Http** (8.0.0): HttpClientFactory
- **Microsoft.Extensions.Logging.Console** (8.0.0): Logging estruturado
- **Polly.Extensions.Http** (3.0.0): Políticas de resilência
- **Microsoft.Extensions.Http.Polly** (8.0.8): Integração Polly + HttpClient

## 📝 Funcionalidades Implementadas

### 1. HttpClient Básico
- Cliente HTTP simples para requisições pontuais
- Configuração mínima através do factory

### 2. HttpClient Nomeado
- Cliente configurado com nome específico
- BaseAddress, headers e timeout personalizados
- Reutilização de configuração

### 3. HttpClient Tipado
- Serviço dedicado (`UsuarioService`) com HttpClient injetado
- Encapsulamento de lógica de API específica
- Logging estruturado integrado

### 4. Políticas de Resilência (Polly)
- **Retry Policy**: 3 tentativas com backoff exponencial
- **Circuit Breaker**: Proteção contra falhas em cascata
- **Timeout Policy**: Limite de tempo para requisições

## 🎯 Classes e Serviços

### UsuarioService
```csharp
public class UsuarioService
{
    // HttpClient tipado para API de usuários
    // Métodos: ObterUsuarioAsync, ObterPostsDoUsuarioAsync
    // Logging e tratamento de erros integrados
}
```

### ApiService
```csharp
public class ApiService
{
    // Demonstra diferentes usos do HttpClientFactory
    // Métodos: FazerRequisicaoBasicaAsync, FazerRequisicaoComClienteNomeadoAsync, TestarResilienciaAsync
}
```

### DemoService
```csharp
public class DemoService
{
    // Orquestra todas as demonstrações
    // Executa exemplos práticos e explica conceitos
}
```

## 🔄 Políticas de Resilência Configuradas

### Retry Policy
- 3 tentativas automáticas
- Backoff exponencial (2^tentativa segundos)
- Aplicada a erros HTTP transientes

### Circuit Breaker
- Abre após 3 falhas consecutivas
- Permanece aberto por 30 segundos
- Logging de estado (aberto/fechado)

### Timeout Policy
- Timeout otimista de 5 segundos
- Cancela requisições que demoram muito

## 📊 Demonstrações Práticas

1. **GitHub API**: Requisição básica para demonstrar cliente simples
2. **JsonPlaceholder**: Cliente nomeado com configuração específica
3. **UsuarioService**: Cliente tipado com operações CRUD
4. **Teste de Resilência**: Demonstração de retry e circuit breaker

## 🚀 Como Executar

```bash
cd Dicas/Dica37-UsandoHttpClientFactory/Dica37.HttpClientFactory
dotnet run
```

## 💡 Conceitos Demonstrados

### Vantagens do HttpClientFactory
- Evita socket exhaustion
- Pool automático de conexões
- Respeita mudanças de DNS
- Integração com DI container
- Configuração centralizada

### Problemas do HttpClient Tradicional
- Vazamento de sockets
- Não renovação de DNS
- Dificuldade de configuração
- Problemas de testabilidade

### Boas Práticas
- Sempre usar HttpClientFactory em aplicações
- Configurar timeouts apropriados
- Implementar políticas de retry para APIs externas
- Usar clients tipados para APIs específicas
- Aplicar logging estruturado

## 🔍 Logs de Exemplo

O projeto gera logs estruturados mostrando:
- Requisições sendo feitas
- Tentativas de retry
- Estados do circuit breaker
- Sucessos e falhas

## 🎯 Cenários de Uso Real

1. **Microserviços**: Comunicação entre serviços
2. **Integração com APIs**: Consumo de APIs externas
3. **Aplicações Web**: Backend fazendo chamadas HTTP
4. **Workers/Background Services**: Processamento assíncrono

Esta implementação serve como referência completa para uso profissional do HttpClientFactory em aplicações .NET modernas.
