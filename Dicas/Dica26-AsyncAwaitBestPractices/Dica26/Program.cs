using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dica26;

/// <summary>
/// Dica 26: Async/Await Best Practices
/// 
/// Esta dica demonstra as melhores práticas ao trabalhar com async/await em C#,
/// incluindo ConfigureAwait, tratamento de exceções, evitar deadlocks e otimizações de performance.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddScoped<AsyncBestPracticesService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var service = host.Services.GetRequiredService<AsyncBestPracticesService>();

        logger.LogInformation("=== Dica 26: Async/Await Best Practices ===\n");

        // 1. ConfigureAwait Best Practices
        logger.LogInformation("1. ConfigureAwait Best Practices:");
        await service.DemonstrateConfigureAwaitAsync();

        // 2. Exception Handling in Async Methods
        logger.LogInformation("\n2. Exception Handling in Async Methods:");
        await service.DemonstrateExceptionHandlingAsync();

        // 3. Avoiding Async Void
        logger.LogInformation("\n3. Avoiding Async Void:");
        await service.DemonstrateAsyncVoidProblemsAsync();

        // 4. Task.WhenAll vs Multiple Awaits
        logger.LogInformation("\n4. Task.WhenAll vs Multiple Awaits:");
        await service.DemonstrateTaskWhenAllAsync();

        // 5. CancellationToken Best Practices
        logger.LogInformation("\n5. CancellationToken Best Practices:");
        await service.DemonstrateCancellationTokenAsync();

        // 6. ValueTask for Hot Paths
        logger.LogInformation("\n6. ValueTask for Hot Paths:");
        await service.DemonstrateValueTaskAsync();

        logger.LogInformation("\n=== Async/Await Best Practices Demo Completed ===");
    }
}

public class AsyncBestPracticesService
{
    private readonly ILogger<AsyncBestPracticesService> _logger;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _cache;

    public AsyncBestPracticesService(ILogger<AsyncBestPracticesService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _cache = new Dictionary<string, string>();
    }

    /// <summary>
    /// Demonstra o uso correto de ConfigureAwait
    /// </summary>
    public async Task DemonstrateConfigureAwaitAsync()
    {
        _logger.LogInformation("   📚 ConfigureAwait: Library vs Application code");

        // ❌ RUIM: Em código de biblioteca, não usar ConfigureAwait(false)
        // pode causar deadlocks em aplicações SynchronizationContext
        await BadLibraryMethodAsync();

        // ✅ BOM: Em código de biblioteca, sempre usar ConfigureAwait(false)
        await GoodLibraryMethodAsync();

        // 📝 NOTA: Em aplicações (não bibliotecas), ConfigureAwait(false) pode ser opcional
        // mas ainda é uma boa prática para evitar overhead desnecessário
        await ApplicationMethodAsync();
    }

    /// <summary>
    /// Método de biblioteca SEM ConfigureAwait - pode causar deadlock
    /// </summary>
    private async Task BadLibraryMethodAsync()
    {
        _logger.LogInformation("      ❌ Library method WITHOUT ConfigureAwait");
        
        // Simula operação I/O - PERIGOSO em biblioteca
        await Task.Delay(100); // Sem ConfigureAwait(false)
        
        _logger.LogInformation("      ❌ Completed - potential deadlock risk");
    }

    /// <summary>
    /// Método de biblioteca COM ConfigureAwait - seguro
    /// </summary>
    private async Task GoodLibraryMethodAsync()
    {
        _logger.LogInformation("      ✅ Library method WITH ConfigureAwait(false)");
        
        // Simula operação I/O - SEGURO em biblioteca
        await Task.Delay(100).ConfigureAwait(false);
        
        _logger.LogInformation("      ✅ Completed - no deadlock risk");
    }

    /// <summary>
    /// Método de aplicação - ConfigureAwait opcional mas recomendado
    /// </summary>
    private async Task ApplicationMethodAsync()
    {
        _logger.LogInformation("      📱 Application method - ConfigureAwait optional");
        
        // Em aplicações, ConfigureAwait(false) é opcional mas recomendado para performance
        await Task.Delay(50).ConfigureAwait(false);
        
        _logger.LogInformation("      📱 Application method completed");
    }

    /// <summary>
    /// Demonstra tratamento correto de exceções em métodos async
    /// </summary>
    public async Task DemonstrateExceptionHandlingAsync()
    {
        _logger.LogInformation("   🔥 Exception Handling Patterns");

        // 1. Exception handling em async methods
        await HandleExceptionsCorrectlyAsync();

        // 2. AggregateException com Task.WhenAll
        await HandleAggregateExceptionsAsync();

        // 3. Exception propagation
        await DemonstrateExceptionPropagationAsync();
    }

    private async Task HandleExceptionsCorrectlyAsync()
    {
        try
        {
            _logger.LogInformation("      🎯 Calling async method that throws...");
            await ThrowingAsyncMethodAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogInformation($"      ✅ Caught expected exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"      ❌ Unexpected exception: {ex.Message}");
        }
    }

    private async Task HandleAggregateExceptionsAsync()
    {
        try
        {
            _logger.LogInformation("      🎯 Using Task.WhenAll with multiple failing tasks...");
            
            var tasks = new[]
            {
                ThrowingAsyncMethodAsync(),
                ThrowingAsyncMethodAsync(),
                Task.Delay(100) // Uma task que não falha
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"      ⚠️  First exception caught: {ex.Message}");
            _logger.LogInformation("      📝 Note: Task.WhenAll only throws the first exception");
        }
    }

    private async Task DemonstrateExceptionPropagationAsync()
    {
        _logger.LogInformation("      🔄 Exception propagation through async chain");
        
        try
        {
            await AsyncMethod1();
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"      ✅ Exception propagated correctly: {ex.Message}");
        }
    }

    private async Task AsyncMethod1()
    {
        await AsyncMethod2();
    }

    private async Task AsyncMethod2()
    {
        await AsyncMethod3();
    }

    private async Task AsyncMethod3()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Exception from deep async call");
    }

    private async Task ThrowingAsyncMethodAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Simulated async exception");
    }

    /// <summary>
    /// Demonstra por que evitar async void
    /// </summary>
    public async Task DemonstrateAsyncVoidProblemsAsync()
    {
        _logger.LogInformation("   ⚠️  Async Void Problems");

        // ✅ BOM: Async Task para métodos que podem ser awaited
        await GoodAsyncMethodAsync();

        // ❌ RUIM: Async void - não pode ser awaited, exceções não podem ser capturadas
        _logger.LogInformation("      ❌ Calling async void method (fire and forget)");
        BadAsyncVoidMethod(); // Não podemos await isso!

        // Espera um pouco para ver se a async void completa
        await Task.Delay(200);

        _logger.LogInformation("      📝 Async void completed (maybe), but we couldn't wait for it");
    }

    private async Task GoodAsyncMethodAsync()
    {
        _logger.LogInformation("      ✅ Good async Task method");
        await Task.Delay(100);
        _logger.LogInformation("      ✅ Good async method completed");
    }

    private async void BadAsyncVoidMethod()
    {
        _logger.LogInformation("      ❌ Bad async void method started");
        await Task.Delay(150);
        _logger.LogInformation("      ❌ Bad async void method completed");
        
        // Se essa linha lançasse uma exceção, seria muito difícil de capturar!
        // throw new InvalidOperationException("Unhandled exception in async void!");
    }

    /// <summary>
    /// Demonstra Task.WhenAll vs multiple awaits sequenciais
    /// </summary>
    public async Task DemonstrateTaskWhenAllAsync()
    {
        _logger.LogInformation("   🚀 Task.WhenAll vs Sequential Awaits");

        // ❌ RUIM: Awaits sequenciais - muito lento
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await SequentialAwaitsAsync();
        stopwatch.Stop();
        _logger.LogInformation($"      ❌ Sequential awaits took: {stopwatch.ElapsedMilliseconds}ms");

        // ✅ BOM: Task.WhenAll - execução paralela
        stopwatch.Restart();
        await ParallelWithWhenAllAsync();
        stopwatch.Stop();
        _logger.LogInformation($"      ✅ Task.WhenAll took: {stopwatch.ElapsedMilliseconds}ms");
    }

    private async Task SequentialAwaitsAsync()
    {
        _logger.LogInformation("      🐌 Starting sequential operations...");
        
        await SimulateApiCallAsync("Service A", 100);
        await SimulateApiCallAsync("Service B", 100);
        await SimulateApiCallAsync("Service C", 100);
        
        _logger.LogInformation("      🐌 Sequential operations completed");
    }

    private async Task ParallelWithWhenAllAsync()
    {
        _logger.LogInformation("      🚀 Starting parallel operations...");
        
        var tasks = new[]
        {
            SimulateApiCallAsync("Service A", 100),
            SimulateApiCallAsync("Service B", 100),
            SimulateApiCallAsync("Service C", 100)
        };

        await Task.WhenAll(tasks);
        
        _logger.LogInformation("      🚀 Parallel operations completed");
    }

    private async Task SimulateApiCallAsync(string serviceName, int delayMs)
    {
        _logger.LogInformation($"         📡 Calling {serviceName}...");
        await Task.Delay(delayMs);
        _logger.LogInformation($"         ✅ {serviceName} responded");
    }

    /// <summary>
    /// Demonstra uso correto de CancellationToken
    /// </summary>
    public async Task DemonstrateCancellationTokenAsync()
    {
        _logger.LogInformation("   🛑 CancellationToken Best Practices");

        using var cts = new CancellationTokenSource();
        
        // Cancela após 500ms
        cts.CancelAfter(500);

        try
        {
            await LongRunningOperationAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("      ✅ Operation was cancelled successfully");
        }

        // Demonstra propagação de CancellationToken
        await DemonstrateCancellationPropagationAsync();
    }

    private async Task LongRunningOperationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("      🔄 Starting long operation...");
        
        for (int i = 0; i < 10; i++)
        {
            // ✅ BOM: Verificar cancellation token regularmente
            cancellationToken.ThrowIfCancellationRequested();
            
            _logger.LogInformation($"         Step {i + 1}/10");
            
            // ✅ BOM: Passar token para operações async
            await Task.Delay(100, cancellationToken);
        }
        
        _logger.LogInformation("      ✅ Long operation completed");
    }

    private async Task DemonstrateCancellationPropagationAsync()
    {
        _logger.LogInformation("      🔗 CancellationToken propagation");
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        try
        {
            await MethodThatPropagatesTokenAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("      ✅ Cancellation propagated through call chain");
        }
    }

    private async Task MethodThatPropagatesTokenAsync(CancellationToken cancellationToken)
    {
        // ✅ BOM: Sempre propagar CancellationToken
        await AnotherMethodAsync(cancellationToken);
    }

    private async Task AnotherMethodAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(300, cancellationToken); // Vai ser cancelado
    }

    /// <summary>
    /// Demonstra quando usar ValueTask vs Task
    /// </summary>
    public async Task DemonstrateValueTaskAsync()
    {
        _logger.LogInformation("   ⚡ ValueTask for Hot Paths");

        // Primeira chamada - cache miss
        _logger.LogInformation("      🔍 First call (cache miss):");
        var result1 = await GetCachedDataAsync("key1");
        _logger.LogInformation($"         Result: {result1}");

        // Segunda chamada - cache hit (ValueTask optimization)
        _logger.LogInformation("      🎯 Second call (cache hit):");
        var result2 = await GetCachedDataAsync("key1");
        _logger.LogInformation($"         Result: {result2}");

        // Demonstra diferença de alocação
        await CompareTaskVsValueTaskAllocationsAsync();
    }

    /// <summary>
    /// Método que retorna ValueTask para otimizar casos síncronos (cache hit)
    /// </summary>
    private ValueTask<string> GetCachedDataAsync(string key)
    {
        // ✅ Cache hit - retorna ValueTask síncrono (sem alocação de Task)
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            _logger.LogInformation("         💨 Cache HIT - ValueTask synchronous return");
            return ValueTask.FromResult(cachedValue);
        }

        // Cache miss - precisa fazer operação async
        _logger.LogInformation("         🐌 Cache MISS - async operation needed");
        return GetDataAsyncInternal(key);
    }

    private async ValueTask<string> GetDataAsyncInternal(string key)
    {
        // Simula operação I/O
        await Task.Delay(100);
        
        var data = $"Data for {key} - {DateTime.Now:HH:mm:ss}";
        _cache[key] = data;
        
        return data;
    }

    private async Task CompareTaskVsValueTaskAllocationsAsync()
    {
        _logger.LogInformation("      📊 Task vs ValueTask allocation comparison:");

        // Task sempre aloca, mesmo para retornos síncronos
        var taskResult = await GetDataWithTaskAsync("cached");
        _logger.LogInformation($"         Task result: {taskResult}");

        // ValueTask não aloca para retornos síncronos
        var valueTaskResult = await GetDataWithValueTaskAsync("cached");
        _logger.LogInformation($"         ValueTask result: {valueTaskResult}");

        _logger.LogInformation("      📝 ValueTask avoids allocation for synchronous returns");
    }

    private Task<string> GetDataWithTaskAsync(string key)
    {
        // ❌ Task sempre aloca, mesmo para retorno imediato
        if (_cache.TryGetValue(key, out var value))
        {
            return Task.FromResult(value); // Ainda aloca um Task
        }
        
        return Task.FromResult($"New data for {key}");
    }

    private ValueTask<string> GetDataWithValueTaskAsync(string key)
    {
        // ✅ ValueTask não aloca para retorno imediato
        if (_cache.TryGetValue(key, out var value))
        {
            return ValueTask.FromResult(value); // Sem alocação!
        }
        
        return ValueTask.FromResult($"New data for {key}");
    }
}
