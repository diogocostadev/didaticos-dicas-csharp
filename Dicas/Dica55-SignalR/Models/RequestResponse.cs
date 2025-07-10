namespace Dica55_SignalR.Models
{
    // ==========================================
    // DTOs PARA ENDPOINTS DE DEMONSTRAÇÃO
    // ==========================================

    /// <summary>
    /// Request para envio de notificação
    /// </summary>
    public class SendNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.Info;
        public string? UserId { get; set; } // null = broadcast para todos
        public string? ActionUrl { get; set; }
    }

    /// <summary>
    /// Request para broadcast de mensagem
    /// </summary>
    public class BroadcastMessage
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Category { get; set; }
        public Dictionary<string, object>? Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Request para simular edição colaborativa
    /// </summary>
    public class SimulateCollaborationRequest
    {
        public string DocumentId { get; set; } = string.Empty;
        public int NumberOfEdits { get; set; } = 5;
        public string[] UserIds { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Request para simular jogo
    /// </summary>
    public class SimulateGameRequest
    {
        public string GameId { get; set; } = string.Empty;
        public GameType GameType { get; set; } = GameType.Multiplayer;
        public string[] PlayerNames { get; set; } = Array.Empty<string>();
        public int NumberOfRounds { get; set; } = 3;
    }

    /// <summary>
    /// Response com estatísticas de conexão
    /// </summary>
    public class ConnectionStatsResponse
    {
        public DateTime Timestamp { get; set; }
        public int TotalConnections { get; set; }
        public Dictionary<string, int> ConnectionsByHub { get; set; } = new();
        public Dictionary<string, int> ConnectionsByGroup { get; set; } = new();
        public TimeSpan AverageConnectionDuration { get; set; }
    }

    /// <summary>
    /// Response com informações do sistema
    /// </summary>
    public class SystemInfoResponse
    {
        public string Version { get; set; } = "1.0.0";
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public string[] AvailableHubs { get; set; } = Array.Empty<string>();
        public string[] AvailableFeatures { get; set; } = Array.Empty<string>();
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    /// <summary>
    /// Response com status de saúde do sistema
    /// </summary>
    public class HealthCheckResponse
    {
        public string Status { get; set; } = "Healthy";
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Checks { get; set; } = new();
        public TimeSpan ResponseTime { get; set; }
    }

    // ==========================================
    // REQUESTS PARA OPERAÇÕES ESPECÍFICAS
    // ==========================================

    /// <summary>
    /// Request para entrar em sala de chat
    /// </summary>
    public class JoinRoomRequest
    {
        public string RoomId { get; set; } = string.Empty;
        public string? Password { get; set; }
    }

    /// <summary>
    /// Request para enviar mensagem no chat
    /// </summary>
    public class SendMessageRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Room { get; set; } = "general";
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string? ReplyToMessageId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Request para mensagem privada
    /// </summary>
    public class PrivateMessageRequest
    {
        public string ToUserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public MessageType MessageType { get; set; } = MessageType.Text;
    }

    /// <summary>
    /// Request para atualizar status do usuário
    /// </summary>
    public class UpdateStatusRequest
    {
        public UserStatus Status { get; set; }
        public string? CustomMessage { get; set; }
    }

    /// <summary>
    /// Request para criar sala de chat
    /// </summary>
    public class CreateRoomRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPrivate { get; set; } = false;
        public string? Password { get; set; }
        public int MaxUsers { get; set; } = 100;
        public Dictionary<string, object>? Settings { get; set; }
    }

    /// <summary>
    /// Request para editar documento
    /// </summary>
    public class EditDocumentRequest
    {
        public string DocumentId { get; set; } = string.Empty;
        public EditOperation Operation { get; set; }
        public int Position { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? Length { get; set; }
    }

    /// <summary>
    /// Request para criar/entrar em jogo
    /// </summary>
    public class JoinGameRequest
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public GameType? GameType { get; set; }
    }

    /// <summary>
    /// Request para fazer jogada
    /// </summary>
    public class MakeMoveRequest
    {
        public string GameId { get; set; } = string.Empty;
        public Dictionary<string, object> Move { get; set; } = new();
    }

    // ==========================================
    // RESPONSES PARA OPERAÇÕES
    // ==========================================

    /// <summary>
    /// Response padrão para operações
    /// </summary>
    public class OperationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Response com lista de salas
    /// </summary>
    public class RoomsListResponse
    {
        public ChatRoom[] Rooms { get; set; } = Array.Empty<ChatRoom>();
        public int TotalCount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Response com histórico de mensagens
    /// </summary>
    public class MessageHistoryResponse
    {
        public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }

    /// <summary>
    /// Response com usuários online
    /// </summary>
    public class OnlineUsersResponse
    {
        public ChatUser[] Users { get; set; } = Array.Empty<ChatUser>();
        public int TotalCount { get; set; }
        public Dictionary<string, int> UsersByRoom { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Response com informações do jogo
    /// </summary>
    public class GameInfoResponse
    {
        public GameRoom Game { get; set; } = new();
        public GamePlayer[] Players { get; set; } = Array.Empty<GamePlayer>();
        public object[] RecentMoves { get; set; } = Array.Empty<object>();
        public Dictionary<string, int> Scores { get; set; } = new();
    }

    /// <summary>
    /// Response com métricas do sistema
    /// </summary>
    public class MetricsResponse
    {
        public LiveMetric[] Metrics { get; set; } = Array.Empty<LiveMetric>();
        public PerformanceData? Performance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Period { get; set; } = "current";
    }
}
