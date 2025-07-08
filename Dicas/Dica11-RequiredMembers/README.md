# Dica 11: Required Members

## 📋 Visão Geral

Esta dica demonstra o uso de **Required Members**, uma funcionalidade introduzida no **C# 11** que permite definir propriedades que devem ser obrigatoriamente inicializadas durante a criação de um objeto.

## 🎯 Objetivo

Mostrar como Required Members oferece uma alternativa mais flexível aos construtores tradicionais, mantendo a garantia de inicialização em tempo de compilação.

## ✨ Características dos Required Members

### 1. **Palavra-chave `required`**
```csharp
public class Usuario
{
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required DateTime DataNascimento { get; init; }
}
```

### 2. **Garantia de Compilação**
```csharp
// ✅ Correto - todos os required members especificados
var usuario = new Usuario
{
    Nome = "João Silva",
    Email = "joao@email.com",
    DataNascimento = new DateTime(1990, 5, 15)
};

// ❌ Erro de compilação - Nome não especificado
var usuarioIncompleto = new Usuario
{
    Email = "joao@email.com",
    DataNascimento = new DateTime(1990, 5, 15)
    // ERRO: Required member 'Usuario.Nome' must be set
};
```

### 3. **Herança**
```csharp
public class Funcionario : Usuario
{
    public required string Cargo { get; init; }
    public required decimal Salario { get; init; }
    // Herda required members da classe base
}
```

### 4. **Atributo `[SetsRequiredMembers]`**
```csharp
public class ContaBancaria
{
    public required string Numero { get; init; }
    public required string Titular { get; init; }
    
    [SetsRequiredMembers]
    public ContaBancaria(string numero, string titular)
    {
        Numero = numero;
        Titular = titular;
    }
}
```

## 🔍 Funcionalidades Demonstradas

### 1. **Criação Básica**
- Object initializers com required members
- Erro de compilação para membros não inicializados
- Propriedades opcionais vs obrigatórias

### 2. **Herança**
- Required members em classes derivadas
- Herança de required members da classe base
- Polimorfismo com interfaces

### 3. **Records**
- Required members em record types
- Compatibilidade com value equality
- Expressões `with` para cópia

### 4. **Construtores**
- Atributo `[SetsRequiredMembers]`
- Construtor parameterless vs construtor com parâmetros
- Flexibilidade de inicialização

### 5. **Serialização JSON**
- System.Text.Json com required members
- Tratamento de required members ausentes
- Deserialização segura

### 6. **Validação**
- Validação customizada em propriedades required
- Métodos factory para criação segura
- Tratamento de erros

## 📊 Benchmarks de Performance

### Cenários Testados

1. **Criação de Objetos**
   - Required members vs construtores tradicionais
   - Object initializers vs construtores

2. **Herança**
   - Performance com classes derivadas
   - Multiple required members

3. **Volume de Dados**
   - Criação de múltiplos objetos
   - Impacto na memória

4. **Validação**
   - Required members com validação
   - Factory methods vs initializers

### Resultados Esperados

- Performance similar entre required members e construtores
- Pequeno overhead para object initializers complexos
- Benefícios claros em flexibilidade e manutenibilidade

## 🎨 Padrões de Uso

### ✅ Quando Usar Required Members

1. **APIs Públicas**: Garantir inicialização de propriedades críticas
2. **DTOs/Models**: Propriedades obrigatórias para serialização
3. **Configuration Classes**: Parâmetros de configuração essenciais
4. **Domain Models**: Propriedades de negócio obrigatórias

### ❌ Quando Evitar

1. **Lógica Complexa**: Construtores são melhores para validação complexa
2. **Compatibilidade**: Projetos que precisam suportar C# < 11
3. **Performance Crítica**: Cenários onde cada nanosegundo importa

## 🔄 Comparação com Alternativas

### Required Members vs Construtores

| Aspecto | Required Members | Construtores |
|---------|------------------|--------------|
| Flexibilidade | ✅ Alta | ❌ Limitada |
| Sintaxe | ✅ Limpa | ⚠️ Verbosa |
| Herança | ✅ Natural | ❌ Complexa |
| Validação | ⚠️ Limitada | ✅ Completa |
| Performance | ✅ Similar | ✅ Similar |

### Required Members vs Nullable Reference Types

| Aspecto | Required Members | Nullable Types |
|---------|------------------|----------------|
| Garantia | ✅ Compilação | ⚠️ Runtime |
| Inicialização | ✅ Obrigatória | ❌ Opcional |
| Flexibilidade | ✅ Alta | ✅ Alta |
| Compatibilidade | ⚠️ C# 11+ | ✅ C# 8+ |

## 🚀 Como Executar

### Demonstração Principal
```bash
cd Dica11
dotnet run
```

### Benchmarks de Performance
```bash
cd Dica11.Benchmark
dotnet run -c Release
```

## 📚 Conceitos Relacionados

1. **[Dica 08 - Record Types](../Dica08-UsandoValueTask)**: Records com required members
2. **[Dica 12 - Primary Constructors](../Dica12-PrimaryConstructors)**: Construtores primários vs required members
3. **[Dica 13 - Collection Expressions](../Dica13-CollectionExpressions)**: Inicialização de coleções

## 📖 Referências

- [Microsoft Docs: Required Members](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required)
- [C# 11 Features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11)
- [Object Initializers](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers)

---

> 💡 **Dica**: Required Members oferecem o melhor dos dois mundos - a flexibilidade dos object initializers com a segurança dos construtores obrigatórios!
