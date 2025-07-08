namespace Dica11;

/// <summary>
/// Demonstração completa de Required Members do C# 11.
/// 
/// Esta classe executa exemplos práticos mostrando:
/// 1. Diferenças entre required members e construtores tradicionais
/// 2. Herança com required members
/// 3. Integração com records
/// 4. Serialização JSON
/// 5. Validação e tratamento de erros
/// 6. Padrões de uso recomendados
/// </summary>
public class RequiredMembersDemonstration
{
    private readonly IRequiredMembersService _service;
    private readonly ILogger<RequiredMembersDemonstration> _logger;

    public RequiredMembersDemonstration(
        IRequiredMembersService service,
        ILogger<RequiredMembersDemonstration> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("=== Demonstração: Required Members em C# 11 ===");
        
        DemonstrarCriacaoBasica();
        await Task.Delay(1000);
        
        DemonstrarHeranca();
        await Task.Delay(1000);
        
        DemonstrarRecords();
        await Task.Delay(1000);
        
        DemonstrarConstrutores();
        await Task.Delay(1000);
        
        DemonstrarPolimorfismo();
        await Task.Delay(1000);
        
        DemonstrarSerializacao();
        await Task.Delay(1000);
        
        DemonstrarValidacao();
        await Task.Delay(1000);
        
        DemonstrarComparacaoComAbordagemTradicional();
        
        _logger.LogInformation("=== Fim da Demonstração ===");
    }

    /// <summary>
    /// Demonstra a criação básica de objetos com required members.
    /// </summary>
    private void DemonstrarCriacaoBasica()
    {
        _logger.LogInformation("\n--- Demonstração: Criação Básica com Required Members ---");
        
        // Criação correta - todos os required members especificados
        var usuario = _service.CriarUsuario(
            "João Silva", 
            "joao@email.com", 
            new DateTime(1990, 5, 15));
        
        _logger.LogInformation("Usuário criado: {Nome}, {Email}, Idade: {Idade}", 
            usuario.Nome, usuario.Email, usuario.Idade);
        
        // Demonstração de erro em tempo de compilação
        // As linhas abaixo causariam erro de compilação:
        
        /*
        // ERRO: Required member 'Usuario.Nome' must be set
        var usuarioIncompleto = new Usuario
        {
            Email = "teste@email.com",
            DataNascimento = DateTime.Now
            // Nome não foi especificado - ERRO!
        };
        */
        
        _logger.LogInformation("Required members garantem que propriedades essenciais sejam sempre inicializadas.");
    }

    /// <summary>
    /// Demonstra como required members funcionam com herança.
    /// </summary>
    private void DemonstrarHeranca()
    {
        _logger.LogInformation("\n--- Demonstração: Herança com Required Members ---");
        
        var funcionario = _service.CriarFuncionario(
            "Maria Santos",
            "maria@empresa.com",
            new DateTime(1985, 8, 20),
            "Desenvolvedora Senior",
            12000.00m,
            "Tecnologia");
        
        _logger.LogInformation("Funcionário criado:");
        _logger.LogInformation("  Nome: {Nome}, Cargo: {Cargo}", funcionario.Nome, funcionario.Cargo);
        _logger.LogInformation("  Salário: {Salario:C}, É Gerente: {Gerente}", funcionario.Salario, funcionario.Gerente);
        _logger.LogInformation("  Departamento: {Departamento}, Idade: {Idade}", funcionario.Departamento, funcionario.Idade);
        
        _logger.LogInformation("Classes derivadas herdam required members da classe base e podem adicionar novos.");
    }

    /// <summary>
    /// Demonstra required members em records.
    /// </summary>
    private void DemonstrarRecords()
    {
        _logger.LogInformation("\n--- Demonstração: Required Members em Records ---");
        
        var produto1 = _service.CriarProduto("Laptop", 2500.00m, "Eletrônicos");
        var produto2 = _service.CriarProduto("Mouse", 50.00m, "Acessórios");
        
        _logger.LogInformation("Produtos criados:");
        _logger.LogInformation("  {Nome}: {Preco:C} ({Categoria})", produto1.Nome, produto1.Preco, produto1.Categoria);
        _logger.LogInformation("  {Nome}: {Preco:C} ({Categoria})", produto2.Nome, produto2.Preco, produto2.Categoria);
        
        // Demonstração de equality em records
        var produto1Copy = produto1 with { Descricao = "Laptop high-end" };
        _logger.LogInformation("Records mantêm value equality mesmo com required members.");
        _logger.LogInformation("produto1 == produto1Copy: {Equal}", produto1.Equals(produto1Copy));
    }

    /// <summary>
    /// Demonstra diferentes formas de satisfazer required members com construtores.
    /// </summary>
    private void DemonstrarConstrutores()
    {
        _logger.LogInformation("\n--- Demonstração: Construtores e Required Members ---");
        
        // Usando construtor que satisfaz required members
        var conta1 = _service.CriarContaPorConstrutor("12345-6", "João Silva", "0001");
        _logger.LogInformation("Conta criada via construtor: {Numero}, Saldo: {Saldo:C}", 
            conta1.Numero, conta1.Saldo);
        
        // Usando construtor parameterless com object initializer
        var conta2 = _service.CriarContaPorInicializador("54321-0", "Maria Santos", "0002");
        _logger.LogInformation("Conta criada via initializer: {Numero}, Saldo: {Saldo:C}", 
            conta2.Numero, conta2.Saldo);
        
        _logger.LogInformation("Atributo [SetsRequiredMembers] permite construtores que satisfazem automaticamente required members.");
    }

    /// <summary>
    /// Demonstra polimorfismo com required members.
    /// </summary>
    private void DemonstrarPolimorfismo()
    {
        _logger.LogInformation("\n--- Demonstração: Polimorfismo com Required Members ---");
        
        var entidades = new List<IEntidade>
        {
            new Documento
            {
                Id = "DOC-001",
                Titulo = "Manual do Usuário",
                Conteudo = "Conteúdo do manual...",
                Autor = "Tech Writer",
                Publico = true
            },
            new Documento
            {
                Id = "DOC-002",
                Titulo = "Documento Interno",
                Conteudo = "Informações confidenciais...",
                Publico = false
            }
        };
        
        _service.ProcessarEntidades(entidades);
        _logger.LogInformation("Required members funcionam perfeitamente com polimorfismo e interfaces.");
    }

    /// <summary>
    /// Demonstra serialização e deserialização com required members.
    /// </summary>
    private void DemonstrarSerializacao()
    {
        _logger.LogInformation("\n--- Demonstração: Serialização JSON com Required Members ---");
        
        var usuario = _service.CriarUsuario("Ana Costa", "ana@email.com", new DateTime(1992, 3, 10));
        
        // Serialização
        var json = _service.SerializarParaJson(usuario);
        _logger.LogInformation("JSON serializado:\n{Json}", json);
        
        // Deserialização bem-sucedida
        var usuarioDeserializado = _service.DeserializarDeJson<Usuario>(json);
        if (usuarioDeserializado != null)
        {
            _logger.LogInformation("Deserialização bem-sucedida: {Nome}, {Email}", 
                usuarioDeserializado.Nome, usuarioDeserializado.Email);
        }
        
        // Tentativa de deserialização com JSON incompleto (faltando required member)
        var jsonIncompleto = """
        {
            "nome": "Pedro Silva",
            "dataNascimento": "1988-07-22T00:00:00"
            // Email está ausente - required member!
        }
        """;
        
        var usuarioIncompleto = _service.DeserializarDeJson<Usuario>(jsonIncompleto);
        if (usuarioIncompleto == null)
        {
            _logger.LogWarning("Deserialização falhou devido à ausência de required members no JSON.");
        }
        
        _logger.LogInformation("Required members são respeitados durante serialização JSON.");
    }

    /// <summary>
    /// Demonstra validação customizada com required members.
    /// </summary>
    private void DemonstrarValidacao()
    {
        _logger.LogInformation("\n--- Demonstração: Validação com Required Members ---");
        
        try
        {
            // Criação com CEP válido
            var enderecoValido = new Endereco
            {
                Rua = "Rua das Flores, 123",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "01234-567",
                Complemento = "Apto 45"
            };
            
            _logger.LogInformation("Endereço válido criado: {Rua}, {Cidade}/{Estado}, CEP: {Cep}", 
                enderecoValido.Rua, enderecoValido.Cidade, enderecoValido.Estado, enderecoValido.Cep);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Erro na validação: {Message}", ex.Message);
        }
        
        try
        {
            // Tentativa com CEP inválido
            var enderecoInvalido = new Endereco
            {
                Rua = "Rua das Palmeiras, 456",
                Cidade = "Rio de Janeiro",
                Estado = "RJ",
                Cep = "123" // CEP inválido!
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validação impediu criação de objeto inválido: {Message}", ex.Message);
        }
        
        // Demonstração de método factory
        var config = ConfiguracaoApi.CriarPadrao("https://api.exemplo.com", "minha-api-key");
        _logger.LogInformation("Configuração criada via factory: {BaseUrl}, Timeout: {Timeout}", 
            config.BaseUrl, config.Timeout);
        
        _logger.LogInformation("Required members podem incluir validação customizada e métodos factory.");
    }

    /// <summary>
    /// Compara abordagem com required members vs. construtores tradicionais.
    /// </summary>
    private void DemonstrarComparacaoComAbordagemTradicional()
    {
        _logger.LogInformation("\n--- Demonstração: Required Members vs. Construtores Tradicionais ---");
        
        // Abordagem com required members - mais flexível
        var usuarioModerno = new Usuario
        {
            Nome = "Carlos Silva",
            Email = "carlos@email.com",
            DataNascimento = new DateTime(1987, 12, 5),
            Telefone = "(11) 77777-7777" // Propriedade opcional
        };
        
        // Abordagem tradicional - menos flexível
        var usuarioTradicional = new UsuarioTradicional("Carlos Silva", "carlos@email.com", new DateTime(1987, 12, 5))
        {
            Telefone = "(11) 77777-7777" // Só propriedades não-constructor podem ser definidas assim
        };
        
        _logger.LogInformation("Required Members oferecem:");
        _logger.LogInformation("  ✓ Flexibilidade de object initializers");
        _logger.LogInformation("  ✓ Garantia de inicialização em tempo de compilação");
        _logger.LogInformation("  ✓ Melhor experiência com herança");
        _logger.LogInformation("  ✓ Compatibilidade com init-only properties");
        _logger.LogInformation("  ✓ Menos código boilerplate");
        
        _logger.LogInformation("Construtores tradicionais ainda são úteis para:");
        _logger.LogInformation("  ✓ Lógica complexa de inicialização");
        _logger.LogInformation("  ✓ Validação durante construção");
        _logger.LogInformation("  ✓ Compatibilidade com versões antigas do C#");
        
        _logger.LogInformation("A escolha entre required members e construtores depende do contexto específico.");
    }
}
