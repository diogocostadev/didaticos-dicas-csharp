using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Runtime.CompilerServices;

BenchmarkRunner.Run<AsyncEnumerableBenchmarks>();

[MemoryDiagnoser]
[SimpleJob]
public class AsyncEnumerableBenchmarks
{
    private const int DataSize = 10_000;
    private const int TakeCount = 100;

    [Benchmark(Baseline = true)]
    public async Task<List<int>> TraditionalListApproach()
    {
        // Carrega todos os dados na memória primeiro
        var allData = await LoadAllDataAsync();
        return allData.Take(TakeCount).ToList();
    }

    [Benchmark]
    public async Task<List<int>> AsyncEnumerableStreaming()
    {
        // Streaming approach - mais eficiente em memória
        var result = new List<int>();
        var count = 0;
        
        await foreach (var item in GenerateDataStreamAsync())
        {
            result.Add(item);
            if (++count >= TakeCount) break;
        }
        
        return result;
    }

    [Benchmark]
    public async Task<List<int>> AsyncEnumerableWithExtensions()
    {
        // Usando extensões customizadas
        var result = new List<int>();
        
        await foreach (var item in GenerateDataStreamAsync().TakeAsync(TakeCount))
        {
            result.Add(item);
        }
        
        return result;
    }

    [Benchmark]
    public async Task<List<ProcessedData>> StreamWithTransformation()
    {
        var result = new List<ProcessedData>();
        var count = 0;
        
        await foreach (var item in TransformDataStreamAsync())
        {
            result.Add(item);
            if (++count >= TakeCount) break;
        }
        
        return result;
    }

    [Benchmark]
    public async Task<List<ProcessedData>> TraditionalTransformation()
    {
        var allData = await LoadAllDataAsync();
        var result = new List<ProcessedData>();
        
        foreach (var item in allData.Take(TakeCount))
        {
            result.Add(await ProcessDataAsync(item));
        }
        
        return result;
    }

    [Benchmark]
    public async Task<List<List<int>>> BatchProcessingAsync()
    {
        var batches = new List<List<int>>();
        var batchCount = 0;
        
        await foreach (var batch in GroupIntoBatchesAsync(GenerateDataStreamAsync(), 10))
        {
            batches.Add(batch);
            if (++batchCount >= 10) break; // Limitar para benchmark
        }
        
        return batches;
    }

    [Benchmark]
    public async Task<List<List<int>>> TraditionalBatching()
    {
        var allData = await LoadAllDataAsync();
        var batches = new List<List<int>>();
        
        for (int i = 0; i < Math.Min(100, allData.Count); i += 10)
        {
            batches.Add(allData.Skip(i).Take(10).ToList());
        }
        
        return batches;
    }

    [Benchmark]
    public async Task<int> AsyncEnumerableFiltering()
    {
        var count = 0;
        
        await foreach (var item in GenerateDataStreamAsync().WhereAsync(async x =>
        {
            await Task.Delay(1); // Simula operação assíncrona
            return x % 2 == 0;
        }))
        {
            count++;
            if (count >= TakeCount) break;
        }
        
        return count;
    }

    // Métodos auxiliares
    private async Task<List<int>> LoadAllDataAsync()
    {
        await Task.Delay(50); // Simula tempo de carregamento
        return Enumerable.Range(1, DataSize).ToList();
    }

    private async IAsyncEnumerable<int> GenerateDataStreamAsync()
    {
        for (int i = 1; i <= DataSize; i++)
        {
            if (i % 1000 == 0) await Task.Delay(1); // Simula I/O ocasional
            yield return i;
        }
    }

    private async IAsyncEnumerable<ProcessedData> TransformDataStreamAsync()
    {
        await foreach (var item in GenerateDataStreamAsync())
        {
            yield return await ProcessDataAsync(item);
        }
    }

    private async Task<ProcessedData> ProcessDataAsync(int input)
    {
        await Task.Delay(1); // Simula processamento assíncrono
        return new ProcessedData
        {
            OriginalValue = input,
            ProcessedValue = input * 2,
            ProcessedAt = DateTime.UtcNow
        };
    }

    private async IAsyncEnumerable<List<T>> GroupIntoBatchesAsync<T>(
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
}

public class ProcessedData
{
    public int OriginalValue { get; set; }
    public int ProcessedValue { get; set; }
    public DateTime ProcessedAt { get; set; }
}

// Extensões para benchmarking
public static class AsyncEnumerableExtensions
{
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
}
