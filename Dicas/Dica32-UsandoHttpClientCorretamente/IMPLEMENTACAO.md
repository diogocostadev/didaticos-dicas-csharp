# Implementação da Dica 32: Usando HttpClient Corretamente

## 🎯 Objetivo

Demonstrar as práticas corretas para uso do `HttpClient` em .NET, evitando problemas comuns como socket exhaustion e issues de DNS resolution.

## 🧪 Estratégia de Implementação

### 1. Exemplos das Práticas Incorretas
- Criação de `HttpClient` a cada requisição (socket exhaustion)
- Uso de `HttpClient` estático sem configuração adequada
- Demonstração dos problemas causados

### 2. Soluções Corretas
- **HttpClient de longa duração**: Com `PooledConnectionLifetime`
- **HttpClientFactory**: Abordagem recomendada para DI
- **Named/Typed Clients**: Para cenários específicos

### 3. Demonstrações Práticas
- Comparação de performance entre abordagens
- Monitoramento de uso de sockets
- Simulação de mudanças de DNS
- Exemplos com APIs reais

## 📊 Estrutura do Projeto

```
Dica32.UsandoHttpClientCorretamente/
├── Program.cs                    # Aplicação principal com exemplos
├── BadPractices/                # Exemplos de práticas incorretas
├── GoodPractices/               # Implementações corretas
├── Services/                    # Serviços para demonstração
└── Utils/                       # Utilitários para monitoramento
```

## 🔧 Tecnologias Utilizadas

- .NET 8
- HttpClient / HttpClientFactory
- Dependency Injection
- Performance Counters (opcional)
- JSON serialization para APIs

## ⚡ Cenários Demonstrados

1. **Problema**: Socket Exhaustion com criação repetida
2. **Problema**: DNS Staleness com cliente estático
3. **Solução**: PooledConnectionLifetime
4. **Solução**: HttpClientFactory com DI
5. **Solução**: Named Clients
6. **Solução**: Typed Clients

## 📈 Métricas Avaliadas

- Número de sockets TCP em uso
- Tempo de resposta das requisições
- Uso de memória
- Resolução DNS (simulada)
- Throughput de requisições

## 🎓 Aprendizados

- Como `HttpClient` gerencia conexões TCP
- Diferença entre `HttpClient` e `HttpMessageHandler`
- Lifecycle management no HttpClientFactory
- Configuração de timeouts e retry policies
- Boas práticas para aplicações de alta performance
