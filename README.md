# 📚 100 Dicas de C# - Implementação Prática

Este repositório contém a implementação prática das 100 dicas de C# mais importantes para desenvolvedores. Cada dica é implementada como um projeto separado com exemplos práticos, benchmarks de performance e documentação detalhada.

## 🎯 Objetivo

Demonstrar as melhores práticas de C# através de código executável, facilitando o aprendizado e a aplicação prática das técnicas mais importantes da linguagem.

## 🏗️ Estrutura do Projeto

```
DicasCSharp/
├── DicasCSharp.sln                 # Solução principal
├── README.md                       # Este documento
├── Dicas/                          # Pasta principal das dicas
│   ├── Dica01-RetornandoColecoesVazias/
│   │   ├── Dica01/                 # Projeto principal
│   │   └── Dica01.Benchmark/       # Benchmark de performance
│   ├── Dica02-RelancandoExcecoesCorretamente/
│   │   └── Dica02/                 # Projeto principal
│   ├── Dica03-TravamentoComAsyncAwait/
│   │   └── Dica03/                 # Projeto principal
│   └── ...
└── Scripts/                        # Scripts de automação
```

## ✅ Dicas Implementadas (16/100)

### ✨ **Últimas Implementações (Esta Sessão)**
- **Dica 15** - CancellationTokens (ASP.NET Core Web API)
- **Dica 23** - DateTimeOffset vs DateTime (Timezone Safety)  
- **Dica 42** - Atribuição Condicional Nula (??=)
- **Dica 62** - Operador nameof
- **Dica 76** - Exceções para Casos Excepcionais
- **Dica 82** - nameof vs Reflexão (**20x+ mais rápido**)
- **Dica 99** - Method Inlining para Hot Paths

### 🚀 Performance & Otimização

| Dica | Nome | Projetos | Benchmark | Descrição |
|------|------|----------|-----------|-----------|
| **01** | [Retornando Coleções Vazias](./Dicas/Dica01-RetornandoColecoesVazias/) | 2 | ✅ | Use `Array.Empty<T>()` em vez de `new T[]` |
| **04** | [Armadilhas de Desempenho LINQ](./Dicas/Dica04-ArmadilhasDesempenhoLINQ/) | 2 | ✅ | Evite múltiplas enumerações com ToList/ToArray |
| **06** | [Acessando Span de Lista](./Dicas/Dica06-AcessandoSpanDeLista/) | 2 | ✅ | Use `CollectionsMarshal.AsSpan()` para performance |
| **09** | [ToList vs ToArray](./Dicas/Dica09-ToListVsToArray/) | 2 | ✅ | Escolha correta entre List e Array |
| **48** | [Usando stackalloc](./Dicas/Dica48-UsandoStackalloc/) | 2 | ✅ | Aloque na stack para performance crítica |
| **51** | [ArrayPool Reutilização](./Dicas/Dica51-ArrayPoolReutilizacao/) | 2 | ✅ | Reutilize arrays com ArrayPool.Shared |
| **73** | [ValueTask vs Task](./Dicas/Dica73-ValueTaskVsTask/) | 2 | ✅ | Use ValueTask para otimizar operações síncronas |
| **82** | [nameof vs Reflexão](./Dicas/Dica82-NameofVsReflexao/) | 2 | ✅ | **20x+ mais rápido** - compile-time vs runtime |
| **99** | [Method Inlining](./Dicas/Dica99-MethodInlining/) | 2 | ✅ | AggressiveInlining para hot paths críticos |

### 🔧 Boas Práticas & APIs

| Dica | Nome | Projetos | Benchmark | Descrição |
|------|------|----------|-----------|-----------|
| **02** | [Relançando Exceções Corretamente](./Dicas/Dica02-RelancandoExcecoesCorretamente/) | 1 | ✅ | Use `throw;` em vez de `throw ex;` |
| **03** | [Travamento com Async/Await](./Dicas/Dica03-TravamentoComAsyncAwait/) | 1 | ✅ | Use `SemaphoreSlim` para locking assíncrono |
| **15** | [CancellationTokens em APIs](./Dicas/Dica15-CancellationTokens/) | 1 | ✅ | Use tokens fornecidos pelo ASP.NET Core |
| **23** | [DateTimeOffset vs DateTime](./Dicas/Dica23-DateTimeOffsetVsDateTime/) | 1 | ✅ | Timezone safety em aplicações globais |
| **25** | [String Performance](./Dicas/Dica25-StringPerformance/) | 2 | ✅ | Interpolação vs StringBuilder vs Concat (180x+ diferença) |
| **42** | [Atribuição Condicional Nula](./Dicas/Dica42-NullConditionalAssignment/) | 1 | ✅ | Operador ??= para lazy initialization |
| **62** | [nameof para Símbolos](./Dicas/Dica62-NameofOperator/) | 1 | ✅ | Use nameof() em vez de strings hard-coded |
| **76** | [Exceções para Casos Excepcionais](./Dicas/Dica76-ExceptionsForExceptionalCases/) | 1 | ✅ | Result Pattern vs exceções (914x+ rápido) |

### ⚡ Async/Await Avançado

| Dica | Nome | Projetos | Benchmark | Descrição |
|------|------|----------|-----------|-----------|
| **27** | [Evitando Bloqueios Async/Await](./Dicas/Dica27-EvitandoBloqueiosAsyncAwait/) | 2 | ✅ | Patterns para evitar deadlock e maximizar paralelismo |

## 🛠️ Tecnologias Utilizadas

- **.NET 9.0** - Framework mais recente
- **C# 13** - Recursos modernos da linguagem
- **BenchmarkDotNet** - Medição precisa de performance
- **Top-level statements** - Sintaxe moderna e concisa
- **Span<T> e Memory<T>** - APIs de alta performance
- **async/await** - Programação assíncrona moderna

## 🚀 Como Executar

### Pré-requisitos
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio Code ou Visual Studio 2022

### Executando uma Dica
```bash
# Executar demonstração de uma dica específica
cd Dicas/Dica01-RetornandoColecoesVazias/Dica01
dotnet run

# Executar benchmark de performance
cd Dicas/Dica01-RetornandoColecoesVazias/Dica01.Benchmark
dotnet run -c Release
```

### Compilando Tudo
```bash
# Na raiz do projeto
dotnet build

# Ou para Release
dotnet build -c Release
```

## 📊 Benchmarks

Cada dica relacionada a performance possui benchmarks abrangentes que demonstram:

- **Tempo de execução** - Medição precisa em nanosegundos
- **Alocação de memória** - Bytes alocados e coletas de lixo
- **Comparações** - Múltiplas abordagens lado a lado
- **Cenários reais** - Casos de uso práticos

### Exemplo de Resultado
```
|                    Method |      Mean |     Error |    StdDev |    Median | Ratio | Gen0 | Allocated |
|-------------------------- |----------:|----------:|----------:|----------:|------:|-----:|----------:|
|   ReturnEmptyArrayOld     | 29.442 ns | 0.6929 ns | 1.9594 ns | 28.890 ns |  1.00 | 0.01 |      24 B |
|   ReturnEmptyArrayNew     |  0.582 ns | 0.0190 ns | 0.0159 ns |  0.584 ns |  0.02 | -    |       - B |
```

## 🎓 Conceitos Abordados

### Performance & Otimização
- **Zero-allocation patterns** (Array.Empty, stackalloc)
- **Memory-efficient processing** (Span<T>, Memory<T>)
- **LINQ optimization** (materialization strategies)
- **Collection performance** (List vs Array characteristics)

### Async/Await Patterns
- **Deadlock prevention** (ConfigureAwait, async all the way)
- **Parallel processing** (Task.WhenAll, batching)
- **Async locking** (SemaphoreSlim vs lock keyword)
- **CPU-bound async** (Task.Run best practices)

### Exception Handling
- **Stack trace preservation** (proper rethrowing)
- **Exception context** (inner exceptions, data)
- **Performance considerations** (exception costs)

### Modern C# Features
- **Top-level statements** (reduced boilerplate)
- **Pattern matching** (expression patterns)
- **Record types** (immutable data)
- **Init-only properties** (object initialization)

## 📈 Roadmap

### Próximas Implementações (Priority High)
- **Dica 15**: Sealed classes performance
- **Dica 42**: String interning strategies
- **Dica 82**: nameof vs reflexão
- **Dica 99**: Method inlining patterns

### Categorias Futuras
- **Memory Management** (10 dicas)
- **LINQ Avançado** (15 dicas)
- **Async Patterns** (12 dicas)
- **Performance Crítica** (20 dicas)
- **Design Patterns** (18 dicas)
- **Testing & Quality** (8 dicas)

## 🤝 Contribuindo

1. **Fork** o repositório
2. **Clone** localmente
3. **Implemente** uma nova dica seguindo o padrão:
   - Projeto principal com demonstrações
   - Benchmark quando aplicável
   - Documentação detalhada
   - Testes quando necessário

### Padrão de Implementação
```csharp
// Projeto principal: demonstração educativa
Console.WriteLine("=== Dica XX: Título ===");
var demo = new DemoClass();
await demo.ShowGoodPractices();
await demo.ShowBadPractices();

// Benchmark: medição de performance
[MemoryDiagnoser]
public class XxxBenchmark
{
    [Benchmark(Baseline = true)]
    public void OldApproach() { ... }
    
    [Benchmark]
    public void NewApproach() { ... }
}
```

## 📚 Recursos Adicionais

- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [.NET Performance Tips](https://docs.microsoft.com/en-us/dotnet/framework/performance/)
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [High-Performance .NET](https://www.manning.com/books/high-performance-dotnet)

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE) - veja o arquivo LICENSE para detalhes.

---

⭐ **Star este repositório** se ele foi útil para você!

🐛 **Encontrou um bug?** Abra uma [issue](../../issues).

💡 **Tem uma sugestão?** Abra uma [discussão](../../discussions).

## 🏆 Status Atual

✅ **9 dicas implementadas** com projetos funcionais  
✅ **18 projetos** na solução (9 principais + 9 benchmarks)  
✅ **7 benchmarks** de performance funcionando  
✅ **Compilação 100% bem-sucedida**  

**Próximo passo**: Continuar implementando as dicas mais importantes, priorizando performance e boas práticas modernas do C#.
