using System.Collections.Concurrent;

Console.WriteLine("=== Dica 27: Evitando Bloqueios em Código Assíncrono ===\n");

var deadlockDemo = new DeadlockDemo();
var solutionDemo = new SolutionDemo();

// Demonstração dos problemas de deadlock
Console.WriteLine("1. Problemas comuns que causam deadlock:");
await deadlockDemo.DemonstrateProblems();

Console.WriteLine("\n" + new string('=', 50) + "\n");

// Demonstração das soluções
Console.WriteLine("2. Soluções para evitar deadlock:");
await solutionDemo.DemonstrateSolutions();

Console.WriteLine("\n" + new string('=', 50));
Console.WriteLine("Resumo das práticas recomendadas:");
Console.WriteLine("✅ Use async/await em toda a cadeia (async all the way)");
Console.WriteLine("✅ Use ConfigureAwait(false) em bibliotecas");
Console.WriteLine("✅ Use Task.Run para operações CPU-intensive");
Console.WriteLine("✅ Use Task.WhenAll para paralelismo");
Console.WriteLine("✅ Controle a concorrência com processamento em lotes");
Console.WriteLine("❌ NUNCA use .Result, .Wait() ou GetAwaiter().GetResult()");
Console.WriteLine("❌ NUNCA misture código síncrono e assíncrono sem cuidado");

public class DeadlockDemo
{
    public async Task DemonstrateProblems()
    {
        Console.WriteLine("⚠️  ATENÇÃO: Os exemplos abaixo podem causar deadlock em ambientes síncronos!");
        Console.WriteLine("Em aplicações reais, evite esses padrões:\n");

        // Exemplo problemático 1: .Result
        Console.WriteLine("❌ Problema 1: Usando .Result em contexto síncrono");
        Console.WriteLine("// NUNCA faça isso:");
        Console.WriteLine("var result = DoAsyncWork().Result; // Pode causar deadlock!");
        
        // Exemplo problemático 2: .Wait()
        Console.WriteLine("\n❌ Problema 2: Usando .Wait() em contexto síncrono");
        Console.WriteLine("// NUNCA faça isso:");
        Console.WriteLine("DoAsyncWork().Wait(); // Pode causar deadlock!");
        
        // Exemplo problemático 3: GetAwaiter().GetResult()
        Console.WriteLine("\n❌ Problema 3: Usando GetAwaiter().GetResult()");
        Console.WriteLine("// NUNCA faça isso:");
        Console.WriteLine("DoAsyncWork().GetAwaiter().GetResult(); // Pode causar deadlock!");

        // Demonstração segura do padrão problemático
        Console.WriteLine("\n✅ Versão segura para demonstração:");
        try
        {
            var result = await DoAsyncWorkSafe();
            Console.WriteLine($"Resultado obtido de forma segura: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro capturado: {ex.Message}");
        }
    }

    private async Task<string> DoAsyncWorkSafe()
    {
        await Task.Delay(100); // Simula operação assíncrona
        return "Operação concluída";
    }
}

public class SolutionDemo
{
    public async Task DemonstrateSolutions()
    {
        Console.WriteLine("✅ Soluções recomendadas:\n");

        // Solução 1: ConfigureAwait(false)
        await DemonstrateConfigureAwait();
        
        // Solução 2: Async all the way
        await DemonstrateAsyncAllTheWay();
        
        // Solução 3: Task.Run para CPU-bound
        await DemonstrateTaskRun();
        
        // Solução 4: Processamento paralelo
        await DemonstrateParallelProcessing();
    }

    private async Task DemonstrateConfigureAwait()
    {
        Console.WriteLine("1. Use ConfigureAwait(false) em bibliotecas:");
        
        var service = new LibraryService();
        var result = await service.ProcessDataAsync("dados importantes");
        Console.WriteLine($"   Resultado: {result}");
    }

    private async Task DemonstrateAsyncAllTheWay()
    {
        Console.WriteLine("\n2. Async/await até a raiz (async all the way):");
        
        var data = await LoadDataAsync();
        var processed = await ProcessDataAsync(data);
        var saved = await SaveDataAsync(processed);
        
        Console.WriteLine($"   Pipeline assíncrono concluído: {saved}");
    }

    private async Task DemonstrateTaskRun()
    {
        Console.WriteLine("\n3. Use Task.Run para operações CPU-intensive:");
        
        var result = await Task.Run(() => IntensiveComputation(1000000));
        Console.WriteLine($"   Resultado do cálculo intensivo: {result}");
    }

    private async Task DemonstrateParallelProcessing()
    {
        Console.WriteLine("\n4. Processamento paralelo com Task.WhenAll:");
        
        var tasks = new[]
        {
            ProcessItemAsync("Item 1"),
            ProcessItemAsync("Item 2"),
            ProcessItemAsync("Item 3"),
            ProcessItemAsync("Item 4")
        };

        var results = await Task.WhenAll(tasks);
        Console.WriteLine($"   Processados {results.Length} itens em paralelo:");
        foreach (var result in results)
        {
            Console.WriteLine($"     - {result}");
        }

        // Demonstração com processamento em lotes
        await DemonstrateBatchProcessing();
    }

    private async Task DemonstrateBatchProcessing()
    {
        Console.WriteLine("\n5. Processamento em lotes para controlar concorrência:");
        
        var items = Enumerable.Range(1, 10).Select(i => $"Item {i}").ToArray();
        const int batchSize = 3;

        var results = new ConcurrentBag<string>();
        
        for (int i = 0; i < items.Length; i += batchSize)
        {
            var batch = items.Skip(i).Take(batchSize);
            var batchTasks = batch.Select(async item =>
            {
                var result = await ProcessItemAsync(item);
                results.Add(result);
                return result;
            });

            await Task.WhenAll(batchTasks);
            Console.WriteLine($"   Lote processado: {string.Join(", ", batch)}");
        }
    }

    private async Task<string> LoadDataAsync()
    {
        await Task.Delay(50); // Simula I/O
        return "dados carregados";
    }

    private async Task<string> ProcessDataAsync(string data)
    {
        await Task.Delay(30); // Simula processamento
        return $"{data} -> processados";
    }

    private async Task<string> SaveDataAsync(string data)
    {
        await Task.Delay(40); // Simula I/O
        return $"{data} -> salvos";
    }

    private async Task<string> ProcessItemAsync(string item)
    {
        await Task.Delay(Random.Shared.Next(50, 200)); // Simula tempo variável
        return $"{item} processado";
    }

    private int IntensiveComputation(int iterations)
    {
        int result = 0;
        for (int i = 0; i < iterations; i++)
        {
            result += i * i % 1000;
        }
        return result;
    }
}

public class LibraryService
{
    public async Task<string> ProcessDataAsync(string data)
    {
        // ✅ SEMPRE use ConfigureAwait(false) em bibliotecas
        await Task.Delay(100).ConfigureAwait(false);
        
        // Simula processamento adicional
        await DoInternalWorkAsync().ConfigureAwait(false);
        
        return $"Biblioteca processou: {data}";
    }

    private async Task DoInternalWorkAsync()
    {
        // ✅ ConfigureAwait(false) em toda a cadeia
        await Task.Delay(50).ConfigureAwait(false);
    }
}
