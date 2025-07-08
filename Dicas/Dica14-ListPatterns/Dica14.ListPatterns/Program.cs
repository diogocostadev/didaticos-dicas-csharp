Console.WriteLine("=== Dica 14: List Patterns no C# 11 ===");

// 1. Introdução aos List Patterns
Console.WriteLine("1. Introdução aos List Patterns:");

int[] numeros = [1, 2, 3, 4, 5];

// Pattern matching tradicional vs. List Patterns
var resultado1 = numeros switch
{
    [1, 2, 3, 4, 5] => "Sequência exata 1-5",
    [1, 2, 3, ..] => "Começa com 1, 2, 3",
    [.., 4, 5] => "Termina com 4, 5",
    [var primeiro, .., var ultimo] => $"Primeiro: {primeiro}, Último: {ultimo}",
    [] => "Array vazio",
    _ => "Outro padrão"
};

Console.WriteLine($"  Array [1,2,3,4,5]: {resultado1}");

// Testando diferentes arrays
int[] vazio = [];
int[] pequeno = [1, 2];
int[] grande = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

Console.WriteLine($"  Array vazio: {TestarArray(vazio)}");
Console.WriteLine($"  Array pequeno [1,2]: {TestarArray(pequeno)}");
Console.WriteLine($"  Array grande [1-10]: {TestarArray(grande)}");

Console.WriteLine();

// 2. Slice patterns com range operator
Console.WriteLine("2. Slice patterns avançados:");

string[] palavras = ["Hello", "Beautiful", "World", "C#", "Programming"];

var resultado2 = palavras switch
{
    ["Hello", .. var meio, "Programming"] => $"Início e fim específicos, meio: [{string.Join(", ", meio)}]",
    ["Hello", var segunda, .. var resto] => $"Hello + {segunda}, resto: [{string.Join(", ", resto)}]",
    [var primeira, .. var intermedias, var ultima] => $"{primeira} ... {ultima} (total: {intermedias.Length + 2})",
    _ => "Padrão não reconhecido"
};

Console.WriteLine($"  Resultado: {resultado2}");

Console.WriteLine();

// 3. List Patterns com condições
Console.WriteLine("3. List Patterns com condições (when):");

var temperaturas = new int[][] {
    [20, 22, 25, 27, 24],        // Semana normal
    [35, 38, 40, 39, 36],        // Semana de calor
    [5, 3, 1, -2, 0],            // Semana de frio
    [15, 18, 22, 25, 28, 30],    // Semana crescente
    []                           // Sem dados
};

foreach (var semana in temperaturas)
{
    var analise = AnalisarTemperaturas(semana);
    Console.WriteLine($"  {string.Join(", ", semana)} → {analise}");
}

Console.WriteLine();

// 4. Pattern matching com records e classes
Console.WriteLine("4. List Patterns com objetos complexos:");

var pessoas = new List<Pessoa> {
    new("João", 25, "Desenvolvedor"),
    new("Maria", 30, "Designer"),
    new("Pedro", 35, "Gerente"),
    new("Ana", 28, "Analista")
};

var analiseEquipe = pessoas switch
{
    [] => "Equipe vazia",
    [var unico] => $"Equipe de uma pessoa: {unico.Nome} ({unico.Cargo})",
    [var lider, .. var membros] when lider.Idade > 30 => 
        $"Líder experiente: {lider.Nome}, {membros.Count} membros",
    [.. var todos] when todos.All(p => p.Idade < 30) => 
        "Equipe jovem (todos < 30 anos)",
    [.. var equipe] => 
        $"Equipe mista: {equipe.Count} pessoas, idades: {string.Join(", ", equipe.Select(p => p.Idade))}"
};

Console.WriteLine($"  Análise da equipe: {analiseEquipe}");

Console.WriteLine();

// 5. List Patterns em algoritmos
Console.WriteLine("5. Algoritmos com List Patterns:");

// Quick Sort usando List Patterns
static List<int> QuickSortListPattern(List<int> lista)
{
    return lista switch
    {
        [] or [_] => lista,
        [var pivot, .. var resto] =>
            [
                .. QuickSortListPattern(resto.Where(x => x < pivot).ToList()),
                pivot,
                .. QuickSortListPattern(resto.Where(x => x >= pivot).ToList())
            ]
    };
}

var numerosDesordenados = new List<int> { 64, 34, 25, 12, 22, 11, 90 };
var numerosOrdenados = QuickSortListPattern(numerosDesordenados);

Console.WriteLine($"  Original: [{string.Join(", ", numerosDesordenados)}]");
Console.WriteLine($"  Ordenado: [{string.Join(", ", numerosOrdenados)}]");

Console.WriteLine();

// 6. Validação de sequências
Console.WriteLine("6. Validação de sequências com List Patterns:");

var sequencias = new string[][] {
    ["GET", "/api/users", "200"],
    ["POST", "/api/users", "201", "application/json"],
    ["DELETE", "/api/users/123"],
    ["INVALID"],
    []
};

foreach (var sequencia in sequencias)
{
    var validacao = ValidarLogHTTP(sequencia);
    Console.WriteLine($"  [{string.Join(", ", sequencia)}] → {validacao}");
}

Console.WriteLine();

// 7. Processamento de comandos
Console.WriteLine("7. Processamento de comandos:");

var comandos = new string[][] {
    ["help"],
    ["create", "file", "teste.txt"],
    ["copy", "origem.txt", "destino.txt"],
    ["delete", "arquivo.txt"],
    ["list", "*.cs"],
    ["backup", "--all", "--compress"],
    ["invalid", "too", "many", "params", "here"]
};

foreach (var comando in comandos)
{
    var resultado = ProcessarComando(comando);
    Console.WriteLine($"  {string.Join(" ", comando)} → {resultado}");
}

Console.WriteLine();

// 8. List Patterns com diferentes tipos de coleção
Console.WriteLine("8. List Patterns com diferentes tipos:");

// Funciona com arrays, listas, spans, etc.
TestArray([1, 2, 3]);
TestList(new List<string> { "A", "B", "C" });
TestSpan([1, 2, 3, 4, 5]);
TestReadOnlySpan("Hello".AsSpan());

Console.WriteLine();

Console.WriteLine("=== Resumo das Vantagens dos List Patterns ===");
Console.WriteLine("✅ Sintaxe declarativa para matching de sequências");
Console.WriteLine("✅ Slice patterns com .. para partes variáveis");
Console.WriteLine("✅ Captura de elementos específicos e sub-arrays");
Console.WriteLine("✅ Integração perfeita com pattern matching");
Console.WriteLine("✅ Funciona com arrays, listas, spans e mais");
Console.WriteLine("✅ Código mais legível que loops tradicionais");
Console.WriteLine("✅ Performance otimizada pelo compilador");

Console.WriteLine("=== Fim da Demonstração ===");

// =================== MÉTODOS AUXILIARES ===================

static string TestarArray(int[] array) => array switch
{
    [] => "Array vazio",
    [var unico] => $"Um elemento: {unico}",
    [var primeiro, var segundo] => $"Dois elementos: {primeiro}, {segundo}",
    [1, ..] => "Começa com 1",
    [.., 10] => "Termina com 10",
    [var inicio, .., var fim] => $"Primeiro: {inicio}, Último: {fim}",
    _ => "Outro padrão"
};

static string AnalisarTemperaturas(int[] temps) => temps switch
{
    [] => "Sem dados",
    [var unica] => $"Temperatura única: {unica}°C",
    [.. var todas] when todas.All(t => t > 30) => $"Semana quente (média: {todas.Average():F1}°C)",
    [.. var todas] when todas.All(t => t < 10) => $"Semana fria (média: {todas.Average():F1}°C)",
    [var primeira, .. var meio, var ultima] when ultima > primeira => 
        $"Tendência de aquecimento ({primeira}°C → {ultima}°C)",
    [var primeira, .. var meio, var ultima] when ultima < primeira => 
        $"Tendência de esfriamento ({primeira}°C → {ultima}°C)",
    _ => $"Semana estável (amplitude: {temps.Max() - temps.Min()}°C)"
};

static string ValidarLogHTTP(string[] log) => log switch
{
    [] => "Log vazio",
    [var metodo] when metodo == "help" => "Comando de ajuda",
    ["GET", var url, var status] => $"GET request: {url} → {status}",
    ["POST", var url, var status, var contentType] => $"POST request: {url} → {status} ({contentType})",
    ["DELETE", var url] => $"DELETE request: {url}",
    [var metodo, ..] when !new[] { "GET", "POST", "PUT", "DELETE", "PATCH" }.Contains(metodo) =>
        $"Método HTTP inválido: {metodo}",
    _ => "Formato de log não reconhecido"
};

static string ProcessarComando(string[] args) => args switch
{
    [] => "Nenhum comando fornecido",
    ["help"] => "Mostrando ajuda...",
    ["create", "file", var nome] => $"Criando arquivo: {nome}",
    ["copy", var origem, var destino] => $"Copiando {origem} para {destino}",
    ["delete", var arquivo] => $"Deletando arquivo: {arquivo}",
    ["list", var filtro] => $"Listando arquivos: {filtro}",
    ["backup", .. var opcoes] => $"Fazendo backup com opções: {string.Join(", ", opcoes)}",
    [var comando, .. var parametros] when parametros.Length > 5 => 
        $"Muitos parâmetros para o comando: {comando}",
    [var comando, ..] => $"Comando '{comando}' executado com {args.Length - 1} parâmetros"
};

static void TestArray<T>(T[] array)
{
    var resultado = array switch
    {
        [] => "Array vazio",
        [var primeiro] => $"Um elemento: {primeiro}",
        [var primeiro, var segundo, var terceiro] => $"Três elementos: {primeiro}, {segundo}, {terceiro}",
        [var primeiro, .. var resto] => $"Primeiro: {primeiro}, resto: {resto.Length} elementos",
        _ => "Outro padrão"
    };
    Console.WriteLine($"  Array: {resultado}");
}

static void TestList<T>(List<T> lista)
{
    var resultado = lista switch
    {
        [] => "Lista vazia",
        [var unico] => $"Um item: {unico}",
        [var primeiro, .. var resto] => $"Primeiro: {primeiro}, resto: {resto.Count} itens",
        _ => "Outro padrão"
    };
    Console.WriteLine($"  Lista: {resultado}");
}

static void TestSpan(Span<int> span)
{
    var resultado = span switch
    {
        [] => "Span vazio",
        [var primeiro, ..] => $"Span - Primeiro: {primeiro}, resto: {span.Length - 1} elementos"
    };
    Console.WriteLine($"  Span: {resultado}");
}

static void TestReadOnlySpan(ReadOnlySpan<char> span)
{
    var resultado = span switch
    {
        [] => "ReadOnlySpan vazio",
        ['H', 'e', 'l', 'l', 'o'] => "É exatamente 'Hello'",
        ['H', ..] when span.Length > 1 => $"Começa com H, resto: {span.Length - 1} chars",
        [.., 'o'] when span.Length > 1 => $"Termina com o, início: {span.Length - 1} chars",
        [var c] => $"Um único caractere: {c}",
        _ => $"ReadOnlySpan com {span.Length} caracteres"
    };
    Console.WriteLine($"  ReadOnlySpan: {resultado}");
}

// =================== CLASSES AUXILIARES ===================

public record Pessoa(string Nome, int Idade, string Cargo);
