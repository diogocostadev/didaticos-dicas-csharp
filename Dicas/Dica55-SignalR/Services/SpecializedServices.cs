using System.Collections.Concurrent;
using Dica55_SignalR.Models;

namespace Dica55_SignalR.Services
{
    // ==========================================
    // INTERFACES DOS SERVIÇOS ESPECIALIZADOS
    // ==========================================

    public interface IMonitoringService
    {
        Task UpdateMetricAsync(LiveMetric metric);
        Task<IEnumerable<LiveMetric>> GetMetricsAsync();
        Task<PerformanceData> GetPerformanceDataAsync();
        Task StartSystemMonitoringAsync();
        Task StopSystemMonitoringAsync();
    }

    public interface ICollaborationService
    {
        Task<CollaborativeDocument> GetDocumentAsync(string documentId);
        Task ApplyEditAsync(string documentId, DocumentEdit edit);
        Task AddCollaboratorAsync(string documentId, string userId);
        Task RemoveCollaboratorAsync(string documentId, string userId);
        Task<IEnumerable<string>> GetActiveCollaboratorsAsync(string documentId);
    }

    public interface IGameService
    {
        Task<GameRoom> CreateGameAsync(string gameId, GameType gameType);
        Task<GameRoom> GetGameAsync(string gameId);
        Task AddPlayerAsync(string gameId, GamePlayer player);
        Task RemovePlayerAsync(string gameId, string playerId);
        Task UpdatePlayerStatusAsync(string gameId, string playerId, PlayerStatus status);
        Task RecordMoveAsync(string gameId, string playerId, object move);
        Task UpdateScoresAsync(string gameId, Dictionary<string, int> scores);
        Task EndGameAsync(string gameId, string? winnerId);
    }

    public interface ISignalRDemoService
    {
        Task StartAutomaticDemosAsync();
        Task StopAutomaticDemosAsync();
        Task SendRandomChatMessagesAsync();
        Task SendRandomNotificationsAsync();
        Task UpdateMonitoringDataAsync();
    }

    // ==========================================
    // IMPLEMENTAÇÕES DOS SERVIÇOS
    // ==========================================

    public class MonitoringService : IMonitoringService
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly ConcurrentDictionary<string, LiveMetric> _metrics;
        private readonly Timer _monitoringTimer;
        private readonly Random _random;

        public MonitoringService(ILogger<MonitoringService> logger)
        {
            _logger = logger;
            _metrics = new ConcurrentDictionary<string, LiveMetric>();
            _random = new Random();
            
            // Timer para atualizar métricas automaticamente
            _monitoringTimer = new Timer(UpdateSystemMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public Task UpdateMetricAsync(LiveMetric metric)
        {
            metric.Timestamp = DateTime.UtcNow;
            _metrics.AddOrUpdate(metric.Name, metric, (key, oldValue) => metric);
            
            _logger.LogInformation("📊 Métrica atualizada: {Name} = {Value} {Unit}", 
                metric.Name, metric.Value, metric.Unit);
            
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LiveMetric>> GetMetricsAsync()
        {
            return Task.FromResult(_metrics.Values.AsEnumerable());
        }

        public Task<PerformanceData> GetPerformanceDataAsync()
        {
            var performance = new PerformanceData
            {
                CpuUsage = _random.NextDouble() * 100,
                MemoryUsage = _random.NextDouble() * 16,
                ActiveConnections = _random.Next(50, 200),
                RequestsPerSecond = _random.Next(100, 1000),
                ResponseTime = _random.NextDouble() * 500,
                ErrorRate = _random.Next(0, 5),
                Timestamp = DateTime.UtcNow
            };

            return Task.FromResult(performance);
        }

        public Task StartSystemMonitoringAsync()
        {
            _logger.LogInformation("🔄 Monitoramento de sistema iniciado");
            return Task.CompletedTask;
        }

        public Task StopSystemMonitoringAsync()
        {
            _logger.LogInformation("⏹️ Monitoramento de sistema parado");
            return Task.CompletedTask;
        }

        private void UpdateSystemMetrics(object? state)
        {
            try
            {
                // Simular métricas do sistema
                var metrics = new[]
                {
                    new LiveMetric 
                    { 
                        Name = "CPU Usage", 
                        Value = Math.Round(_random.NextDouble() * 100, 2), 
                        Unit = "%",
                        Timestamp = DateTime.UtcNow
                    },
                    new LiveMetric 
                    { 
                        Name = "Memory Usage", 
                        Value = Math.Round(_random.NextDouble() * 16, 2), 
                        Unit = "GB",
                        Timestamp = DateTime.UtcNow
                    },
                    new LiveMetric 
                    { 
                        Name = "Disk I/O", 
                        Value = _random.Next(10, 100), 
                        Unit = "MB/s",
                        Timestamp = DateTime.UtcNow
                    },
                    new LiveMetric 
                    { 
                        Name = "Network Traffic", 
                        Value = _random.Next(1, 50), 
                        Unit = "Mbps",
                        Timestamp = DateTime.UtcNow
                    }
                };

                foreach (var metric in metrics)
                {
                    _metrics.AddOrUpdate(metric.Name, metric, (key, oldValue) => metric);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao atualizar métricas do sistema");
            }
        }
    }

    public class CollaborationService : ICollaborationService
    {
        private readonly ILogger<CollaborationService> _logger;
        private readonly ConcurrentDictionary<string, CollaborativeDocument> _documents;
        private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _documentCollaborators;

        public CollaborationService(ILogger<CollaborationService> logger)
        {
            _logger = logger;
            _documents = new ConcurrentDictionary<string, CollaborativeDocument>();
            _documentCollaborators = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        }

        public Task<CollaborativeDocument> GetDocumentAsync(string documentId)
        {
            var document = _documents.GetOrAdd(documentId, id => new CollaborativeDocument
            {
                Id = id,
                Title = $"Documento {id}",
                Content = "# Documento Colaborativo\n\nInicie sua edição aqui...",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                Version = 1
            });

            return Task.FromResult(document);
        }

        public Task ApplyEditAsync(string documentId, DocumentEdit edit)
        {
            var document = _documents.GetOrAdd(documentId, id => new CollaborativeDocument
            {
                Id = id,
                Title = $"Documento {id}",
                Content = "",
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                Version = 1
            });

            // Aplicar a edição ao conteúdo
            try
            {
                switch (edit.Operation)
                {
                    case EditOperation.Insert:
                        if (edit.Position <= document.Content.Length)
                        {
                            document.Content = document.Content.Insert(edit.Position, edit.Content);
                        }
                        break;
                    
                    case EditOperation.Delete:
                        if (edit.Position < document.Content.Length)
                        {
                            var length = Math.Min(edit.Length ?? 1, document.Content.Length - edit.Position);
                            document.Content = document.Content.Remove(edit.Position, length);
                        }
                        break;
                    
                    case EditOperation.Replace:
                        if (edit.Position < document.Content.Length)
                        {
                            var length = Math.Min(edit.Length ?? 1, document.Content.Length - edit.Position);
                            document.Content = document.Content.Remove(edit.Position, length)
                                                             .Insert(edit.Position, edit.Content);
                        }
                        break;
                }

                document.LastModified = DateTime.UtcNow;
                document.Version++;
                
                _logger.LogInformation("📝 Edição aplicada no documento {DocumentId} por {UserId}: {Operation} na posição {Position}", 
                    documentId, edit.UserId, edit.Operation, edit.Position);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao aplicar edição no documento {DocumentId}", documentId);
            }

            return Task.CompletedTask;
        }

        public Task AddCollaboratorAsync(string documentId, string userId)
        {
            var collaborators = _documentCollaborators.GetOrAdd(documentId, _ => new ConcurrentBag<string>());
            
            if (!collaborators.Contains(userId))
            {
                collaborators.Add(userId);
                _logger.LogInformation("👥 Colaborador {UserId} adicionado ao documento {DocumentId}", userId, documentId);
            }

            return Task.CompletedTask;
        }

        public Task RemoveCollaboratorAsync(string documentId, string userId)
        {
            if (_documentCollaborators.TryGetValue(documentId, out var collaborators))
            {
                // Criar nova lista sem o usuário (ConcurrentBag não tem Remove)
                var newCollaborators = new ConcurrentBag<string>(
                    collaborators.Where(c => c != userId));
                
                _documentCollaborators.TryUpdate(documentId, newCollaborators, collaborators);
                
                _logger.LogInformation("👋 Colaborador {UserId} removido do documento {DocumentId}", userId, documentId);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> GetActiveCollaboratorsAsync(string documentId)
        {
            if (_documentCollaborators.TryGetValue(documentId, out var collaborators))
            {
                return Task.FromResult(collaborators.AsEnumerable());
            }

            return Task.FromResult(Enumerable.Empty<string>());
        }
    }

    public class GameService : IGameService
    {
        private readonly ILogger<GameService> _logger;
        private readonly ConcurrentDictionary<string, GameRoom> _games;
        private readonly ConcurrentDictionary<string, List<object>> _gameMoves;

        public GameService(ILogger<GameService> logger)
        {
            _logger = logger;
            _games = new ConcurrentDictionary<string, GameRoom>();
            _gameMoves = new ConcurrentDictionary<string, List<object>>();
        }

        public Task<GameRoom> CreateGameAsync(string gameId, GameType gameType)
        {
            var game = new GameRoom
            {
                Id = gameId,
                Type = gameType,
                Status = GameStatus.Waiting,
                CreatedAt = DateTime.UtcNow,
                MaxPlayers = gameType switch
                {
                    GameType.TicTacToe => 2,
                    GameType.Checkers => 2,
                    GameType.Multiplayer => 10,
                    _ => 4
                },
                Players = new List<GamePlayer>()
            };

            _games.TryAdd(gameId, game);
            _gameMoves.TryAdd(gameId, new List<object>());

            _logger.LogInformation("🎮 Jogo criado: {GameId} do tipo {GameType}", gameId, gameType);
            return Task.FromResult(game);
        }

        public Task<GameRoom> GetGameAsync(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return Task.FromResult(game ?? throw new ArgumentException($"Jogo {gameId} não encontrado"));
        }

        public Task AddPlayerAsync(string gameId, GamePlayer player)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                if (!game.Players.Any(p => p.Id == player.Id))
                {
                    if (game.Players.Count < game.MaxPlayers)
                    {
                        player.JoinedAt = DateTime.UtcNow;
                        game.Players.Add(player);
                        
                        _logger.LogInformation("👤 Jogador {PlayerId} entrou no jogo {GameId}", player.Id, gameId);

                        // Iniciar jogo se atingir número mínimo de jogadores
                        if (game.Players.Count >= 2 && game.Status == GameStatus.Waiting)
                        {
                            game.Status = GameStatus.InProgress;
                            game.StartedAt = DateTime.UtcNow;
                            _logger.LogInformation("🚀 Jogo {GameId} iniciado com {PlayerCount} jogadores", gameId, game.Players.Count);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Jogo {gameId} está cheio");
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task RemovePlayerAsync(string gameId, string playerId)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                var player = game.Players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                {
                    game.Players.Remove(player);
                    _logger.LogInformation("👋 Jogador {PlayerId} saiu do jogo {GameId}", playerId, gameId);

                    // Finalizar jogo se não houver jogadores suficientes
                    if (game.Players.Count < 2 && game.Status == GameStatus.InProgress)
                    {
                        game.Status = GameStatus.Ended;
                        game.EndedAt = DateTime.UtcNow;
                        _logger.LogInformation("⏹️ Jogo {GameId} finalizado por falta de jogadores", gameId);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task UpdatePlayerStatusAsync(string gameId, string playerId, PlayerStatus status)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                var player = game.Players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                {
                    player.Status = status;
                    _logger.LogInformation("🔄 Status do jogador {PlayerId} no jogo {GameId} atualizado para {Status}", 
                        playerId, gameId, status);
                }
            }

            return Task.CompletedTask;
        }

        public Task RecordMoveAsync(string gameId, string playerId, object move)
        {
            if (_gameMoves.TryGetValue(gameId, out var moves))
            {
                moves.Add(new 
                { 
                    PlayerId = playerId, 
                    Move = move, 
                    Timestamp = DateTime.UtcNow 
                });

                _logger.LogInformation("🎯 Jogada registrada no jogo {GameId} pelo jogador {PlayerId}", gameId, playerId);
            }

            return Task.CompletedTask;
        }

        public Task UpdateScoresAsync(string gameId, Dictionary<string, int> scores)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                foreach (var player in game.Players)
                {
                    if (scores.ContainsKey(player.Id))
                    {
                        player.Score = scores[player.Id];
                    }
                }

                _logger.LogInformation("🏆 Pontuações atualizadas no jogo {GameId}", gameId);
            }

            return Task.CompletedTask;
        }

        public Task EndGameAsync(string gameId, string? winnerId)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.Status = GameStatus.Ended;
                game.EndedAt = DateTime.UtcNow;
                game.WinnerId = winnerId;

                _logger.LogInformation("🏁 Jogo {GameId} finalizado. Vencedor: {WinnerId}", gameId, winnerId ?? "Empate");
            }

            return Task.CompletedTask;
        }
    }

    public class SignalRDemoService : ISignalRDemoService
    {
        private readonly ILogger<SignalRDemoService> _logger;
        private readonly Random _random;
        private Timer? _demoTimer;

        public SignalRDemoService(ILogger<SignalRDemoService> logger)
        {
            _logger = logger;
            _random = new Random();
        }

        public Task StartAutomaticDemosAsync()
        {
            _demoTimer = new Timer(async _ => await RunDemoTasks(), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
            _logger.LogInformation("🤖 Demonstrações automáticas iniciadas");
            return Task.CompletedTask;
        }

        public Task StopAutomaticDemosAsync()
        {
            _demoTimer?.Dispose();
            _logger.LogInformation("⏹️ Demonstrações automáticas paradas");
            return Task.CompletedTask;
        }

        public Task SendRandomChatMessagesAsync()
        {
            var messages = new[]
            {
                "Olá! Como estão todos?",
                "SignalR é incrível para comunicação em tempo real!",
                "Quem quer fazer um chat colaborativo?",
                "As possibilidades com WebSockets são infinitas!",
                "Vamos construir algo incrível juntos!"
            };

            var message = messages[_random.Next(messages.Length)];
            _logger.LogInformation("💬 Mensagem aleatória do chat: {Message}", message);
            
            return Task.CompletedTask;
        }

        public Task SendRandomNotificationsAsync()
        {
            var notifications = new[]
            {
                "Nova mensagem recebida",
                "Sistema atualizado com sucesso",
                "Backup concluído",
                "Novo usuário conectado",
                "Relatório mensal disponível"
            };

            var notification = notifications[_random.Next(notifications.Length)];
            _logger.LogInformation("🔔 Notificação aleatória: {Notification}", notification);
            
            return Task.CompletedTask;
        }

        public Task UpdateMonitoringDataAsync()
        {
            var metrics = new[]
            {
                $"CPU: {_random.Next(10, 90)}%",
                $"Memória: {_random.Next(2, 12)}GB",
                $"Conexões: {_random.Next(50, 200)}",
                $"Requisições/s: {_random.Next(100, 1000)}"
            };

            _logger.LogInformation("📊 Dados de monitoramento atualizados: {Metrics}", string.Join(", ", metrics));
            return Task.CompletedTask;
        }

        private async Task RunDemoTasks()
        {
            try
            {
                await SendRandomChatMessagesAsync();
                await Task.Delay(10000);
                
                await SendRandomNotificationsAsync();
                await Task.Delay(10000);
                
                await UpdateMonitoringDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro nas demonstrações automáticas");
            }
        }
    }
}
