# 📚 101 Dicas de C# - Repositório Didático

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Dicas](https://img.shields.io/badge/Dicas-101-blue)](./Dicas/)

> **Repositório educacional completo** com 101 dicas práticas de C# para desenvolvedores de todos os níveis. Cada dica é um projeto executável com exemplos, benchmarks e documentação.

## 🎯 Sobre o Projeto

Este repositório contém **101 projetos práticos** demonstrando as melhores práticas, padrões e técnicas de programação em C#. Foi criado para ser uma referência completa para desenvolvedores que desejam:

- 📖 **Aprender** - Conceitos fundamentais a avançados
- ⚡ **Otimizar** - Técnicas de alta performance
- 🛡️ **Evitar erros** - Armadilhas comuns e como evitá-las
- 🚀 **Modernizar** - Recursos do C# 12/13 e .NET 9

## 🏗️ Estrutura do Projeto

```
didaticos-dicas-csharp/
├── DicasCSharp.sln              # Solução principal
├── README.md                    # Este documento
├── QUICKSTART.md               # Guia de início rápido
├── GUIA_BOAS_PRATICAS_CSHARP.md # Guia completo para AI/Devs
└── Dicas/                      # 101 projetos organizados
    ├── Dica01-RetornandoColecoesVazias/
    ├── Dica02-RelancandoExcecoesCorretamente/
    ├── ...
    └── Dica101-EvitandoCatchVazio/
```

## 📋 Índice de Dicas

### 🔴 Críticas (Obrigatórias)

| # | Dica | Descrição |
|---|------|-----------|
| 02 | [Relançando Exceções](./Dicas/Dica02-RelancandoExcecoesCorretamente/) | Use `throw;` para preservar stack trace |
| 03 | [Async/Await Deadlocks](./Dicas/Dica03-TravamentoComAsyncAwait/) | Evite .Result e .Wait() |
| 26 | [Async Best Practices](./Dicas/Dica26-AsyncAwaitBestPractices/) | Async all the way |
| 27 | [Bloqueios Async](./Dicas/Dica27-EvitandoBloqueiosAsyncAwait/) | SemaphoreSlim vs lock |
| 30 | [Monitor Maligno](./Dicas/Dica30-MonitorMaligno/) | Cuidados com reentrância |
| 32 | [HttpClient Correto](./Dicas/Dica32-UsandoHttpClientCorretamente/) | Evite socket exhaustion |
| 37 | [HttpClientFactory](./Dicas/Dica37-UsandoHttpClientFactory/) | Factory pattern para HTTP |
| 101 | [Evitando Catch Vazio](./Dicas/Dica101-EvitandoCatchVazio/) | Nunca engula exceções |

### ⚡ Performance

| # | Dica | Descrição |
|---|------|-----------|
| 01 | [Coleções Vazias](./Dicas/Dica01-RetornandoColecoesVazias/) | Array.Empty<T>() vs new T[] |
| 04 | [LINQ Performance](./Dicas/Dica04-ArmadilhasDesempenhoLINQ/) | Evite múltiplas enumerações |
| 06 | [Span de Lista](./Dicas/Dica06-AcessandoSpanDeLista/) | CollectionsMarshal.AsSpan() |
| 09 | [ToList vs ToArray](./Dicas/Dica09-ToListVsToArray/) | Quando usar cada um |
| 25 | [String Performance](./Dicas/Dica25-StringPerformance/) | StringBuilder vs interpolation |
| 40 | [Memory e Span](./Dicas/Dica40-MemoryESpan/) | Zero-allocation patterns |
| 48 | [Stackalloc](./Dicas/Dica48-UsandoStackalloc/) | Alocação na stack |
| 51 | [ArrayPool](./Dicas/Dica51-ArrayPoolReutilizacao/) | Reutilização de arrays |
| 100 | [Compiled Regex](./Dicas/Dica100-CompiledRegex/) | GeneratedRegex |

### 🔧 Boas Práticas

| # | Dica | Descrição |
|---|------|-----------|
| 07 | [Logging Correto](./Dicas/Dica07-LoggingCorreto/) | Message templates |
| 15 | [CancellationTokens](./Dicas/Dica15-CancellationTokens/) | Propagação correta |
| 23 | [DateTimeOffset](./Dicas/Dica23-DateTimeOffsetVsDateTime/) | Timezone safety |
| 24 | [Nullable Types](./Dicas/Dica24-NullableReferenceTypes/) | Reference types anuláveis |
| 35 | [ConfigureAwait](./Dicas/Dica35-ConfigureAwaitFalse/) | False em bibliotecas |
| 43 | [Polly](./Dicas/Dica43-Polly/) | Resiliência HTTP |
| 44 | [MediatR](./Dicas/Dica44-MediatR/) | CQRS Pattern |
| 54 | [EF Performance](./Dicas/Dica54-EntityFrameworkPerformance/) | N+1 e AsNoTracking |

### 🆕 C# 12/13 Features

| # | Dica | Descrição |
|---|------|-----------|
| 12 | [Primary Constructors](./Dicas/Dica12-PrimaryConstructors/) | Construtores simplificados |
| 16 | [Collection Init](./Dicas/Dica16-InicializadoresColecoesC12/) | Sintaxe [] |
| 22 | [Type Aliases](./Dicas/Dica22-AliasParaQualquerTipo/) | using para qualquer tipo |
| 29 | [Params Span](./Dicas/Dica29-ParamsComTiposEnumerable/) | params com ReadOnlySpan |
| 58 | [Records](./Dicas/Dica58-UsingRecordTypes/) | Tipos imutáveis |

### 🏗️ Arquitetura

| # | Dica | Descrição |
|---|------|-----------|
| 55 | [SignalR](./Dicas/Dica55-SignalR/) | Real-time communications |
| 56 | [gRPC](./Dicas/Dica56-gRPC/) | Comunicação eficiente |
| 61 | [DI Lifetimes](./Dicas/Dica61-DependencyInjection/) | Scopes e ciclo de vida |
| 80 | [Clean Architecture](./Dicas/Dica80-CleanArchitecture/) | Organização de projetos |

[📖 **Ver todas as 101 dicas no GUIA_BOAS_PRATICAS_CSHARP.md**](./GUIA_BOAS_PRATICAS_CSHARP.md)

## 🚀 Como Usar

### Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 ou VS Code

### Executando uma Dica

```bash
# Clone o repositório
git clone https://github.com/diogocostadev/didaticos-dicas-csharp.git
cd didaticos-dicas-csharp

# Execute uma dica específica
cd Dicas/Dica32-UsandoHttpClientCorretamente/Dica32.UsandoHttpClientCorretamente
dotnet run

# Execute os benchmarks (quando disponível)
cd Dicas/Dica01-RetornandoColecoesVazias/Dica01.Benchmark
dotnet run -c Release
```

### Compilando Tudo

```bash
# Na raiz do projeto
dotnet build DicasCSharp.sln

# Em modo Release
dotnet build DicasCSharp.sln -c Release
```

## 📊 Benchmarks

Muitas dicas incluem benchmarks com BenchmarkDotNet demonstrando diferenças de performance:

```
|                    Method |      Mean |  Allocated |
|-------------------------- |----------:|-----------:|
| ReturnNewEmptyArray       | 29.442 ns |      24 B  |
| ReturnArrayEmpty          |  0.582 ns |       0 B  | ✅ 50x mais rápido!
```

## 🛠️ Tecnologias

- **.NET 9.0** - Framework mais recente
- **C# 13** - Recursos modernos
- **BenchmarkDotNet** - Medição de performance
- **Top-level statements** - Código limpo
- **Span<T> / Memory<T>** - Alta performance

## 📖 Documentação

| Arquivo | Descrição |
|---------|-----------|
| [README.md](./README.md) | Este documento |
| [QUICKSTART.md](./QUICKSTART.md) | Guia rápido de início |
| [GUIA_BOAS_PRATICAS_CSHARP.md](./GUIA_BOAS_PRATICAS_CSHARP.md) | Guia completo com todas as 101 dicas, exemplos de código e checklist |

## 🤝 Contribuindo

Contribuições são bem-vindas! Siga o padrão:

1. Fork o repositório
2. Crie uma branch (`git checkout -b feature/nova-dica`)
3. Implemente seguindo o padrão existente
4. Commit (`git commit -m 'Adiciona Dica XX'`)
5. Push (`git push origin feature/nova-dica`)
6. Abra um Pull Request

### Padrão de Projeto

```
Dica##-NomeDaDica/
├── Dica##.NomeDaDica/
│   ├── Dica##.NomeDaDica.csproj
│   ├── Program.cs
│   └── README.md
└── Dica##.NomeDaDica.Benchmark/ (opcional)
    └── ...
```

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

## 👨‍💻 Autor

**Diogo Costa** - [@diogocostadev](https://github.com/diogocostadev)

---

⭐ **Dê uma estrela** se este repositório foi útil!

🐛 **Encontrou um problema?** [Abra uma issue](../../issues)

💡 **Tem uma sugestão?** [Inicie uma discussão](../../discussions)
