# 📋 Guia Completo de Boas Práticas C# / .NET

> **Instruções para Agentes de IA**: Este documento contém regras obrigatórias para desenvolvimento em C#/.NET. Siga estas práticas em TODO código gerado. As dicas estão organizadas por categoria e prioridade.

---

# 🔴 SEÇÃO 1: REGRAS CRÍTICAS (NUNCA VIOLAR)

## 1.1 HttpClient - Socket Exhaustion (Dica 32, 37)
```csharp
// ❌ NUNCA: Criar HttpClient em cada requisição
using var client = new HttpClient();
await client.GetAsync(url);
// PROBLEMA: Socket fica em TIME_WAIT por 240 segundos. Em alta carga, esgota sockets!

// ❌ NUNCA: HttpClient estático sem PooledConnectionLifetime
private static readonly HttpClient _client = new HttpClient();
// PROBLEMA: DNS nunca é atualizado!

// ✅ SEMPRE: Usar IHttpClientFactory
public class MeuServico
{
    private readonly HttpClient _client;
    public MeuServico(IHttpClientFactory factory) 
    {
        _client = factory.CreateClient("NomeDoClient");
    }
}

// ✅ OU: Cliente de longa duração com PooledConnectionLifetime
private static readonly HttpClient _client = new HttpClient(new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
});

// ✅ CONFIGURAÇÃO COMPLETA COM FACTORY:
services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://api.exemplo.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "MeuApp/1.0");
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15),
    MaxConnectionsPerServer = 100
});
```

## 1.2 Async/Await - Deadlocks (Dica 3, 26, 27, 35)
```csharp
// ❌ NUNCA: Bloquear código assíncrono
var result = GetDataAsync().Result;                    // DEADLOCK!
var result = GetDataAsync().GetAwaiter().GetResult();  // DEADLOCK!
GetDataAsync().Wait();                                 // DEADLOCK!

// ✅ SEMPRE: Async all the way
var result = await GetDataAsync();

// ✅ EM BIBLIOTECAS: Usar ConfigureAwait(false)
public async Task<string> ProcessarAsync()
{
    var data = await _httpClient.GetAsync(url).ConfigureAwait(false);
    var json = await data.Content.ReadAsStringAsync().ConfigureAwait(false);
    return json;
}

// ✅ PARA CPU-BOUND: Use Task.Run
var resultado = await Task.Run(() => ProcessamentoIntensivo(dados));

// ✅ PARA PARALELISMO: Use Task.WhenAll
var tarefas = ids.Select(id => ProcessarItemAsync(id));
var resultados = await Task.WhenAll(tarefas);
```

## 1.3 Exceções - Stack Trace (Dica 2)
```csharp
// ❌ NUNCA: Relançar com variável (perde stack trace)
catch (Exception ex)
{
    _logger.LogError(ex, "Erro");
    throw ex;  // PERDE STACK TRACE!
}

// ✅ SEMPRE: Usar throw; sem variável
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar {Id}", id);
    throw;  // PRESERVA STACK TRACE
}

// ✅ OU: Encapsular em nova exceção (preserva inner)
catch (Exception ex)
{
    throw new ProcessamentoException($"Falha ao processar {id}", ex);
}
```

## 1.4 Catch Vazio - Exceções Engolidas (Dica 101)
```csharp
// ❌ NUNCA: Catch vazio ou silencioso
catch (Exception) { }
catch (Exception) { return null; }
catch (Exception) { return false; }
catch (Exception ex) { Console.WriteLine(ex.Message); }  // Ninguém vê em produção!

// ✅ SEMPRE: Log + tratamento adequado
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao processar pedido {PedidoId}", pedidoId);
    throw;  // ou return Result.Failure("mensagem");
}

// ✅ TRATAMENTO ESPECÍFICO COM FALLBACK:
catch (SmtpException ex)
{
    _logger.LogWarning(ex, "Falha SMTP para {Email}", email);
    await EnviarViaSmsAsync(telefone, mensagem);  // Fallback
}
catch (TimeoutException ex)
{
    _logger.LogError(ex, "Timeout ao enviar notificação");
    await AgendarRetentativaAsync(destinatario, mensagem);
}

// ✅ RESULT PATTERN PARA OPERAÇÕES QUE PODEM FALHAR:
public async Task<Result<Pedido>> ProcessarPedidoAsync(string id)
{
    try
    {
        var pedido = await _repository.BuscarAsync(id);
        return Result<Pedido>.Success(pedido);
    }
    catch (PedidoNaoEncontradoException ex)
    {
        _logger.LogWarning(ex, "Pedido {Id} não encontrado", id);
        return Result<Pedido>.Failure("Pedido não encontrado");
    }
}
```

## 1.5 Thread.Sleep vs Task.Delay (Dica 26)
```csharp
// ❌ NUNCA: Thread.Sleep em código async
await SomeOperationAsync();
Thread.Sleep(1000);  // BLOQUEIA A THREAD!

// ✅ SEMPRE: Task.Delay em código async
await SomeOperationAsync();
await Task.Delay(1000);  // Não bloqueia

// ✅ COM CANCELLATION:
await Task.Delay(1000, cancellationToken);
```

## 1.6 Travamento com async/await (Dica 3, 30)
```csharp
// ❌ NUNCA: lock com await dentro
lock (_syncObject)
{
    await DoSomethingAsync();  // ERRO DE COMPILAÇÃO ou DEADLOCK!
}

// ✅ SEMPRE: SemaphoreSlim para código async
private readonly SemaphoreSlim _semaphore = new(1, 1);

await _semaphore.WaitAsync(cancellationToken);
try
{
    await DoSomethingAsync();
}
finally
{
    _semaphore.Release();
}

// ❌ NUNCA: Monitor/lock recursivo sem cuidado (Dica 30)
// Monitor é "reentrant" - pode causar problemas sutis
```

---

# 🟡 SEÇÃO 2: REGRAS IMPORTANTES (Alta Prioridade)

## 2.1 Logging Estruturado (Dica 7)
```csharp
// ❌ NUNCA: String interpolation em logs
_logger.LogInformation($"Usuário {userId} fez login às {DateTime.Now}");
_logger.LogError($"Erro: {ex.Message}");

// ✅ SEMPRE: Message templates com parâmetros
_logger.LogInformation("Usuário {UserId} fez login às {LoginTime}", userId, DateTime.Now);
_logger.LogError(ex, "Erro ao processar pedido {PedidoId}", pedidoId);

// ✅ COM SCOPES PARA CONTEXTO:
using (_logger.BeginScope("CorrelationId: {CorrelationId}", correlationId))
{
    _logger.LogInformation("Iniciando processamento");
    // Todos os logs dentro terão o CorrelationId
}

// ✅ COM EVENT IDS:
private static readonly EventId PedidoProcessado = new(1001, "PedidoProcessado");
_logger.LogInformation(PedidoProcessado, "Pedido {PedidoId} processado", pedidoId);
```

## 2.2 Coleções Vazias (Dica 1)
```csharp
// ❌ NUNCA: Criar nova instância para coleção vazia
return new List<Item>();    // Aloca memória desnecessária
return new Item[0];         // Aloca memória desnecessária

// ✅ SEMPRE: Usar métodos estáticos
return Array.Empty<Item>();       // Zero alocação, singleton
return Enumerable.Empty<Item>();  // Zero alocação
return [];                        // C# 12+ - Compilador otimiza
```

## 2.3 Nullable Reference Types (Dica 24)
```csharp
// ❌ NUNCA: Usar null! sem necessidade real
string nome = null!;  // Esconde problema

// ✅ SEMPRE: Declarar nullability corretamente
string? nome = null;  // Nullable explícito
string nome = "";     // Não nullable com valor padrão

// ✅ SEMPRE: Verificar null antes de usar
if (usuario?.Email is not null)
{
    await EnviarEmail(usuario.Email);
}

// ✅ OPERADORES ÚTEIS:
var nome = usuario?.Nome ?? "Desconhecido";  // Null coalescing
usuario?.Notificar();                         // Null conditional
nomeCompleto ??= CalcularNome();             // Null coalescing assignment (Dica 42)
```

## 2.4 IDisposable e Using (Dica vários)
```csharp
// ❌ NUNCA: Esquecer de fazer Dispose
var stream = new FileStream(path, FileMode.Open);
// ... usa stream
// Esqueceu de fechar!

// ✅ SEMPRE: Usar using statement (C# 8+)
using var stream = new FileStream(path, FileMode.Open);
// Dispose automático no fim do escopo

// ✅ OU: using block tradicional
using (var stream = new FileStream(path, FileMode.Open))
{
    // ...
}

// ✅ PARA ASYNC:
await using var connection = new SqlConnection(connectionString);
```

## 2.5 LINQ - Materialização (Dica 4, 47, 91)
```csharp
// ❌ CUIDADO: Múltiplas enumerações (Deferred Execution)
var items = GetItems().Where(x => x.Active);  // Ainda não executou!
var count = items.Count();      // Enumera 1x (vai ao banco)
var first = items.FirstOrDefault(); // Enumera 2x (vai ao banco DE NOVO)
var list = items.ToList();      // Enumera 3x (vai ao banco DE NOVO)

// ✅ MELHOR: Materializar uma vez
var items = GetItems().Where(x => x.Active).ToList();  // Executa UMA vez
var count = items.Count;         // Usa lista em memória
var first = items.FirstOrDefault();  // Usa lista em memória

// ✅ PARA PERFORMANCE: Escolher método certo
items.Any()       // Melhor que Count() > 0
items.FirstOrDefault()  // Melhor que Where().First()
items.Find(x => x.Id == id)  // Para List<T>, mais rápido que FirstOrDefault
```

## 2.6 ToList vs ToArray (Dica 9)
```csharp
// ✅ PREFERIR ToArray quando:
// - Tamanho final é conhecido ou estimado
// - Não vai adicionar/remover itens
// - Quer menor overhead de memória
var array = items.ToArray();

// ✅ PREFERIR ToList quando:
// - Vai modificar a coleção depois
// - Tamanho vai variar
var list = items.ToList();

// ✅ MELHOR AINDA: Especificar capacidade quando conhecida
var list = new List<Item>(expectedCount);
```

## 2.7 String Performance (Dica 25, 41)
```csharp
// ❌ EVITAR: Concatenação em loop
string result = "";
foreach (var item in items)
{
    result += item.Name + ", ";  // Aloca nova string a cada iteração!
}

// ✅ PREFERIR: StringBuilder para muitas concatenações
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.Append(item.Name).Append(", ");
}
var result = sb.ToString();

// ✅ PREFERIR: String.Join para coleções
var result = string.Join(", ", items.Select(x => x.Name));

// ✅ INTERPOLAÇÃO É OK para poucas strings:
var msg = $"Usuário {nome} criado às {data:HH:mm}";  // Compilador otimiza
```

## 2.8 DateTime vs DateTimeOffset (Dica 23)
```csharp
// ❌ EVITAR: DateTime para dados que viajam entre sistemas
var agora = DateTime.Now;  // Sem informação de timezone!

// ✅ PREFERIR: DateTimeOffset para APIs e banco de dados
var agora = DateTimeOffset.UtcNow;  // Inclui offset UTC

// ✅ PARA COMPARAÇÕES DE TEMPO:
var inicio = Stopwatch.GetTimestamp();  // Alta precisão
// ... operação
var elapsed = Stopwatch.GetElapsedTime(inicio);
```

## 2.9 CancellationToken (Dica 15)
```csharp
// ❌ EVITAR: Operações longas sem suporte a cancelamento
public async Task ProcessarAsync()
{
    await Task.Delay(60000);  // Não pode cancelar!
}

// ✅ SEMPRE: Aceitar e propagar CancellationToken
public async Task ProcessarAsync(CancellationToken ct = default)
{
    await Task.Delay(60000, ct);  // Pode cancelar!
    
    ct.ThrowIfCancellationRequested();  // Check manual
    
    await _httpClient.GetAsync(url, ct);  // Propagar
}

// ✅ LINKED TOKENS para combinar timeouts:
using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
cts.CancelAfter(TimeSpan.FromSeconds(30));
await OperacaoAsync(cts.Token);
```

## 2.10 Polly - Resiliência (Dica 43)
```csharp
// ❌ EVITAR: Sem retry em chamadas HTTP
var response = await _client.GetAsync(url);

// ✅ PREFERIR: Configurar políticas de resiliência
services.AddHttpClient("ApiClient")
    .AddPolicyHandler(Policy
        .HandleTransientHttpErrors()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
    .AddPolicyHandler(Policy
        .HandleTransientHttpErrors()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// ✅ POLÍTICAS COMBINADAS:
var policy = Policy.WrapAsync(
    Policy.TimeoutAsync(TimeSpan.FromSeconds(10)),
    Policy.Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(i)),
    Policy.Handle<HttpRequestException>()
        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1))
);
```

---

# 🟢 SEÇÃO 3: BOAS PRÁTICAS RECOMENDADAS

## 3.1 Record Types para DTOs (Dica 58)
```csharp
// ❌ VERBOSO: Classe com boilerplate
public class PessoaDto
{
    public string Nome { get; set; }
    public int Idade { get; set; }
    // Equals, GetHashCode, ToString...
}

// ✅ PREFERIR: Record para imutabilidade
public record PessoaDto(string Nome, int Idade);

// ✅ RECORD COM PROPRIEDADES ADICIONAIS:
public record PedidoDto(int Id, string Cliente)
{
    public DateTime CriadoEm { get; init; } = DateTime.UtcNow;
}

// ✅ RECORD STRUCT para value types pequenos:
public readonly record struct Coordenada(double Lat, double Lng);
```

## 3.2 Pattern Matching (Dica 39)
```csharp
// ❌ VERBOSO: Múltiplos if/else
if (animal is Dog)
{
    var dog = (Dog)animal;
    dog.Bark();
}

// ✅ PREFERIR: Pattern matching
if (animal is Dog dog)
{
    dog.Bark();
}

// ✅ SWITCH EXPRESSION:
var sound = animal switch
{
    Dog d when d.Size == "Large" => d.LoudBark(),
    Dog d => d.Bark(),
    Cat { Age: > 10 } c => c.SleepyMeow(),
    Cat c => c.Meow(),
    null => throw new ArgumentNullException(nameof(animal)),
    _ => "Unknown"
};

// ✅ PROPERTY PATTERNS:
if (pessoa is { Idade: >= 18, Ativo: true })
{
    // Pessoa adulta e ativa
}
```

## 3.3 Primary Constructors (Dica 12) - C# 12+
```csharp
// ❌ VERBOSO: Construtor tradicional
public class PedidoService
{
    private readonly IRepository _repository;
    private readonly ILogger<PedidoService> _logger;
    
    public PedidoService(IRepository repository, ILogger<PedidoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}

// ✅ PREFERIR: Primary constructor
public class PedidoService(IRepository repository, ILogger<PedidoService> logger)
{
    public async Task ProcessarAsync(int id)
    {
        logger.LogInformation("Processando {Id}", id);
        await repository.SalvarAsync(id);
    }
}
```

## 3.4 Collection Expressions (Dica 16) - C# 12+
```csharp
// ❌ ANTIGO:
var lista = new List<int> { 1, 2, 3 };
var array = new int[] { 1, 2, 3 };

// ✅ NOVO C# 12+:
List<int> lista = [1, 2, 3];
int[] array = [1, 2, 3];
Span<int> span = [1, 2, 3];

// ✅ SPREAD OPERATOR:
int[] combinado = [..array1, ..array2, 99];
```

## 3.5 Span<T> e Memory<T> (Dica 6, 40)
```csharp
// ❌ EVITAR: Substring para parsing (aloca)
var header = line.Substring(0, 10);

// ✅ PREFERIR: Span para operações sem alocação
ReadOnlySpan<char> header = line.AsSpan(0, 10);

// ✅ PARA ARRAYS:
Span<byte> buffer = stackalloc byte[256];  // Stack allocation
var slice = buffer.Slice(0, 10);

// ✅ ACESSANDO SPAN DE LISTA (Dica 6):
var list = new List<int> { 1, 2, 3, 4, 5 };
Span<int> span = CollectionsMarshal.AsSpan(list);
```

## 3.6 Stackalloc (Dica 48)
```csharp
// ❌ EVITAR: Alocação no heap para buffers pequenos
var buffer = new byte[128];

// ✅ PREFERIR: stackalloc para buffers pequenos
Span<byte> buffer = stackalloc byte[128];

// ✅ COM LIMITE DE SEGURANÇA:
Span<byte> buffer = size <= 256 
    ? stackalloc byte[size] 
    : new byte[size];
```

## 3.7 ValueTask vs Task (Dica 8, 73)
```csharp
// ✅ USE Task quando:
// - Operação é sempre assíncrona
// - Resultado pode ser awaited múltiplas vezes
public async Task<Data> GetDataAsync() => await _repository.GetAsync();

// ✅ USE ValueTask quando:
// - Operação frequentemente retorna de forma síncrona (cache)
// - Performance é crítica
// - Resultado será awaited apenas UMA vez
public ValueTask<Data> GetCachedDataAsync()
{
    if (_cache.TryGet(out var data))
        return ValueTask.FromResult(data);  // Sem alocação!
    
    return new ValueTask<Data>(LoadFromDatabaseAsync());
}

// ❌ NUNCA: Await ValueTask múltiplas vezes
var vt = GetDataAsync();
await vt;
await vt;  // COMPORTAMENTO INDEFINIDO!
```

## 3.8 IAsyncEnumerable (Dica 16-BACKUP)
```csharp
// ✅ PARA STREAMING DE DADOS:
public async IAsyncEnumerable<Item> GetItemsAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    await foreach (var item in _repository.StreamAsync(ct))
    {
        yield return item;
    }
}

// ✅ CONSUMINDO:
await foreach (var item in GetItemsAsync(ct))
{
    await ProcessarAsync(item);
}
```

## 3.9 Channels (Dica 38)
```csharp
// ✅ PARA PRODUCER-CONSUMER:
var channel = Channel.CreateBounded<Message>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait
});

// Producer
await channel.Writer.WriteAsync(message, ct);

// Consumer
await foreach (var msg in channel.Reader.ReadAllAsync(ct))
{
    await ProcessarAsync(msg);
}
```

## 3.10 nameof Operator (Dica 62, 82)
```csharp
// ❌ NUNCA: Strings hard-coded para nomes
throw new ArgumentNullException("parametro");
_logger.LogError("Erro no método ProcessarPedido");

// ✅ SEMPRE: nameof para refactoring-safe
throw new ArgumentNullException(nameof(parametro));
_logger.LogError("Erro no método {Metodo}", nameof(ProcessarPedido));

// ✅ PARA PROPRIEDADES:
public event PropertyChangedEventHandler? PropertyChanged;
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nome)));
```

## 3.11 Target-Typed New (Dica 59)
```csharp
// ❌ VERBOSO:
Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();

// ✅ PREFERIR:
Dictionary<string, List<int>> dict = new();
List<Item> items = new() { new Item(), new Item() };
```

## 3.12 Dependency Injection (Dica 61)
```csharp
// ✅ LIFETIMES CORRETOS:
services.AddSingleton<ICache>();      // Uma instância para toda aplicação
services.AddScoped<IRepository>();    // Uma instância por request/scope
services.AddTransient<IValidator>();  // Nova instância sempre

// ❌ CUIDADO: Captive Dependency
// Singleton que depende de Scoped = Bug!
services.AddSingleton<MeuSingleton>();  // Singleton
services.AddScoped<MeuScoped>();        // Scoped
// Se MeuSingleton injetar MeuScoped, vai usar mesma instância sempre!

// ✅ KEYED SERVICES (.NET 8+):
services.AddKeyedSingleton<INotifier, EmailNotifier>("email");
services.AddKeyedSingleton<INotifier, SmsNotifier>("sms");

public class Service([FromKeyedServices("email")] INotifier notifier) { }
```

## 3.13 Options Pattern (Dica 81)
```csharp
// ✅ CONFIGURAÇÃO TIPADA:
public class ApiSettings
{
    public const string SectionName = "Api";
    public string BaseUrl { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;
}

// ✅ REGISTRO:
services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));

// ✅ USO:
public class ApiService(IOptions<ApiSettings> options)
{
    private readonly ApiSettings _settings = options.Value;
}

// ✅ PARA RELOAD AUTOMÁTICO:
public class ApiService(IOptionsMonitor<ApiSettings> options)
{
    // options.CurrentValue sempre atualizado
}
```

## 3.14 Health Checks (Dica 64)
```csharp
// ✅ REGISTRAR HEALTH CHECKS:
services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "database")
    .AddRedis(redisConnection, name: "cache")
    .AddUrlGroup(new Uri("https://api.externa.com"), name: "api-externa");

// ✅ ENDPOINT:
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

## 3.15 FluentValidation (Dica 50)
```csharp
// ✅ VALIDAÇÃO DECLARATIVA:
public class PedidoValidator : AbstractValidator<Pedido>
{
    public PedidoValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty()
            .WithMessage("Cliente é obrigatório");
            
        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser positivo");
            
        RuleFor(x => x.Itens)
            .NotEmpty()
            .ForEach(item => item.SetValidator(new ItemValidator()));
    }
}

// ✅ REGISTRO:
services.AddValidatorsFromAssemblyContaining<PedidoValidator>();
```

## 3.16 MediatR (Dica 44)
```csharp
// ✅ CQRS COM MEDIATOR:
public record CriarPedidoCommand(int ClienteId, List<Item> Itens) : IRequest<int>;

public class CriarPedidoHandler : IRequestHandler<CriarPedidoCommand, int>
{
    public async Task<int> Handle(CriarPedidoCommand request, CancellationToken ct)
    {
        // Implementação
        return pedidoId;
    }
}

// ✅ USO:
var pedidoId = await _mediator.Send(new CriarPedidoCommand(clienteId, itens));

// ✅ BEHAVIORS PARA CROSS-CUTTING:
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Logging automático para todos os handlers
}
```

## 3.17 Refit (Dica 34)
```csharp
// ✅ API CLIENT DECLARATIVO:
public interface IGitHubApi
{
    [Get("/users/{username}")]
    Task<User> GetUserAsync(string username);
    
    [Post("/repos/{owner}/{repo}/issues")]
    Task<Issue> CreateIssueAsync(string owner, string repo, [Body] CreateIssue issue);
}

// ✅ REGISTRO:
services.AddRefitClient<IGitHubApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com"));
```

## 3.18 Background Services (Dica 70)
```csharp
// ✅ WORKER SERVICE:
public class ProcessadorFila : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarProximoItemAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Erro no processamento");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
```

## 3.19 Comparação de Strings (várias dicas)
```csharp
// ❌ EVITAR: ToLower() para comparação
if (input.ToLower() == "admin")  // Aloca nova string!

// ✅ PREFERIR: StringComparison
if (input.Equals("admin", StringComparison.OrdinalIgnoreCase))
if (input.Contains("admin", StringComparison.OrdinalIgnoreCase))
if (input.StartsWith("admin", StringComparison.OrdinalIgnoreCase))
```

## 3.20 Paralelismo Controlado (Dica 27)
```csharp
// ❌ EVITAR: Task.WhenAll sem limite
var tasks = urls.Select(url => ProcessUrlAsync(url));  // 10000 tasks paralelas!
await Task.WhenAll(tasks);

// ✅ PREFERIR: SemaphoreSlim para controlar concorrência
var semaphore = new SemaphoreSlim(10);  // Máximo 10 paralelas
var tasks = urls.Select(async url =>
{
    await semaphore.WaitAsync();
    try
    {
        return await ProcessUrlAsync(url);
    }
    finally
    {
        semaphore.Release();
    }
});
await Task.WhenAll(tasks);

// ✅ OU: Parallel.ForEachAsync (.NET 6+)
await Parallel.ForEachAsync(urls, 
    new ParallelOptions { MaxDegreeOfParallelism = 10 },
    async (url, ct) => await ProcessUrlAsync(url, ct));
```

---

# 🔧 SEÇÃO 4: FERRAMENTAS E UTILITÁRIOS

## 4.1 C# REPL (Dica 5)
```bash
# Instalar
dotnet tool install -g csharprepl

# Usar
csharprepl
> var x = 10;
> x * 2
20
```

## 4.2 Verificar Pacotes Desatualizados (Dica 17)
```bash
dotnet list package --outdated
```

## 4.3 dotnet retest (Dica 28)
```bash
# Rerrodar apenas testes que falharam
dotnet retest
```

## 4.4 Hot Reload (Dica 53)
```bash
dotnet watch run  # Reload automático
```

## 4.5 Compiled Regex (Dica 100)
```csharp
// ❌ EVITAR: Regex criado a cada uso
var match = Regex.Match(input, @"\d+");

// ✅ PREFERIR: Regex compilado
[GeneratedRegex(@"\d+", RegexOptions.Compiled)]
private static partial Regex NumeroRegex();

var match = NumeroRegex().Match(input);
```

## 4.6 StringSyntax Attribute (Dica 11)
```csharp
// ✅ INTELLISENSE PARA STRINGS ESPECIAIS:
public void ProcessarRegex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
{
    // IDE mostra highlighting de regex
}

public void ExecutarSql([StringSyntax(StringSyntaxAttribute.Sql)] string sql)
{
    // IDE mostra highlighting de SQL
}
```

## 4.7 UUIDs e ULIDs (Dica 13, 36)
```csharp
// ✅ UUID v7 (ordenável por tempo) - .NET 9+
var id = Guid.CreateVersion7();

// ✅ ULID (alternativa ordenável)
var ulid = Ulid.NewUlid();
```

## 4.8 Assembly Markers (Dica 10)
```csharp
// ✅ PARA REGISTRAR SERVIÇOS DE ASSEMBLY:
public interface IAssemblyMarker { }

services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());
services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
```

## 4.9 Testes Snapshot com Verify (Dica 33)
```csharp
// ✅ TESTES DE SNAPSHOT:
[Fact]
public async Task DeveRetornarPedidoCorreto()
{
    var pedido = await _service.ObterPedidoAsync(1);
    await Verify(pedido);  // Compara com snapshot salvo
}
```

## 4.10 Naughty Strings para Testes (Dica 20)
```csharp
// ✅ TESTAR COM STRINGS PROBLEMÁTICAS:
// Use a biblioteca Big List of Naughty Strings para testar inputs
// - SQL Injection attempts
// - XSS payloads
// - Unicode edge cases
// - Empty/null/whitespace
```

---

# 🏗️ SEÇÃO 5: ARQUITETURA E PADRÕES

## 5.1 Entity Framework Performance (Dica 54)
```csharp
// ❌ EVITAR: N+1 queries
var pedidos = await _context.Pedidos.ToListAsync();
foreach (var pedido in pedidos)
{
    Console.WriteLine(pedido.Cliente.Nome);  // Query para cada pedido!
}

// ✅ PREFERIR: Include para eager loading
var pedidos = await _context.Pedidos
    .Include(p => p.Cliente)
    .Include(p => p.Itens)
    .ToListAsync();

// ✅ PARA READ-ONLY:
var pedidos = await _context.Pedidos
    .AsNoTracking()
    .ToListAsync();

// ✅ SPLIT QUERIES para muitos includes:
var pedidos = await _context.Pedidos
    .Include(p => p.Itens)
    .AsSplitQuery()
    .ToListAsync();

// ✅ PROJEÇÃO quando não precisa entidade completa:
var resumos = await _context.Pedidos
    .Select(p => new { p.Id, p.Total, ClienteNome = p.Cliente.Nome })
    .ToListAsync();
```

## 5.2 Clean Architecture (Dica 80)
```
/src
  /Domain           # Entidades, Value Objects, Interfaces
  /Application      # Use Cases, DTOs, Validators
  /Infrastructure   # EF, APIs externas, Email
  /WebApi           # Controllers, Middlewares
```

## 5.3 Microservices Patterns (Dica 78, 90)
```csharp
// ✅ SAGA PATTERN para transações distribuídas
// ✅ CIRCUIT BREAKER para resiliência
// ✅ EVENT SOURCING para auditoria
// ✅ CQRS para escalabilidade
```

## 5.4 Rate Limiting (Dica 66)
```csharp
// ✅ .NET 7+:
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
    });
});

app.UseRateLimiter();
```

## 5.5 API Versioning (Dica 67)
```csharp
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

## 5.6 SignalR Real-time (Dica 55, 92)
```csharp
// ✅ HUB:
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

// ✅ REGISTRO:
services.AddSignalR();
app.MapHub<ChatHub>("/chat");
```

## 5.7 gRPC (Dica 56)
```csharp
// ✅ PARA COMUNICAÇÃO EFICIENTE ENTRE SERVIÇOS:
services.AddGrpc();
app.MapGrpcService<GreeterService>();

// Cliente:
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);
```

## 5.8 Message Queues (Dica 57)
```csharp
// ✅ PARA PROCESSAMENTO ASSÍNCRONO:
// - RabbitMQ
// - Azure Service Bus
// - AWS SQS
// Usar MassTransit ou NServiceBus para abstração
```

## 5.9 GraphQL (Dica 79)
```csharp
// ✅ HOT CHOCOLATE:
services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
```

## 5.10 OpenTelemetry (Dica 65)
```csharp
// ✅ OBSERVABILIDADE:
services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());
```

---

# 🔒 SEÇÃO 6: SEGURANÇA

## 6.1 Criptografia (Dica 93, 97)
```csharp
// ✅ HASHING DE SENHAS:
var hash = BCrypt.Net.BCrypt.HashPassword(password);
var isValid = BCrypt.Net.BCrypt.Verify(password, hash);

// ✅ NUNCA: MD5, SHA1 para senhas
// ✅ SEMPRE: BCrypt, Argon2, ou PBKDF2
```

## 6.2 Secrets Management
```csharp
// ❌ NUNCA: Secrets no código
var apiKey = "sk-12345...";

// ✅ SEMPRE: User Secrets (dev) ou Azure Key Vault (prod)
var apiKey = configuration["ApiKey"];
```

## 6.3 Input Validation
```csharp
// ✅ SEMPRE validar entrada do usuário
// ✅ Usar parameterized queries (EF faz automaticamente)
// ✅ Escapar output HTML
// ✅ Usar HTTPS
// ✅ Implementar CORS corretamente
```

---

# ⚡ SEÇÃO 7: PERFORMANCE AVANÇADA

## 7.1 ArrayPool (Dica 51)
```csharp
var pool = ArrayPool<byte>.Shared;
var buffer = pool.Rent(1024);
try
{
    // Usar buffer
}
finally
{
    pool.Return(buffer);
}
```

## 7.2 Object Pooling
```csharp
services.AddSingleton<ObjectPool<StringBuilder>>(
    new DefaultObjectPoolProvider().CreateStringBuilderPool());
```

## 7.3 Ref Structs (Dica 45)
```csharp
// Para tipos que DEVEM ficar na stack
public ref struct SpanParser
{
    private ReadOnlySpan<char> _data;
    // Não pode ser boxed, não pode ser campo de classe
}
```

## 7.4 Palavra-chave In (Dica 46)
```csharp
// ✅ PARA STRUCTS GRANDES: Passar por referência read-only
public void Processar(in LargeStruct data)
{
    // data é passado por referência, mas não pode ser modificado
}
```

## 7.5 SIMD/Intrinsics (Dica 74)
```csharp
// Para operações vetoriais de alta performance
if (Vector.IsHardwareAccelerated)
{
    // Usar Vector<T> para operações paralelas na CPU
}
```

## 7.6 Native AOT (Dica 83)
```xml
<!-- Para apps que precisam de startup instantâneo -->
<PublishAot>true</PublishAot>
```

## 7.7 Garbage Collection Tuning (Dica 87)
```csharp
// Para apps de alta performance
GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
```

## 7.8 Method Inlining (Dica 99)
```csharp
// ✅ PARA MÉTODOS PEQUENOS E HOT PATH:
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static int Add(int a, int b) => a + b;
```

## 7.9 Evitando Dynamic (Dica 75)
```csharp
// ❌ EVITAR: dynamic é lento
dynamic obj = GetObject();
obj.DoSomething();  // Resolução em runtime!

// ✅ PREFERIR: Generics ou interfaces
T obj = GetObject<T>();
```

## 7.10 Memory Mapped Files (Dica 72)
```csharp
// ✅ PARA ARQUIVOS GRANDES:
using var mmf = MemoryMappedFile.CreateFromFile(path);
using var accessor = mmf.CreateViewAccessor();
// Leitura/escrita eficiente
```

---

# 🌐 SEÇÃO 8: COMUNICAÇÃO E INTEGRAÇÕES

## 8.1 COM Interop (Dica 84)
```csharp
// ✅ PARA INTEGRAÇÃO COM COM:
// Usar ComWrappers em .NET 5+
// Sempre fazer Marshal.ReleaseComObject
```

## 8.2 P/Invoke (Dica 85)
```csharp
// ✅ PARA CHAMAR CÓDIGO NATIVO:
[LibraryImport("user32.dll")]
private static partial int MessageBox(IntPtr hWnd, string text, string caption, int type);

// .NET 7+ usa LibraryImport (source generated)
```

## 8.3 Assembly Loading (Dica 86)
```csharp
// ✅ PARA PLUGINS:
var context = new AssemblyLoadContext("plugins", isCollectible: true);
var assembly = context.LoadFromAssemblyPath(path);
// context.Unload() quando não precisar mais
```

---

# ☁️ SEÇÃO 9: CLOUD E CONTAINERS

## 9.1 Cloud Native Containers (Dica 98)
```csharp
// ✅ DOCKERFILE OTIMIZADO:
// - Multi-stage build
// - .NET runtime apenas (não SDK)
// - Non-root user

// ✅ HEALTH CHECKS para Kubernetes:
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
```

## 9.2 AI/ML Integration (Dica 95)
```csharp
// ✅ ML.NET ou Azure Cognitive Services
// ✅ Semantic Kernel para LLMs
```

---

# 📋 CHECKLIST RÁPIDO

Antes de finalizar qualquer código, verifique:

## 🔴 Crítico
- [ ] HttpClient usa IHttpClientFactory ou PooledConnectionLifetime?
- [ ] Não há .Result, .Wait() ou .GetAwaiter().GetResult()?
- [ ] Todos os catch têm log adequado (nunca vazios)?
- [ ] throw; sem variável para preservar stack trace?
- [ ] Não há Thread.Sleep em código async?
- [ ] Não há lock com await dentro?

## 🟡 Importante
- [ ] Logs usam message templates (não interpolação)?
- [ ] CancellationToken está sendo propagado?
- [ ] IDisposable está sendo disposed (using)?
- [ ] Coleções vazias usam Array.Empty ou []?
- [ ] LINQ é materializado apenas uma vez?
- [ ] Nullable types estão corretos?

## 🟢 Recomendado
- [ ] Strings grandes usam StringBuilder?
- [ ] Operações HTTP têm retry/circuit breaker (Polly)?
- [ ] Paralelismo tem limite de concorrência?
- [ ] EF usa AsNoTracking para read-only?
- [ ] Configurações usam Options Pattern?
- [ ] Regex usa GeneratedRegex?

---

# 🔗 Índice de Dicas por Número

| # | Dica | Categoria | Crítico |
|---|------|-----------|---------|
| 01 | Retornando Coleções Vazias | Collections | |
| 02 | Relançando Exceções Corretamente | Exceptions | 🔴 |
| 03 | Travamento com Async/Await | Async | 🔴 |
| 04 | Armadilhas de Desempenho LINQ | Performance | |
| 05 | C# REPL | Tools | |
| 06 | Acessando Span de Lista | Memory | |
| 07 | Logging Correto | Logging | 🟡 |
| 08 | Tipos Vazios / ValueTask | Memory | |
| 09 | ToList vs ToArray | Collections | |
| 10 | Marcadores de Assembly | Patterns | |
| 11 | StringSyntax Attribute | Tools | |
| 12 | Primary Constructors | C# 12 | |
| 13 | UUID v7 | Identifiers | |
| 14 | Menor Programa Válido | Basics | |
| 15 | CancellationTokens | Async | 🟡 |
| 16 | Collection Initializers C# 12 | C# 12 | |
| 17 | Verificando Pacotes Desatualizados | Tools | |
| 18 | Geração de Texto (Waffle) | Testing | |
| 19 | Métodos WebApplication | ASP.NET | |
| 20 | Validando Naughty Strings | Testing | |
| 21 | Interpolated Parser | Strings | |
| 22 | Alias para Qualquer Tipo | C# 12 | |
| 23 | DateTimeOffset vs DateTime | DateTime | 🟡 |
| 24 | Nullable Reference Types | Nullability | 🟡 |
| 25 | String Performance | Performance | |
| 26 | Async/Await Best Practices | Async | 🔴 |
| 27 | Evitando Bloqueios Async/Await | Async | 🔴 |
| 28 | dotnet retest | Tools | |
| 29 | Params com Tipos Enumerable | C# 13 | |
| 30 | Monitor Maligno | Threading | 🔴 |
| 31 | Convenção Underscore | Style | |
| 32 | Usando HttpClient Corretamente | HTTP | 🔴 |
| 33 | Testes Snapshot com Verify | Testing | |
| 34 | Chamando APIs com Refit | HTTP | |
| 35 | ConfigureAwait(false) | Async | 🟡 |
| 36 | ULIDs | Identifiers | |
| 37 | Usando HttpClientFactory | HTTP | 🔴 |
| 38 | Usando Channels | Concurrency | |
| 39 | Pattern Matching Switch | Patterns | |
| 40 | Memory e Span | Memory | |
| 41 | Interpolated Strings | Strings | |
| 42 | Null Conditional Assignment | Nullability | |
| 43 | Polly | Resilience | 🟡 |
| 44 | MediatR | Patterns | |
| 45 | Ref Structs | Memory | |
| 46 | Palavra-chave In | Performance | |
| 47 | LINQ Deferred Execution | LINQ | 🟡 |
| 48 | Usando Stackalloc | Memory | |
| 49 | Static Abstract Members | Interfaces | |
| 50 | FluentValidation | Validation | |
| 51 | ArrayPool Reutilização | Memory | |
| 52 | Source Generators | Metaprogramming | |
| 53 | Hot Reload / Edit and Continue | Tools | |
| 54 | Entity Framework Performance | EF Core | 🟡 |
| 55 | SignalR | Real-time | |
| 56 | gRPC | Communication | |
| 57 | Message Queues | Messaging | |
| 58 | Using Record Types | Records | |
| 59 | Target-Typed New | C# 9 | |
| 60 | Configuration | Configuration | |
| 61 | Dependency Injection | DI | 🟡 |
| 62 | nameof Operator | Refactoring | |
| 63 | Generic Math | Generics | |
| 64 | Health Checks | Monitoring | |
| 65 | OpenTelemetry | Observability | |
| 66 | Rate Limiting | Security | |
| 67 | API Versioning | API | |
| 68 | ValueTuples vs Tuple | Types | |
| 69 | Unsafe Code / Fixed Buffers | Unsafe | |
| 70 | Background Services | Workers | |
| 71 | Comparando Tuplas | Types | |
| 72 | Memory Mapped Files | IO | |
| 73 | ValueTask vs Task | Async | |
| 74 | Intrinsics SIMD | Performance | |
| 75 | Evitando Dynamic | Performance | |
| 76 | Exceptions para Casos Excepcionais | Exceptions | |
| 77 | Blazor Performance | Blazor | |
| 78 | Microservices Communication | Architecture | |
| 79 | GraphQL Hot Chocolate | API | |
| 80 | Clean Architecture | Architecture | |
| 81 | Options Pattern | Configuration | |
| 82 | nameof vs Reflexão | Performance | |
| 83 | Native AOT | Performance | |
| 84 | COM Interop | Interop | |
| 85 | P/Invoke | Interop | |
| 86 | Assembly Loading | Runtime | |
| 87 | Garbage Collection Tuning | Performance | |
| 88 | Performance Profiling | Tools | |
| 89 | Memory Optimization | Memory | |
| 90 | Microservices Patterns | Architecture | |
| 91 | Advanced LINQ | LINQ | |
| 92 | Real-Time Communications | Real-time | |
| 93 | Advanced Security | Security | |
| 94 | Advanced Networking | Networking | |
| 95 | AI/ML Integration | AI | |
| 96 | Performance Optimization Profiling | Performance | |
| 97 | Security Cryptography Advanced | Security | |
| 98 | Cloud Native Containers | Cloud | |
| 99 | Method Inlining | Performance | |
| 100 | Compiled Regex | Performance | |
| 101 | Evitando Catch Vazio | Exceptions | 🔴 |

---

# 📝 SEÇÃO 10: DICAS COMPLEMENTARES (Descrições)

As dicas abaixo complementam o guia com conceitos importantes que não requerem exemplos extensos de código, mas são regras essenciais a serem seguidas.

## 10.1 Menor Programa Válido (Dica 14)
```csharp
// ✅ C# 9+ - Top-level statements
Console.WriteLine("Hello, World!");
// Arquivo inteiro - sem namespace, classe ou Main!

// ❌ ANTIGO (verboso):
namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```
> **REGRA**: Para scripts e programas simples, use top-level statements. Para bibliotecas e projetos grandes, mantenha a estrutura tradicional.

## 10.2 Geração de Texto Waffle (Dica 18)
```csharp
// ✅ PARA TESTES - Biblioteca Waffle gera texto realista
// Install-Package WaffleGenerator.Bogus

var faker = new Faker();
var texto = faker.WaffleText(paragraphs: 3);
var titulo = faker.WaffleTitle();

// Útil para:
// - Seed de banco de dados em testes
// - Demonstrações e mockups
// - Testes de UI com texto realista
```
> **REGRA**: Use geradores de dados falsos (Bogus, Waffle) para testes, NUNCA dados reais de produção.

## 10.3 Métodos WebApplication (Dica 19)
```csharp
// ✅ MINIMAL APIs - Métodos úteis do WebApplication
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Mapeamento direto de endpoints
app.MapGet("/", () => "Hello World");
app.MapPost("/users", (User user) => Results.Created($"/users/{user.Id}", user));
app.MapPut("/users/{id}", (int id, User user) => Results.Ok(user));
app.MapDelete("/users/{id}", (int id) => Results.NoContent());

// Groups para organização
var api = app.MapGroup("/api/v1");
api.MapGet("/produtos", GetProdutos);
api.MapGet("/produtos/{id}", GetProdutoById);
```
> **REGRA**: Para APIs simples, prefira Minimal APIs. Para APIs complexas com muita lógica, use Controllers.

## 10.4 Interpolated String Handler (Dica 21)
```csharp
// ✅ CUSTOM INTERPOLATED STRING HANDLER - Performance em logging
[InterpolatedStringHandler]
public ref struct LogInterpolatedStringHandler
{
    // Permite que strings interpoladas sejam avaliadas APENAS se necessário
    // Útil para logging onde o nível pode estar desabilitado
}

// Uso prático - ILogger já faz isso internamente:
// Se LogLevel.Debug está desabilitado, os argumentos NÃO são avaliados
_logger.LogDebug("Processando {Item} de {Total}", item, total);
```
> **REGRA**: Para APIs de alta performance que recebem strings, considere implementar InterpolatedStringHandler.

## 10.5 Alias para Qualquer Tipo (Dica 22) - C# 12+
```csharp
// ✅ USING ALIAS para tipos complexos
using UserId = System.Int32;
using Email = System.String;
using UserDict = System.Collections.Generic.Dictionary<int, User>;
using Point = (int X, int Y);  // Alias para tupla!

// Uso
UserId id = 42;
Point ponto = (10, 20);
UserDict usuarios = new();

// ✅ GLOBAL USING ALIAS (no topo do arquivo ou GlobalUsings.cs)
global using UserId = System.Int32;
```
> **REGRA**: Use type aliases para tornar código mais legível, especialmente com tipos genéricos complexos ou tuplas.

## 10.6 Params com ReadOnlySpan (Dica 29) - C# 13+
```csharp
// ✅ C# 13 - params com qualquer tipo collection
public void ProcessarItens(params ReadOnlySpan<int> items)
{
    foreach (var item in items)
        Console.WriteLine(item);
}

// ✅ params com IEnumerable
public void ProcessarNomes(params IEnumerable<string> nomes) { }

// Chamadas - todas válidas:
ProcessarItens(1, 2, 3);
ProcessarItens([1, 2, 3]);
ProcessarItens(stackalloc int[] { 1, 2, 3 });
```
> **REGRA**: Em C# 13+, prefira `params ReadOnlySpan<T>` para melhor performance em métodos que aceitam número variável de argumentos.

## 10.7 Convenção Underscore (Dica 31)
```csharp
// ✅ CONVENÇÃO PARA CAMPOS PRIVADOS
public class PedidoService
{
    private readonly IRepository _repository;  // ✅ Underscore prefix
    private readonly ILogger _logger;          // ✅ Underscore prefix
    
    public PedidoService(IRepository repository, ILogger logger)
    {
        _repository = repository;  // Fácil distinguir de parâmetro
        _logger = logger;
    }
}

// ❌ EVITAR: Sem underscore (confunde com parâmetros)
private readonly IRepository repository;
```
> **REGRA**: Use prefixo `_` para campos privados. Isso é convenção do .NET runtime e facilita distinção de parâmetros e variáveis locais.

## 10.8 Static Abstract Members (Dica 49)
```csharp
// ✅ INTERFACES COM MEMBROS ESTÁTICOS ABSTRATOS - C# 11+
public interface IParsable<TSelf> where TSelf : IParsable<TSelf>
{
    static abstract TSelf Parse(string s);
    static abstract bool TryParse(string s, out TSelf result);
}

// Implementação
public readonly struct Temperatura : IParsable<Temperatura>
{
    public double Valor { get; }
    
    public static Temperatura Parse(string s) => new(double.Parse(s));
    public static bool TryParse(string s, out Temperatura result) { ... }
}

// Uso genérico
public T ParseGenerico<T>(string input) where T : IParsable<T>
{
    return T.Parse(input);  // Chamada estática via interface!
}
```
> **REGRA**: Use static abstract members para criar APIs genéricas que requerem factory methods ou operadores.

## 10.9 Source Generators (Dica 52)
```csharp
// ✅ SOURCE GENERATORS geram código em tempo de compilação
// Exemplos no .NET:
// - System.Text.Json (JsonSerializable)
// - Regex (GeneratedRegex)
// - Logging (LoggerMessage)

[JsonSerializable(typeof(Pessoa))]
public partial class MeuJsonContext : JsonSerializerContext { }

[LoggerMessage(Level = LogLevel.Information, Message = "Processando {Id}")]
public static partial void LogProcessando(this ILogger logger, int id);
```
> **REGRA**: Prefira Source Generators a Reflection para melhor performance e compatibilidade com AOT. Use-os para serialização, logging e validação.

## 10.10 Configuration Binding (Dica 60)
```csharp
// ✅ CONFIGURAÇÃO TIPADA COM BINDING
// appsettings.json:
// { "Email": { "SmtpServer": "smtp.example.com", "Port": 587 } }

public class EmailSettings
{
    public string SmtpServer { get; set; } = "";
    public int Port { get; set; }
}

// Registro
services.Configure<EmailSettings>(configuration.GetSection("Email"));

// ✅ OU: Binding direto
var settings = configuration.GetSection("Email").Get<EmailSettings>();

// ✅ COM VALIDAÇÃO:
services.AddOptions<EmailSettings>()
    .Bind(configuration.GetSection("Email"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```
> **REGRA**: Sempre use configuração tipada com IOptions<T>. Valide configurações no startup com ValidateOnStart().

## 10.11 Generic Math (Dica 63) - C# 11+
```csharp
// ✅ MATEMÁTICA GENÉRICA com INumber<T>
public T Somar<T>(T a, T b) where T : INumber<T>
{
    return a + b;  // Funciona com int, double, decimal, etc.
}

public T Media<T>(IEnumerable<T> valores) where T : INumber<T>
{
    var count = T.Zero;
    var soma = T.Zero;
    foreach (var v in valores)
    {
        soma += v;
        count++;
    }
    return soma / count;
}

// Uso
var somaInt = Somar(5, 3);        // int
var somaDouble = Somar(5.5, 3.2); // double
```
> **REGRA**: Para algoritmos matemáticos genéricos, use as interfaces INumber<T>, IAdditionOperators<T>, etc.

## 10.12 ValueTuple vs Tuple (Dica 68)
```csharp
// ❌ EVITAR: Tuple (classe - heap allocation)
Tuple<int, string> tuple = Tuple.Create(1, "texto");
var valor = tuple.Item1;  // Nomes genéricos

// ✅ PREFERIR: ValueTuple (struct - stack)
(int Id, string Nome) pessoa = (1, "João");
var id = pessoa.Id;       // Nomes descritivos!

// ✅ DECONSTRUCTION:
var (id, nome) = GetPessoa();

// ✅ COMO RETORNO:
public (bool Sucesso, string Mensagem) Validar(string input)
{
    if (string.IsNullOrEmpty(input))
        return (false, "Input vazio");
    return (true, "OK");
}
```
> **REGRA**: Sempre use ValueTuple (sintaxe com parênteses) em vez de Tuple<>. Para DTOs complexos, prefira records.

## 10.13 Unsafe Code e Fixed (Dica 69)
```csharp
// ⚠️ CÓDIGO UNSAFE - apenas quando necessário para performance extrema
unsafe
{
    int[] array = { 1, 2, 3, 4, 5 };
    fixed (int* ptr = array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine(*(ptr + i));
        }
    }
}

// ✅ PREFERIR: Span<T> quando possível (safe e quase tão rápido)
Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
```
> **REGRA**: Evite código unsafe. Use Span<T> e Memory<T> primeiro. Unsafe apenas para interop ou performance crítica comprovada por benchmarks.

## 10.14 Comparando ValueTuples (Dica 71)
```csharp
// ✅ VALUETUPLE TEM COMPARAÇÃO BUILT-IN
var t1 = (1, "a");
var t2 = (1, "a");
var t3 = (2, "b");

Console.WriteLine(t1 == t2);  // True - compara valores!
Console.WriteLine(t1 == t3);  // False

// ✅ ORDENAÇÃO FUNCIONA:
var lista = new List<(int Ano, int Mes)> { (2024, 3), (2024, 1), (2023, 12) };
lista.Sort();  // Ordena por Ano, depois por Mes

// ✅ COMO CHAVE DE DICIONÁRIO:
var dict = new Dictionary<(int, int), string>();
dict[(1, 2)] = "valor";
```
> **REGRA**: ValueTuples podem ser comparados diretamente e usados como chaves. Útil para chaves compostas.

## 10.15 Exceptions para Casos Excepcionais (Dica 76)
```csharp
// ❌ EVITAR: Exception para controle de fluxo
try
{
    var user = repository.GetById(id);
}
catch (UserNotFoundException)
{
    // Fluxo normal quando não encontra
}

// ✅ PREFERIR: Retornos que indicam ausência
var user = repository.GetByIdOrDefault(id);
if (user is null) { /* tratar */ }

// ✅ OU: TryGet pattern
if (repository.TryGetById(id, out var user))
{
    // Encontrou
}

// ✅ OU: Result pattern
var result = repository.GetById(id);
if (result.IsFailure) { /* tratar */ }
```
> **REGRA**: Exceptions são CARAS (~100x mais lentas). Use apenas para situações verdadeiramente excepcionais, não para fluxo normal.

## 10.16 Blazor Performance (Dica 77)
```csharp
// ✅ VIRTUALIZATION para listas grandes
<Virtualize Items="@items" Context="item">
    <ItemContent>
        <ItemComponent Item="@item" />
    </ItemContent>
</Virtualize>

// ✅ SHOULDRENDER para evitar re-renders
protected override bool ShouldRender() => _shouldRender;

// ✅ @KEY para ajudar diffing
@foreach (var item in items)
{
    <ItemComponent @key="item.Id" Item="@item" />
}

// ✅ STATEHASCHANGED apenas quando necessário
// Evite chamar em loops!
```
> **REGRA**: Em Blazor, minimize re-renders com ShouldRender, use @key em loops, e Virtualize para listas grandes.

## 10.17 Performance Profiling (Dica 88)
```bash
# ✅ FERRAMENTAS DE PROFILING:

# dotnet-counters - métricas em tempo real
dotnet-counters monitor --process-id <PID>

# dotnet-trace - tracing detalhado
dotnet-trace collect --process-id <PID>

# dotnet-dump - análise de memória
dotnet-dump collect --process-id <PID>
dotnet-dump analyze <dump-file>

# Visual Studio Profiler
# - CPU Usage
# - Memory Usage  
# - .NET Object Allocation

# BenchmarkDotNet para micro-benchmarks
[Benchmark]
public void MeuMetodo() { }
```
> **REGRA**: SEMPRE meça antes de otimizar. Use BenchmarkDotNet para comparações, dotnet-counters para produção.

## 10.18 Memory Optimization (Dica 89)
```csharp
// ✅ TÉCNICAS DE OTIMIZAÇÃO DE MEMÓRIA:

// 1. ArrayPool para buffers temporários
var buffer = ArrayPool<byte>.Shared.Rent(1024);

// 2. Span<T> para evitar alocações
ReadOnlySpan<char> slice = str.AsSpan(0, 10);

// 3. stackalloc para arrays pequenos
Span<int> small = stackalloc int[16];

// 4. String interning para strings repetidas
var interned = string.Intern(frequentString);

// 5. Struct em vez de class para tipos pequenos
public readonly struct Point(int X, int Y);

// 6. IMemoryOwner para controle de lifetime
using var owner = MemoryPool<byte>.Shared.Rent(1024);
```
> **REGRA**: Reduza alocações no heap. Use structs pequenas, pooling, e Span<T>. Monitore com dotnet-counters gc-heap-size.

## 10.19 Advanced Networking (Dica 94)
```csharp
// ✅ SOCKETS MODERNOS com SocketsHttpHandler
var handler = new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(15),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    MaxConnectionsPerServer = 100,
    EnableMultipleHttp2Connections = true
};

// ✅ HTTP/2 e HTTP/3
var client = new HttpClient(handler)
{
    DefaultRequestVersion = HttpVersion.Version20,
    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
};

// ✅ QUIC (HTTP/3) - .NET 7+
// Requer Windows 11 / Linux com libmsquic

// ✅ DNS OVER HTTPS
// Configure no sistema ou use resolvers customizados
```
> **REGRA**: Configure SocketsHttpHandler corretamente. Use HTTP/2 quando possível. Monitore conexões com métricas.

## 10.20 Performance Optimization Deep (Dica 96)
```csharp
// ✅ TÉCNICAS AVANÇADAS:

// 1. Frozen Collections (.NET 8+) - Read-only otimizado
var frozen = myList.ToFrozenDictionary(x => x.Key);

// 2. SearchValues para buscas múltiplas
SearchValues<char> vowels = SearchValues.Create("aeiou");
int index = text.AsSpan().IndexOfAny(vowels);

// 3. CompositeFormat para strings frequentes
CompositeFormat format = CompositeFormat.Parse("Olá, {0}!");
string result = string.Format(null, format, nome);

// 4. Evite boxing
void Process<T>(T value) where T : struct { }  // Não faz boxing

// 5. Use [SkipLocalsInit] para performance crítica
[SkipLocalsInit]
public void HotPath() { }
```
> **REGRA**: Otimizações avançadas apenas após profiling. FrozenCollections para dados estáticos, SearchValues para buscas.

---

# 📋 RESUMO DE TODAS AS DICAS (Quick Reference)

## 🔴 CRÍTICO - Memorize estas regras:
| # | Dica | Regra |
|---|------|-------|
| 02 | Exceções | Use `throw;` sem variável para preservar stack trace |
| 03 | Async Deadlock | NUNCA use .Result, .Wait() ou .GetAwaiter().GetResult() |
| 26 | Async Best | NUNCA use Thread.Sleep em código async, use Task.Delay |
| 27 | Async Lock | NUNCA use lock com await, use SemaphoreSlim |
| 30 | Monitor | Cuidado com reentrância de Monitor/lock |
| 32 | HttpClient | NUNCA crie HttpClient em cada request |
| 37 | HttpClientFactory | SEMPRE use IHttpClientFactory ou PooledConnectionLifetime |
| 101 | Catch Vazio | NUNCA deixe catch vazio, sempre log + tratamento |

## 🟡 IMPORTANTE - Siga sempre:
| # | Dica | Regra |
|---|------|-------|
| 01 | Coleções Vazias | Use Array.Empty<T>() ou [] para coleções vazias |
| 07 | Logging | Use message templates, não interpolação |
| 15 | CancellationToken | Propague CancellationToken em todas operações async |
| 23 | DateTime | Use DateTimeOffset para dados que viajam entre sistemas |
| 24 | Nullable | Declare nullability corretamente, não use null! |
| 35 | ConfigureAwait | Use ConfigureAwait(false) em bibliotecas |
| 43 | Polly | Configure retry e circuit breaker para HTTP |
| 47 | LINQ | Materialize queries uma única vez |
| 54 | EF Core | Use AsNoTracking para read-only, evite N+1 |
| 61 | DI | Use lifetimes corretos, evite captive dependencies |

## 🟢 RECOMENDADO - Melhores práticas:
| # | Dica | Regra |
|---|------|-------|
| 04 | LINQ | Evite múltiplas enumerações de IEnumerable |
| 09 | Collections | ToArray para size fixo, ToList para mutável |
| 12 | Primary Ctors | Use primary constructors em C# 12+ |
| 16 | Collection Init | Use [] para inicialização em C# 12+ |
| 25 | Strings | StringBuilder para muitas concatenações |
| 39 | Pattern Match | Use switch expression para múltiplas condições |
| 40 | Span | Use Span<T> para operações sem alocação |
| 58 | Records | Use records para DTOs imutáveis |
| 76 | Exceptions | Não use exceptions para controle de fluxo |
| 100 | Regex | Use GeneratedRegex para regex compilado |

---

> **Versão**: 3.0 - Guia Completo Expandido (101 Dicas)  
> **Última atualização**: Janeiro 2026  
> **Repositório**: [didaticos-dicas-csharp](https://github.com/diogocostadev/didaticos-dicas-csharp)
