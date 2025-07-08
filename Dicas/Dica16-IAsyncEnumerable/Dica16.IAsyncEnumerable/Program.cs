using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;

Console.WriteLine("=== Dica 16: IAsyncEnumerable - Async Streams ===\n");

// 1. IAsyncEnumerable básico
Console.WriteLine("1. IAsyncEnumerable Básico - Streaming de Dados:");
await foreach (var numero in GerarNumerosAsync())
{
    Console.WriteLine($"  Recebido: {numero}");
}

// 2. Cancelamento com CancellationToken
Console.WriteLine("\n2. IAsyncEnumerable com CancellationToken:");
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
try
{
    await foreach (var item in GerarDadosComCancelamentoAsync(cts.Token))
    {
        Console.WriteLine($"  Processando: {item}");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("  ⚠️ Operação cancelada por timeout");
}

// 3. Processamento de arquivos grandes
Console.WriteLine("\n3. Processamento de Arquivo Linha por Linha:");
var tempFile = Path.GetTempFileName();
await File.WriteAllLinesAsync(tempFile, [
    "Linha 1: Dados importantes",
    "Linha 2: Mais dados",
    "Linha 3: Dados finais"
]);

await foreach (var linha in LerArquivoLinhasPorLinhaAsync(tempFile))
{
    Console.WriteLine($"  Lida: {linha}");
}
File.Delete(tempFile);

// 4. Combinando múltiplos async streams
Console.WriteLine("\n4. Combinando Múltiplos Async Streams:");
await foreach (var resultado in CombinarStreamAsync())
{
    Console.WriteLine($"  Combinado: {resultado}");
}

// 5. API REST com streaming
Console.WriteLine("\n5. Simulação API REST com Streaming:");
var apiService = new ApiStreamingService();
await foreach (var usuario in apiService.GetUsuariosPaginadosAsync())
{
    Console.WriteLine($"  Usuário: {usuario.Nome} ({usuario.Email})");
}

// 6. Processamento em tempo real
Console.WriteLine("\n6. Processamento de Dados em Tempo Real:");
var realTimeProcessor = new RealTimeDataProcessor();
var processedCount = 0;

await foreach (var dadoProcessado in realTimeProcessor.ProcessarDadosAsync())
{
    Console.WriteLine($"  Processado: {dadoProcessado}");
    if (++processedCount >= 5) break; // Limitar para demo
}

// 7. Transformações de streams
Console.WriteLine("\n7. Transformações de Streams:");
await foreach (var transformado in TransformarStreamAsync())
{
    Console.WriteLine($"  Transformado: {transformado}");
}

// 8. Buffer e batching
Console.WriteLine("\n8. Buffer e Batching de Streams:");
await foreach (var batch in AgruparEmBatchesAsync(GerarSequenciaAsync(), batchSize: 3))
{
    Console.WriteLine($"  Batch: [{string.Join(", ", batch)}]");
}

// 9. Tratamento de erros
Console.WriteLine("\n9. Tratamento de Erros em Async Streams:");
await foreach (var item in StreamComErrosAsync())
{
    Console.WriteLine($"  Item seguro: {item}");
}

// 10. Performance comparison
Console.WriteLine("\n10. Comparação de Performance:");
await CompararPerformanceAsync();

Console.WriteLine("\n=== Resumo das Vantagens do IAsyncEnumerable ===");
Console.WriteLine("✅ Streaming de dados com baixo uso de memória");
Console.WriteLine("✅ Suporte nativo a CancellationToken");
Console.WriteLine("✅ Lazy evaluation com async/await");
Console.WriteLine("✅ Composição e transformação de streams");
Console.WriteLine("✅ Ideal para APIs, processamento de arquivos e dados em tempo real");
Console.WriteLine("✅ Melhor experiência com await foreach");

Console.WriteLine("\n=== Fim da Demonstração ===");

// Métodos de demonstração

static async IAsyncEnumerable<int> GerarNumerosAsync()
{
    for (int i = 1; i <= 5; i++)
    {
        await Task.Delay(500); // Simula operação assíncrona
        yield return i * i;
    }
}

static async IAsyncEnumerable<string> GerarDadosComCancelamentoAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    for (int i = 1; i <= 10; i++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(1000, cancellationToken);
        yield return $"Item {i} - {DateTime.Now:HH:mm:ss}";
    }
}

static async IAsyncEnumerable<string> LerArquivoLinhasPorLinhaAsync(string filePath)
{
    using var reader = new StreamReader(filePath);
    string? linha;
    while ((linha = await reader.ReadLineAsync()) != null)
    {
        yield return linha;
    }
}

static async IAsyncEnumerable<string> CombinarStreamAsync()
{
    var stream1 = GerarStreamAsync("A", 3);
    var stream2 = GerarStreamAsync("B", 2);
    
    var tasks = new[]
    {
        CollectStreamAsync(stream1),
        CollectStreamAsync(stream2)
    };
    
    var results = await Task.WhenAll(tasks);
    
    foreach (var item in results.SelectMany(r => r).OrderBy(x => x))
    {
        yield return item;
    }
}

static async IAsyncEnumerable<string> GerarStreamAsync(string prefix, int count)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(300);
        yield return $"{prefix}{i}";
    }
}

static async Task<List<string>> CollectStreamAsync(IAsyncEnumerable<string> stream)
{
    var result = new List<string>();
    await foreach (var item in stream)
    {
        result.Add(item);
    }
    return result;
}

static async IAsyncEnumerable<string> TransformarStreamAsync()
{
    await foreach (var numero in GerarNumerosAsync())
    {
        // Transformação assíncrona
        await Task.Delay(100);
        yield return $"Número transformado: {numero * 2}";
    }
}

static async IAsyncEnumerable<int> GerarSequenciaAsync()
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(100);
        yield return i;
    }
}

static async IAsyncEnumerable<List<T>> AgruparEmBatchesAsync<T>(
    IAsyncEnumerable<T> source, 
    int batchSize)
{
    var batch = new List<T>(batchSize);
    
    await foreach (var item in source)
    {
        batch.Add(item);
        
        if (batch.Count == batchSize)
        {
            yield return new List<T>(batch);
            batch.Clear();
        }
    }
    
    if (batch.Count > 0)
    {
        yield return batch;
    }
}

static async IAsyncEnumerable<string> StreamComErrosAsync()
{
    for (int i = 1; i <= 5; i++)
    {
        await Task.Delay(200);
        
        if (i == 3)
        {
            Console.WriteLine($"  ⚠️ Erro capturado: Erro simulado no item {i}");
            yield return $"Item {i} processado com erro tratado";
        }
        else
        {
            yield return $"Item {i} processado com sucesso";
        }
    }
}

static async Task CompararPerformanceAsync()
{
    Console.WriteLine("  Comparando List vs IAsyncEnumerable para grandes volumes:");
    
    // Método tradicional - carrega tudo na memória
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var lista = await GerarListaCompletaAsync(1000);
    var primeirosList = lista.Take(5).ToList();
    stopwatch.Stop();
    Console.WriteLine($"    List completa (1000 items): {stopwatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"    Primeiros 5: [{string.Join(", ", primeirosList)}]");
    
    // IAsyncEnumerable - streaming
    stopwatch.Restart();
    var primeirosStream = new List<int>();
    await foreach (var item in GerarStreamLongoAsync(1000))
    {
        primeirosStream.Add(item);
        if (primeirosStream.Count >= 5) break;
    }
    stopwatch.Stop();
    Console.WriteLine($"    IAsyncEnumerable stream: {stopwatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"    Primeiros 5: [{string.Join(", ", primeirosStream)}]");
}

static async Task<List<int>> GerarListaCompletaAsync(int count)
{
    var lista = new List<int>();
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(1); // Simula operação
        lista.Add(i);
    }
    return lista;
}

static async IAsyncEnumerable<int> GerarStreamLongoAsync(int count)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(1); // Simula operação
        yield return i;
    }
}

// Classes auxiliares

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CriadoEm { get; set; }
}

public class ApiStreamingService
{
    public async IAsyncEnumerable<Usuario> GetUsuariosPaginadosAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int pageSize = 2;
        var totalPages = 3;
        
        for (int page = 1; page <= totalPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Simula chamada HTTP
            await Task.Delay(300, cancellationToken);
            
            var usuarios = await SimularChamadaApiAsync(page, pageSize);
            
            foreach (var usuario in usuarios)
            {
                yield return usuario;
            }
        }
    }
    
    private async Task<List<Usuario>> SimularChamadaApiAsync(int page, int pageSize)
    {
        await Task.Delay(100); // Simula latência de rede
        
        var usuarios = new List<Usuario>();
        var startId = (page - 1) * pageSize + 1;
        
        for (int i = 0; i < pageSize; i++)
        {
            usuarios.Add(new Usuario
            {
                Id = startId + i,
                Nome = $"Usuário {startId + i}",
                Email = $"usuario{startId + i}@email.com",
                CriadoEm = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
            });
        }
        
        return usuarios;
    }
}

public class RealTimeDataProcessor
{
    public async IAsyncEnumerable<string> ProcessarDadosAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var sensors = new[] { "Temperatura", "Umidade", "Pressão", "Luminosidade" };
        
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var sensor in sensors)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Simula leitura de sensor
                await Task.Delay(Random.Shared.Next(200, 800), cancellationToken);
                
                var valor = Random.Shared.NextDouble() * 100;
                var timestamp = DateTime.Now;
                
                yield return $"{sensor}: {valor:F2} - {timestamp:HH:mm:ss.fff}";
            }
        }
    }
}

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Task<TResult>> selector)
    {
        await foreach (var item in source)
        {
            yield return await selector(item);
        }
    }
    
    public static async IAsyncEnumerable<T> WhereAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, Task<bool>> predicate)
    {
        await foreach (var item in source)
        {
            if (await predicate(item))
            {
                yield return item;
            }
        }
    }
    
    public static async IAsyncEnumerable<T> TakeAsync<T>(
        this IAsyncEnumerable<T> source,
        int count)
    {
        var taken = 0;
        await foreach (var item in source)
        {
            if (taken >= count) break;
            yield return item;
            taken++;
        }
    }
}
