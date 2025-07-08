using Dica11;

namespace Dica11.Benchmark;

/// <summary>
/// Benchmarks comparando performance entre Required Members e construtores tradicionais.
/// 
/// Estes benchmarks demonstram que:
/// 1. Required Members têm performance similar aos construtores tradicionais
/// 2. Object initializers podem ter pequeno overhead vs construtores
/// 3. A diferença é geralmente desprezível para casos práticos
/// 4. A escolha deve ser baseada em design, não em performance
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class RequiredMembersBenchmarks
{
    private readonly Random _random = new(42);
    private readonly string[] _nomes = { "João", "Maria", "Pedro", "Ana", "Carlos", "Beatriz" };
    private readonly string[] _sobrenomes = { "Silva", "Santos", "Oliveira", "Costa", "Ferreira", "Alves" };
    private readonly string[] _cargos = { "Desenvolvedor", "Analista", "Gerente", "Coordenador" };
    private readonly string[] _departamentos = { "TI", "RH", "Financeiro", "Marketing" };

    [GlobalSetup]
    public void Setup()
    {
        // Aquecimento do JIT
        for (int i = 0; i < 100; i++)
        {
            CriarUsuarioRequiredMembers();
            CriarUsuarioTradicional();
        }
    }

    /// <summary>
    /// Benchmark: Criação de usuário usando Required Members.
    /// </summary>
    [Benchmark(Baseline = true)]
    public Usuario CriarUsuarioRequiredMembers()
    {
        var nome = _nomes[_random.Next(_nomes.Length)];
        var sobrenome = _sobrenomes[_random.Next(_sobrenomes.Length)];
        
        return new Usuario
        {
            Nome = $"{nome} {sobrenome}",
            Email = $"{nome.ToLower()}.{sobrenome.ToLower()}@email.com",
            DataNascimento = DateTime.Now.AddYears(-_random.Next(18, 65)),
            Telefone = $"(11) {_random.Next(10000, 99999)}-{_random.Next(1000, 9999)}"
        };
    }

    /// <summary>
    /// Benchmark: Criação de usuário usando construtor tradicional.
    /// </summary>
    [Benchmark]
    public UsuarioTradicional CriarUsuarioTradicional()
    {
        var nome = _nomes[_random.Next(_nomes.Length)];
        var sobrenome = _sobrenomes[_random.Next(_sobrenomes.Length)];
        
        return new UsuarioTradicional(
            $"{nome} {sobrenome}",
            $"{nome.ToLower()}.{sobrenome.ToLower()}@email.com",
            DateTime.Now.AddYears(-_random.Next(18, 65)))
        {
            Telefone = $"(11) {_random.Next(10000, 99999)}-{_random.Next(1000, 9999)}"
        };
    }

    /// <summary>
    /// Benchmark: Criação de funcionário com herança e Required Members.
    /// </summary>
    [Benchmark]
    public Funcionario CriarFuncionarioRequiredMembers()
    {
        var nome = _nomes[_random.Next(_nomes.Length)];
        var sobrenome = _sobrenomes[_random.Next(_sobrenomes.Length)];
        
        return new Funcionario
        {
            Nome = $"{nome} {sobrenome}",
            Email = $"{nome.ToLower()}.{sobrenome.ToLower()}@empresa.com",
            DataNascimento = DateTime.Now.AddYears(-_random.Next(22, 60)),
            Cargo = _cargos[_random.Next(_cargos.Length)],
            Salario = _random.Next(3000, 15000),
            Departamento = _departamentos[_random.Next(_departamentos.Length)]
        };
    }

    /// <summary>
    /// Benchmark: Criação de produto usando Record com Required Members.
    /// </summary>
    [Benchmark]
    public Produto CriarProdutoRecord()
    {
        return new Produto
        {
            Nome = $"Produto {_random.Next(1000, 9999)}",
            Preco = (decimal)(_random.NextDouble() * 1000),
            Categoria = _departamentos[_random.Next(_departamentos.Length)],
            Descricao = "Descrição do produto"
        };
    }

    /// <summary>
    /// Benchmark: Criação de conta bancária usando construtor com [SetsRequiredMembers].
    /// </summary>
    [Benchmark]
    public ContaBancaria CriarContaPorConstrutor()
    {
        return new ContaBancaria(
            $"{_random.Next(10000, 99999)}-{_random.Next(0, 9)}",
            $"{_nomes[_random.Next(_nomes.Length)]} {_sobrenomes[_random.Next(_sobrenomes.Length)]}",
            $"{_random.Next(1000, 9999)}")
        {
            Saldo = (decimal)(_random.NextDouble() * 10000)
        };
    }

    /// <summary>
    /// Benchmark: Criação de conta bancária usando object initializer.
    /// </summary>
    [Benchmark]
    public ContaBancaria CriarContaPorInitializer()
    {
        return new ContaBancaria
        {
            Numero = $"{_random.Next(10000, 99999)}-{_random.Next(0, 9)}",
            Titular = $"{_nomes[_random.Next(_nomes.Length)]} {_sobrenomes[_random.Next(_sobrenomes.Length)]}",
            Agencia = $"{_random.Next(1000, 9999)}",
            Saldo = (decimal)(_random.NextDouble() * 10000)
        };
    }

    /// <summary>
    /// Benchmark: Criação de múltiplos objetos - Required Members.
    /// </summary>
    [Benchmark]
    [Arguments(1000)]
    public List<Usuario> CriarMultiplosUsuariosRequiredMembers(int quantidade)
    {
        var usuarios = new List<Usuario>(quantidade);
        
        for (int i = 0; i < quantidade; i++)
        {
            usuarios.Add(new Usuario
            {
                Nome = $"Usuário {i}",
                Email = $"usuario{i}@email.com",
                DataNascimento = DateTime.Now.AddYears(-_random.Next(18, 65))
            });
        }
        
        return usuarios;
    }

    /// <summary>
    /// Benchmark: Criação de múltiplos objetos - Construtor tradicional.
    /// </summary>
    [Benchmark]
    [Arguments(1000)]
    public List<UsuarioTradicional> CriarMultiplosUsuariosTradicionais(int quantidade)
    {
        var usuarios = new List<UsuarioTradicional>(quantidade);
        
        for (int i = 0; i < quantidade; i++)
        {
            usuarios.Add(new UsuarioTradicional(
                $"Usuário {i}",
                $"usuario{i}@email.com",
                DateTime.Now.AddYears(-_random.Next(18, 65))));
        }
        
        return usuarios;
    }

    /// <summary>
    /// Benchmark: Validação com Required Members customizados.
    /// </summary>
    [Benchmark]
    public Endereco CriarEnderecoComValidacao()
    {
        return new Endereco
        {
            Rua = $"Rua {_random.Next(1, 999)}",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = $"{_random.Next(10000, 99999):00000}-{_random.Next(100, 999):000}",
            Numero = _random.Next(1, 9999).ToString()
        };
    }

    /// <summary>
    /// Benchmark: Criação usando factory method.
    /// </summary>
    [Benchmark]
    public ConfiguracaoApi CriarConfiguracaoFactory()
    {
        return ConfiguracaoApi.CriarPadrao(
            "https://api.exemplo.com",
            $"key-{_random.Next(1000, 9999)}");
    }

    /// <summary>
    /// Benchmark: Criação de configuração usando object initializer.
    /// </summary>
    [Benchmark]
    public ConfiguracaoApi CriarConfiguracaoInitializer()
    {
        return new ConfiguracaoApi
        {
            BaseUrl = "https://api.exemplo.com",
            ApiKey = $"key-{_random.Next(1000, 9999)}",
            Timeout = TimeSpan.FromSeconds(30),
            MaxRetentativas = 3
        };
    }
}
