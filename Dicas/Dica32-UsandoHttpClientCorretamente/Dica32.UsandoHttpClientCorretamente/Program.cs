using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

namespace Dica32.UsandoHttpClientCorretamente;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🌐 Dica 32: Usando HttpClient Corretamente");
        Console.WriteLine("==========================================\n");

        Console.WriteLine("Este exemplo demonstra as práticas corretas e incorretas no uso do HttpClient.\n");

        // Demonstração das práticas incorretas
        await DemonstrarPraticasIncorretas();
        
        Console.WriteLine("\n" + new string('=', 60) + "\n");
        
        // Demonstração das práticas corretas
        await DemonstrarPraticasCorretas();
        
        Console.WriteLine("\n" + new string('=', 60) + "\n");
        
        // Demonstração com HttpClientFactory
        await DemonstrarHttpClientFactory();
        
        Console.WriteLine("\n✅ Demonstração concluída!");
        Console.WriteLine("\n💡 Lições aprendidas:");
        Console.WriteLine("   • Nunca crie HttpClient a cada requisição");
        Console.WriteLine("   • Configure PooledConnectionLifetime para clientes de longa duração");
        Console.WriteLine("   • Use HttpClientFactory quando possível");
        Console.WriteLine("   • Monitore o uso de sockets em produção");
    }

    // ❌ PRÁTICAS INCORRETAS
    static async Task DemonstrarPraticasIncorretas()
    {
        Console.WriteLine("❌ DEMONSTRAÇÃO - PRÁTICAS INCORRETAS");
        Console.WriteLine("=====================================\n");

        Console.WriteLine("🔴 Problema 1: Criando HttpClient a cada requisição");
        Console.WriteLine("   (Isso pode causar Socket Exhaustion!)\n");

        var stopwatch = Stopwatch.StartNew();
        
        // Simulando múltiplas requisições criando HttpClient a cada vez
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await RequisicaoComNovoHttpClient($"Requisição {i + 1}");
                await Task.Delay(100); // Pequeno delay para simular uso real
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Erro: {ex.Message}");
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine($"   ⏱️  Tempo total (prática incorreta): {stopwatch.ElapsedMilliseconds}ms\n");

        Console.WriteLine("🔴 Problema 2: HttpClient estático sem configuração");
        Console.WriteLine("   (DNS não é atualizado após criação!)\n");
        
        await DemonstrarProblemaHttpClientEstatico();
    }

    // Método que demonstra o problema de criar HttpClient a cada requisição
    static async Task RequisicaoComNovoHttpClient(string identificacao)
    {
        try
        {
            // ❌ RUIM: Criando HttpClient a cada requisição
            // Problema: Mesmo após Dispose(), o socket fica em TIME_WAIT por ~240 segundos
            // Em alta carga, isso esgota os sockets disponíveis (Socket Exhaustion)
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            Console.WriteLine($"   📡 {identificacao}: Criando novo HttpClient...");
            
            // Usando uma API pública que sempre responde
            var response = await client.GetStringAsync("https://httpbin.org/json");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine($"   ✅ {identificacao}: Resposta recebida (length: {response.Length})");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"   ❌ {identificacao}: Erro HTTP - {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine($"   ⏰ {identificacao}: Timeout na requisição");
        }
    }

    // HttpClient estático - problema de DNS
    private static readonly HttpClient _clientEstatico = new HttpClient();

    static async Task DemonstrarProblemaHttpClientEstatico()
    {
        Console.WriteLine("   📡 Usando HttpClient estático (sem PooledConnectionLifetime)...");
        
        try
        {
            var response = await _clientEstatico.GetStringAsync("https://httpbin.org/ip");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine($"   📍 IP detectado: {data.GetProperty("origin").GetString()}");
            Console.WriteLine("   ⚠️  Problema: Se o DNS mudar, este cliente não perceberá!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Erro com cliente estático: {ex.Message}");
        }
    }

    // ✅ PRÁTICAS CORRETAS
    static async Task DemonstrarPraticasCorretas()
    {
        Console.WriteLine("✅ DEMONSTRAÇÃO - PRÁTICAS CORRETAS");
        Console.WriteLine("===================================\n");

        Console.WriteLine("🟢 Solução 1: HttpClient de longa duração com PooledConnectionLifetime");
        Console.WriteLine("       (Resolve problemas de Socket e DNS!)\n");

        var stopwatch = Stopwatch.StartNew();
        
        // ✅ BOM: HttpClient configurado corretamente
        using var clientCorreto = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15) // DNS é renovado a cada 15min
        });
        
        clientCorreto.Timeout = TimeSpan.FromSeconds(5);

        for (int i = 0; i < 3; i++)
        {
            try
            {
                await RequisicaoComClienteCorreto(clientCorreto, $"Requisição {i + 1}");
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Erro: {ex.Message}");
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine($"   ⏱️  Tempo total (prática correta): {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine("   💡 Mesmo cliente reutilizado, com DNS refresh automático!\n");

        await DemonstrarConfiguracaoAvancada();
    }

    static async Task RequisicaoComClienteCorreto(HttpClient client, string identificacao)
    {
        try
        {
            Console.WriteLine($"   📡 {identificacao}: Reutilizando HttpClient configurado...");
            
            var response = await client.GetStringAsync("https://httpbin.org/uuid");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine($"   ✅ {identificacao}: UUID recebido - {data.GetProperty("uuid").GetString()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ {identificacao}: Erro - {ex.Message}");
        }
    }

    static async Task DemonstrarConfiguracaoAvancada()
    {
        Console.WriteLine("🔧 Configuração Avançada: Timeouts e Headers\n");

        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            ConnectTimeout = TimeSpan.FromSeconds(10),
            MaxConnectionsPerServer = 100
        };

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("User-Agent", "Dica32-HttpClient-Demo/1.0");
        client.Timeout = TimeSpan.FromSeconds(30);

        try
        {
            Console.WriteLine("   📡 Requisição com configuração avançada...");
            var response = await client.GetStringAsync("https://httpbin.org/headers");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine("   ✅ Headers enviados:");
            var headers = data.GetProperty("headers");
            foreach (var header in headers.EnumerateObject())
            {
                Console.WriteLine($"      {header.Name}: {header.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Erro na configuração avançada: {ex.Message}");
        }
    }

    // ✅ HTTPLIENT FACTORY (MELHOR PRÁTICA)
    static async Task DemonstrarHttpClientFactory()
    {
        Console.WriteLine("🏭 DEMONSTRAÇÃO - HTTP CLIENT FACTORY");
        Console.WriteLine("=====================================\n");

        Console.WriteLine("🟢 Melhor Prática: Usando HttpClientFactory com Dependency Injection\n");

        // Configurando o Host com DI
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // ✅ Registrando HttpClientFactory
                services.AddHttpClient();
                
                // ✅ Named Client
                services.AddHttpClient("ApiClient", client =>
                {
                    client.BaseAddress = new Uri("https://httpbin.org/");
                    client.DefaultRequestHeaders.Add("User-Agent", "Dica32-Factory-Demo/1.0");
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                // ✅ Typed Client
                services.AddHttpClient<ApiService>();
                
                // Registrando nossos serviços
                services.AddTransient<HttpClientDemoService>();
            })
            .Build();

        using (host)
        {
            var demoService = host.Services.GetRequiredService<HttpClientDemoService>();
            await demoService.DemonstrarUsoDiversosClientes();
        }
    }
}

// Serviço para demonstrar o uso do HttpClientFactory
public class HttpClientDemoService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpClientDemoService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task DemonstrarUsoDiversosClientes()
    {
        Console.WriteLine("📋 Testando diferentes tipos de clientes do Factory:\n");

        // Cliente padrão
        await TestarClientePadrao();
        
        // Named client
        await TestarNamedClient();
    }

    private async Task TestarClientePadrao()
    {
        Console.WriteLine("🔸 Cliente Padrão (via IHttpClientFactory):");
        
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://httpbin.org/json");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine("   ✅ Resposta recebida com cliente padrão");
            Console.WriteLine($"   📊 Tamanho da resposta: {response.Length} bytes\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Erro com cliente padrão: {ex.Message}\n");
        }
    }

    private async Task TestarNamedClient()
    {
        Console.WriteLine("🔸 Named Client (pré-configurado):");
        
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetStringAsync("delay/1"); // Relative URL
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            
            Console.WriteLine("   ✅ Resposta recebida com named client");
            Console.WriteLine($"   📍 URL utilizada: {data.GetProperty("url").GetString()}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Erro com named client: {ex.Message}\n");
        }
    }
}

// Exemplo de Typed Client
public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://httpbin.org/");
    }

    public async Task<string> GetUserAgentAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("user-agent");
            var data = JsonSerializer.Deserialize<JsonElement>(response);
            return data.GetProperty("user-agent").GetString() ?? "Unknown";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
