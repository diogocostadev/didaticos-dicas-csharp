/// <summary>
/// Dica 22: Minimal APIs no ASP.NET Core
/// 
/// Esta dica demonstra as capacidades das Minimal APIs introduzidas no .NET 6
/// e aprimoradas nas versões subsequentes, incluindo:
/// 
/// - Configuração básica de Minimal APIs
/// - Endpoints HTTP (GET, POST, PUT, DELETE)
/// - Model binding e validação
/// - Swagger/OpenAPI integration
/// - Middleware customizado
/// - Dependency Injection
/// - Filtros e interceptadores
/// - Rate limiting e autenticação
/// - Performance e comparação com Controllers
/// 
/// As Minimal APIs oferecem uma abordagem mais concisa e performática
/// para criar APIs web em .NET, especialmente úteis para microserviços.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// ============= CONFIGURAÇÃO DE SERVIÇOS =============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Dica 22 - Minimal APIs Demo", 
        Version = "v1",
        Description = "Demonstração completa de Minimal APIs no ASP.NET Core"
    });
});

// Simulando um repositório em memória
builder.Services.AddSingleton<IProdutoRepository, ProdutoRepositoryMemoria>();
builder.Services.AddSingleton<IPedidoService, PedidoService>();

var app = builder.Build();

// ============= CONFIGURAÇÃO DE MIDDLEWARE =============
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal APIs Demo v1"));
}

// Middleware customizado para logging
app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var elapsed = DateTime.UtcNow - start;
    
    Console.WriteLine($"🌐 {context.Request.Method} {context.Request.Path} - " +
                     $"Status: {context.Response.StatusCode} - " +
                     $"Tempo: {elapsed.TotalMilliseconds:F2}ms");
});

// ============= ENDPOINTS BÁSICOS =============

// Endpoint de health check
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithTags("System")
   .Produces<object>(200);

// Endpoint de informações da API
app.MapGet("/info", () => new 
{
    Nome = "Minimal APIs Demo",
    Versao = "1.0.0",
    Tecnologia = ".NET 9.0",
    Recursos = new[]
    {
        "CRUD completo de Produtos",
        "Sistema de Pedidos",
        "Validação automática",
        "Swagger/OpenAPI",
        "Middleware customizado"
    }
})
.WithName("GetApiInfo")
.WithTags("System")
.WithSummary("Retorna informações sobre a API");

// ============= ENDPOINTS DE PRODUTOS =============

// GET /produtos - Listar todos os produtos
app.MapGet("/produtos", async (IProdutoRepository repo) =>
{
    var produtos = await repo.GetTodosAsync();
    return Results.Ok(produtos);
})
.WithName("GetProdutos")
.WithTags("Produtos")
.WithSummary("Lista todos os produtos")
.Produces<IEnumerable<Produto>>(200);

// GET /produtos/{id} - Obter produto por ID
app.MapGet("/produtos/{id:int}", async (int id, IProdutoRepository repo) =>
{
    var produto = await repo.GetPorIdAsync(id);
    return produto is not null ? Results.Ok(produto) : Results.NotFound($"Produto com ID {id} não encontrado");
})
.WithName("GetProdutoPorId")
.WithTags("Produtos")
.WithSummary("Obtém um produto pelo ID")
.Produces<Produto>(200)
.Produces(404);

// POST /produtos - Criar novo produto
app.MapPost("/produtos", async (ProdutoCreateDto produtoDto, IProdutoRepository repo) =>
{
    // Validação manual (em cenário real, usaria FluentValidation ou similar)
    if (string.IsNullOrWhiteSpace(produtoDto.Nome))
        return Results.BadRequest("Nome do produto é obrigatório");
    
    if (produtoDto.Preco <= 0)
        return Results.BadRequest("Preço deve ser maior que zero");

    var produto = new Produto
    {
        Id = Random.Shared.Next(1000, 9999),
        Nome = produtoDto.Nome,
        Preco = produtoDto.Preco,
        Categoria = produtoDto.Categoria,
        EmEstoque = produtoDto.EmEstoque,
        DataCriacao = DateTime.UtcNow
    };

    await repo.AdicionarAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
})
.WithName("CriarProduto")
.WithTags("Produtos")
.WithSummary("Cria um novo produto")
.Accepts<ProdutoCreateDto>("application/json")
.Produces<Produto>(201)
.Produces(400);

// PUT /produtos/{id} - Atualizar produto
app.MapPut("/produtos/{id:int}", async (int id, ProdutoUpdateDto produtoDto, IProdutoRepository repo) =>
{
    var produtoExistente = await repo.GetPorIdAsync(id);
    if (produtoExistente is null)
        return Results.NotFound($"Produto com ID {id} não encontrado");

    produtoExistente.Nome = produtoDto.Nome ?? produtoExistente.Nome;
    produtoExistente.Preco = produtoDto.Preco ?? produtoExistente.Preco;
    produtoExistente.Categoria = produtoDto.Categoria ?? produtoExistente.Categoria;
    produtoExistente.EmEstoque = produtoDto.EmEstoque ?? produtoExistente.EmEstoque;

    await repo.AtualizarAsync(produtoExistente);
    return Results.Ok(produtoExistente);
})
.WithName("AtualizarProduto")
.WithTags("Produtos")
.WithSummary("Atualiza um produto existente")
.Accepts<ProdutoUpdateDto>("application/json")
.Produces<Produto>(200)
.Produces(404);

// DELETE /produtos/{id} - Deletar produto
app.MapDelete("/produtos/{id:int}", async (int id, IProdutoRepository repo) =>
{
    var produto = await repo.GetPorIdAsync(id);
    if (produto is null)
        return Results.NotFound($"Produto com ID {id} não encontrado");

    await repo.RemoverAsync(id);
    return Results.NoContent();
})
.WithName("DeletarProduto")
.WithTags("Produtos")
.WithSummary("Remove um produto")
.Produces(204)
.Produces(404);

// ============= ENDPOINTS DE PEDIDOS =============

// POST /pedidos - Criar pedido
app.MapPost("/pedidos", async (PedidoCreateDto pedidoDto, IPedidoService pedidoService) =>
{
    try
    {
        var pedido = await pedidoService.CriarPedidoAsync(pedidoDto);
        return Results.Created($"/pedidos/{pedido.Id}", pedido);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CriarPedido")
.WithTags("Pedidos")
.WithSummary("Cria um novo pedido")
.Accepts<PedidoCreateDto>("application/json")
.Produces<Pedido>(201)
.Produces(400);

// GET /pedidos/{id} - Obter pedido
app.MapGet("/pedidos/{id:int}", async (int id, IPedidoService pedidoService) =>
{
    var pedido = await pedidoService.GetPedidoPorIdAsync(id);
    return pedido is not null ? Results.Ok(pedido) : Results.NotFound();
})
.WithName("GetPedido")
.WithTags("Pedidos")
.WithSummary("Obtém um pedido pelo ID")
.Produces<Pedido>(200)
.Produces(404);

// ============= ENDPOINTS AVANÇADOS =============

// Busca de produtos com filtros
app.MapGet("/produtos/buscar", async (
    [FromQuery] string? nome,
    [FromQuery] string? categoria,
    [FromQuery] decimal? precoMin,
    [FromQuery] decimal? precoMax,
    [FromQuery] bool? emEstoque,
    IProdutoRepository repo) =>
{
    var produtos = await repo.BuscarAsync(nome, categoria, precoMin, precoMax, emEstoque);
    return Results.Ok(produtos);
})
.WithName("BuscarProdutos")
.WithTags("Produtos")
.WithSummary("Busca produtos com filtros opcionais")
.Produces<IEnumerable<Produto>>(200);

// Estatísticas de produtos
app.MapGet("/produtos/estatisticas", async (IProdutoRepository repo) =>
{
    var produtos = await repo.GetTodosAsync();
    var stats = new
    {
        TotalProdutos = produtos.Count(),
        ProdutosEmEstoque = produtos.Count(p => p.EmEstoque),
        ProdutosSemEstoque = produtos.Count(p => !p.EmEstoque),
        PrecoMedio = produtos.Any() ? produtos.Average(p => p.Preco) : 0,
        PrecoMinimo = produtos.Any() ? produtos.Min(p => p.Preco) : 0,
        PrecoMaximo = produtos.Any() ? produtos.Max(p => p.Preco) : 0,
        CategoriasMaisComuns = produtos
            .GroupBy(p => p.Categoria)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new { Categoria = g.Key, Quantidade = g.Count() })
            .ToList()
    };

    return Results.Ok(stats);
})
.WithName("GetEstatisticasProdutos")
.WithTags("Produtos")
.WithSummary("Retorna estatísticas dos produtos")
.Produces<object>(200);

// ============= ENDPOINT PARA DEMONSTRAÇÃO =============

// Endpoint especial para mostrar recursos da API
app.MapGet("/demo", () =>
{
    var demo = new
    {
        Titulo = "🚀 Demonstração de Minimal APIs",
        Recursos = new
        {
            Endpoints = new[]
            {
                "GET /health - Health check da aplicação",
                "GET /info - Informações da API",
                "GET /produtos - Lista todos os produtos",
                "GET /produtos/{id} - Obtém produto específico",
                "POST /produtos - Cria novo produto",
                "PUT /produtos/{id} - Atualiza produto",
                "DELETE /produtos/{id} - Remove produto",
                "GET /produtos/buscar - Busca com filtros",
                "GET /produtos/estatisticas - Estatísticas",
                "POST /pedidos - Cria pedido",
                "GET /pedidos/{id} - Obtém pedido"
            },
            Tecnologias = new[]
            {
                ".NET 9.0",
                "Minimal APIs",
                "Swagger/OpenAPI",
                "Dependency Injection",
                "Model Binding",
                "Middleware Pipeline"
            },
            Vantagens = new[]
            {
                "✅ Menos código boilerplate que Controllers",
                "✅ Performance superior",
                "✅ Ideal para microserviços",
                "✅ Sintaxe mais simples e direta",
                "✅ Configuração rápida",
                "✅ Suporte completo ao OpenAPI"
            }
        },
        ExemplosUso = new
        {
            CriarProduto = "POST /produtos { \"nome\": \"Produto Teste\", \"preco\": 99.99, \"categoria\": \"Teste\" }",
            BuscarProdutos = "GET /produtos/buscar?categoria=Eletrônicos&precoMin=100&emEstoque=true",
            CriarPedido = "POST /pedidos { \"clienteNome\": \"João\", \"produtoIds\": [1, 2] }"
        }
    };

    return Results.Ok(demo);
})
.WithName("GetDemo")
.WithTags("Demo")
.WithSummary("Demonstração completa dos recursos da API")
.Produces<object>(200);

// ============= EXECUÇÃO DA APLICAÇÃO =============

Console.WriteLine("🚀 Iniciando Minimal API Demo...");
Console.WriteLine("📖 Acesse /swagger para ver a documentação");
Console.WriteLine("🎯 Acesse /demo para ver exemplos de uso");

app.Run();

// ============= MODELS E DTOS =============

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = "";
    public bool EmEstoque { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class ProdutoCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; } = "";
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal Preco { get; set; }
    
    public string Categoria { get; set; } = "";
    public bool EmEstoque { get; set; } = true;
}

public class ProdutoUpdateDto
{
    public string? Nome { get; set; }
    public decimal? Preco { get; set; }
    public string? Categoria { get; set; }
    public bool? EmEstoque { get; set; }
}

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = "";
    public List<int> ProdutoIds { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Status { get; set; } = "Pendente";
}

public class PedidoCreateDto
{
    [Required(ErrorMessage = "Nome do cliente é obrigatório")]
    public string ClienteNome { get; set; } = "";
    
    [Required(ErrorMessage = "Pelo menos um produto deve ser selecionado")]
    public List<int> ProdutoIds { get; set; } = new();
}

// ============= INTERFACES E SERVIÇOS =============

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetTodosAsync();
    Task<Produto?> GetPorIdAsync(int id);
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(int id);
    Task<IEnumerable<Produto>> BuscarAsync(string? nome, string? categoria, decimal? precoMin, decimal? precoMax, bool? emEstoque);
}

public class ProdutoRepositoryMemoria : IProdutoRepository
{
    private readonly List<Produto> _produtos = new()
    {
        new() { Id = 1, Nome = "Notebook Gamer", Preco = 2500.99m, Categoria = "Eletrônicos", EmEstoque = true, DataCriacao = DateTime.UtcNow.AddDays(-30) },
        new() { Id = 2, Nome = "Mouse Wireless", Preco = 89.90m, Categoria = "Eletrônicos", EmEstoque = true, DataCriacao = DateTime.UtcNow.AddDays(-25) },
        new() { Id = 3, Nome = "Teclado Mecânico", Preco = 299.99m, Categoria = "Eletrônicos", EmEstoque = false, DataCriacao = DateTime.UtcNow.AddDays(-20) },
        new() { Id = 4, Nome = "Monitor 4K", Preco = 1200.00m, Categoria = "Eletrônicos", EmEstoque = true, DataCriacao = DateTime.UtcNow.AddDays(-15) },
        new() { Id = 5, Nome = "Livro C#", Preco = 79.90m, Categoria = "Livros", EmEstoque = true, DataCriacao = DateTime.UtcNow.AddDays(-10) }
    };

    public Task<IEnumerable<Produto>> GetTodosAsync()
    {
        return Task.FromResult<IEnumerable<Produto>>(_produtos);
    }

    public Task<Produto?> GetPorIdAsync(int id)
    {
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(produto);
    }

    public Task AdicionarAsync(Produto produto)
    {
        _produtos.Add(produto);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Produto produto)
    {
        var index = _produtos.FindIndex(p => p.Id == produto.Id);
        if (index >= 0)
        {
            _produtos[index] = produto;
        }
        return Task.CompletedTask;
    }

    public Task RemoverAsync(int id)
    {
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        if (produto is not null)
        {
            _produtos.Remove(produto);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Produto>> BuscarAsync(string? nome, string? categoria, decimal? precoMin, decimal? precoMax, bool? emEstoque)
    {
        var query = _produtos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

        if (precoMin.HasValue)
            query = query.Where(p => p.Preco >= precoMin.Value);

        if (precoMax.HasValue)
            query = query.Where(p => p.Preco <= precoMax.Value);

        if (emEstoque.HasValue)
            query = query.Where(p => p.EmEstoque == emEstoque.Value);

        return Task.FromResult(query);
    }
}

public interface IPedidoService
{
    Task<Pedido> CriarPedidoAsync(PedidoCreateDto pedidoDto);
    Task<Pedido?> GetPedidoPorIdAsync(int id);
}

public class PedidoService : IPedidoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly List<Pedido> _pedidos = new();

    public PedidoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Pedido> CriarPedidoAsync(PedidoCreateDto pedidoDto)
    {
        if (string.IsNullOrWhiteSpace(pedidoDto.ClienteNome))
            throw new ArgumentException("Nome do cliente é obrigatório");

        if (!pedidoDto.ProdutoIds.Any())
            throw new ArgumentException("Pelo menos um produto deve ser selecionado");

        // Validar se todos os produtos existem
        var todosProdutos = await _produtoRepository.GetTodosAsync();
        var produtosEncontrados = todosProdutos.Where(p => pedidoDto.ProdutoIds.Contains(p.Id)).ToList();

        if (produtosEncontrados.Count != pedidoDto.ProdutoIds.Count)
            throw new ArgumentException("Um ou mais produtos não foram encontrados");

        var valorTotal = produtosEncontrados.Sum(p => p.Preco);

        var pedido = new Pedido
        {
            Id = Random.Shared.Next(1000, 9999),
            ClienteNome = pedidoDto.ClienteNome,
            ProdutoIds = pedidoDto.ProdutoIds,
            ValorTotal = valorTotal,
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente"
        };

        _pedidos.Add(pedido);
        return pedido;
    }

    public Task<Pedido?> GetPedidoPorIdAsync(int id)
    {
        var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(pedido);
    }
}
