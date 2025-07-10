using Dica55_SignalR.Models;
using System.Collections.Concurrent;

namespace Dica55_SignalR.Services;

// ==========================================
// INTERFACES DOS SERVIÇOS PRINCIPAIS
// ==========================================

public interface IChatService
{
    Task RegisterUserAsync(string userId, string connectionId);
    Task UnregisterUserAsync(string userId, string connectionId);
    Task<List<ChatUser>> GetOnlineUsersAsync();
    Task SaveMessageAsync(ChatMessage message);
    Task SavePrivateMessageAsync(ChatMessage message, string fromUserId, string toUserId);
    Task<bool> CanJoinRoomAsync(string userId, string roomId, string? password);
    Task AddUserToRoomAsync(string userId, string roomId);
    Task RemoveUserFromRoomAsync(string userId, string roomId);
    Task<List<ChatMessage>> GetRecentMessagesAsync(string roomId, int count);
    Task UpdateUserStatusAsync(string userId, UserStatus status);
    Task CreateRoomAsync(ChatRoom room, string? password);
    Task KickUserFromRoomAsync(string userId, string roomId, string moderatorId, string reason);
}

public interface INotificationService
{
    Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
    Task MarkAsReadAsync(string notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task<List<Notification>> GetNotificationHistoryAsync(string userId, int page, int pageSize);
    Task SendNotificationAsync(Notification notification);
    Task SendBroadcastAsync(BroadcastMessage broadcast);
}

// ==========================================
// IMPLEMENTAÇÕES DOS SERVIÇOS
// ==========================================

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    
    // Dicionários thread-safe para armazenamento em memória
    private readonly ConcurrentDictionary<string, ChatUser> _onlineUsers;
    private readonly ConcurrentDictionary<string, List<string>> _userConnections;
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _roomMessages;
    private readonly ConcurrentDictionary<string, ChatRoom> _chatRooms;
    private readonly ConcurrentDictionary<string, string> _roomPasswords;

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
        _onlineUsers = new ConcurrentDictionary<string, ChatUser>();
        _userConnections = new ConcurrentDictionary<string, List<string>>();
        _roomMessages = new ConcurrentDictionary<string, List<ChatMessage>>();
        _chatRooms = new ConcurrentDictionary<string, ChatRoom>();
        _roomPasswords = new ConcurrentDictionary<string, string>();
        
        // Criar sala padrão
        _chatRooms.TryAdd("general", new ChatRoom
        {
            Id = "general",
            Name = "Geral",
            Description = "Sala de chat geral",
            CreatedBy = "system",
            IsPrivate = false
        });
    }

    public Task RegisterUserAsync(string userId, string connectionId)
    {
        // Adicionar conexão ao usuário
        _userConnections.AddOrUpdate(userId, 
            new List<string> { connectionId },
            (key, existing) => { existing.Add(connectionId); return existing; });

        // Atualizar usuário online
        _onlineUsers.AddOrUpdate(userId, 
            new ChatUser 
            { 
                Id = userId, 
                Name = userId, 
                Status = UserStatus.Online,
                ConnectionId = connectionId,
                LastSeen = DateTime.UtcNow
            },
            (key, existing) => 
            {
                existing.Status = UserStatus.Online;
                existing.ConnectionId = connectionId;
                existing.LastSeen = DateTime.UtcNow;
                return existing;
            });

        _logger.LogInformation("👤 Usuário {UserId} registrado com conexão {ConnectionId}", userId, connectionId);
        return Task.CompletedTask;
    }

    public Task UnregisterUserAsync(string userId, string connectionId)
    {
        // Remover conexão específica
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            
            // Se não há mais conexões, marcar como offline
            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId, out _);
                
                if (_onlineUsers.TryGetValue(userId, out var user))
                {
                    user.Status = UserStatus.Offline;
                    user.LastSeen = DateTime.UtcNow;
                }
            }
        }

        _logger.LogInformation("👋 Usuário {UserId} desregistrado (conexão {ConnectionId})", userId, connectionId);
        return Task.CompletedTask;
    }

    public Task<List<ChatUser>> GetOnlineUsersAsync()
    {
        var onlineUsers = _onlineUsers.Values
            .Where(u => u.Status != UserStatus.Offline)
            .OrderBy(u => u.Name)
            .ToList();

        return Task.FromResult(onlineUsers);
    }

    public Task SaveMessageAsync(ChatMessage message)
    {
        message.Id = Guid.NewGuid().ToString();
        message.Timestamp = DateTime.UtcNow;

        var messages = _roomMessages.GetOrAdd(message.Room, _ => new List<ChatMessage>());
        
        lock (messages)
        {
            messages.Add(message);
            
            // Manter apenas as últimas 1000 mensagens por sala
            if (messages.Count > 1000)
            {
                messages.RemoveAt(0);
            }
        }

        // Atualizar contador de mensagens da sala
        if (_chatRooms.TryGetValue(message.Room, out var room))
        {
            room.MessageCount++;
            room.LastActivity = DateTime.UtcNow;
        }

        _logger.LogInformation("💬 Mensagem salva na sala {Room}: {User} - {Message}", 
            message.Room, message.User, message.Message);

        return Task.CompletedTask;
    }

    public Task SavePrivateMessageAsync(ChatMessage message, string fromUserId, string toUserId)
    {
        message.Id = Guid.NewGuid().ToString();
        message.Timestamp = DateTime.UtcNow;
        message.Room = $"private_{fromUserId}_{toUserId}";

        var roomKey = GetPrivateRoomKey(fromUserId, toUserId);
        var messages = _roomMessages.GetOrAdd(roomKey, _ => new List<ChatMessage>());
        
        lock (messages)
        {
            messages.Add(message);
            
            if (messages.Count > 500)
            {
                messages.RemoveAt(0);
            }
        }

        _logger.LogInformation("🔒 Mensagem privada salva: {FromUser} -> {ToUser}", fromUserId, toUserId);
        return Task.CompletedTask;
    }

    public Task<bool> CanJoinRoomAsync(string userId, string roomId, string? password)
    {
        if (!_chatRooms.TryGetValue(roomId, out var room))
            return Task.FromResult(false);

        if (!room.IsPrivate)
            return Task.FromResult(true);

        if (_roomPasswords.TryGetValue(roomId, out var roomPassword))
        {
            return Task.FromResult(roomPassword == password);
        }

        return Task.FromResult(true);
    }

    public Task AddUserToRoomAsync(string userId, string roomId)
    {
        if (_chatRooms.TryGetValue(roomId, out var room))
        {
            if (!room.Members.Contains(userId))
            {
                room.Members.Add(userId);
                _logger.LogInformation("🚪 Usuário {UserId} entrou na sala {RoomId}", userId, roomId);
            }
        }

        return Task.CompletedTask;
    }

    public Task RemoveUserFromRoomAsync(string userId, string roomId)
    {
        if (_chatRooms.TryGetValue(roomId, out var room))
        {
            room.Members.Remove(userId);
            _logger.LogInformation("🚪 Usuário {UserId} saiu da sala {RoomId}", userId, roomId);
        }

        return Task.CompletedTask;
    }

    public Task<List<ChatMessage>> GetRecentMessagesAsync(string roomId, int count)
    {
        if (_roomMessages.TryGetValue(roomId, out var messages))
        {
            lock (messages)
            {
                return Task.FromResult(messages.TakeLast(count).ToList());
            }
        }

        return Task.FromResult(new List<ChatMessage>());
    }

    public Task UpdateUserStatusAsync(string userId, UserStatus status)
    {
        if (_onlineUsers.TryGetValue(userId, out var user))
        {
            user.Status = status;
            user.LastSeen = DateTime.UtcNow;
            _logger.LogInformation("🔄 Status do usuário {UserId} atualizado para {Status}", userId, status);
        }

        return Task.CompletedTask;
    }

    public Task CreateRoomAsync(ChatRoom room, string? password)
    {
        room.Id = room.Id.ToLowerInvariant();
        room.CreatedAt = DateTime.UtcNow;
        room.LastActivity = DateTime.UtcNow;

        _chatRooms.TryAdd(room.Id, room);

        if (!string.IsNullOrEmpty(password))
        {
            _roomPasswords.TryAdd(room.Id, password);
        }

        _logger.LogInformation("🏠 Sala criada: {RoomName} ({RoomId})", room.Name, room.Id);
        return Task.CompletedTask;
    }

    public Task KickUserFromRoomAsync(string userId, string roomId, string moderatorId, string reason)
    {
        if (_chatRooms.TryGetValue(roomId, out var room))
        {
            if (room.Admins.Contains(moderatorId))
            {
                room.Members.Remove(userId);
                _logger.LogWarning("⚠️ Usuário {UserId} foi expulso da sala {RoomId} por {ModeratorId}. Motivo: {Reason}", 
                    userId, roomId, moderatorId, reason);
            }
        }

        return Task.CompletedTask;
    }

    private static string GetPrivateRoomKey(string user1, string user2)
    {
        // Garantir ordem consistente
        var users = new[] { user1, user2 }.OrderBy(x => x).ToArray();
        return $"private_{users[0]}_{users[1]}";
    }
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly ConcurrentDictionary<string, List<Notification>> _userNotifications;
    private readonly ConcurrentDictionary<string, HashSet<string>> _readNotifications;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
        _userNotifications = new ConcurrentDictionary<string, List<Notification>>();
        _readNotifications = new ConcurrentDictionary<string, HashSet<string>>();
    }

    public Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        var userNotifs = _userNotifications.GetOrAdd(userId, _ => new List<Notification>());
        var readIds = _readNotifications.GetOrAdd(userId, _ => new HashSet<string>());

        var unread = userNotifs.Where(n => !readIds.Contains(n.Id)).ToList();
        return Task.FromResult(unread);
    }

    public Task MarkAsReadAsync(string notificationId, string userId)
    {
        var readIds = _readNotifications.GetOrAdd(userId, _ => new HashSet<string>());
        readIds.Add(notificationId);

        _logger.LogInformation("✅ Notificação {NotificationId} marcada como lida para {UserId}", notificationId, userId);
        return Task.CompletedTask;
    }

    public Task MarkAllAsReadAsync(string userId)
    {
        var userNotifs = _userNotifications.GetOrAdd(userId, _ => new List<Notification>());
        var readIds = _readNotifications.GetOrAdd(userId, _ => new HashSet<string>());

        foreach (var notif in userNotifs)
        {
            readIds.Add(notif.Id);
        }

        _logger.LogInformation("✅ Todas as notificações marcadas como lidas para {UserId}", userId);
        return Task.CompletedTask;
    }

    public Task<List<Notification>> GetNotificationHistoryAsync(string userId, int page, int pageSize)
    {
        var userNotifs = _userNotifications.GetOrAdd(userId, _ => new List<Notification>());
        
        var pagedResults = userNotifs
            .OrderByDescending(n => n.CreatedAt)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(pagedResults);
    }

    public Task SendNotificationAsync(Notification notification)
    {
        notification.Id = Guid.NewGuid().ToString();
        notification.CreatedAt = DateTime.UtcNow;

        if (notification.UserId == "all")
        {
            // Broadcast para todos os usuários
            _logger.LogInformation("📢 Notificação broadcast enviada: {Title}", notification.Title);
        }
        else
        {
            // Notificação para usuário específico
            var userNotifs = _userNotifications.GetOrAdd(notification.UserId, _ => new List<Notification>());
            userNotifs.Add(notification);

            // Manter apenas as últimas 100 notificações por usuário
            if (userNotifs.Count > 100)
            {
                userNotifs.RemoveAt(0);
            }

            _logger.LogInformation("🔔 Notificação enviada para {UserId}: {Title}", notification.UserId, notification.Title);
        }

        return Task.CompletedTask;
    }

    public Task SendBroadcastAsync(BroadcastMessage broadcast)
    {
        broadcast.Id = Guid.NewGuid().ToString();
        broadcast.CreatedAt = DateTime.UtcNow;

        _logger.LogInformation("📢 Broadcast enviado: {Title}", broadcast.Title);
        return Task.CompletedTask;
    }
}
