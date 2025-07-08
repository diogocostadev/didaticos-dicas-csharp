namespace Dica18.RequiredMembers;

// =================== CASOS DE USO PRÁTICOS ===================

/// <summary>
/// Base para entidades de domínio - comum em Entity Framework
/// </summary>
public class EntidadeBase
{
    public required Guid Id { get; set; }
    public required DateTime DataCriacao { get; set; }
    public required string CriadoPor { get; set; }
    
    public DateTime? DataAtualizacao { get; set; }
    public string? AtualizadoPor { get; set; }
    public bool Excluido { get; set; } = false;
    public DateTime? DataExclusao { get; set; }
}

/// <summary>
/// Configuração de banco de dados com required members
/// </summary>
public class DatabaseConfig
{
    [Required(ErrorMessage = "Connection string é obrigatória")]
    public required string ConnectionString { get; set; }
    
    [Required(ErrorMessage = "Provider é obrigatório")]
    public required string Provider { get; set; }
    
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSegundos { get; set; } = 30;
    public bool EnableRetry { get; set; } = true;
    public bool LogSqlCommands { get; set; } = false;
    
    // Propriedades derivadas
    public bool EhSqlServer => Provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase);
    public bool EhPostgreSQL => Provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Modelo de domínio para e-commerce
/// </summary>
public class Pedido
{
    [Required(ErrorMessage = "Número do pedido é obrigatório")]
    [StringLength(20, MinimumLength = 5)]
    public required string Numero { get; set; }
    
    public required Guid ClienteId { get; set; }
    public required DateTime DataPedido { get; set; }
    public required List<ItemPedido> Itens { get; set; }
    
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;
    public string? Observacoes { get; set; }
    public decimal? Desconto { get; set; }
    public decimal? Frete { get; set; }
    
    // Propriedades calculadas
    public decimal Subtotal => Itens?.Sum(i => i.Total) ?? 0;
    public decimal Total => Subtotal + (Frete ?? 0) - (Desconto ?? 0);
    public int QuantidadeTotal => Itens?.Sum(i => i.Quantidade) ?? 0;
}

public class ItemPedido
{
    public required Guid ProdutoId { get; set; }
    public required int Quantidade { get; set; }
    public required decimal PrecoUnitario { get; set; }
    
    public string? Observacoes { get; set; }
    
    public decimal Total => Quantidade * PrecoUnitario;
}

public enum StatusPedido
{
    Pendente,
    Confirmado,
    Processando,
    Enviado,
    Entregue,
    Cancelado
}

// =================== CONFIGURAÇÕES DE APLICAÇÃO ===================

/// <summary>
/// Configuração para microserviços
/// </summary>
public class MicroserviceConfig
{
    [Required(ErrorMessage = "Nome do serviço é obrigatório")]
    public required string ServiceName { get; set; }
    
    [Required(ErrorMessage = "Porta é obrigatória")]
    [Range(1, 65535, ErrorMessage = "Porta deve estar entre 1 e 65535")]
    public required int Port { get; set; }
    
    public string Environment { get; set; } = "Development";
    public string Version { get; set; } = "1.0.0";
    public bool EnableHealthCheck { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    
    // Configurações de rede
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxConcurrentRequests { get; set; } = 100;
    
    // Configurações de log
    public LogLevel MinLogLevel { get; set; } = LogLevel.Info;
    public bool LogRequestResponse { get; set; } = false;
}

/// <summary>
/// Configuração para cache distribuído
/// </summary>
public class CacheConfig
{
    [Required(ErrorMessage = "Provider de cache é obrigatório")]
    public required string Provider { get; set; } // Redis, Memory, etc.
    
    [Required(ErrorMessage = "Connection string é obrigatória")]
    public required string ConnectionString { get; set; }
    
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableCompression { get; set; } = true;
    public string? KeyPrefix { get; set; }
    public int Database { get; set; } = 0;
    
    // Configurações específicas do Redis
    public bool AbortOnConnectFail { get; set; } = false;
    public int ConnectRetry { get; set; } = 3;
}

// =================== MODELS PARA AUTENTICAÇÃO ===================

/// <summary>
/// Dados de autenticação JWT
/// </summary>
public class JwtConfig
{
    [Required(ErrorMessage = "Secret key é obrigatória")]
    [MinLength(32, ErrorMessage = "Secret key deve ter pelo menos 32 caracteres")]
    public required string SecretKey { get; set; }
    
    [Required(ErrorMessage = "Issuer é obrigatório")]
    public required string Issuer { get; set; }
    
    [Required(ErrorMessage = "Audience é obrigatório")]
    public required string Audience { get; set; }
    
    public TimeSpan AccessTokenExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
}

/// <summary>
/// Dados de usuário para claims
/// </summary>
public class UserClaims
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required List<string> Roles { get; set; }
    
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public Dictionary<string, string> CustomClaims { get; set; } = new();
    
    // Método para criar claims
    public List<System.Security.Claims.Claim> ToClaims()
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new("sub", UserId),
            new("username", Username)
        };
        
        claims.AddRange(Roles.Select(role => new System.Security.Claims.Claim("role", role)));
        
        if (!string.IsNullOrEmpty(Email))
            claims.Add(new("email", Email));
            
        if (!string.IsNullOrEmpty(FullName))
            claims.Add(new("name", FullName));
            
        claims.AddRange(CustomClaims.Select(kv => new System.Security.Claims.Claim(kv.Key, kv.Value)));
        
        return claims;
    }
}
