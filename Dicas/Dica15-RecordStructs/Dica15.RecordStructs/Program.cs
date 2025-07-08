using System.Diagnostics;

Console.WriteLine("=== Dica 15: Record Structs no C# 10 ===");

// 1. DEMONSTRAÇÃO: Record Structs Básicos
Console.WriteLine("1. Record Structs básicos:");

// Record struct simples com sintaxe posicional
var ponto2D = new Ponto2D(10, 20);
var ponto3D = new Ponto3D(10, 20, 30);

Console.WriteLine($"  Ponto 2D: {ponto2D}");
Console.WriteLine($"  Ponto 3D: {ponto3D}");

// 2. DEMONSTRAÇÃO: Imutabilidade e métodos 'with'
Console.WriteLine("\n2. Imutabilidade e expressões 'with':");

var coordenadaOriginal = new Coordenada(45.123, -123.456, DateTime.Now);
var coordenadaNova = coordenadaOriginal with { Latitude = 46.789 };

Console.WriteLine($"  Original: {coordenadaOriginal}");
Console.WriteLine($"  Modificada: {coordenadaNova}");
Console.WriteLine($"  São iguais? {coordenadaOriginal == coordenadaNova}");

// 3. DEMONSTRAÇÃO: Igualdade por valor
Console.WriteLine("\n3. Igualdade por valor:");

var cor1 = new CorRGB(255, 128, 64);
var cor2 = new CorRGB(255, 128, 64);
var cor3 = new CorRGB(128, 255, 64);

Console.WriteLine($"  Cor1: {cor1}");
Console.WriteLine($"  Cor2: {cor2}");
Console.WriteLine($"  Cor3: {cor3}");
Console.WriteLine($"  Cor1 == Cor2: {cor1 == cor2}");
Console.WriteLine($"  Cor1 == Cor3: {cor1 == cor3}");
Console.WriteLine($"  Hash Cor1: {cor1.GetHashCode()}");
Console.WriteLine($"  Hash Cor2: {cor2.GetHashCode()}");

// 4. DEMONSTRAÇÃO: Record Structs vs Structs tradicionais
Console.WriteLine("\n4. Comparação: Record Struct vs Struct tradicional:");

var tempRecord = new TemperaturaRecord(25.5, "Celsius");
var tempStruct = new TemperaturaStruct(25.5, "Celsius");

Console.WriteLine($"  Record: {tempRecord}");
Console.WriteLine($"  Struct: {tempStruct}");

// Testando igualdade
var tempRecord2 = new TemperaturaRecord(25.5, "Celsius");
var tempStruct2 = new TemperaturaStruct(25.5, "Celsius");

Console.WriteLine($"  Records iguais: {tempRecord == tempRecord2}");
Console.WriteLine($"  Structs iguais: {tempStruct.Equals(tempStruct2)}");

// 5. DEMONSTRAÇÃO: Performance comparativa
Console.WriteLine("\n5. Teste de performance:");
TestPerformance();

// 6. DEMONSTRAÇÃO: Record Structs mutáveis
Console.WriteLine("\n6. Record Structs mutáveis:");

var config = new ConfiguracaoMutavel { Host = "localhost", Porta = 8080, Ativo = true };
Console.WriteLine($"  Config inicial: {config}");

config.Porta = 9090;
config.Ativo = false;
Console.WriteLine($"  Config modificada: {config}");

// 7. DEMONSTRAÇÃO: Casos de uso práticos
Console.WriteLine("\n7. Casos de uso práticos:");

// Sistema de coordenadas para jogos
var posicaoJogador = new PosicaoJogo(100.5f, 200.3f, 1);
var proximaPosicao = posicaoJogador with { X = 105.2f, Y = 198.1f };

Console.WriteLine($"  Posição atual: {posicaoJogador}");
Console.WriteLine($"  Próxima posição: {proximaPosicao}");

// Valores financeiros
var moeda = new Moeda(150.75m, "BRL");
var moedaConvertida = moeda with { Valor = moeda.Valor * 0.18m, Codigo = "USD" };

Console.WriteLine($"  Valor original: {moeda}");
Console.WriteLine($"  Valor convertido: {moedaConvertida}");

// 8. DEMONSTRAÇÃO: Desconstrução
Console.WriteLine("\n8. Desconstrução de Record Structs:");

var (r, g, b) = cor1;
Console.WriteLine($"  Cor desconstruída - R: {r}, G: {g}, B: {b}");

var (x, y, nivel) = posicaoJogador;
Console.WriteLine($"  Posição desconstruída - X: {x}, Y: {y}, Nível: {nivel}");

Console.WriteLine("\n=== Resumo das Vantagens dos Record Structs ===");
Console.WriteLine("✅ Sintaxe concisa para value types");
Console.WriteLine("✅ Igualdade por valor automática");
Console.WriteLine("✅ Imutabilidade por padrão");
Console.WriteLine("✅ Métodos 'with' para modificações");
Console.WriteLine("✅ ToString() automático e desconstrução");
Console.WriteLine("✅ Performance de struct com benefícios de record");
Console.WriteLine("✅ Ideal para DTOs, coordenadas, valores simples");

Console.WriteLine("\n=== Fim da Demonstração ===");

static void TestPerformance()
{
    const int iterations = 1_000_000;
    
    // Teste com Record Struct
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
        var temp = new TemperaturaRecord(25.5 + i, "Celsius");
        var nova = temp with { Valor = temp.Valor + 1 };
    }
    sw.Stop();
    var recordTime = sw.ElapsedMilliseconds;
    
    // Teste com Struct tradicional
    sw.Restart();
    for (int i = 0; i < iterations; i++)
    {
        var temp = new TemperaturaStruct(25.5 + i, "Celsius");
        var nova = new TemperaturaStruct(temp.Valor + 1, temp.Unidade);
    }
    sw.Stop();
    var structTime = sw.ElapsedMilliseconds;
    
    Console.WriteLine($"  Record Struct: {recordTime}ms");
    Console.WriteLine($"  Struct tradicional: {structTime}ms");
    
    if (structTime > 0)
    {
        var improvement = ((double)(structTime - recordTime) / structTime) * 100;
        Console.WriteLine($"  Diferença: {improvement:F1}% {'*'}");
    }
    
    Console.WriteLine("  * Negative values indicate traditional struct is faster");
}

// =================== RECORD STRUCTS ===================

// Record struct básico com sintaxe posicional
public readonly record struct Ponto2D(int X, int Y);

// Record struct com propriedades adicionais
public readonly record struct Ponto3D(int X, int Y, int Z)
{
    public double DistanciaOrigem => Math.Sqrt(X * X + Y * Y + Z * Z);
    
    public override string ToString() => $"({X}, {Y}, {Z}) - Distância: {DistanciaOrigem:F2}";
}

// Record struct para coordenadas geográficas
public readonly record struct Coordenada(double Latitude, double Longitude, DateTime Timestamp)
{
    public bool EhValida => Latitude is >= -90 and <= 90 && Longitude is >= -180 and <= 180;
}

// Record struct para cores RGB
public readonly record struct CorRGB(byte R, byte G, byte B)
{
    public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";
    
    public CorRGB Inverter() => new((byte)(255 - R), (byte)(255 - G), (byte)(255 - B));
}

// Record struct para comparação com struct tradicional
public readonly record struct TemperaturaRecord(double Valor, string Unidade);

// Struct tradicional para comparação
public readonly struct TemperaturaStruct
{
    public double Valor { get; }
    public string Unidade { get; }
    
    public TemperaturaStruct(double valor, string unidade)
    {
        Valor = valor;
        Unidade = unidade;
    }
    
    public override string ToString() => $"{Valor}°{Unidade}";
    
    public override bool Equals(object? obj) =>
        obj is TemperaturaStruct other && 
        Valor.Equals(other.Valor) && 
        Unidade == other.Unidade;
    
    public override int GetHashCode() => HashCode.Combine(Valor, Unidade);
}

// Record struct mutável (não recomendado, mas possível)
public record struct ConfiguracaoMutavel
{
    public string Host { get; set; }
    public int Porta { get; set; }
    public bool Ativo { get; set; }
}

// =================== CASOS DE USO PRÁTICOS ===================

// Para sistemas de jogos
public readonly record struct PosicaoJogo(float X, float Y, int Nivel)
{
    public float DistanciaPara(PosicaoJogo outra) =>
        MathF.Sqrt(MathF.Pow(X - outra.X, 2) + MathF.Pow(Y - outra.Y, 2));
}

// Para valores financeiros
public readonly record struct Moeda(decimal Valor, string Codigo)
{
    public Moeda ConverterPara(string novoCodigo, decimal taxa) =>
        new(Valor * taxa, novoCodigo);
        
    public override string ToString() => $"{Valor:F2} {Codigo}";
}

// Para representar intervalos de tempo
public readonly record struct IntervaloTempo(TimeSpan Inicio, TimeSpan Fim)
{
    public TimeSpan Duracao => Fim - Inicio;
    public bool Contem(TimeSpan tempo) => tempo >= Inicio && tempo <= Fim;
}

// Para dados de pixel em imagens
public readonly record struct Pixel(int X, int Y, CorRGB Cor)
{
    public bool EstaNoRetangulo(int largura, int altura) =>
        X >= 0 && X < largura && Y >= 0 && Y < altura;
}
