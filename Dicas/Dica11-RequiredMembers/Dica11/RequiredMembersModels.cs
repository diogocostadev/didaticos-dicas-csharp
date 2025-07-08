namespace Dica11;

/// <summary>
/// Demonstra diferentes cenários de Required Members.
/// 
/// Required Members são propriedades que devem ser inicializadas
/// durante a criação do objeto, oferecendo:
/// 1. Garantia de inicialização em tempo de compilação
/// 2. Flexibilidade maior que construtores obrigatórios
/// 3. Melhor experiência com object initializers
/// 4. Suporte à herança e polimorfismo
/// </summary>

// 1. Exemplo básico com Required Members
public class Usuario
{
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required DateTime DataNascimento { get; init; }
    
    // Propriedades opcionais
    public string? Telefone { get; init; }
    public bool Ativo { get; init; } = true;
    
    // Propriedade calculada
    public int Idade => DateTime.Now.Year - DataNascimento.Year;
}

// 2. Classe tradicional para comparação (sem required)
public class UsuarioTradicional
{
    public UsuarioTradicional(string nome, string email, DateTime dataNascimento)
    {
        Nome = nome;
        Email = email;
        DataNascimento = dataNascimento;
    }
    
    public string Nome { get; }
    public string Email { get; }
    public DateTime DataNascimento { get; }
    public string? Telefone { get; init; }
    public bool Ativo { get; init; } = true;
    public int Idade => DateTime.Now.Year - DataNascimento.Year;
}

// 3. Herança com Required Members
public class Funcionario : Usuario
{
    public required string Cargo { get; init; }
    public required decimal Salario { get; init; }
    public required string Departamento { get; init; }
    
    public DateTime DataAdmissao { get; init; } = DateTime.Now;
    public bool Gerente { get; init; }
}

// 4. Record com Required Members
public record Produto
{
    public required string Nome { get; init; }
    public required decimal Preco { get; init; }
    public required string Categoria { get; init; }
    
    public string? Descricao { get; init; }
    public bool Disponivel { get; init; } = true;
    public DateTime DataCriacao { get; init; } = DateTime.Now;
}

// 5. Classe com construtor que satisfaz Required Members
public class ContaBancaria
{
    public required string Numero { get; init; }
    public required string Titular { get; init; }
    public required string Agencia { get; init; }
    
    public decimal Saldo { get; set; }
    public bool Ativa { get; init; } = true;
    
    // Construtor que satisfaz os required members usando [SetsRequiredMembers]
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ContaBancaria(string numero, string titular, string agencia)
    {
        Numero = numero;
        Titular = titular;
        Agencia = agencia;
    }
    
    // Construtor parameterless ainda requer inicialização dos required members
    public ContaBancaria()
    {
    }
}

// 6. Interface para demonstrar polimorfismo
public interface IEntidade
{
    string Id { get; }
    DateTime DataCriacao { get; }
}

// 7. Implementação com Required Members
public class Documento : IEntidade
{
    public required string Id { get; init; }
    public required string Titulo { get; init; }
    public required string Conteudo { get; init; }
    
    public DateTime DataCriacao { get; init; } = DateTime.Now;
    public string? Autor { get; init; }
    public bool Publico { get; init; } = true;
}

// 8. Classe com validação customizada
public class Endereco
{
    private string _cep = string.Empty;
    
    public required string Rua { get; init; }
    public required string Cidade { get; init; }
    public required string Estado { get; init; }
    
    public required string Cep 
    { 
        get => _cep;
        init => _cep = ValidarCep(value) ? value : throw new ArgumentException("CEP inválido");
    }
    
    public string? Complemento { get; init; }
    public string? Numero { get; init; }
    
    private static bool ValidarCep(string cep) => 
        !string.IsNullOrWhiteSpace(cep) && cep.Replace("-", "").Length == 8;
}

// 9. Classe para configuração com Required Members
public class ConfiguracaoApi
{
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
    public required TimeSpan Timeout { get; init; }
    
    public bool UsarHttps { get; init; } = true;
    public int MaxRetentativas { get; init; } = 3;
    public string? UserAgent { get; init; }
    
    // Método de factory que garante configuração válida
    public static ConfiguracaoApi CriarPadrao(string baseUrl, string apiKey) => new()
    {
        BaseUrl = baseUrl,
        ApiKey = apiKey,
        Timeout = TimeSpan.FromSeconds(30)
    };
}
