using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/*
 * Dica 101: Evitando Catch Vazio (Exceções Engolidas)
 * 
 * Um dos piores anti-patterns em C# é o "catch vazio" ou "Pokemon Exception Handling"
 * (gotta catch 'em all!). Isso acontece quando capturamos uma exceção e não fazemos
 * NADA com ela - nem log, nem rethrow, nem tratamento.
 * 
 * PROBLEMAS:
 * - Erros silenciosos que são IMPOSSÍVEIS de diagnosticar
 * - Bugs que parecem "fantasmas" - algo não funciona mas não há erro
 * - Horas/dias de debugging procurando onde o problema está
 * - Dados corrompidos silenciosamente
 * - Operações que parecem ter sucesso mas falharam
 * 
 * REGRA DE OURO: Se você captura uma exceção, FAÇA ALGO com ela!
 */

Console.WriteLine("=== Dica 101: Evitando Catch Vazio (Exceções Engolidas) ===\n");

// Setup do host com logging
using var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<PedidoService>();
        services.AddSingleton<NotificacaoService>();
        services.AddSingleton<IntegracaoExternaService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var pedidoService = host.Services.GetRequiredService<PedidoService>();
var notificacaoService = host.Services.GetRequiredService<NotificacaoService>();
var integracaoService = host.Services.GetRequiredService<IntegracaoExternaService>();

// ══════════════════════════════════════════════════════════════════════════════
// 1. O PROBLEMA: Catch Vazio - O Assassino Silencioso
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("🚨 1. O PROBLEMA: Catch Vazio - O Assassino Silencioso");
Console.WriteLine("════════════════════════════════════════════════════════\n");

await DemonstrarProblemasCatchVazio(pedidoService, logger);

// ══════════════════════════════════════════════════════════════════════════════
// 2. PADRÕES RUINS QUE VOCÊ VAI ENCONTRAR (E CORRIGIR!)
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\n🔴 2. PADRÕES RUINS QUE VOCÊ VAI ENCONTRAR:");
Console.WriteLine("════════════════════════════════════════════\n");

MostrarPadroesRuins();

// ══════════════════════════════════════════════════════════════════════════════
// 3. SOLUÇÕES CORRETAS
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\n✅ 3. SOLUÇÕES CORRETAS:");
Console.WriteLine("═════════════════════════\n");

await DemonstrarSolucoesCorretas(pedidoService, notificacaoService, integracaoService, logger);

// ══════════════════════════════════════════════════════════════════════════════
// 4. QUANDO É ACEITÁVEL "IGNORAR" UMA EXCEÇÃO
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\n⚠️ 4. QUANDO É ACEITÁVEL 'IGNORAR' UMA EXCEÇÃO:");
Console.WriteLine("═════════════════════════════════════════════════\n");

DemonstrarCasosAceitaveis(logger);

// ══════════════════════════════════════════════════════════════════════════════
// 5. RESUMO E BOAS PRÁTICAS
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\n📋 5. RESUMO E BOAS PRÁTICAS:");
Console.WriteLine("══════════════════════════════\n");

MostrarResumo();

Console.WriteLine("\n=== Demonstração Concluída ===");

// ═══════════════════════════════════════════════════════════════════════════════
// MÉTODOS DE DEMONSTRAÇÃO
// ═══════════════════════════════════════════════════════════════════════════════

static async Task DemonstrarProblemasCatchVazio(PedidoService pedidoService, ILogger logger)
{
    Console.WriteLine("Cenário: Sistema de pedidos onde exceções são engolidas\n");

    // Simula um pedido que vai falhar silenciosamente
    var pedidoId = "PED-12345";
    
    Console.WriteLine($"📦 Processando pedido {pedidoId}...");
    
    // Este método tem catch vazio - o erro é ENGOLIDO
    var resultadoRuim = await pedidoService.ProcessarPedidoComCatchVazioAsync(pedidoId);
    
    Console.WriteLine($"   Resultado retornado: {(resultadoRuim ? "✅ Sucesso" : "❌ Falha")}");
    Console.WriteLine();
    
    // Mas espera... o pedido realmente foi processado?
    Console.WriteLine("🤔 Pergunta: O pedido foi realmente processado?");
    Console.WriteLine("   RESPOSTA: NÃO! Uma exceção ocorreu mas foi ENGOLIDA!");
    Console.WriteLine("   O método retornou 'false' mas você não sabe POR QUÊ.");
    Console.WriteLine();
    
    Console.WriteLine("💀 Problemas causados:");
    Console.WriteLine("   • Cliente acha que pedido foi feito, mas não foi");
    Console.WriteLine("   • Nenhum log de erro para investigar");
    Console.WriteLine("   • Suporte não consegue diagnosticar");
    Console.WriteLine("   • Você vai passar HORAS debugando");
}

static void MostrarPadroesRuins()
{
    Console.WriteLine("❌ PADRÃO 1: Catch totalmente vazio");
    Console.WriteLine("────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await ProcessarPagamentoAsync(pedido);
    }
    catch (Exception)
    {
        // 💀 DESASTRE: Exceção engolida completamente!
    }
");

    Console.WriteLine("❌ PADRÃO 2: Catch com apenas return");
    Console.WriteLine("─────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        return await BuscarDadosAsync(id);
    }
    catch (Exception)
    {
        return null; // 💀 Retorna null sem explicar o motivo!
    }
");

    Console.WriteLine("❌ PADRÃO 3: Catch com return false/default");
    Console.WriteLine("────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await EnviarEmailAsync(destinatario, mensagem);
        return true;
    }
    catch (Exception)
    {
        return false; // 💀 Falha silenciosa - por que falhou?
    }
");

    Console.WriteLine("❌ PADRÃO 4: Catch genérico sem diferenciar exceções");
    Console.WriteLine("──────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await ChamarApiExternaAsync();
    }
    catch (Exception ex)
    {
        // 💀 Trata timeout igual a erro de validação!
        Console.WriteLine(""Erro"");
    }
");

    Console.WriteLine("❌ PADRÃO 5: Catch com Console.WriteLine em produção");
    Console.WriteLine("─────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await SalvarNoBancoAsync(dados);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message); // 💀 Ninguém vê isso em produção!
    }
");
}

static async Task DemonstrarSolucoesCorretas(
    PedidoService pedidoService,
    NotificacaoService notificacaoService,
    IntegracaoExternaService integracaoService,
    ILogger logger)
{
    // SOLUÇÃO 1: Log + Rethrow
    Console.WriteLine("✅ SOLUÇÃO 1: Log + Rethrow (quando o chamador deve tratar)");
    Console.WriteLine("─────────────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await ProcessarPagamentoAsync(pedido);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, ""Erro ao processar pagamento do pedido {PedidoId}"", pedido.Id);
        throw; // ✅ Relança para o chamador tratar
    }
");
    
    Console.WriteLine("📝 Demonstrando na prática:");
    try
    {
        await pedidoService.ProcessarPedidoComLogEThrowAsync("PED-99999");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   ✅ Exceção capturada no chamador: {ex.Message}");
        Console.WriteLine($"   ✅ Stack trace preservado para debugging");
    }
    Console.WriteLine();

    // SOLUÇÃO 2: Log + Tratamento Específico
    Console.WriteLine("✅ SOLUÇÃO 2: Log + Tratamento Específico por Tipo de Exceção");
    Console.WriteLine("──────────────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await EnviarNotificacaoAsync(usuario, mensagem);
    }
    catch (SmtpException ex)
    {
        _logger.LogWarning(ex, ""Falha ao enviar email para {Email}"", usuario.Email);
        await EnviarViaSmsAsync(usuario, mensagem); // ✅ Fallback
    }
    catch (TimeoutException ex)
    {
        _logger.LogError(ex, ""Timeout ao enviar notificação"");
        await AgendarRetentativaAsync(usuario, mensagem); // ✅ Retry
    }
");

    Console.WriteLine("📝 Demonstrando na prática:");
    await notificacaoService.EnviarNotificacaoComFallbackAsync("usuario@email.com", "Olá!");
    Console.WriteLine();

    // SOLUÇÃO 3: Result Pattern
    Console.WriteLine("✅ SOLUÇÃO 3: Result Pattern (para operações que podem falhar)");
    Console.WriteLine("───────────────────────────────────────────────────────────────");
    Console.WriteLine(@"
    public async Task<Result<Pedido>> ProcessarPedidoAsync(string pedidoId)
    {
        try
        {
            var pedido = await _repository.BuscarAsync(pedidoId);
            await _pagamento.ProcessarAsync(pedido);
            return Result<Pedido>.Success(pedido);
        }
        catch (PedidoNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, ""Pedido {PedidoId} não encontrado"", pedidoId);
            return Result<Pedido>.Failure(""Pedido não encontrado"");
        }
        catch (PagamentoRecusadoException ex)
        {
            _logger.LogWarning(ex, ""Pagamento recusado para pedido {PedidoId}"", pedidoId);
            return Result<Pedido>.Failure($""Pagamento recusado: {ex.Motivo}"");
        }
    }
");

    Console.WriteLine("📝 Demonstrando na prática:");
    var resultado = await pedidoService.ProcessarPedidoComResultPatternAsync("PED-TESTE");
    Console.WriteLine($"   Sucesso: {resultado.IsSuccess}");
    Console.WriteLine($"   Mensagem: {resultado.ErrorMessage ?? "OK"}");
    Console.WriteLine();

    // SOLUÇÃO 4: Operação não crítica com log
    Console.WriteLine("✅ SOLUÇÃO 4: Operação não crítica (log + continuar)");
    Console.WriteLine("─────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await EnviarMetricasAsync(dados);
    }
    catch (Exception ex)
    {
        // ✅ Log completo mesmo que continue
        _logger.LogWarning(ex, 
            ""Falha ao enviar métricas - operação não crítica. "" +
            ""Dados: {Dados}"", dados);
        // Continua execução - métricas não são críticas
    }
");

    Console.WriteLine("📝 Demonstrando na prática:");
    await integracaoService.EnviarMetricasNaoCriticasAsync(new { Evento = "Login", Usuario = "joao" });
    Console.WriteLine();
}

static void DemonstrarCasosAceitaveis(ILogger logger)
{
    Console.WriteLine("Existem POUCOS casos onde 'ignorar' uma exceção é aceitável,");
    Console.WriteLine("mas SEMPRE com log ou comentário explicando o motivo:\n");

    Console.WriteLine("⚠️ CASO 1: Cleanup/Dispose que pode falhar");
    Console.WriteLine("──────────────────────────────────────────");
    Console.WriteLine(@"
    finally
    {
        try
        {
            connection?.Dispose();
        }
        catch (Exception ex)
        {
            // ✅ Aceitável: Dispose não deve impedir o fluxo principal
            // Mas ainda assim logamos!
            _logger.LogDebug(ex, ""Erro ao fazer dispose da conexão - ignorado"");
        }
    }
");

    Console.WriteLine("⚠️ CASO 2: Operação de cancelamento");
    Console.WriteLine("────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        await operacaoAsync.WaitAsync(cancellationToken);
    }
    catch (OperationCanceledException)
    {
        // ✅ Aceitável: Cancelamento é esperado e intencional
        _logger.LogDebug(""Operação cancelada pelo usuário"");
    }
");

    Console.WriteLine("⚠️ CASO 3: Verificação de existência");
    Console.WriteLine("─────────────────────────────────────");
    Console.WriteLine(@"
    public bool ArquivoExiste(string caminho)
    {
        try
        {
            return File.Exists(caminho);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            // ✅ Aceitável: Se não conseguimos verificar, tratamos como não existe
            _logger.LogDebug(ex, ""Não foi possível verificar arquivo {Caminho}"", caminho);
            return false;
        }
    }
");

    Console.WriteLine("⚠️ CASO 4: Best-effort logging (evitar loop infinito)");
    Console.WriteLine("──────────────────────────────────────────────────────");
    Console.WriteLine(@"
    try
    {
        _logger.LogError(ex, ""Erro crítico"");
    }
    catch
    {
        // ✅ Aceitável: Se o próprio log falhar, não podemos logar o erro do log!
        // Isso evita loop infinito
        Debug.WriteLine($""Falha ao logar: {ex.Message}"");
    }
");
}

static void MostrarResumo()
{
    Console.WriteLine("📌 REGRAS DE OURO:\n");
    
    Console.WriteLine("1️⃣  NUNCA deixe um catch completamente vazio");
    Console.WriteLine("    ❌ catch (Exception) { }");
    Console.WriteLine("    ✅ catch (Exception ex) { _logger.LogError(ex, \"...\"); throw; }\n");

    Console.WriteLine("2️⃣  SEMPRE logue a exceção com contexto");
    Console.WriteLine("    ❌ _logger.LogError(\"Erro\");");
    Console.WriteLine("    ✅ _logger.LogError(ex, \"Erro ao processar pedido {PedidoId}\", id);\n");

    Console.WriteLine("3️⃣  Use exceções específicas quando possível");
    Console.WriteLine("    ❌ catch (Exception ex)");
    Console.WriteLine("    ✅ catch (HttpRequestException ex) when (ex.StatusCode == 404)\n");

    Console.WriteLine("4️⃣  Se retornar um valor padrão, LOGUE o motivo");
    Console.WriteLine("    ❌ catch (Exception) { return null; }");
    Console.WriteLine("    ✅ catch (Exception ex) { _logger.LogWarning(ex, \"...\"); return null; }\n");

    Console.WriteLine("5️⃣  Considere o Result Pattern para operações que podem falhar");
    Console.WriteLine("    ❌ return false; // Por quê?");
    Console.WriteLine("    ✅ return Result.Failure(\"Motivo específico\");\n");

    Console.WriteLine("6️⃣  throw; preserva o stack trace, throw ex; não!");
    Console.WriteLine("    ❌ catch (Exception ex) { Log(ex); throw ex; }");
    Console.WriteLine("    ✅ catch (Exception ex) { Log(ex); throw; }\n");

    Console.WriteLine("═══════════════════════════════════════════════════════════");
    Console.WriteLine("💡 LEMBRE-SE: Um catch vazio hoje = horas de debugging amanhã!");
    Console.WriteLine("═══════════════════════════════════════════════════════════");
}

// ═══════════════════════════════════════════════════════════════════════════════
// CLASSES DE SERVIÇO
// ═══════════════════════════════════════════════════════════════════════════════

public class PedidoService
{
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(ILogger<PedidoService> logger)
    {
        _logger = logger;
    }

    // ❌ RUIM: Catch vazio que engole exceções
    public async Task<bool> ProcessarPedidoComCatchVazioAsync(string pedidoId)
    {
        try
        {
            // Simula processamento que vai falhar
            await Task.Delay(100);
            throw new InvalidOperationException("Estoque insuficiente para o pedido!");
        }
        catch (Exception)
        {
            // 💀 DESASTRE: Exceção completamente engolida!
            // Ninguém sabe que houve erro
            return false;
        }
    }

    // ✅ BOM: Log + Rethrow
    public async Task ProcessarPedidoComLogEThrowAsync(string pedidoId)
    {
        try
        {
            await Task.Delay(100);
            throw new InvalidOperationException("Estoque insuficiente para o pedido!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pedido {PedidoId}. Detalhes: {Detalhes}",
                pedidoId, ex.Message);
            throw; // ✅ Relança preservando stack trace
        }
    }

    // ✅ BOM: Result Pattern
    public async Task<Result<string>> ProcessarPedidoComResultPatternAsync(string pedidoId)
    {
        try
        {
            await Task.Delay(100);
            
            // Simula validações que podem falhar
            if (pedidoId.Contains("INVALIDO"))
            {
                throw new ArgumentException("ID de pedido inválido");
            }

            if (pedidoId.Contains("ESTOQUE"))
            {
                throw new InvalidOperationException("Estoque insuficiente");
            }

            // Simula sucesso
            return Result<string>.Success($"Pedido {pedidoId} processado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validação falhou para pedido {PedidoId}", pedidoId);
            return Result<string>.Failure($"Erro de validação: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operação inválida para pedido {PedidoId}", pedidoId);
            return Result<string>.Failure($"Erro de negócio: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar pedido {PedidoId}", pedidoId);
            return Result<string>.Failure("Erro interno. Tente novamente mais tarde.");
        }
    }
}

public class NotificacaoService
{
    private readonly ILogger<NotificacaoService> _logger;

    public NotificacaoService(ILogger<NotificacaoService> logger)
    {
        _logger = logger;
    }

    // ✅ BOM: Tratamento específico com fallback
    public async Task EnviarNotificacaoComFallbackAsync(string email, string mensagem)
    {
        try
        {
            _logger.LogInformation("Tentando enviar email para {Email}", email);
            await EnviarEmailAsync(email, mensagem);
            _logger.LogInformation("   ✅ Email enviado com sucesso!");
        }
        catch (SmtpException ex)
        {
            _logger.LogWarning(ex, "Falha no SMTP ao enviar para {Email}. Tentando SMS...", email);
            
            try
            {
                await EnviarSmsAsync(ExtrairTelefoneDoEmail(email), mensagem);
                _logger.LogInformation("   ✅ SMS enviado como fallback!");
            }
            catch (Exception smsEx)
            {
                _logger.LogError(smsEx, "Falha total ao notificar {Email}. Email e SMS falharam.", email);
                throw new NotificacaoException("Não foi possível notificar o usuário", smsEx);
            }
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout ao enviar notificação para {Email}", email);
            await AgendarRetentativaAsync(email, mensagem);
            _logger.LogInformation("   ⏰ Retentativa agendada para mais tarde");
        }
    }

    private async Task EnviarEmailAsync(string email, string mensagem)
    {
        await Task.Delay(50);
        // Simula falha de SMTP
        throw new SmtpException("Servidor SMTP indisponível");
    }

    private async Task EnviarSmsAsync(string telefone, string mensagem)
    {
        await Task.Delay(50);
        // Simula sucesso do SMS
    }

    private async Task AgendarRetentativaAsync(string email, string mensagem)
    {
        await Task.Delay(10);
        // Simula agendamento
    }

    private string ExtrairTelefoneDoEmail(string email) => "+5511999999999";
}

public class IntegracaoExternaService
{
    private readonly ILogger<IntegracaoExternaService> _logger;

    public IntegracaoExternaService(ILogger<IntegracaoExternaService> logger)
    {
        _logger = logger;
    }

    // ✅ BOM: Operação não crítica com log adequado
    public async Task EnviarMetricasNaoCriticasAsync(object dados)
    {
        try
        {
            _logger.LogDebug("Enviando métricas: {Dados}", dados);
            await Task.Delay(50);
            
            // Simula falha
            throw new HttpRequestException("Serviço de métricas indisponível");
        }
        catch (Exception ex)
        {
            // ✅ Log completo mesmo que a operação não seja crítica
            _logger.LogWarning(ex,
                "Falha ao enviar métricas (operação não crítica). " +
                "Dados: {Dados}. A execução continuará normalmente.",
                dados);
            
            // Não relança - métricas não são críticas
            // Mas o erro está LOGADO para investigação se necessário
            Console.WriteLine("   ⚠️ Métricas não enviadas (não crítico) - log registrado");
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CLASSES AUXILIARES
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Result Pattern - Uma forma elegante de retornar sucesso ou falha
/// sem usar exceções para controle de fluxo
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, T? value, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
}

// Exceções customizadas para demonstração
public class SmtpException : Exception
{
    public SmtpException(string message) : base(message) { }
}

public class NotificacaoException : Exception
{
    public NotificacaoException(string message, Exception inner) : base(message, inner) { }
}
