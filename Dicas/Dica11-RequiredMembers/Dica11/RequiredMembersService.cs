namespace Dica11;

/// <summary>
/// Interface para serviços que demonstram Required Members.
/// </summary>
public interface IRequiredMembersService
{
    Usuario CriarUsuario(string nome, string email, DateTime dataNascimento);
    Funcionario CriarFuncionario(string nome, string email, DateTime dataNascimento, string cargo, decimal salario, string departamento);
    Produto CriarProduto(string nome, decimal preco, string categoria);
    ContaBancaria CriarContaPorConstrutor(string numero, string titular, string agencia);
    ContaBancaria CriarContaPorInicializador(string numero, string titular, string agencia);
    void ProcessarEntidades(IList<IEntidade> entidades);
    string SerializarParaJson<T>(T objeto);
    T? DeserializarDeJson<T>(string json);
}

/// <summary>
/// Implementação do serviço que demonstra Required Members.
/// 
/// Esta classe mostra diferentes padrões de uso:
/// 1. Criação usando object initializers
/// 2. Uso com construtores que satisfazem required members
/// 3. Serialização/deserialização JSON
/// 4. Polimorfismo com interfaces
/// </summary>
public class RequiredMembersService : IRequiredMembersService
{
    private readonly ILogger<RequiredMembersService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RequiredMembersService(ILogger<RequiredMembersService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Cria um usuário usando object initializer.
    /// Required members devem ser especificados obrigatoriamente.
    /// </summary>
    public Usuario CriarUsuario(string nome, string email, DateTime dataNascimento)
    {
        _logger.LogInformation("Criando usuário com required members");
        
        // Required members devem ser inicializados
        return new Usuario
        {
            Nome = nome,
            Email = email,
            DataNascimento = dataNascimento,
            // Propriedades opcionais podem ser omitidas
            Telefone = nome.Contains("João") ? "(11) 99999-9999" : null
        };
    }

    /// <summary>
    /// Cria um funcionário demonstrando herança com required members.
    /// </summary>
    public Funcionario CriarFuncionario(string nome, string email, DateTime dataNascimento, string cargo, decimal salario, string departamento)
    {
        _logger.LogInformation("Criando funcionário com herança de required members");
        
        return new Funcionario
        {
            // Required members da classe base
            Nome = nome,
            Email = email,
            DataNascimento = dataNascimento,
            
            // Required members da classe derivada
            Cargo = cargo,
            Salario = salario,
            Departamento = departamento,
            
            // Propriedades opcionais
            Gerente = salario > 10000,
            Telefone = "(11) 88888-8888"
        };
    }

    /// <summary>
    /// Cria um produto usando record com required members.
    /// </summary>
    public Produto CriarProduto(string nome, decimal preco, string categoria)
    {
        _logger.LogInformation("Criando produto com record e required members");
        
        return new Produto
        {
            Nome = nome,
            Preco = preco,
            Categoria = categoria,
            Descricao = $"Produto da categoria {categoria}"
        };
    }

    /// <summary>
    /// Cria conta bancária usando construtor que satisfaz required members.
    /// </summary>
    public ContaBancaria CriarContaPorConstrutor(string numero, string titular, string agencia)
    {
        _logger.LogInformation("Criando conta bancária via construtor");
        
        // Construtor satisfaz automaticamente os required members
        return new ContaBancaria(numero, titular, agencia)
        {
            Saldo = 1000.00m // Propriedade não-required pode ser definida
        };
    }

    /// <summary>
    /// Cria conta bancária usando construtor parameterless e object initializer.
    /// </summary>
    public ContaBancaria CriarContaPorInicializador(string numero, string titular, string agencia)
    {
        _logger.LogInformation("Criando conta bancária via object initializer");
        
        // Required members devem ser especificados mesmo com construtor parameterless
        return new ContaBancaria
        {
            Numero = numero,
            Titular = titular,
            Agencia = agencia,
            Saldo = 500.00m
        };
    }

    /// <summary>
    /// Processa uma lista de entidades demonstrando polimorfismo.
    /// </summary>
    public void ProcessarEntidades(IList<IEntidade> entidades)
    {
        _logger.LogInformation("Processando {Count} entidades", entidades.Count);
        
        foreach (var entidade in entidades)
        {
            _logger.LogInformation("Entidade: {Id}, Criada em: {DataCriacao}", 
                entidade.Id, entidade.DataCriacao);
                
            // Pattern matching para tipos específicos
            switch (entidade)
            {
                case Documento doc:
                    _logger.LogInformation("Documento: {Titulo}, Público: {Publico}", 
                        doc.Titulo, doc.Publico);
                    break;
                    
                default:
                    _logger.LogInformation("Tipo de entidade: {Tipo}", entidade.GetType().Name);
                    break;
            }
        }
    }

    /// <summary>
    /// Serializa objeto para JSON mantendo required members.
    /// </summary>
    public string SerializarParaJson<T>(T objeto)
    {
        _logger.LogInformation("Serializando objeto do tipo {Tipo}", typeof(T).Name);
        return JsonSerializer.Serialize(objeto, _jsonOptions);
    }

    /// <summary>
    /// Deserializa JSON para objeto com required members.
    /// Required members devem estar presentes no JSON.
    /// </summary>
    public T? DeserializarDeJson<T>(string json)
    {
        try
        {
            _logger.LogInformation("Deserializando JSON para tipo {Tipo}", typeof(T).Name);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar JSON. Required members podem estar ausentes.");
            return default;
        }
    }
}
