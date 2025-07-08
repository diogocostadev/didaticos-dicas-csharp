namespace Dica18.RequiredMembers;

// =================== DTOs PARA APIs ===================

/// <summary>
/// DTO para requisições de criação de usuário
/// </summary>
public class CriarUsuarioRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2)]
    public required string Nome { get; set; }
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 8)]
    public required string Senha { get; set; }
    
    [Required(ErrorMessage = "Perfil é obrigatório")]
    public required string Perfil { get; set; }
    
    public string? Telefone { get; set; }
    public Dictionary<string, string>? MetadadosExtras { get; set; }
}

/// <summary>
/// DTO para respostas de usuário (sem dados sensíveis)
/// </summary>
public class UsuarioResponse
{
    public required Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required DateTime DataCriacao { get; set; }
    public required bool Ativo { get; set; }
    
    public string? Telefone { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public List<string> Permissoes { get; set; } = new();
}

/// <summary>
/// DTO para atualização de usuário
/// </summary>
public class AtualizarUsuarioRequest
{
    // Para updates, geralmente nem tudo é required
    public string? Nome { get; set; }
    public string? Telefone { get; set; }
    public bool? Ativo { get; set; }
    
    // Mas algumas validações ainda podem ser required se fornecidas
    [EmailAddress(ErrorMessage = "Email inválido se fornecido")]
    public string? Email { get; set; }
}

// =================== SETSREQUIREDMEMBERS ATTRIBUTE ===================

/// <summary>
/// Demonstra o uso de [SetsRequiredMembers] para construtores
/// que satisfazem os required members
/// </summary>
public class ConfiguracaoAvancada
{
    public required string Nome { get; set; }
    public required string BaseUrl { get; set; }
    
    public int TimeoutSegundos { get; set; } = 30;
    public bool LogarRequisicoes { get; set; } = true;
    public Dictionary<string, string> Headers { get; set; } = new();
    
    // Construtor que satisfaz todos os required members
    [SetsRequiredMembers]
    public ConfiguracaoAvancada(string nome, string baseUrl)
    {
        Nome = nome;
        BaseUrl = baseUrl;
    }
    
    // Construtor parameterless para object initializer
    public ConfiguracaoAvancada() { }
    
    // Construtor mais específico
    [SetsRequiredMembers]
    public ConfiguracaoAvancada(string nome, string baseUrl, int timeout, bool logarReqs = true)
    {
        Nome = nome;
        BaseUrl = baseUrl;
        TimeoutSegundos = timeout;
        LogarRequisicoes = logarReqs;
    }
}

// =================== REQUIRED COM RECORDS ===================

/// <summary>
/// Record com required members para eventos de sistema
/// </summary>
public record EventoSistema
{
    public required string Tipo { get; init; }
    public required DateTime Timestamp { get; init; }
    public required Guid UsuarioId { get; init; }
    
    public string? Origem { get; init; }
    public Dictionary<string, object> Dados { get; init; } = new();
    public SeveridadeEvento Severidade { get; init; } = SeveridadeEvento.Info;
}

public enum SeveridadeEvento
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Record class com required members (C# 10+)
/// </summary>
public record class LogEntry
{
    public required string Mensagem { get; init; }
    public required DateTime Timestamp { get; init; }
    public required LogLevel Level { get; init; }
    
    public string? Categoria { get; init; }
    public Exception? Excecao { get; init; }
    public Dictionary<string, object>? Propriedades { get; init; }
    
    // Método para formatação
    public string FormatarParaConsole()
    {
        var nivel = Level switch
        {
            LogLevel.Debug => "🐛",
            LogLevel.Info => "ℹ️",
            LogLevel.Warning => "⚠️",
            LogLevel.Error => "❌",
            LogLevel.Critical => "💥",
            _ => "📝"
        };
        
        return $"{nivel} [{Timestamp:HH:mm:ss}] {Categoria ?? "App"}: {Mensagem}";
    }
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}
