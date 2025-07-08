// =================== DEMONSTRAÇÃO: FILE-SCOPED NAMESPACE ===================
// Sintaxe C# 10: namespace sem chaves reduz nível de indentação
namespace Dica17.GlobalUsings;

// Note que não precisamos de mais usings aqui!
// Todos os usings globais definidos em GlobalUsings.cs estão disponíveis automaticamente

public class Program
{
    public static async Task Main(string[] args)
    {
        WriteLine("=== Dica 17: Global Usings e File-Scoped Namespaces ===");

        // 1. DEMONSTRAÇÃO: Usando tipos dos Global Usings sem declarar using
        WriteLine("\n1. Usando tipos sem declarar using:");

        // Dictionary<string, string> → StringDict (alias global)
        var configuracoes = new StringDict
        {
            ["host"] = "localhost",
            ["porta"] = "8080",
            ["ambiente"] = "desenvolvimento"
        };

        // List<int> → IntList (alias global)
        var numeros = new IntList { 1, 2, 3, 4, 5 };

        // JsonSerializerOptions → JsonOptions (alias global)
        var jsonOpts = new JsonOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        WriteLine($"  Configurações: {string.Join(", ", configuracoes.Select(kv => $"{kv.Key}={kv.Value}"))}");
        WriteLine($"  Números: [{string.Join(", ", numeros)}]");
        WriteLine($"  JSON Options configurado: WriteIndented={jsonOpts.WriteIndented}");

        // 2. DEMONSTRAÇÃO: Collections avançadas (disponíveis via global using)
        WriteLine("\n2. Collections avançadas sem using explícito:");

        var concurrentDict = new ConcurrentDictionary<string, int>();
        concurrentDict.TryAdd("usuarios", 100);
        concurrentDict.TryAdd("produtos", 500);

        var immutableList = ImmutableList.Create("primeiro", "segundo", "terceiro");
        var novaLista = immutableList.Add("quarto");

        WriteLine($"  ConcurrentDictionary: {string.Join(", ", concurrentDict.Select(kv => $"{kv.Key}={kv.Value}"))}");
        WriteLine($"  ImmutableList original: [{string.Join(", ", immutableList)}]");
        WriteLine($"  ImmutableList nova: [{string.Join(", ", novaLista)}]");

        // 3. DEMONSTRAÇÃO: Delegates com aliases globais
        WriteLine("\n3. Delegates com aliases globais:");

        StringAction exibirMensagem = msg => WriteLine($"    📢 {msg}");
        StringFunc processarTexto = texto => texto.ToUpperInvariant();
        AsyncStringFunc processarAsync = async texto =>
        {
            await Task.Delay(10); // Simula processamento assíncrono
            return new string(texto.Reverse().ToArray());
        };

        exibirMensagem("Usando StringAction!");
        WriteLine($"  StringFunc resultado: {processarTexto("texto processado")}");

        var resultadoAsync = await processarAsync("async");
        WriteLine($"  AsyncStringFunc resultado: {resultadoAsync}");

        // 4. DEMONSTRAÇÃO: Usando classes de diferentes arquivos (file-scoped namespace)
        WriteLine("\n4. Usando classes do mesmo namespace:");

        var processador = new ProcessadorTexto();
        var resultado = processador.ProcessarComContadores("Exemplo de texto para análise");
        WriteLine($"  Análise de texto: {resultado}");

        var cache = new CacheMemoria<string>();
        cache.Adicionar("chave1", "valor1", TimeSpan.FromMinutes(5));
        var valorCache = cache.Obter("chave1");
        WriteLine($"  Cache: {valorCache ?? "não encontrado"}");

        // 5. DEMONSTRAÇÃO: Serialização JSON sem using explícito
        WriteLine("\n5. Serialização JSON:");

        var pessoa = new Pessoa("João Silva", 30, "Desenvolvedor");
        var json = JsonSerializer.Serialize(pessoa, jsonOpts);
        WriteLine($"  JSON serializado:\n{json}");

        var pessoaDeserializada = JsonSerializer.Deserialize<Pessoa>(json, jsonOpts);
        WriteLine($"  Objeto deserializado: {pessoaDeserializada}");

        // 6. DEMONSTRAÇÃO: Performance e diagnósticos
        WriteLine("\n6. Performance e diagnósticos:");

        var stopwatch = Stopwatch.StartNew();
        await ProcessarOperacaoComplexaAsync();
        stopwatch.Stop();

        WriteLine($"  Operação executada em: {stopwatch.ElapsedMilliseconds}ms");
        WriteLine($"  Informações do caller: {ObterInformacoesCaller()}");

        // 7. DEMONSTRAÇÃO: Comparação before/after
        WriteLine("\n7. Comparação de código:");
        AnalisadorCodigo.CompararAntesDepois();

        WriteLine("\n=== Resumo das Vantagens ===");
        WriteLine("✅ Global Usings reduzem repetição de imports");
        WriteLine("✅ File-Scoped Namespaces reduzem indentação");
        WriteLine("✅ Aliases globais simplificam tipos complexos");
        WriteLine("✅ Código mais limpo e focado na lógica");
        WriteLine("✅ Menos boilerplate, mais produtividade");
        WriteLine("✅ Configuração centralizada de usings");
        WriteLine("✅ Compatível com todas as features do C#");

        WriteLine("\n=== Fim da Demonstração ===");
    }

    // =================== MÉTODOS AUXILIARES ===================

    static async Task ProcessarOperacaoComplexaAsync()
    {
        // Simula processamento usando tipos dos global usings
        var tasks = Enumerable.Range(1, 5)
            .Select(async i =>
            {
                await Task.Delay(10);
                return i * i;
            });
        
        var resultados = await Task.WhenAll(tasks);
        WriteLine($"    Processados {resultados.Length} itens");
    }

    static string ObterInformacoesCaller(
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var fileName = Path.GetFileName(filePath);
        return $"{memberName} em {fileName}:{lineNumber}";
    }
}
