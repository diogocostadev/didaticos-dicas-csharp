namespace Dica18.RequiredMembers;

// =================== CLASSES BÁSICAS COM REQUIRED MEMBERS ===================

/// <summary>
/// Demonstra required properties básicas
/// </summary>
public class Usuario
{
    // Required members devem ser inicializados na criação do objeto
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required DateTime DataNascimento { get; set; }
    
    // Propriedades opcionais
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    // Propriedade calculada
    public int Idade => DateTime.Today.Year - DataNascimento.Year - 
        (DateTime.Today.DayOfYear < DataNascimento.DayOfYear ? 1 : 0);
}

/// <summary>
/// Demonstra required com init accessors (imutabilidade)
/// </summary>
public class ConfiguracaoApp
{
    public required string NomeApp { get; init; }
    public required string VersaoMinima { get; init; }
    public required string ConnectionString { get; init; }
    
    // Nullable required member
    public string? ChaveAPI { get; init; }
    
    // Propriedade com valor padrão
    public int TimeoutSegundos { get; init; } = 30;
    public bool DebugMode { get; init; } = false;
}

// =================== HERANÇA COM REQUIRED MEMBERS ===================

/// <summary>
/// Classe base com required members
/// </summary>
public class Funcionario
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required DateTime DataNascimento { get; set; }
    
    public string? Departamento { get; set; }
    public decimal Salario { get; set; }
    public DateTime DataAdmissao { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Classe derivada adiciona seus próprios required members
/// </summary>
public class Desenvolvedor : Funcionario
{
    public required List<string> Linguagens { get; set; }
    public required SenioridadeLevel NivelSenioridade { get; set; }
    
    public string? Framework { get; set; }
    public int AnosExperiencia { get; set; }
}

/// <summary>
/// Outra classe derivada com diferentes required members
/// </summary>
public class Gerente : Funcionario
{
    public required List<string> Equipe { get; set; }
    public required decimal Orcamento { get; set; }
    
    public int NumeroSubordinados => Equipe?.Count ?? 0;
}

public enum SenioridadeLevel
{
    Junior,
    Pleno,
    Senior,
    Staff,
    Principal
}

// =================== VALIDATION COM REQUIRED MEMBERS ===================

/// <summary>
/// Demonstra required members com validação
/// </summary>
public class ProdutoComValidacao
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2)]
    public required string Nome { get; set; }
    
    [Required(ErrorMessage = "Código é obrigatório")]
    [RegularExpression(@"^[A-Z]{2}-\d{3}$", ErrorMessage = "Código deve estar no formato XX-000")]
    public required string Codigo { get; set; }
    
    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "Preço deve estar entre 0,01 e 999.999,99")]
    public required decimal Preco { get; set; }
    
    [Required(ErrorMessage = "Categoria é obrigatória")]
    public required string Categoria { get; set; }
    
    public string? Descricao { get; set; }
    public bool Disponivel { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    public ResultadoValidacao Validar()
    {
        var context = new ValidationContext(this);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, results, true);
        
        return new ResultadoValidacao
        {
            EhValido = isValid,
            Erros = results.Select(r => r.ErrorMessage ?? "Erro desconhecido").ToList()
        };
    }
}

public class ResultadoValidacao
{
    public bool EhValido { get; set; }
    public List<string> Erros { get; set; } = new();
}
