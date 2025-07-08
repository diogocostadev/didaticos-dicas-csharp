Console.WriteLine("=== Dica 13: Collection Expressions no C# 12 ===");

// 1. Demonstrando a nova sintaxe de Collection Expressions
Console.WriteLine("1. Comparação entre sintaxe tradicional e Collection Expressions:");

// Sintaxe tradicional
var numerosTradicional = new List<int> { 1, 2, 3, 4, 5 };
var nomesTradicionais = new string[] { "Ana", "Bruno", "Carlos" };

// ✨ Collection Expressions - sintaxe moderna do C# 12
List<int> numerosModernos = [1, 2, 3, 4, 5];
string[] nomesModernos = ["Ana", "Bruno", "Carlos"];

Console.WriteLine($"  Números tradicionais: [{string.Join(", ", numerosTradicional)}]");
Console.WriteLine($"  Números modernos: [{string.Join(", ", numerosModernos)}]");
Console.WriteLine($"  Nomes tradicionais: [{string.Join(", ", nomesTradicionais)}]");
Console.WriteLine($"  Nomes modernos: [{string.Join(", ", nomesModernos)}]");

Console.WriteLine();

// 2. Diferentes tipos de coleções
Console.WriteLine("2. Collection Expressions com diferentes tipos:");

// Arrays
int[] array = [10, 20, 30];
Console.WriteLine($"  Array: [{string.Join(", ", array)}]");

// Lists
List<string> lista = ["Apple", "Banana", "Cherry"];
Console.WriteLine($"  List: [{string.Join(", ", lista)}]");

// HashSet
HashSet<int> conjunto = [1, 2, 3, 2, 1]; // Duplicatas serão removidas
Console.WriteLine($"  HashSet: [{string.Join(", ", conjunto)}]");

// Span e ReadOnlySpan
Span<int> span = [100, 200, 300];
ReadOnlySpan<char> roSpan = ['H', 'e', 'l', 'l', 'o'];
Console.WriteLine($"  Span: [{string.Join(", ", span.ToArray())}]");
Console.WriteLine($"  ReadOnlySpan: [{string.Join(", ", roSpan.ToArray())}]");

Console.WriteLine();

// 3. Spread operator com Collection Expressions
Console.WriteLine("3. Spread operator (..) com Collection Expressions:");

int[] primeira = [1, 2, 3];
int[] segunda = [4, 5, 6];
int[] terceira = [7, 8, 9];

// Combinando arrays com spread operator
int[] combinada = [..primeira, ..segunda, ..terceira];
Console.WriteLine($"  Combinada: [{string.Join(", ", combinada)}]");

// Inserindo elementos no meio
int[] comElementosExtras = [0, ..primeira, 10, ..segunda, 20, ..terceira, 30];
Console.WriteLine($"  Com extras: [{string.Join(", ", comElementosExtras)}]");

// Trabalhando com diferentes tipos de coleções
List<string> frutas = ["Maçã", "Banana"];
string[] vegetais = ["Cenoura", "Brócolis"];
IEnumerable<string> temperos = new[] { "Sal", "Pimenta" };

List<string> ingredientes = [..frutas, ..vegetais, ..temperos, "Azeite"];
Console.WriteLine($"  Ingredientes: [{string.Join(", ", ingredientes)}]");

Console.WriteLine();

// 4. Collection Expressions em métodos
Console.WriteLine("4. Usando Collection Expressions em métodos:");

static int Somar(params int[] numeros) => numeros.Sum();
static void ExibirItens<T>(IEnumerable<T> itens) => 
    Console.WriteLine($"    Itens: [{string.Join(", ", itens)}]");

// Passando Collection Expressions diretamente
var soma1 = Somar([1, 2, 3, 4, 5]);
var soma2 = Somar([10, 20, 30]);

Console.WriteLine($"  Soma 1: {soma1}");
Console.WriteLine($"  Soma 2: {soma2}");

ExibirItens([1, 2, 3]);
ExibirItens(["A", "B", "C"]);
ExibirItens([1.1, 2.2, 3.3]);

Console.WriteLine();

// 5. Casos de uso práticos
Console.WriteLine("5. Casos de uso práticos:");

// Configurações
var configuracoes = ProcessarConfiguracoes([
    new("Database", "localhost"),
    new("Port", "5432"),
    new("Timeout", "30")
]);

Console.WriteLine($"  Configurações processadas: {configuracoes.Count}");

// Validação de dados
var dadosValidos = ValidarDados([
    "email@exemplo.com",
    "usuario123",
    "senha_forte",
    "", // Inválido
    "outro@email.com"
]);

Console.WriteLine($"  Dados válidos: {dadosValidos} de 5");

// Transformação de dados
var numerosTransformados = TransformarNumeros([1, 2, 3, 4, 5]);
Console.WriteLine($"  Transformados: [{string.Join(", ", numerosTransformados)}]");

Console.WriteLine();

// 6. Collection Expressions com LINQ
Console.WriteLine("6. Collection Expressions com LINQ:");

int[] numeros = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

// Filtragem com Collection Expression
var pares = numeros.Where(x => x % 2 == 0).ToArray();
var impares = numeros.Where(x => x % 2 != 0).ToArray();

// Recombinando com Collection Expressions
int[] reorganizados = [..pares, 0, ..impares];
Console.WriteLine($"  Pares: [{string.Join(", ", pares)}]");
Console.WriteLine($"  Ímpares: [{string.Join(", ", impares)}]");
Console.WriteLine($"  Reorganizados: [{string.Join(", ", reorganizados)}]");

// Transformação com Collection Expressions
string[] textos = ["Hello", "World", "C#", "Collection", "Expressions"];
var resultado = textos
    .Where(t => t.Length > 3)
    .Select(t => t.ToUpper())
    .ToArray();

string[] final = ["Inicio", ..resultado, "Fim"];
Console.WriteLine($"  Final: [{string.Join(", ", final)}]");

Console.WriteLine();

// 7. Performance e memory efficiency
Console.WriteLine("7. Análise de performance:");

// Medindo tempo de criação
var sw = System.Diagnostics.Stopwatch.StartNew();

// Método tradicional
for (int i = 0; i < 10000; i++)
{
    var tradicional = new List<int> { 1, 2, 3, 4, 5 };
}

sw.Stop();
var tempoTradicional = sw.ElapsedTicks;

sw.Restart();

// Collection Expressions
for (int i = 0; i < 10000; i++)
{
    List<int> moderno = [1, 2, 3, 4, 5];
}

sw.Stop();
var tempoModerno = sw.ElapsedTicks;

Console.WriteLine($"  Tempo tradicional: {tempoTradicional} ticks");
Console.WriteLine($"  Tempo moderno: {tempoModerno} ticks");
Console.WriteLine($"  Diferença: {((double)(tempoTradicional - tempoModerno) / tempoTradicional * 100):F1}%");

Console.WriteLine();

// 8. Inicialização de objetos complexos
Console.WriteLine("8. Inicialização de objetos complexos:");

var pessoas = CriarPessoas([
    ("João", 30, ["C#", "Python"]),
    ("Maria", 25, ["JavaScript", "React"]),
    ("Pedro", 35, ["Java", "Spring", "Docker"])
]);

foreach (var pessoa in pessoas)
{
    Console.WriteLine($"  {pessoa.Nome}, {pessoa.Idade} anos, Skills: [{string.Join(", ", pessoa.Skills)}]");
}

Console.WriteLine();

Console.WriteLine("=== Resumo das Vantagens das Collection Expressions ===");
Console.WriteLine("✅ Sintaxe mais limpa e concisa");
Console.WriteLine("✅ Suporte ao spread operator (..)");
Console.WriteLine("✅ Type inference automático");
Console.WriteLine("✅ Funciona com múltiplos tipos de coleção");
Console.WriteLine("✅ Melhor performance em muitos cenários");
Console.WriteLine("✅ Mais legível que inicializadores tradicionais");
Console.WriteLine("✅ Facilita combinação de coleções");

Console.WriteLine("=== Fim da Demonstração ===");

// =================== MÉTODOS AUXILIARES ===================

static Dictionary<string, string> ProcessarConfiguracoes((string Key, string Value)[] configs)
{
    var resultado = new Dictionary<string, string>();
    foreach (var (key, value) in configs)
    {
        resultado[key] = value;
    }
    return resultado;
}

static int ValidarDados(string[] dados)
{
    return dados.Count(d => !string.IsNullOrWhiteSpace(d) && d.Length > 2);
}

static int[] TransformarNumeros(int[] numeros)
{
    return [..numeros.Select(x => x * 2), ..numeros.Select(x => x * 3)];
}

static List<Pessoa> CriarPessoas((string Nome, int Idade, string[] Skills)[] dados)
{
    return dados.Select(d => new Pessoa(d.Nome, d.Idade, [..d.Skills])).ToList();
}

// =================== CLASSES AUXILIARES ===================

public record Pessoa(string Nome, int Idade, List<string> Skills);
