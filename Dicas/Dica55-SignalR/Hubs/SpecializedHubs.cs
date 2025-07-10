using Microsoft.AspNetCore.SignalR;
using Dica55_SignalR.Models;
using Dica55_SignalR.Services;

namespace Dica55_SignalR.Hubs;

/// <summary>
/// Hub especializado para notificações em tempo real
/// </summary>
public class NotificationHub : Hub
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(INotificationService notificationService, ILogger<NotificationHub> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        // Adicionar aos grupos apropriados
        await Groups.AddToGroupAsync(connectionId, SignalRGroups.Notifications);
        await Groups.AddToGroupAsync(connectionId, SignalRGroups.User(userId));

        // Enviar notificações não lidas
        var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync(userId);
        if (unreadNotifications.Any())
        {
            await Clients.Caller.SendAsync("UnreadNotifications", unreadNotifications);
        }

        _logger.LogInformation("🔔 Usuário {UserId} conectado ao hub de notificações", userId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Marcar notificação como lida
    /// </summary>
    public async Task MarkAsRead(string notificationId)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            await _notificationService.MarkAsReadAsync(notificationId, userId);
            await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao marcar notificação {NotificationId} como lida", notificationId);
            await Clients.Caller.SendAsync("Error", $"Erro ao marcar notificação como lida: {ex.Message}");
        }
    }

    /// <summary>
    /// Marcar todas as notificações como lidas
    /// </summary>
    public async Task MarkAllAsRead()
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            await Clients.Caller.SendAsync("AllNotificationsMarkedAsRead");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao marcar todas as notificações como lidas para {UserId}", userId);
            await Clients.Caller.SendAsync("Error", $"Erro ao marcar todas as notificações como lidas: {ex.Message}");
        }
    }

    /// <summary>
    /// Obter histórico de notificações
    /// </summary>
    public async Task GetNotificationHistory(int page = 1, int pageSize = 20)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            var notifications = await _notificationService.GetNotificationHistoryAsync(userId, page, pageSize);
            await Clients.Caller.SendAsync("NotificationHistory", notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao obter histórico de notificações para {UserId}", userId);
            await Clients.Caller.SendAsync("Error", $"Erro ao obter histórico: {ex.Message}");
        }
    }
}

/// <summary>
/// Hub para monitoramento em tempo real de sistema e performance
/// </summary>
public class MonitoringHub : Hub
{
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<MonitoringHub> _logger;

    public MonitoringHub(IMonitoringService monitoringService, ILogger<MonitoringHub> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        // Adicionar ao grupo de monitoramento
        await Groups.AddToGroupAsync(connectionId, SignalRGroups.Monitoring);

        // Enviar dados iniciais
        var currentMetrics = await _monitoringService.GetCurrentMetricsAsync();
        await Clients.Caller.SendAsync("CurrentMetrics", currentMetrics);

        var systemStatus = await _monitoringService.GetSystemStatusAsync();
        await Clients.Caller.SendAsync("SystemStatus", systemStatus);

        _logger.LogInformation("📊 Usuário {UserId} conectado ao hub de monitoramento", userId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Subscrever a métricas específicas
    /// </summary>
    public async Task SubscribeToMetrics(List<string> metricNames)
    {
        var connectionId = Context.ConnectionId;

        foreach (var metricName in metricNames)
        {
            await Groups.AddToGroupAsync(connectionId, $"Metric_{metricName}");
        }

        await Clients.Caller.SendAsync("SubscribedToMetrics", metricNames);
    }

    /// <summary>
    /// Dessubs crever de métricas
    /// </summary>
    public async Task UnsubscribeFromMetrics(List<string> metricNames)
    {
        var connectionId = Context.ConnectionId;

        foreach (var metricName in metricNames)
        {
            await Groups.RemoveFromGroupAsync(connectionId, $"Metric_{metricName}");
        }

        await Clients.Caller.SendAsync("UnsubscribedFromMetrics", metricNames);
    }
}

/// <summary>
/// Hub para colaboração em documentos em tempo real
/// </summary>
public class CollaborationHub : Hub
{
    private readonly ICollaborationService _collaborationService;
    private readonly ILogger<CollaborationHub> _logger;

    public CollaborationHub(ICollaborationService collaborationService, ILogger<CollaborationHub> logger)
    {
        _collaborationService = collaborationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("📝 Usuário {UserId} conectado ao hub de colaboração", userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        // Remover cursores do usuário de todos os documentos
        await _collaborationService.RemoveUserCursorsAsync(userId, connectionId);

        _logger.LogInformation("📝 Usuário {UserId} desconectado do hub de colaboração", userId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Entrar em um documento para colaboração
    /// </summary>
    public async Task JoinDocument(string documentId)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        try
        {
            // Verificar permissões
            var canAccess = await _collaborationService.CanAccessDocumentAsync(userId, documentId);
            if (!canAccess)
            {
                await Clients.Caller.SendAsync("Error", "Acesso negado ao documento");
                return;
            }

            // Adicionar ao grupo do documento
            await Groups.AddToGroupAsync(connectionId, SignalRGroups.Document(documentId));

            // Registrar colaborador
            await _collaborationService.AddCollaboratorAsync(documentId, userId, connectionId);

            // Notificar outros colaboradores
            await Clients.GroupExcept(SignalRGroups.Document(documentId), connectionId)
                         .SendAsync(SignalREvents.CollaboratorJoined, new
                         {
                             DocumentId = documentId,
                             UserId = userId,
                             Timestamp = DateTime.UtcNow
                         });

            // Enviar estado atual do documento
            var document = await _collaborationService.GetDocumentAsync(documentId);
            await Clients.Caller.SendAsync("DocumentState", document);

            // Enviar cursores ativos
            var activeCursors = await _collaborationService.GetActiveCursorsAsync(documentId);
            await Clients.Caller.SendAsync("ActiveCursors", activeCursors);

            _logger.LogInformation("📝 Usuário {UserId} entrou no documento {DocumentId}", userId, documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao entrar no documento {DocumentId}", documentId);
            await Clients.Caller.SendAsync("Error", $"Erro ao entrar no documento: {ex.Message}");
        }
    }

    /// <summary>
    /// Sair de um documento
    /// </summary>
    public async Task LeaveDocument(string documentId)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        try
        {
            // Remover do grupo
            await Groups.RemoveFromGroupAsync(connectionId, SignalRGroups.Document(documentId));

            // Remover colaborador
            await _collaborationService.RemoveCollaboratorAsync(documentId, userId, connectionId);

            // Notificar outros colaboradores
            await Clients.Group(SignalRGroups.Document(documentId))
                         .SendAsync(SignalREvents.CollaboratorLeft, new
                         {
                             DocumentId = documentId,
                             UserId = userId,
                             Timestamp = DateTime.UtcNow
                         });

            _logger.LogInformation("📝 Usuário {UserId} saiu do documento {DocumentId}", userId, documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao sair do documento {DocumentId}", documentId);
            await Clients.Caller.SendAsync("Error", $"Erro ao sair do documento: {ex.Message}");
        }
    }

    /// <summary>
    /// Aplicar edição ao documento
    /// </summary>
    public async Task EditDocument(DocumentEditRequest request)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            // Aplicar edição
            var edit = await _collaborationService.ApplyEditAsync(request, userId);

            // Notificar outros colaboradores
            await Clients.GroupExcept(SignalRGroups.Document(request.DocumentId), Context.ConnectionId)
                         .SendAsync(SignalREvents.DocumentEdited, edit);

            _logger.LogInformation("📝 Documento {DocumentId} editado por {UserId}", request.DocumentId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao editar documento {DocumentId}", request.DocumentId);
            await Clients.Caller.SendAsync("Error", $"Erro ao editar documento: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualizar posição do cursor
    /// </summary>
    public async Task UpdateCursor(CursorUpdateRequest request)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            // Atualizar cursor
            var cursor = await _collaborationService.UpdateCursorAsync(request, userId);

            // Notificar outros colaboradores
            await Clients.GroupExcept(SignalRGroups.Document(request.DocumentId), Context.ConnectionId)
                         .SendAsync(SignalREvents.CursorMoved, cursor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao atualizar cursor no documento {DocumentId}", request.DocumentId);
        }
    }
}

/// <summary>
/// Hub para jogos multiplayer em tempo real
/// </summary>
public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameHub> _logger;

    public GameHub(IGameService gameService, ILogger<GameHub> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("🎮 Usuário {UserId} conectado ao hub de jogos", userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        // Remover jogador de jogos ativos
        await _gameService.HandlePlayerDisconnectionAsync(userId, connectionId);

        _logger.LogInformation("🎮 Usuário {UserId} desconectado do hub de jogos", userId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Entrar em uma sala de jogo
    /// </summary>
    public async Task JoinGame(string gameId)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";
        var connectionId = Context.ConnectionId;

        try
        {
            // Verificar se pode entrar no jogo
            var canJoin = await _gameService.CanJoinGameAsync(userId, gameId);
            if (!canJoin)
            {
                await Clients.Caller.SendAsync("Error", "Não é possível entrar neste jogo");
                return;
            }

            // Adicionar jogador
            await Groups.AddToGroupAsync(connectionId, SignalRGroups.Game(gameId));
            var player = await _gameService.AddPlayerAsync(gameId, userId, connectionId);

            // Notificar outros jogadores
            await Clients.Group(SignalRGroups.Game(gameId))
                         .SendAsync(SignalREvents.PlayerJoined, player);

            // Enviar estado atual do jogo
            var gameState = await _gameService.GetGameStateAsync(gameId);
            await Clients.Caller.SendAsync("GameState", gameState);

            _logger.LogInformation("🎮 Usuário {UserId} entrou no jogo {GameId}", userId, gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao entrar no jogo {GameId}", gameId);
            await Clients.Caller.SendAsync("Error", $"Erro ao entrar no jogo: {ex.Message}");
        }
    }

    /// <summary>
    /// Fazer uma jogada
    /// </summary>
    public async Task MakeMove(string gameId, Dictionary<string, object> moveData)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            // Processar jogada
            var result = await _gameService.ProcessMoveAsync(gameId, userId, moveData);

            // Notificar todos os jogadores
            await Clients.Group(SignalRGroups.Game(gameId))
                         .SendAsync(SignalREvents.PlayerMove, result);

            // Verificar se jogo terminou
            if (result.ContainsKey("gameEnded") && (bool)result["gameEnded"])
            {
                await Clients.Group(SignalRGroups.Game(gameId))
                             .SendAsync(SignalREvents.GameEnded, result);
            }

            _logger.LogInformation("🎮 Jogada feita por {UserId} no jogo {GameId}", userId, gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar jogada no jogo {GameId}", gameId);
            await Clients.Caller.SendAsync("Error", $"Erro ao processar jogada: {ex.Message}");
        }
    }

    /// <summary>
    /// Enviar resposta em quiz
    /// </summary>
    public async Task SubmitAnswer(string gameId, string questionId, int answerIndex)
    {
        var userId = Context.User?.Identity?.Name ?? "Anonymous";

        try
        {
            // Processar resposta
            var result = await _gameService.ProcessQuizAnswerAsync(gameId, userId, questionId, answerIndex);

            // Notificar resultado
            await Clients.Caller.SendAsync(SignalREvents.AnswerSubmitted, result);

            // Atualizar placar se necessário
            if (result.ContainsKey("scoreUpdate"))
            {
                await Clients.Group(SignalRGroups.Game(gameId))
                             .SendAsync(SignalREvents.ScoreUpdate, result["scoreUpdate"]);
            }

            _logger.LogInformation("🎮 Resposta enviada por {UserId} no quiz {GameId}", userId, gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar resposta no quiz {GameId}", gameId);
            await Clients.Caller.SendAsync("Error", $"Erro ao processar resposta: {ex.Message}");
        }
    }
}
