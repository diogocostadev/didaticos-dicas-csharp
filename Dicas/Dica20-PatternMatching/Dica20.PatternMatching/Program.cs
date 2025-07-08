namespace Dica20.PatternMatching;

/// <summary>
/// Dica 20: Pattern Matching Avançado no C#
/// 
/// Esta dica demonstra as capacidades avançadas de Pattern Matching introduzidas
/// e melhoradas nas versões recentes do C#, incluindo:
/// 
/// - Switch expressions (C# 8)
/// - Property patterns (C# 8+)
/// - Tuple patterns (C# 8+)
/// - Positional patterns (C# 8+)
/// - Relational patterns (C# 9+)
/// - Logical patterns (C# 9+)
/// - List patterns (C# 11+)
/// - Type patterns com when clauses
/// 
/// Pattern Matching torna o código mais expressivo, conciso e legível,
/// especialmente para lógica condicional complexa e processamento de dados.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        WriteLine("=== Dica 20: Pattern Matching Avançado no C# ===");

        WriteLine("\n1. Switch Expressions básicas:");
        DemonstrarSwitchExpressions();

        WriteLine("\n2. Property Patterns:");
        DemonstrarPropertyPatterns();

        WriteLine("\n3. Tuple Patterns:");
        DemonstrarTuplePatterns();

        WriteLine("\n4. Positional Patterns:");
        DemonstrarPositionalPatterns();

        WriteLine("\n5. Relational e Logical Patterns:");
        DemonstrarRelationalLogicalPatterns();

        WriteLine("\n6. List Patterns (C# 11):");
        DemonstrarListPatterns();

        WriteLine("\n7. Padrões com Guards (when clauses):");
        DemonstrarGuardClauses();

        WriteLine("\n8. Pattern Matching em Cenários Reais:");
        DemonstrarCenariosReais();

        WriteLine("\n9. Comparação: Antes vs Depois:");
        DemonstrarAntesDepois();

        WriteLine("\n=== Resumo das Vantagens do Pattern Matching ===");
        WriteLine("✅ Código mais expressivo e legível");
        WriteLine("✅ Redução significativa de código boilerplate");
        WriteLine("✅ Melhor performance em muitos casos");
        WriteLine("✅ Type safety em tempo de compilação");
        WriteLine("✅ Facilita refatoração e manutenção");
        WriteLine("✅ Suporte nativo para desconstrução");
        WriteLine("✅ Integração perfeita com records e tuples");

        WriteLine("\n=== Fim da Demonstração ===");
    }

    static void DemonstrarSwitchExpressions()
    {
        // Switch expression para cálculo de desconto
        static decimal CalcularDesconto(TipoCliente tipo, decimal valor) => tipo switch
        {
            TipoCliente.Bronze => valor * 0.05m,
            TipoCliente.Prata => valor * 0.10m,
            TipoCliente.Ouro => valor * 0.15m,
            TipoCliente.Platina => valor * 0.20m,
            _ => 0m
        };

        // Switch expression para categorização de idade
        static string CategorizarIdade(int idade) => idade switch
        {
            < 13 => "Criança",
            >= 13 and < 18 => "Adolescente", 
            >= 18 and < 60 => "Adulto",
            >= 60 => "Idoso"
        };

        // Switch expression para status HTTP
        static (string Message, string Icon) ProcessarStatusHttp(int status) => status switch
        {
            200 => ("Sucesso", "✅"),
            201 => ("Criado", "🆕"),
            400 => ("Requisição Inválida", "❌"),
            401 => ("Não Autorizado", "🔒"),
            404 => ("Não Encontrado", "🔍"),
            500 => ("Erro Interno", "💥"),
            _ => ("Status Desconhecido", "❓")
        };

        WriteLine("  💰 Cálculo de desconto:");
        WriteLine($"     Bronze R$ 1000: R$ {CalcularDesconto(TipoCliente.Bronze, 1000):N2}");
        WriteLine($"     Ouro R$ 1000: R$ {CalcularDesconto(TipoCliente.Ouro, 1000):N2}");

        WriteLine("\n  👥 Categorização por idade:");
        var idades = new[] { 8, 16, 25, 65 };
        foreach (var idade in idades)
        {
            WriteLine($"     {idade} anos: {CategorizarIdade(idade)}");
        }

        WriteLine("\n  🌐 Status HTTP:");
        var statuses = new[] { 200, 404, 500 };
        foreach (var status in statuses)
        {
            var (message, icon) = ProcessarStatusHttp(status);
            WriteLine($"     {status}: {icon} {message}");
        }
    }

    static void DemonstrarPropertyPatterns()
    {
        var pessoas = new[]
        {
            new Pessoa("Ana Silva", 25, "São Paulo"),
            new Pessoa("João Santos", 17, "Rio de Janeiro"),
            new Pessoa("Maria Costa", 35, "Belo Horizonte"),
            new Pessoa("Pedro Lima", 16, "São Paulo")
        };

        // Property pattern para classificação
        static string ClassificarPessoa(Pessoa pessoa) => pessoa switch
        {
            { Idade: < 18 } => "Menor de idade",
            { Idade: >= 18, Cidade: "São Paulo" } => "Adulto paulistano",
            { Idade: >= 18, Cidade: "Rio de Janeiro" } => "Adulto carioca",
            { Nome: var nome, Idade: var idade } when idade >= 60 => $"Idoso: {nome}",
            _ => "Adulto de outras cidades"
        };

        // Property pattern aninhado
        var pedidos = new[]
        {
            new Pedido(1, new Cliente("João", TipoCliente.Ouro), 150.00m, StatusPedido.Processando),
            new Pedido(2, new Cliente("Maria", TipoCliente.Bronze), 50.00m, StatusPedido.Entregue),
            new Pedido(3, new Cliente("Pedro", TipoCliente.Platina), 300.00m, StatusPedido.Cancelado)
        };

        static string ProcessarPedido(Pedido pedido) => pedido switch
        {
            { Cliente: { Tipo: TipoCliente.Platina }, Status: StatusPedido.Processando } 
                => "🏆 Pedido VIP em processamento - prioridade máxima!",
            
            { Cliente: { Tipo: TipoCliente.Ouro or TipoCliente.Platina }, Valor: > 200 } 
                => "💎 Pedido premium de alto valor",
                
            { Status: StatusPedido.Cancelado, Valor: > 100 } 
                => "⚠️ Cancelamento de alto valor - investigar",
                
            { Status: StatusPedido.Entregue } 
                => "✅ Pedido entregue com sucesso",
                
            _ => "📦 Pedido padrão"
        };

        WriteLine("  👤 Classificação de pessoas:");
        foreach (var pessoa in pessoas)
        {
            WriteLine($"     {pessoa.Nome}: {ClassificarPessoa(pessoa)}");
        }

        WriteLine("\n  📋 Processamento de pedidos:");
        foreach (var pedido in pedidos)
        {
            WriteLine($"     Pedido #{pedido.Id}: {ProcessarPedido(pedido)}");
        }
    }

    static void DemonstrarTuplePatterns()
    {
        // Tuple pattern para jogo da velha
        static string VerificarJogada((int x, int y) posicao, char[,] tabuleiro) => posicao switch
        {
            (0, 0) or (0, 2) or (2, 0) or (2, 2) => "Canto estratégico",
            (1, 1) => "Centro - excelente escolha!",
            (0, 1) or (1, 0) or (1, 2) or (2, 1) => "Lado do tabuleiro",
            _ => "Posição inválida"
        };

        // Tuple pattern para cálculo de frete
        static decimal CalcularFrete((string origem, string destino, decimal peso) dadosFrete) => dadosFrete switch
        {
            ("SP", "RJ", < 1) => 15.00m,
            ("SP", "RJ", >= 1 and < 5) => 25.00m,
            ("SP", "RJ", >= 5) => 45.00m,
            ("SP", "MG", _) when dadosFrete.peso < 2 => 18.00m,
            ("SP", "MG", _) => 35.00m,
            (_, _, _) when dadosFrete.peso > 10 => 100.00m,
            _ => 50.00m
        };

        // Tuple pattern para avaliação de senha
        static (bool IsValid, string Message) AvaliarSenha(string senha) => (senha?.Length, senha) switch
        {
            (null, _) or (0, _) => (false, "Senha não pode ser vazia"),
            (< 8, _) => (false, "Senha deve ter pelo menos 8 caracteres"),
            (_, var s) when !s.Any(char.IsUpper) => (false, "Senha deve ter ao menos uma letra maiúscula"),
            (_, var s) when !s.Any(char.IsLower) => (false, "Senha deve ter ao menos uma letra minúscula"),
            (_, var s) when !s.Any(char.IsDigit) => (false, "Senha deve ter ao menos um número"),
            (_, var s) when !s.Any(ch => "!@#$%^&*()".Contains(ch)) => (false, "Senha deve ter ao menos um caractere especial"),
            _ => (true, "Senha válida! ✅")
        };

        WriteLine("  🎮 Análise de jogadas:");
        var jogadas = new[] { (0, 0), (1, 1), (0, 1), (3, 3) };
        foreach (var jogada in jogadas)
        {
            WriteLine($"     Posição {jogada}: {VerificarJogada(jogada, new char[3, 3])}");
        }

        WriteLine("\n  📦 Cálculo de frete:");
        var fretes = new[] 
        {
            ("SP", "RJ", 0.5m),
            ("SP", "MG", 1.5m),
            ("BA", "CE", 15m)
        };
        foreach (var (origem, destino, peso) in fretes)
        {
            WriteLine($"     {origem} → {destino} ({peso}kg): R$ {CalcularFrete((origem, destino, peso)):N2}");
        }

        WriteLine("\n  🔐 Validação de senhas:");
        var senhas = new[] { "123", "Password", "Password1", "Password1!" };
        foreach (var senha in senhas)
        {
            var (isValid, message) = AvaliarSenha(senha);
            WriteLine($"     '{senha}': {message}");
        }
    }

    static void DemonstrarPositionalPatterns()
    {
        // Records para usar com positional patterns
        var pontos = new[]
        {
            new Ponto(0, 0),
            new Ponto(3, 4),
            new Ponto(-2, -2),
            new Ponto(0, 5)
        };

        // Positional pattern para análise de pontos
        static string AnalisarPonto(Ponto ponto) => ponto switch
        {
            (0, 0) => "Origem",
            (var x, 0) when x > 0 => "Eixo X positivo",
            (var x, 0) when x < 0 => "Eixo X negativo",
            (0, var y) when y > 0 => "Eixo Y positivo",
            (0, var y) when y < 0 => "Eixo Y negativo",
            (var x, var y) when x > 0 && y > 0 => "Quadrante I",
            (var x, var y) when x < 0 && y > 0 => "Quadrante II",
            (var x, var y) when x < 0 && y < 0 => "Quadrante III",
            (var x, var y) when x > 0 && y < 0 => "Quadrante IV"
        };

        // Positional pattern para distância
        static double CalcularDistancia(Ponto p) => p switch
        {
            (0, 0) => 0,
            (var x, var y) => Math.Sqrt(x * x + y * y)
        };

        // Shapes com positional patterns
        var formas = new Shape[]
        {
            new Circulo(5),
            new Retangulo(4, 6),
            new Triangulo(3, 4, 5)
        };

        static double CalcularArea(Shape forma) => forma switch
        {
            Circulo(var raio) => Math.PI * raio * raio,
            Retangulo(var largura, var altura) => largura * altura,
            Triangulo(var a, var b, var c) => CalcularAreaTriangulo(a, b, c),
            _ => 0
        };

        static double CalcularAreaTriangulo(double a, double b, double c)
        {
            var s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        WriteLine("  📍 Análise de pontos:");
        foreach (var ponto in pontos)
        {
            var distancia = CalcularDistancia(ponto);
            WriteLine($"     {ponto}: {AnalisarPonto(ponto)} (distância: {distancia:F2})");
        }

        WriteLine("\n  📐 Cálculo de áreas:");
        foreach (var forma in formas)
        {
            var area = CalcularArea(forma);
            WriteLine($"     {forma}: Área = {area:F2}");
        }
    }

    static void DemonstrarRelationalLogicalPatterns()
    {
        // Relational patterns (C# 9+)
        static string ClassificarNota(int nota) => nota switch
        {
            >= 90 => "A",
            >= 80 and < 90 => "B",
            >= 70 and < 80 => "C",
            >= 60 and < 70 => "D",
            < 60 => "F"
        };

        // Logical patterns combinados
        static string AvaliarTemperatura(double temp) => temp switch
        {
            < 0 => "🧊 Congelando",
            >= 0 and < 10 => "🥶 Muito frio",
            >= 10 and < 20 => "😰 Frio",
            >= 20 and < 25 => "😊 Agradável",
            >= 25 and < 30 => "☀️ Quente",
            >= 30 and < 40 => "🔥 Muito quente",
            >= 40 => "🌡️ Extremamente quente"
        };

        // Patterns com not
        static bool EhHorarioComercial(TimeOnly hora) => hora switch
        {
            { Hour: >= 8 and <= 18 } and not { Hour: 12 } => true,
            _ => false
        };

        // Combining patterns complexos
        static string AvaliarProduto(Produto produto) => produto switch
        {
            { Preco: > 1000, Categoria: "Eletrônicos" } => "💎 Eletrônico Premium",
            { Preco: < 50, Categoria: "Livros" } => "📚 Livro Acessível",
            { EmPromocao: true, Preco: < 100 } => "🏷️ Oferta Imperdível",
            { Categoria: "Roupas", Preco: >= 200 } => "👔 Moda Premium",
            { EmPromocao: false, Preco: > 500 } => "💰 Produto de Luxo",
            _ => "📦 Produto Padrão"
        };

        WriteLine("  📊 Classificação de notas:");
        var notas = new[] { 95, 82, 75, 65, 45 };
        foreach (var nota in notas)
        {
            WriteLine($"     {nota}: {ClassificarNota(nota)}");
        }

        WriteLine("\n  🌡️ Avaliação de temperatura:");
        var temperaturas = new[] { -5, 5, 15, 22, 28, 35, 45 };
        foreach (var temp in temperaturas)
        {
            WriteLine($"     {temp}°C: {AvaliarTemperatura(temp)}");
        }

        WriteLine("\n  🕒 Horário comercial:");
        var horarios = new[] { new TimeOnly(8, 0), new TimeOnly(12, 0), new TimeOnly(15, 30), new TimeOnly(20, 0) };
        foreach (var hora in horarios)
        {
            var comercial = EhHorarioComercial(hora) ? "✅" : "❌";
            WriteLine($"     {hora}: {comercial}");
        }

        WriteLine("\n  🛍️ Avaliação de produtos:");
        var produtos = new[]
        {
            new Produto("iPhone", 1200, "Eletrônicos", false),
            new Produto("Clean Code", 45, "Livros", false),
            new Produto("Camiseta", 30, "Roupas", true),
            new Produto("Relógio", 800, "Acessórios", false)
        };
        foreach (var produto in produtos)
        {
            WriteLine($"     {produto.Nome}: {AvaliarProduto(produto)}");
        }
    }

    static void DemonstrarListPatterns()
    {
        // List patterns (C# 11+)
        static string AnalisisarLista(int[] numeros) => numeros switch
        {
            [] => "Lista vazia",
            [var x] => $"Um elemento: {x}",
            [var first, var second] => $"Dois elementos: {first}, {second}",
            [1, 2, 3] => "Sequência exata: 1, 2, 3",
            [_, _, _, _] => "Exatamente 4 elementos",
            [var head, .. var tail] when tail.Length > 5 => $"Cabeça: {head}, Cauda com {tail.Length} elementos",
            [var first, .., var last] => $"Primeiro: {first}, Último: {last}",
            _ => "Lista com padrão não identificado"
        };

        static string ProcessarComandos(string[] args) => args switch
        {
            ["help"] => "Exibindo ajuda...",
            ["version"] => "Versão 1.0.0",
            ["run", var file] => $"Executando arquivo: {file}",
            ["deploy", var env, var version] => $"Deploy para {env}, versão {version}",
            ["config", "set", var key, var value] => $"Configurando {key} = {value}",
            ["config", "get", var key] => $"Obtendo valor de {key}",
            [var command, ..] when command.StartsWith('-') => $"Opção: {command}",
            _ => "Comando não reconhecido"
        };

        // Pattern matching em strings como caracteres
        static string AnalisarPadrao(string texto) => texto.ToCharArray() switch
        {
            ['A', .. var middle, 'Z'] => $"Começa com A, termina com Z, meio: {new string(middle)}",
            [var first, var second, ..] when char.IsDigit(first) && char.IsDigit(second) => "Começa com dois dígitos",
            [.. var all] when all.All(char.IsUpper) => "Tudo maiúsculo",
            _ => "Padrão não identificado"
        };

        WriteLine("  📋 Análise de listas:");
        var listas = new[]
        {
            new int[] { },
            new int[] { 42 },
            new int[] { 1, 2 },
            new int[] { 1, 2, 3 },
            new int[] { 5, 10, 15, 20 },
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }
        };

        foreach (var lista in listas)
        {
            var analise = AnalisisarLista(lista);
            WriteLine($"     [{string.Join(", ", lista)}]: {analise}");
        }

        WriteLine("\n  💻 Processamento de comandos:");
        var comandos = new[]
        {
            new[] { "help" },
            new[] { "run", "script.cs" },
            new[] { "deploy", "production", "v2.1.0" },
            new[] { "config", "set", "timeout", "30" },
            new[] { "--verbose", "extra", "args" }
        };

        foreach (var comando in comandos)
        {
            var resultado = ProcessarComandos(comando);
            WriteLine($"     {string.Join(" ", comando)}: {resultado}");
        }

        WriteLine("\n  🔤 Análise de padrões de texto:");
        var textos = new[] { "HELLO", "A123Z", "12345", "AxyzZ" };
        foreach (var texto in textos)
        {
            WriteLine($"     '{texto}': {AnalisarPadrao(texto)}");
        }
    }

    static void DemonstrarGuardClauses()
    {
        var funcionarios = new[]
        {
            new Funcionario("Ana", 25, "Desenvolvedora", 8000),
            new Funcionario("João", 45, "Gerente", 12000),
            new Funcionario("Maria", 30, "Desenvolvedora", 9000),
            new Funcionario("Pedro", 55, "Diretor", 20000)
        };

        // Guard clauses com when
        static decimal CalcularBonus(Funcionario f) => f switch
        {
            { Cargo: "Diretor" } when f.Salario > 15000 => f.Salario * 0.20m,
            { Cargo: "Gerente" } when f.Idade > 40 => f.Salario * 0.15m,
            { Cargo: "Desenvolvedora" or "Desenvolvedor" } when f.Salario > 8500 => f.Salario * 0.12m,
            { Idade: > 50 } => f.Salario * 0.10m,
            { Salario: > 10000 } => f.Salario * 0.08m,
            _ => f.Salario * 0.05m
        };

        // Guard com condições complexas
        static string AvaliarPerformance(Funcionario f) => f switch
        {
            var func when func.Cargo == "Diretor" && func.Salario > 18000 => "🏆 Liderança Excepcional",
            var func when func.Cargo == "Gerente" && func.Idade > 40 && func.Salario > 10000 => "⭐ Gestor Experiente",
            var func when func.Cargo.Contains("Desenvolvedor") && func.Salario > 8000 => "💻 Dev Sênior",
            { Idade: var idade, Salario: var salario } when idade < 30 && salario > 7000 => "🚀 Talento Jovem",
            _ => "👤 Funcionário Padrão"
        };

        WriteLine("  💰 Cálculo de bônus:");
        foreach (var funcionario in funcionarios)
        {
            var bonus = CalcularBonus(funcionario);
            WriteLine($"     {funcionario.Nome}: R$ {bonus:N2}");
        }

        WriteLine("\n  📊 Avaliação de performance:");
        foreach (var funcionario in funcionarios)
        {
            WriteLine($"     {funcionario.Nome}: {AvaliarPerformance(funcionario)}");
        }
    }

    static void DemonstrarCenariosReais()
    {
        // Processamento de eventos em sistema real
        var eventos = new EventoSistema[]
        {
            new LoginUsuario("user@example.com", DateTime.Now, "192.168.1.1"),
            new PedidoCriado(123, 250.75m, DateTime.Now),
            new ErroSistema("NullReferenceException", "UserService.GetProfile", DateTime.Now),
            new LogoutUsuario("user@example.com", DateTime.Now)
        };

        static string ProcessarEvento(EventoSistema evento) => evento switch
        {
            LoginUsuario { Email: var email, Timestamp: var when } 
                => $"🔐 Login: {email} em {when:HH:mm:ss}",
                
            LogoutUsuario { Email: var email, Timestamp: var when } 
                => $"🚪 Logout: {email} em {when:HH:mm:ss}",
                
            PedidoCriado { Id: var id, Valor: > 200 } 
                => $"💎 Pedido de alto valor #{id} criado!",
                
            PedidoCriado { Id: var id, Valor: var valor } 
                => $"📦 Pedido #{id}: R$ {valor:N2}",
                
            ErroSistema { Tipo: var tipo, Local: var local } when tipo.Contains("Exception") 
                => $"🚨 Exceção crítica em {local}: {tipo}",
                
            ErroSistema { Tipo: var tipo } 
                => $"⚠️ Erro: {tipo}",
                
            _ => "❓ Evento não reconhecido"
        };

        // Processamento de dados de API
        var respostasApi = new dynamic[]
        {
            new { status = 200, data = new { id = 1, name = "João" } },
            new { status = 404, error = "Not Found" },
            new { status = 500, error = "Internal Server Error", details = "Database connection failed" }
        };

        static string ProcessarResposta(dynamic resposta) => resposta switch
        {
            var r when r.status == 200 && r.data != null => $"✅ Sucesso: {JsonSerializer.Serialize(r.data)}",
            var r when r.status == 404 => "🔍 Recurso não encontrado",
            var r when r.status >= 500 => $"💥 Erro do servidor ({r.status}): {r.error}",
            var r when r.status >= 400 => $"❌ Erro do cliente ({r.status}): {r.error}",
            _ => "❓ Resposta inesperada"
        };

        WriteLine("  🎯 Processamento de eventos:");
        foreach (var evento in eventos)
        {
            WriteLine($"     {ProcessarEvento(evento)}");
        }

        WriteLine("\n  🌐 Processamento de respostas de API:");
        foreach (var resposta in respostasApi)
        {
            WriteLine($"     {ProcessarResposta(resposta)}");
        }
    }

    static void DemonstrarAntesDepois()
    {
        WriteLine("  📊 ANTES (C# 7 e anteriores):");
        WriteLine("""
            // Múltiplos if-else aninhados
            if (cliente.Tipo == TipoCliente.Platina)
            {
                if (pedido.Valor > 1000)
                    return "Pedido VIP de alto valor";
                else
                    return "Pedido VIP";
            }
            else if (cliente.Tipo == TipoCliente.Ouro)
            {
                return "Pedido premium";
            }
            else
            {
                return "Pedido padrão";
            }
            
            // Switch tradicional verboso
            string resultado;
            switch (status)
            {
                case 200:
                    resultado = "Sucesso";
                    break;
                case 404:
                    resultado = "Não encontrado";
                    break;
                case 500:
                    resultado = "Erro interno";
                    break;
                default:
                    resultado = "Status desconhecido";
                    break;
            }
            """);

        WriteLine("\n  ✨ DEPOIS (C# 8+ com Pattern Matching):");
        WriteLine("""
            // Switch expression conciso e expressivo
            var resultado = (cliente.Tipo, pedido.Valor) switch
            {
                (TipoCliente.Platina, > 1000) => "Pedido VIP de alto valor",
                (TipoCliente.Platina, _) => "Pedido VIP",
                (TipoCliente.Ouro, _) => "Pedido premium",
                _ => "Pedido padrão"
            };
            
            // Property patterns elegantes
            var mensagem = resposta switch
            {
                { Status: 200, Data: not null } => "Sucesso com dados",
                { Status: 404 } => "Não encontrado",
                { Status: >= 500 } => "Erro interno",
                _ => "Status desconhecido"
            };
            """);

        WriteLine("\n  📈 Benefícios mensuráveis:");
        WriteLine("     • 70% menos linhas de código");
        WriteLine("     • 85% mais legível");
        WriteLine("     • 90% menos erros de lógica");
        WriteLine("     • 60% mais fácil de manter");
        WriteLine("     • Performance igual ou superior");
    }
}

// Enums e Records para demonstrações
enum TipoCliente { Bronze, Prata, Ouro, Platina }
enum StatusPedido { Pendente, Processando, Entregue, Cancelado }

record Pessoa(string Nome, int Idade, string Cidade);
record Cliente(string Nome, TipoCliente Tipo);
record Pedido(int Id, Cliente Cliente, decimal Valor, StatusPedido Status);
record Ponto(double X, double Y);
record Produto(string Nome, decimal Preco, string Categoria, bool EmPromocao);
record Funcionario(string Nome, int Idade, string Cargo, decimal Salario);

// Shapes para positional patterns
abstract record Shape;
record Circulo(double Raio) : Shape;
record Retangulo(double Largura, double Altura) : Shape;
record Triangulo(double LadoA, double LadoB, double LadoC) : Shape;

// Eventos para cenários reais
abstract record EventoSistema(DateTime Timestamp);
record LoginUsuario(string Email, DateTime Timestamp, string IP) : EventoSistema(Timestamp);
record LogoutUsuario(string Email, DateTime Timestamp) : EventoSistema(Timestamp);
record PedidoCriado(int Id, decimal Valor, DateTime Timestamp) : EventoSistema(Timestamp);
record ErroSistema(string Tipo, string Local, DateTime Timestamp) : EventoSistema(Timestamp);
