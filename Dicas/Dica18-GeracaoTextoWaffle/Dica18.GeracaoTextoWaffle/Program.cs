using WaffleGenerator;
using Bogus;

Console.WriteLine("==== Dica 18: Geração de Texto Waffle (Waffle Generation) ====");
Console.WriteLine("Esta dica demonstra como gerar texto realista e personalizável");
Console.WriteLine("usando WaffleGenerator ao invés do tradicional Lorem Ipsum.\n");

// 1. Problema com Lorem Ipsum
Console.WriteLine("1. Por que Lorem Ipsum não é ideal:");
Console.WriteLine("   ❌ Texto em latim antigo - não é realista");
Console.WriteLine("   ❌ Sempre o mesmo padrão - previsível");
Console.WriteLine("   ❌ Não se adapta ao contexto - genérico");
Console.WriteLine("   ❌ Não ajuda a testar layout real - artificial");
Console.WriteLine();

// 2. Demonstração básica do WaffleGenerator
Console.WriteLine("2. WaffleGenerator - Geração de texto inteligente:");

Console.WriteLine("\n   📄 Texto simples:");
var textoSimples = WaffleEngine.Text(paragraphs: 1, includeHeading: false);
Console.WriteLine($"   {textoSimples}");

Console.WriteLine("\n   📝 Texto com título:");
var textoComTitulo = WaffleEngine.Text(paragraphs: 1, includeHeading: true);
Console.WriteLine($"   {textoComTitulo}");

Console.WriteLine("\n   🔤 Apenas palavras:");
var palavras = WaffleEngine.Text(paragraphs: 1, includeHeading: false);
var primeiras20Palavras = string.Join(" ", palavras.Split(' ').Take(10));
Console.WriteLine($"   {primeiras20Palavras}");

// 3. Formatos diferentes
Console.WriteLine("\n3. Diferentes formatos de saída:");

Console.WriteLine("\n   📋 Markdown:");
var markdownContent = WaffleEngine.Markdown(paragraphs: 2, includeHeading: true);
Console.WriteLine($"   {markdownContent}");

// 4. Integração com Bogus
Console.WriteLine("\n4. Integração com Bogus para dados realistas:");

// Criando faker com WaffleGenerator
var faker = new Faker<Artigo>("pt_BR")
    .RuleFor(a => a.Titulo, f => f.Company.CatchPhrase())
    .RuleFor(a => a.Autor, f => f.Person.FullName)
    .RuleFor(a => a.Conteudo, f => WaffleEngine.Text(paragraphs: 3, includeHeading: false))
    .RuleFor(a => a.DataPublicacao, f => f.Date.Recent(days: 30))
    .RuleFor(a => a.Tags, f => new[] { f.Hacker.Noun(), f.Hacker.Noun(), f.Hacker.Noun() })
    .RuleFor(a => a.Resumo, f => WaffleEngine.Text(paragraphs: 1, includeHeading: false));

var artigos = faker.Generate(3);

foreach (var (artigo, index) in artigos.Select((a, i) => (a, i + 1)))
{
    Console.WriteLine($"\n   📰 Artigo {index}:");
    Console.WriteLine($"      Título: {artigo.Titulo}");
    Console.WriteLine($"      Autor: {artigo.Autor}");
    Console.WriteLine($"      Data: {artigo.DataPublicacao:dd/MM/yyyy}");
    Console.WriteLine($"      Tags: [{string.Join(", ", artigo.Tags)}]");
    Console.WriteLine($"      Resumo: {artigo.Resumo[..Math.Min(100, artigo.Resumo.Length)]}...");
}

// 5. Casos de uso práticos
Console.WriteLine("\n5. Casos de uso práticos:");

// a) Mockup de blog
Console.WriteLine("\n   📝 Mockup de Blog:");
var postsBlog = new Faker<object>("pt_BR")
    .CustomInstantiator(f => new {
        Id = f.Random.Int(1, 1000),
        Titulo = f.Lorem.Sentence(3, 6),
        ConteudoWaffle = WaffleEngine.Text(paragraphs: 2, includeHeading: false),
        Visualizacoes = f.Random.Int(10, 10000),
        DataCriacao = f.Date.Recent(days: 90)
    })
    .Generate(2);

foreach (var post in postsBlog)
{
    Console.WriteLine($"   • {post}");
}

// b) Conteúdo para e-commerce
Console.WriteLine("\n   🛒 Descrições de Produtos:");
var produtosFaker = new Faker<object>("pt_BR")
    .CustomInstantiator(f => new {
        Nome = f.Commerce.ProductName(),
        Descricao = WaffleEngine.Text(paragraphs: 1, includeHeading: false),
        Preco = f.Commerce.Price(10, 1000, 2, "R$ "),
        Categoria = f.Commerce.Categories(1).First()
    })
    .Generate(2);

foreach (var produto in produtosFaker)
{
    Console.WriteLine($"   • {produto}");
}

// 6. Configurações avançadas
Console.WriteLine("\n6. Configurações avançadas do WaffleGenerator:");

// Texto técnico/corporativo
var textoTecnico = WaffleEngine.Text(paragraphs: 1, includeHeading: true);
Console.WriteLine($"\n   💼 Texto corporativo:\n   {textoTecnico}");

// Controle de tamanho
var textoControlado = WaffleEngine.Text(paragraphs: 1, includeHeading: false);
var primeiras25Palavras = string.Join(" ", textoControlado.Split(' ').Take(25));
Console.WriteLine($"\n   📏 Texto controlado (25 palavras):\n   {primeiras25Palavras}");

// 7. Comparação com Lorem Ipsum
Console.WriteLine("\n7. Comparação Lorem Ipsum vs WaffleGenerator:");

Console.WriteLine("\n   ❌ Lorem Ipsum tradicional:");
Console.WriteLine("   Lorem ipsum dolor sit amet, consectetur adipiscing elit...");

Console.WriteLine("\n   ✅ WaffleGenerator realista:");
var textoRealista = WaffleEngine.Text(paragraphs: 1, includeHeading: false);
Console.WriteLine($"   {textoRealista}");

Console.WriteLine("\n=== Resumo dos Benefícios ===");
Console.WriteLine("✅ Texto realista em inglês comercial");
Console.WriteLine("✅ Múltiplos formatos (texto, HTML, Markdown)");
Console.WriteLine("✅ Integração perfeita com Bogus");
Console.WriteLine("✅ Controle de tamanho e estrutura");
Console.WriteLine("✅ Ideal para protótipos e testes");
Console.WriteLine("✅ Contexto mais apropriado que Lorem Ipsum");
Console.WriteLine("✅ Ajuda a identificar problemas de layout reais");

// Definindo classe de exemplo para demonstração
public class Artigo
{
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataPublicacao { get; set; }
    public string[] Tags { get; set; } = [];
    public string Resumo { get; set; } = string.Empty;
}
