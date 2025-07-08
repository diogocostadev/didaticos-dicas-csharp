# Dica 14: List Patterns no C# 11

## 📋 Descrição

Esta dica explora os poderosos **List Patterns** introduzidos no C# 11, que revolucionam o pattern matching permitindo fazer match em sequências e listas de forma declarativa, incluindo o uso do slice operator (`..`) para capturar sub-sequências.

## 🎯 Objetivo de Aprendizado

Dominar o uso de List Patterns para criar código mais expressivo e legível ao trabalhar com arrays, listas e outras sequências, utilizando patterns avançados como slice patterns, captura de elementos específicos e validação de estruturas de dados.

## ⚡ Sintaxe dos List Patterns

### Padrões Básicos

```csharp
var resultado = array switch
{
    [] => "Array vazio",
    [var unico] => $"Um elemento: {unico}",
    [1, 2, 3] => "Sequência exata 1, 2, 3",
    [var primeiro, var segundo] => $"Dois elementos: {primeiro}, {segundo}",
    _ => "Outro padrão"
};
```

### Slice Patterns com Range Operator (..)

```csharp
var resultado = array switch
{
    [var primeiro, .. var meio, var ultimo] => "Primeiro, meio, último",
    [1, 2, .. var resto] => "Começa com 1, 2",
    [.. var inicio, 9, 10] => "Termina com 9, 10",
    [var cabeca, .. ] => "Pelo menos um elemento",
    _ => "Outro padrão"
};
```

## 🔧 Funcionalidades Principais

### 1. **Pattern Matching Básico**

```csharp
int[] numeros = [1, 2, 3, 4, 5];

var tipo = numeros switch
{
    [] => "Vazio",
    [_] => "Um elemento",
    [_, _] => "Dois elementos", 
    [1, 2, 3, 4, 5] => "Sequência específica",
    _ => "Outros"
};
```

### 2. **Slice Patterns Avançados**

```csharp
string[] palavras = ["Hello", "Beautiful", "World", "C#"];

var analise = palavras switch
{
    ["Hello", .. var meio, "C#"] => $"Inicia Hello, termina C#, meio: {meio.Length}",
    [var primeira, .. var resto] => $"Primeira: {primeira}, resto: {resto.Length}",
    _ => "Outro padrão"
};
```

### 3. **Condições com When**

```csharp
var classificacao = temperaturas switch
{
    [] => "Sem dados",
    [.. var todas] when todas.All(t => t > 30) => "Semana quente",
    [.. var todas] when todas.All(t => t < 10) => "Semana fria",
    [var inicio, .., var fim] when fim > inicio => "Aquecimento",
    _ => "Estável"
};
```

### 4. **Algoritmos Funcionais**

```csharp
static List<int> QuickSort(List<int> lista) => lista switch
{
    [] or [_] => lista,
    [var pivot, .. var resto] => [
        .. QuickSort(resto.Where(x => x < pivot).ToList()),
        pivot,
        .. QuickSort(resto.Where(x => x >= pivot).ToList())
    ]
};
```

## 📊 Demonstrações Incluídas

1. **Patterns Básicos** - Matching simples e captura de elementos
2. **Slice Patterns** - Uso avançado do range operator (..)
3. **Condições When** - Combinação com expressões condicionais
4. **Objetos Complexos** - Pattern matching com records e classes
5. **Algoritmos** - Quick Sort implementado com list patterns
6. **Validação** - Validação de logs HTTP e comandos
7. **Processamento** - Sistema de comandos baseado em patterns
8. **Tipos Diversos** - Arrays, Lists, Spans, ReadOnlySpans

## ✅ Vantagens dos List Patterns

- **🎯 Expressividade**: Código declarativo em vez de imperativo
- **📖 Legibilidade**: Intenção clara através de patterns visuais
- **🔧 Flexibilidade**: Slice patterns para sub-sequências variáveis
- **⚡ Performance**: Otimizações do compilador para matching
- **🏗️ Versatilidade**: Funciona com qualquer tipo sequencial
- **🎨 Elegância**: Reduz loops e condicionais complexas
- **🔄 Composição**: Combina perfeitamente com outros patterns

## 🎯 Casos de Uso Comuns

- **Parsing e Validação** de formatos de dados estruturados
- **Processamento de Comandos** em CLIs e APIs
- **Análise de Sequências** em dados científicos ou logs
- **Algoritmos Funcionais** como ordenação e busca
- **State Machines** baseadas em sequências de estados
- **Protocolo Parsing** para comunicação de rede
- **Data Pipeline** para transformação de dados

## 🏗️ Estrutura do Projeto

```bash
Dica14-ListPatterns/
├── Dica14.ListPatterns/
│   ├── Program.cs                    # Demonstração completa
│   └── Dica14.ListPatterns.csproj
└── README.md                         # Esta documentação
```

## 🚀 Como Executar

```bash
cd "Dica14-ListPatterns/Dica14.ListPatterns"
dotnet run
```

## 🔍 Pontos de Aprendizado

1. **Sintaxe Base**: Use `[patterns]` para fazer match em sequências
2. **Slice Operator**: Use `..` para capturar sub-sequências variáveis
3. **Captura**: Use `var nome` para capturar elementos ou sub-arrays
4. **Underscore**: Use `_` para ignorar elementos específicos
5. **Condições**: Combine com `when` para lógica adicional
6. **Types**: Funciona com arrays, listas, spans e qualquer tipo sequencial

## 💡 Dicas Importantes

- List patterns são avaliados em ordem - padrões mais específicos primeiro
- Slice patterns (`..`) podem aparecer apenas uma vez por pattern
- Funciona com qualquer tipo que implemente indexação e contagem
- Performance é otimizada - evita enumeração desnecessária
- Pode ser combinado com outros tipos de pattern matching
- Especialmente útil para parsing e validação de estruturas

## 🎓 Conceitos Relacionados

- **Pattern Matching**: Evolução do pattern matching do C#
- **Destructuring**: Decomposição de estruturas de dados
- **Functional Programming**: Paradigmas funcionais em C#
- **Slice Operator**: Uso do range operator (..) 
- **Switch Expressions**: Expressões switch modernas
- **Sequence Processing**: Processamento declarativo de sequências

## 📈 Impacto na Qualidade do Código

- **Menos Bugs**: Menos loops manuais significa menos erros
- **Manutenibilidade**: Código autodocumentado e declarativo  
- **Performance**: Otimizações automáticas do compilador
- **Testabilidade**: Patterns claros facilitam testes unitários
- **Legibilidade**: Intenção óbvia através da estrutura visual
