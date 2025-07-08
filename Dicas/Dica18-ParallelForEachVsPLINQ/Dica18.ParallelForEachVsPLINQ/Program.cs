using System.Collections.Concurrent;

Console.WriteLine("=== Dica 18: Parallel.ForEach vs PLINQ ===\n");

// 1. Processamento Sequencial vs Paralelo
Console.WriteLine("1. Comparação Básica - Processamento de Números:");
var numeros = Enumerable.Range(1, 5_000).ToArray(); // Reduzir para evitar overflow

// Sequencial
var inicioSequencial = DateTime.Now;
var resultadoSequencial = numeros
    .Where(n => n % 2 == 0)
    .Select(n => (long)ProcessarNumero(n))
    .Sum();
var tempoSequencial = DateTime.Now - inicioSequencial;

// PLINQ
var inicioPLINQ = DateTime.Now;
var resultadoPLINQ = numeros
    .AsParallel()
    .Where(n => n % 2 == 0)
    .Select(n => (long)ProcessarNumero(n))
    .Sum();
var tempoPLINQ = DateTime.Now - inicioPLINQ;

// Parallel.ForEach
var inicioParallelForEach = DateTime.Now;
var resultadoParallelForEach = 0L;
var lockObj = new object();

Parallel.ForEach(numeros.Where(n => n % 2 == 0), numero =>
{
    var processado = (long)ProcessarNumero(numero);
    lock (lockObj)
    {
        resultadoParallelForEach += processado;
    }
});
var tempoParallelForEach = DateTime.Now - inicioParallelForEach;

Console.WriteLine($"  Sequencial: {tempoSequencial.TotalMilliseconds:F1}ms - Resultado: {resultadoSequencial}");
Console.WriteLine($"  PLINQ: {tempoPLINQ.TotalMilliseconds:F1}ms - Resultado: {resultadoPLINQ}");
Console.WriteLine($"  Parallel.ForEach: {tempoParallelForEach.TotalMilliseconds:F1}ms - Resultado: {resultadoParallelForEach}");

// 2. Processamento de Arquivos
Console.WriteLine("\n2. Processamento de Arquivos (Simulado):");
var arquivos = Enumerable.Range(1, 1000)
    .Select(i => $"arquivo_{i:D4}.txt")
    .ToArray();

ProcessarArquivosComPLINQ(arquivos);
ProcessarArquivosComParallelForEach(arquivos);

// 3. Configuração de Paralelismo
Console.WriteLine("\n3. Configuração de Paralelismo:");
ConfigurarParalelismo();

// 4. Quando usar cada abordagem
Console.WriteLine("\n4. Casos de Uso Específicos:");
DemonstrarCasosDeUso();

// 5. Tratamento de Exceções
Console.WriteLine("\n5. Tratamento de Exceções:");
TratarExcecoes();

// 6. Particionamento Personalizado
Console.WriteLine("\n6. Particionamento Personalizado:");
DemonstrarParticionamento();

// 7. Performance com diferentes cargas
Console.WriteLine("\n7. Performance com Diferentes Cargas:");
CompararPerformance();

Console.WriteLine("\n=== Resumo das Melhores Práticas ===");
Console.WriteLine("📋 PLINQ:");
Console.WriteLine("  ✅ Use para queries complexas com múltiplas operações");
Console.WriteLine("  ✅ Use quando precisar de ordering (.AsOrdered())");
Console.WriteLine("  ✅ Use para pipelines de transformação de dados");
Console.WriteLine("  ✅ Melhor para operações funcionais (Select, Where, etc.)");

Console.WriteLine("\n📋 Parallel.ForEach:");
Console.WriteLine("  ✅ Use para operações com side effects");
Console.WriteLine("  ✅ Use quando precisar de controle fino sobre paralelismo");
Console.WriteLine("  ✅ Use para I/O intensivo (files, network, database)");
Console.WriteLine("  ✅ Melhor para processamento imperativo");

Console.WriteLine("\n⚠️ Cuidados:");
Console.WriteLine("  ❌ Evite para coleções pequenas (overhead > benefício)");
Console.WriteLine("  ❌ Cuidado com shared state (use locks ou Concurrent collections)");
Console.WriteLine("  ❌ Monitore uso de CPU (não exagere no paralelismo)");

Console.WriteLine("\n=== Fim da Demonstração ===");

static int ProcessarNumero(int numero)
{
    // Simula processamento CPU-intensivo (reduzido para evitar overflow)
    var result = 0;
    for (int i = 0; i < 100; i++)
    {
        result += (numero + i) % 100;
    }
    return result;
}

static void ProcessarArquivosComPLINQ(string[] arquivos)
{
    Console.WriteLine("  PLINQ - Processamento de Arquivos:");
    var inicio = DateTime.Now;
    
    var resultados = arquivos
        .AsParallel()
        .WithDegreeOfParallelism(Environment.ProcessorCount)
        .Select(arquivo => ProcessarArquivo(arquivo))
        .Where(resultado => resultado.Sucesso)
        .ToArray();
    
    var tempo = DateTime.Now - inicio;
    Console.WriteLine($"    {resultados.Length} arquivos processados em {tempo.TotalMilliseconds:F1}ms");
}

static void ProcessarArquivosComParallelForEach(string[] arquivos)
{
    Console.WriteLine("  Parallel.ForEach - Processamento de Arquivos:");
    var inicio = DateTime.Now;
    var sucessos = 0;
    
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };
    
    Parallel.ForEach(arquivos, options, arquivo =>
    {
        var resultado = ProcessarArquivo(arquivo);
        if (resultado.Sucesso)
        {
            Interlocked.Increment(ref sucessos);
        }
    });
    
    var tempo = DateTime.Now - inicio;
    Console.WriteLine($"    {sucessos} arquivos processados em {tempo.TotalMilliseconds:F1}ms");
}

static ResultadoProcessamento ProcessarArquivo(string arquivo)
{
    // Simula I/O de arquivo
    Thread.Sleep(Random.Shared.Next(1, 5));
    
    // Simula algumas falhas
    var sucesso = Random.Shared.Next(100) > 5; // 95% success rate
    
    return new ResultadoProcessamento(arquivo, sucesso);
}

static void ConfigurarParalelismo()
{
    var dados = Enumerable.Range(1, 1000).ToArray();
    
    Console.WriteLine("  Configurando grau de paralelismo:");
    
    // PLINQ com configuração
    var resultadoPLINQ = dados
        .AsParallel()
        .WithDegreeOfParallelism(2) // Limitar a 2 threads
        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
        .Select(x => x * x)
        .Sum();
    
    // Parallel.ForEach com configuração
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = 2,
        CancellationToken = CancellationToken.None
    };
    
    var resultadoParallel = 0L;
    Parallel.ForEach(dados, options, numero =>
    {
        var valor = numero * numero;
        Interlocked.Add(ref resultadoParallel, valor);
    });
    
    Console.WriteLine($"    PLINQ (2 threads): {resultadoPLINQ}");
    Console.WriteLine($"    Parallel.ForEach (2 threads): {resultadoParallel}");
}

static void DemonstrarCasosDeUso()
{
    // Caso 1: PLINQ para transformação de dados
    Console.WriteLine("  PLINQ - Pipeline de transformação:");
    var vendas = GerarVendas(1000);
    
    var relatorio = vendas
        .AsParallel()
        .Where(v => v.Valor > 100)
        .GroupBy(v => v.Categoria)
        .Select(g => new { Categoria = g.Key, Total = g.Sum(v => v.Valor) })
        .OrderByDescending(r => r.Total)
        .Take(3)
        .ToArray();
    
    foreach (var item in relatorio)
    {
        Console.WriteLine($"    {item.Categoria}: {item.Total:C}");
    }
    
    // Caso 2: Parallel.ForEach para operações com side-effects
    Console.WriteLine("  Parallel.ForEach - Envio de emails:");
    var emails = GenerateEmails(100);
    var enviados = 0;
    
    Parallel.ForEach(emails, email =>
    {
        EnviarEmail(email);
        Interlocked.Increment(ref enviados);
    });
    
    Console.WriteLine($"    {enviados} emails enviados");
}

static void TratarExcecoes()
{
    var numeros = Enumerable.Range(1, 100).ToArray();
    
    // PLINQ com tratamento de exceções
    try
    {
        var resultadoPLINQ = numeros
            .AsParallel()
            .Select(n => n == 50 ? throw new InvalidOperationException($"Erro no número {n}") : n * 2)
            .ToArray();
    }
    catch (AggregateException ex)
    {
        Console.WriteLine($"  PLINQ capturou {ex.InnerExceptions.Count} exceções");
        foreach (var inner in ex.InnerExceptions.Take(3))
        {
            Console.WriteLine($"    {inner.Message}");
        }
    }
    
    // Parallel.ForEach com tratamento
    var exceptions = new ConcurrentBag<Exception>();
    
    Parallel.ForEach(numeros, numero =>
    {
        try
        {
            if (numero == 75)
                throw new InvalidOperationException($"Erro no número {numero}");
            
            // Processar número
            var result = numero * 2;
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
        }
    });
    
    Console.WriteLine($"  Parallel.ForEach capturou {exceptions.Count} exceções");
        foreach (var ex in exceptions.Take(3))
        {
            Console.WriteLine($"    {ex.Message}");
        }
}

static void DemonstrarParticionamento()
{
    var dados = Enumerable.Range(1, 10000).ToArray();
    
    // Particionamento padrão
    var inicio1 = DateTime.Now;
    var resultado1 = dados
        .AsParallel()
        .Select(ProcessarNumero)
        .Sum();
    var tempo1 = DateTime.Now - inicio1;
    
    // Particionamento personalizado
    var inicio2 = DateTime.Now;
    var resultado2 = Partitioner.Create(dados, loadBalance: true)
        .AsParallel()
        .Select(ProcessarNumero)
        .Sum();
    var tempo2 = DateTime.Now - inicio2;
    
    Console.WriteLine($"  Particionamento padrão: {tempo1.TotalMilliseconds:F1}ms");
    Console.WriteLine($"  Particionamento customizado: {tempo2.TotalMilliseconds:F1}ms");
}

static void CompararPerformance()
{
    // Carga CPU-intensiva
    TesteCargaCPU();
    
    // Carga I/O-intensiva
    TesteCargaIO();
    
    // Carga mista
    TesteCargaMista();
}

static void TesteCargaCPU()
{
    Console.WriteLine("  Carga CPU-intensiva:");
    var dados = Enumerable.Range(1, 5000).ToArray();
    
    var inicio1 = DateTime.Now;
    var resultado1 = dados.Select(n => (long)ProcessarNumero(n)).Sum();
    var tempo1 = DateTime.Now - inicio1;
    
    var inicio2 = DateTime.Now;
    var resultado2 = dados.AsParallel().Select(n => (long)ProcessarNumero(n)).Sum();
    var tempo2 = DateTime.Now - inicio2;
    
    Console.WriteLine($"    Sequencial: {tempo1.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    PLINQ: {tempo2.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    Speedup: {tempo1.TotalMilliseconds / tempo2.TotalMilliseconds:F1}x");
}

static void TesteCargaIO()
{
    Console.WriteLine("  Carga I/O-intensiva:");
    var arquivos = Enumerable.Range(1, 200).Select(i => $"file_{i}").ToArray();
    
    var inicio1 = DateTime.Now;
    foreach (var arquivo in arquivos)
    {
        SimularIO(arquivo);
    }
    var tempo1 = DateTime.Now - inicio1;
    
    var inicio2 = DateTime.Now;
    Parallel.ForEach(arquivos, SimularIO);
    var tempo2 = DateTime.Now - inicio2;
    
    Console.WriteLine($"    Sequencial: {tempo1.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    Parallel.ForEach: {tempo2.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    Speedup: {tempo1.TotalMilliseconds / tempo2.TotalMilliseconds:F1}x");
}

static void TesteCargaMista()
{
    Console.WriteLine("  Carga mista (CPU + I/O):");
    var dados = Enumerable.Range(1, 500).ToArray();
    
    var inicio1 = DateTime.Now;
    var resultado1 = dados
        .Select(n => 
        {
            SimularIO($"data_{n}");
            return (long)ProcessarNumero(n);
        })
        .Sum();
    var tempo1 = DateTime.Now - inicio1;
    
    var inicio2 = DateTime.Now;
    var resultado2 = dados
        .AsParallel()
        .Select(n => 
        {
            SimularIO($"data_{n}");
            return (long)ProcessarNumero(n);
        })
        .Sum();
    var tempo2 = DateTime.Now - inicio2;
    
    Console.WriteLine($"    Sequencial: {tempo1.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    PLINQ: {tempo2.TotalMilliseconds:F1}ms");
    Console.WriteLine($"    Speedup: {tempo1.TotalMilliseconds / tempo2.TotalMilliseconds:F1}x");
}

static void SimularIO(string item)
{
    Thread.Sleep(Random.Shared.Next(1, 3));
}

static Venda[] GerarVendas(int quantidade)
{
    var categorias = new[] { "Eletrônicos", "Roupas", "Livros", "Casa", "Esportes" };
    var vendas = new Venda[quantidade];
    
    for (int i = 0; i < quantidade; i++)
    {
        vendas[i] = new Venda
        {
            Id = i + 1,
            Categoria = categorias[Random.Shared.Next(categorias.Length)],
            Valor = Random.Shared.Next(50, 1000)
        };
    }
    
    return vendas;
}

static string[] GenerateEmails(int quantidade)
{
    return Enumerable.Range(1, quantidade)
        .Select(i => $"user{i}@email.com")
        .ToArray();
}

static void EnviarEmail(string email)
{
    // Simula envio de email
    Thread.Sleep(Random.Shared.Next(5, 15));
}

// Classes de apoio
public record ResultadoProcessamento(string Arquivo, bool Sucesso);

public class Venda
{
    public int Id { get; set; }
    public string Categoria { get; set; } = "";
    public decimal Valor { get; set; }
}
