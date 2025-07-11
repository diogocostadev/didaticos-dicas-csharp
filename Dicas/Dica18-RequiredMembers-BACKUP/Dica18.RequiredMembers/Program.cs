namespace Dica18.RequiredMembers;

public class Program
{
    public static void Main(string[] args)
    {
        WriteLine("=== Dica 18: Required Members no C# 11 ===");

        // 1. DEMONSTRAÇÃO: Required properties básicas
        WriteLine("\n1. Required properties básicas:");
        DemonstrarRequiredBasico();

        // 2. DEMONSTRAÇÃO: Required com init accessors
        WriteLine("\n2. Required com init accessors:");
        DemonstrarRequiredInit();

        // 3. DEMONSTRAÇÃO: Required com herança
        WriteLine("\n3. Required com herança:");
        DemonstrarRequiredHeranca();

        // 4. DEMONSTRAÇÃO: Required com validation
        WriteLine("\n4. Required com validation:");
        DemonstrarRequiredValidation();

        // 5. DEMONSTRAÇÃO: Required em DTOs e APIs
        WriteLine("\n5. Required em DTOs e APIs:");
        DemonstrarRequiredDTOs();

        // 6. DEMONSTRAÇÃO: SetsRequiredMembers attribute
        WriteLine("\n6. SetsRequiredMembers attribute:");
        DemonstrarSetsRequiredMembers();

        // 7. DEMONSTRAÇÃO: Required com record types
        WriteLine("\n7. Required com record types:");
        DemonstrarRequiredRecords();

        // 8. DEMONSTRAÇÃO: Casos de uso práticos
        WriteLine("\n8. Casos de uso práticos:");
        DemonstrarCasosUso();

        WriteLine("\n=== Resumo das Vantagens dos Required Members ===");
        WriteLine("✅ Garantia de inicialização em tempo de compilação");
        WriteLine("✅ Alternativa mais flexível aos construtores");
        WriteLine("✅ Melhor experiência de desenvolvimento com IntelliSense");
        WriteLine("✅ Redução de NullReferenceException");
        WriteLine("✅ Compatível com object initializers");
        WriteLine("✅ Suporte a herança e polimorfismo");
        WriteLine("✅ Integração com serialização JSON");

        WriteLine("\n=== Fim da Demonstração ===");
    }

    static void DemonstrarRequiredBasico()
    {
        // ✅ Compilação bem-sucedida - todos os required members são fornecidos
        var usuario = new Usuario
        {
            Nome = "João Silva",
            Email = "joao@exemplo.com",
            DataNascimento = new DateTime(1990, 5, 15)
        };

        WriteLine($"  ✅ Usuário criado: {usuario.Nome} ({usuario.Email})");

        // ❌ Esta linha causaria erro de compilação:
        // var usuarioInvalido = new Usuario { Nome = "Teste" }; // Email é required!

        WriteLine("  ⚠️  Tentativa de criar usuário sem email causaria erro de compilação");
    }

    static void DemonstrarRequiredInit()
    {
        var configuracao = new ConfiguracaoApp
        {
            NomeApp = "Sistema de Vendas",
            VersaoMinima = "1.0.0",
            ConnectionString = "Server=localhost;Database=vendas;",
            // ChaveAPI não é obrigatória (nullable)
        };

        WriteLine($"  ✅ Configuração: {configuracao.NomeApp} v{configuracao.VersaoMinima}");

        // Demonstrando imutabilidade após inicialização
        // configuracao.NomeApp = "Outro Nome"; // ❌ Erro - init only!
        WriteLine("  🔒 Configuração é imutável após inicialização");
    }

    static void DemonstrarRequiredHeranca()
    {
        var desenvolvedor = new Desenvolvedor
        {
            // Required da classe base
            Nome = "Ana Costa",
            Email = "ana@empresa.com",
            DataNascimento = new DateTime(1985, 8, 22),
            
            // Required da classe derivada
            Linguagens = new List<string> { "C#", "TypeScript", "Python" },
            NivelSenioridade = SenioridadeLevel.Senior
        };

        WriteLine($"  ✅ Desenvolvedor: {desenvolvedor.Nome}");
        WriteLine($"      Linguagens: [{string.Join(", ", desenvolvedor.Linguagens)}]");
        WriteLine($"      Nível: {desenvolvedor.NivelSenioridade}");

        var gerente = new Gerente
        {
            Nome = "Carlos Santos",
            Email = "carlos@empresa.com", 
            DataNascimento = new DateTime(1975, 3, 10),
            Equipe = new List<string> { "Time Frontend", "Time Backend" },
            Orcamento = 500000.00m
        };

        WriteLine($"  ✅ Gerente: {gerente.Nome}, Orçamento: {gerente.Orcamento:C}");
    }

    static void DemonstrarRequiredValidation()
    {
        try
        {
            var produto = new ProdutoComValidacao
            {
                Nome = "Notebook Gamer",
                Codigo = "NB-001",
                Preco = 2500.00m,
                Categoria = "Eletrônicos"
            };

            var validacao = produto.Validar();
            
            if (validacao.EhValido)
            {
                WriteLine($"  ✅ Produto válido: {produto.Nome} - {produto.Preco:C}");
            }
            else
            {
                WriteLine($"  ❌ Produto inválido: {string.Join(", ", validacao.Erros)}");
            }
        }
        catch (Exception ex)
        {
            WriteLine($"  ❌ Erro na validação: {ex.Message}");
        }
    }

    static void DemonstrarRequiredDTOs()
    {
        // Simulando dados que viriam de uma API
        var criarUsuarioRequest = new CriarUsuarioRequest
        {
            Nome = "Maria Oliveira",
            Email = "maria@exemplo.com",
            Senha = "MinhaSenh@123",
            Perfil = "Administrador"
        };

        WriteLine($"  📤 Request: Criar usuário {criarUsuarioRequest.Nome}");

        // Simulando resposta da API
        var response = new UsuarioResponse
        {
            Id = Guid.NewGuid(),
            Nome = criarUsuarioRequest.Nome,
            Email = criarUsuarioRequest.Email,
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        WriteLine($"  📥 Response: Usuário criado com ID {response.Id:N}");

        // Serialização JSON automática
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        WriteLine($"  📄 JSON Response:\n{json}");
    }

    static void DemonstrarSetsRequiredMembers()
    {
        // Usando construtor que satisfaz required members
        var config1 = new ConfiguracaoAvancada("App Production", "https://api.prod.com");
        WriteLine($"  ✅ Config via construtor: {config1.Nome}");

        // Usando object initializer
        var config2 = new ConfiguracaoAvancada
        {
            Nome = "App Development",
            BaseUrl = "https://api.dev.com",
            TimeoutSegundos = 30
        };
        WriteLine($"  ✅ Config via initializer: {config2.Nome}");
    }

    static void DemonstrarRequiredRecords()
    {
        var evento = new EventoSistema
        {
            Tipo = "UserLogin",
            Timestamp = DateTime.UtcNow,
            UsuarioId = Guid.NewGuid(),
            Dados = new Dictionary<string, object>
            {
                ["IP"] = "192.168.1.100",
                ["UserAgent"] = "Mozilla/5.0..."
            }
        };

        WriteLine($"  📅 Evento: {evento.Tipo} às {evento.Timestamp:HH:mm:ss}");

        // Criando novo evento baseado no anterior
        var eventoLogout = evento with 
        { 
            Tipo = "UserLogout",
            Timestamp = DateTime.UtcNow.AddMinutes(30)
        };

        WriteLine($"  📅 Evento derivado: {eventoLogout.Tipo} às {eventoLogout.Timestamp:HH:mm:ss}");
    }

    static void DemonstrarCasosUso()
    {
        WriteLine("  🎯 Entity Framework-like scenarios:");
        
        var entidade = new EntidadeBase
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow,
            CriadoPor = "sistema"
        };

        WriteLine($"     Entidade criada: {entidade.Id:N}");

        WriteLine("\n  🎯 Configuration scenarios:");
        
        var dbConfig = new DatabaseConfig
        {
            ConnectionString = "Server=localhost;Database=app;",
            Provider = "SqlServer",
            MaxRetries = 3
        };

        WriteLine($"     Database: {dbConfig.Provider} com {dbConfig.MaxRetries} tentativas");

        WriteLine("\n  🎯 Domain model scenarios:");
        
        var pedido = new Pedido
        {
            Numero = "PED-2025-001",
            ClienteId = Guid.NewGuid(),
            DataPedido = DateTime.UtcNow,
            Itens = new List<ItemPedido>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 50.00m },
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 1, PrecoUnitario = 100.00m }
            }
        };

        var total = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        WriteLine($"     Pedido {pedido.Numero}: {pedido.Itens.Count} itens, total: {total:C}");
    }
}
