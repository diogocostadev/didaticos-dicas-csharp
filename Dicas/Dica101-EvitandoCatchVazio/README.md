# Dica 101: Evitando Catch Vazio (Exceções Engolidas)

## 🎯 O Problema

Um dos piores anti-patterns em C# é o **"catch vazio"** ou **"Pokemon Exception Handling"** (gotta catch 'em all!). Isso acontece quando capturamos uma exceção e não fazemos **NADA** com ela.

```csharp
// ❌ NUNCA FAÇA ISSO!
try
{
    await ProcessarPagamentoAsync(pedido);
}
catch (Exception)
{
    // 💀 Exceção engolida completamente!
}
```

## 💀 Por Que É Perigoso?

1. **Erros Silenciosos**: Bugs que parecem "fantasmas" - algo não funciona mas não há erro
2. **Debugging Impossível**: Horas/dias procurando onde o problema está
3. **Dados Corrompidos**: Operações que parecem ter sucesso mas falharam
4. **Suporte Cego**: Equipe de suporte não consegue diagnosticar problemas
5. **Cliente Frustrado**: "Fiz o pedido mas não chegou" - e você não sabe por quê

## 🔴 Padrões Ruins Comuns

### 1. Catch Totalmente Vazio
```csharp
catch (Exception) { }
```

### 2. Catch com Return Silencioso
```csharp
catch (Exception) { return null; }
catch (Exception) { return false; }
```

### 3. Console.WriteLine em Produção
```csharp
catch (Exception ex)
{
    Console.WriteLine(ex.Message); // Ninguém vê isso em produção!
}
```

### 4. Catch Genérico sem Diferenciação
```csharp
catch (Exception ex)
{
    // Trata timeout igual a erro de validação!
    Console.WriteLine("Erro");
}
```

## ✅ Soluções Corretas

### 1. Log + Rethrow
Quando o **chamador deve tratar** a exceção:

```csharp
try
{
    await ProcessarPagamentoAsync(pedido);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar pagamento do pedido {PedidoId}", pedido.Id);
    throw; // Relança preservando stack trace
}
```

### 2. Tratamento Específico por Tipo
```csharp
try
{
    await EnviarNotificacaoAsync(usuario, mensagem);
}
catch (SmtpException ex)
{
    _logger.LogWarning(ex, "Falha SMTP para {Email}", usuario.Email);
    await EnviarViaSmsAsync(usuario, mensagem); // Fallback
}
catch (TimeoutException ex)
{
    _logger.LogError(ex, "Timeout ao enviar notificação");
    await AgendarRetentativaAsync(usuario, mensagem); // Retry
}
```

### 3. Result Pattern
Para operações que podem falhar de forma esperada:

```csharp
public async Task<Result<Pedido>> ProcessarPedidoAsync(string pedidoId)
{
    try
    {
        var pedido = await _repository.BuscarAsync(pedidoId);
        return Result<Pedido>.Success(pedido);
    }
    catch (PedidoNaoEncontradoException ex)
    {
        _logger.LogWarning(ex, "Pedido {PedidoId} não encontrado", pedidoId);
        return Result<Pedido>.Failure("Pedido não encontrado");
    }
}
```

### 4. Operação Não Crítica (com log!)
```csharp
try
{
    await EnviarMetricasAsync(dados);
}
catch (Exception ex)
{
    // Log completo mesmo que continue
    _logger.LogWarning(ex, 
        "Falha ao enviar métricas (não crítico). Dados: {Dados}", dados);
    // Continua execução - métricas não são críticas
}
```

## ⚠️ Quando É Aceitável "Ignorar"

Existem **poucos** casos aceitáveis, mas **sempre com log**:

### Cleanup/Dispose
```csharp
finally
{
    try { connection?.Dispose(); }
    catch (Exception ex)
    {
        _logger.LogDebug(ex, "Erro no dispose - ignorado");
    }
}
```

### Cancelamento
```csharp
catch (OperationCanceledException)
{
    _logger.LogDebug("Operação cancelada pelo usuário");
}
```

## 📋 Regras de Ouro

| Regra | Errado | Certo |
|-------|--------|-------|
| Nunca catch vazio | `catch { }` | `catch { Log(); throw; }` |
| Sempre log com contexto | `Log("Erro")` | `Log(ex, "Erro em {Id}", id)` |
| Exceções específicas | `catch (Exception)` | `catch (HttpRequestException)` |
| Preservar stack trace | `throw ex;` | `throw;` |

## 🚀 Como Executar

```bash
cd Dicas/Dica101-EvitandoCatchVazio/Dica101.EvitandoCatchVazio
dotnet run
```

## 📚 Referências

- [Exception Handling Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
- [Logging Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggingdotnetcore-and-aspnetcore)
- [Result Pattern](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)

## 💡 Lembre-se

> **Um catch vazio hoje = horas de debugging amanhã!**

Se você captura uma exceção, **FAÇA ALGO** com ela:
- Log com contexto
- Rethrow se necessário
- Retorne um Result explicativo
- Implemente fallback/retry
