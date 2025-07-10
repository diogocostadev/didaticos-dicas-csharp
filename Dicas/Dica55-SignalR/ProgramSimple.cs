using Microsoft.AspNetCore.SignalR;
using Dica55_SignalR.Hubs;
using Dica55_SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURAÇÃO DE SERVIÇOS
// ==========================================

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configurar SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
})
.AddMessagePackProtocol();

// Registrar serviços
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// ==========================================
// CONFIGURAÇÃO DO PIPELINE
// ==========================================

app.UseCors();

app.UseRouting();

// Mapear Hubs do SignalR
app.MapHub<ChatHub>("/hub/chat");

// ==========================================
// ENDPOINTS BÁSICOS
// ==========================================

app.MapGet("/", () => """
🎯 Dica 55: SignalR - Comunicação em Tempo Real

🔥 HUBS DISPONÍVEIS:
• /hub/chat              - Chat em tempo real

📊 DEMONSTRAÇÕES PRÁTICAS:
• GET  /demo/chat-simulation     - Simular chat automatizado

🌐 CLIENTES DE TESTE:
• GET /client/chat       - Cliente de chat básico

💡 OBJETIVO: Demonstrar comunicação bidirecional em tempo real
""")
.WithTags("Home")
.WithSummary("Página inicial com demonstrações SignalR");

// Simulação de chat
app.MapGet("/demo/chat-simulation", async (IHubContext<ChatHub> hubContext, IChatService chatService) =>
{
    var messages = new[]
    {
        new { user = "Bot1", message = "Olá pessoal! Como estão?" },
        new { user = "Bot2", message = "Tudo bem! Trabalhando com SignalR hoje." },
        new { user = "Bot3", message = "SignalR é incrível para tempo real!" }
    };

    foreach (var msg in messages)
    {
        await hubContext.Clients.All.SendAsync("ReceiveMessage", new 
        { 
            user = msg.user, 
            message = msg.message, 
            timestamp = DateTime.UtcNow 
        });
        
        await Task.Delay(2000);
    }

    return Results.Ok(new { message = "Simulação concluída", messagesCount = messages.Length });
});

// Cliente de teste
app.MapGet("/client/chat", () => Results.Content("""
<!DOCTYPE html>
<html>
<head>
    <title>Chat Cliente - SignalR</title>
    <script src="https://unpkg.com/@microsoft/signalr/dist/browser/signalr.min.js"></script>
</head>
<body>
    <h1>🎯 Chat Cliente SignalR</h1>
    <div id="status">Desconectado</div>
    <div>
        <input type="text" id="userInput" placeholder="Seu nome" />
        <button onclick="connect()">Conectar</button>
    </div>
    <div>
        <input type="text" id="messageInput" placeholder="Digite sua mensagem" />
        <button onclick="sendMessage()">Enviar</button>
    </div>
    <div id="messages"></div>

    <script>
        let connection = null;
        
        async function connect() {
            const user = document.getElementById('userInput').value;
            if (!user) return;
            
            connection = new signalR.HubConnectionBuilder()
                .withUrl("/hub/chat")
                .build();
                
            connection.on("ReceiveMessage", (message) => {
                const messages = document.getElementById('messages');
                messages.innerHTML += `<div><strong>${message.user}:</strong> ${message.message}</div>`;
            });
            
            await connection.start();
            document.getElementById('status').innerText = 'Conectado como ' + user;
        }
        
        async function sendMessage() {
            const message = document.getElementById('messageInput').value;
            if (!message || !connection) return;
            
            await connection.invoke("SendMessage", message);
            document.getElementById('messageInput').value = '';
        }
    </script>
</body>
</html>
""", "text/html"))
.WithTags("Test Clients");

Console.WriteLine("""

🎯 DICA 55: SIGNALR - COMUNICAÇÃO EM TEMPO REAL
═══════════════════════════════════════════════════════════════

🔥 RECURSOS IMPLEMENTADOS:
• Chat em tempo real com SignalR
• WebSockets com fallback automático
• Broadcast de mensagens
• Clientes JavaScript integrados

⚡ COMO USAR:
1. Acesse http://localhost:5000 para ver as opções
2. Use /client/chat para testar chat básico
3. Execute /demo/chat-simulation para ver automação

💡 OBJETIVO: Demonstrar comunicação bidirecional em tempo real
═══════════════════════════════════════════════════════════════

""");

app.Run();
