# Dica 29: Implementação e Cronograma

## 🗓️ Status da Implementação

### ⚠️ C# 13 - Em Desenvolvimento

Esta dica demonstra um recurso **futuro** do C# 13 que ainda está em desenvolvimento:

- **Status atual**: Preview/Experimental
- **Lançamento previsto**: Final de 2024 / Início de 2025
- **Disponibilidade**: .NET 9 quando lançado

### 🔧 Como Testar Hoje

Para experimentar os recursos do C# 13:

```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
```

**Requisitos:**
- Visual Studio 2024 Preview
- .NET 9 Preview SDK
- Ou usar try.dot.net online

## 💡 Alternativas Atuais (C# 8-12)

Enquanto aguardamos o C# 13, você pode usar:

### 1. ReadOnlySpan como Parâmetro Normal
```csharp
public static void Process(ReadOnlySpan<int> data)
{
    foreach (var item in data)
        Console.WriteLine(item);
}

// Uso:
Process(stackalloc int[] { 1, 2, 3 });
Process(new int[] { 1, 2, 3 });
```

### 2. Overloads para Flexibilidade
```csharp
public static void Process(params int[] array) => Process(array.AsSpan());
public static void Process(ReadOnlySpan<int> span) { /* implementação */ }
```

### 3. Extension Methods
```csharp
public static class SpanExtensions
{
    public static void Process(this ReadOnlySpan<int> span)
    {
        foreach (var item in span)
            Console.WriteLine(item);
    }
}

// Uso:
stackalloc int[] { 1, 2, 3 }.Process();
```

## 🎯 Preparando-se para C# 13

### Design APIs Futuro-Compatíveis
```csharp
// Design atual que será fácil de migrar
public static class MathUtils
{
    // Versão atual
    public static double Average(params double[] numbers)
        => Average(numbers.AsSpan());
    
    // Implementação principal (pronta para C# 13)
    private static double Average(ReadOnlySpan<double> numbers)
    {
        if (numbers.IsEmpty) return 0;
        
        double sum = 0;
        foreach (var num in numbers)
            sum += num;
        return sum / numbers.Length;
    }
}
```

## 🚀 Migração Futura

Quando C# 13 estiver disponível:

```csharp
// Apenas mude a assinatura
public static double Average(params ReadOnlySpan<double> numbers)
{
    // Implementação permanece igual
    if (numbers.IsEmpty) return 0;
    
    double sum = 0;
    foreach (var num in numbers)
        sum += num;
    return sum / numbers.Length;
}
```

## 📚 Recursos de Aprendizado

- [C# 13 Preview Features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13)
- [Span<T> Performance Guide](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)
- [try.dot.net - Experimente online](https://try.dot.net)
