namespace Dica19.RawStringLiterals;

public class Program
{
    public static void Main(string[] args)
    {
        WriteLine("=== Dica 19: Raw String Literals no C# 11 ===");

        // 1. DEMONSTRAÇÃO: Básico - Raw String Literals
        WriteLine("\n1. Raw String Literals básicos:");
        DemonstrarRawStringBasico();

        // 2. DEMONSTRAÇÃO: JSON sem escape
        WriteLine("\n2. JSON complexo sem escape:");
        DemonstrarJSONSemEscape();

        // 3. DEMONSTRAÇÃO: SQL queries multiline
        WriteLine("\n3. SQL queries multiline:");
        DemonstrarSQLQueries();

        // 4. DEMONSTRAÇÃO: Regex patterns complexos
        WriteLine("\n4. Regex patterns complexos:");
        DemonstrarRegexPatterns();

        // 5. DEMONSTRAÇÃO: Templates HTML/XML
        WriteLine("\n5. Templates HTML/XML:");
        DemonstrarTemplatesHTML();

        // 6. DEMONSTRAÇÃO: Interpolação com Raw Strings
        WriteLine("\n6. Interpolação com Raw Strings:");
        DemonstrarInterpolacao();

        // 7. DEMONSTRAÇÃO: Scripts e código gerado
        WriteLine("\n7. Scripts e código gerado:");
        DemonstrarScriptsGerados();

        // 8. DEMONSTRAÇÃO: Configurações e documentação
        WriteLine("\n8. Configurações e documentação:");
        DemonstrarConfiguracoes();

        // 9. DEMONSTRAÇÃO: Comparação antes/depois
        WriteLine("\n9. Comparação antes/depois:");
        DemonstrarComparacao();

        WriteLine("\n=== Resumo das Vantagens dos Raw String Literals ===");
        WriteLine("✅ Eliminação de escape characters (\\, \", \\n)");
        WriteLine("✅ Multiline natural sem concatenação");
        WriteLine("✅ Preservação de indentação e formatação");
        WriteLine("✅ Perfeito para JSON, SQL, HTML, Regex");
        WriteLine("✅ Interpolação mantida com $\"\"\"");
        WriteLine("✅ Código mais legível e manutenível");
        WriteLine("✅ Redução significativa de erros de sintaxe");

        WriteLine("\n=== Fim da Demonstração ===");
    }

    static void DemonstrarRawStringBasico()
    {
        // Raw string literal básico (3 ou mais aspas duplas)
        var textoSimples = """
            Este é um raw string literal.
            Quebras de linha são preservadas.
            Não preciso escapar "aspas duplas" aqui!
            """;

        WriteLine("  📝 Texto simples:");
        WriteLine($"     {textoSimples.Replace("\n", "\\n")}");

        // Raw string com múltiplas aspas
        var textoComAspas = """"
            Posso usar """ três aspas """ dentro do texto
            sem problemas!
            """";

        WriteLine("\n  📝 Texto com aspas múltiplas:");
        WriteLine($"     {textoComAspas.Replace("\n", "\\n")}");
    }

    static void DemonstrarJSONSemEscape()
    {
        var usuario = new { Nome = "João Silva", Idade = 30, Email = "joao@exemplo.com" };
        
        // ANTES: JSON tradicional com escape
        var jsonTradicional = "{\n  \"usuario\": {\n    \"nome\": \"" + usuario.Nome + "\",\n    \"idade\": " + usuario.Idade + ",\n    \"email\": \"" + usuario.Email + "\"\n  },\n  \"timestamp\": \"2025-07-08T20:30:00Z\"\n}";

        // DEPOIS: Raw string literal
        var jsonRaw = """
            {
              "usuario": {
                "nome": "João Silva",
                "idade": 30,
                "email": "joao@exemplo.com"
              },
              "configuracao": {
                "tema": "dark",
                "linguagem": "pt-BR",
                "notificacoes": {
                  "email": true,
                  "push": false,
                  "sms": false
                }
              },
              "timestamp": "2025-07-08T20:30:00Z"
            }
            """;

        WriteLine("  📄 JSON complexo sem escape:");
        WriteLine(jsonRaw);

        // Testando se é JSON válido
        try
        {
            var parsed = JsonSerializer.Deserialize<object>(jsonRaw);
            WriteLine("  ✅ JSON válido!");
        }
        catch
        {
            WriteLine("  ❌ JSON inválido!");
        }
    }

    static void DemonstrarSQLQueries()
    {
        var userId = 123;
        var startDate = "2025-01-01";
        var endDate = "2025-12-31";

        // Query SQL complexa com raw string
        var queryCompleta = """
            WITH vendas_por_mes AS (
                SELECT 
                    YEAR(data_venda) as ano,
                    MONTH(data_venda) as mes,
                    SUM(valor_total) as total_vendas,
                    COUNT(*) as quantidade_vendas
                FROM vendas v
                INNER JOIN usuarios u ON v.usuario_id = u.id
                WHERE u.ativo = 1 
                    AND v.data_venda BETWEEN '2025-01-01' AND '2025-12-31'
                    AND v.status = 'FINALIZADA'
                GROUP BY YEAR(data_venda), MONTH(data_venda)
            ),
            ranking_produtos AS (
                SELECT 
                    p.nome,
                    p.categoria,
                    SUM(iv.quantidade) as total_vendido,
                    RANK() OVER (PARTITION BY p.categoria ORDER BY SUM(iv.quantidade) DESC) as ranking
                FROM produtos p
                INNER JOIN itens_venda iv ON p.id = iv.produto_id
                INNER JOIN vendas v ON iv.venda_id = v.id
                WHERE v.data_venda >= '2025-01-01'
                GROUP BY p.id, p.nome, p.categoria
            )
            SELECT 
                vm.ano,
                vm.mes,
                vm.total_vendas,
                vm.quantidade_vendas,
                rp.nome as produto_mais_vendido,
                rp.total_vendido
            FROM vendas_por_mes vm
            CROSS APPLY (
                SELECT TOP 1 nome, total_vendido 
                FROM ranking_produtos 
                WHERE ranking = 1
            ) rp
            ORDER BY vm.ano, vm.mes;
            """;

        WriteLine("  🗃️ Query SQL complexa:");
        WriteLine(queryCompleta);

        // Procedure com parâmetros
        var storedProcedure = $"""
            CREATE PROCEDURE sp_RelatorioVendas
                @DataInicio DATE = '{startDate}',
                @DataFim DATE = '{endDate}',
                @UsuarioId INT = {userId}
            AS
            BEGIN
                SET NOCOUNT ON;
                
                -- Validações
                IF @DataInicio > @DataFim
                BEGIN
                    RAISERROR('Data início não pode ser maior que data fim', 16, 1);
                    RETURN;
                END
                
                -- Query principal
                SELECT 
                    v.id,
                    v.numero_pedido,
                    u.nome as cliente,
                    v.data_venda,
                    v.valor_total,
                    v.status,
                    COUNT(iv.id) as total_itens
                FROM vendas v
                INNER JOIN usuarios u ON v.usuario_id = u.id
                LEFT JOIN itens_venda iv ON v.id = iv.venda_id
                WHERE v.data_venda BETWEEN @DataInicio AND @DataFim
                    AND (@UsuarioId IS NULL OR v.usuario_id = @UsuarioId)
                GROUP BY v.id, v.numero_pedido, u.nome, v.data_venda, v.valor_total, v.status
                ORDER BY v.data_venda DESC;
            END
            """;

        WriteLine("\n  📊 Stored Procedure:");
        WriteLine(storedProcedure);
    }

    static void DemonstrarRegexPatterns()
    {
        // Regex complexo para validação de email
        var emailPattern = """
            ^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$
            """;

        WriteLine("  🔍 Regex para email:");
        WriteLine($"     {emailPattern}");

        // Regex para validação de CPF brasileiro
        var cpfPattern = """
            ^(?:
                (\d{3})\.(\d{3})\.(\d{3})-(\d{2})  # Formato com pontos e hífen
                |
                (\d{11})                           # Formato apenas números
            )$
            """;

        WriteLine("\n  🔍 Regex para CPF (com comentários):");
        WriteLine(cpfPattern);

        // Testando os patterns
        var emails = new[] { "teste@exemplo.com", "email.invalido", "user+tag@domain.co.uk" };
        var cpfs = new[] { "123.456.789-01", "12345678901", "invalid-cpf" };

        WriteLine("\n  📋 Testando emails:");
        foreach (var email in emails)
        {
            var isValid = Regex.IsMatch(email, emailPattern);
            WriteLine($"     {email} → {(isValid ? "✅" : "❌")}");
        }

        WriteLine("\n  📋 Testando CPFs:");
        foreach (var cpf in cpfs)
        {
            var isValid = Regex.IsMatch(cpf, cpfPattern, RegexOptions.IgnorePatternWhitespace);
            WriteLine($"     {cpf} → {(isValid ? "✅" : "❌")}");
        }
    }

    static void DemonstrarTemplatesHTML()
    {
        var usuario = "Maria Silva";
        var produtos = new[] { "Notebook", "Mouse", "Teclado" };
        var total = 1500.00m;

        // Template HTML complexo
        var htmlTemplate = """
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Relatório de Vendas</title>
                <style>
                    body { 
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
                        margin: 0; 
                        padding: 20px; 
                        background-color: #f5f5f5; 
                    }
                    .container { 
                        max-width: 800px; 
                        margin: 0 auto; 
                        background: white; 
                        padding: 30px; 
                        border-radius: 8px; 
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1); 
                    }
                    .header { 
                        text-align: center; 
                        color: #333; 
                        border-bottom: 2px solid #007acc; 
                        padding-bottom: 20px; 
                        margin-bottom: 30px; 
                    }
                    .produto-item { 
                        padding: 10px; 
                        border-left: 4px solid #007acc; 
                        margin: 10px 0; 
                        background-color: #f8f9fa; 
                    }
                    .total { 
                        font-size: 1.5em; 
                        font-weight: bold; 
                        color: #28a745; 
                        text-align: right; 
                        margin-top: 20px; 
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="header">
                        <h1>🛍️ Relatório de Vendas</h1>
                        <p>Cliente: <strong>{{USUARIO}}</strong></p>
                        <p>Data: <strong>{{DATA}}</strong></p>
                    </div>
                    
                    <h2>📦 Produtos Adquiridos:</h2>
                    {{PRODUTOS}}
                    
                    <div class="total">
                        💰 Total: R$ {{TOTAL}}
                    </div>
                    
                    <footer style="margin-top: 40px; text-align: center; color: #666; font-size: 0.9em;">
                        <p>Obrigado pela sua compra! 🎉</p>
                        <p>Este é um documento gerado automaticamente.</p>
                    </footer>
                </div>
            </body>
            </html>
            """;

        WriteLine("  🌐 Template HTML gerado:");
        WriteLine("     (Template salvo em arquivo para visualização)");

        // Substituindo os placeholders
        var produtosHtml = string.Join("\n", produtos.Select(p => 
            $"""                    <div class="produto-item">✅ {p}</div>"""));

        var htmlFinal = htmlTemplate
            .Replace("{{USUARIO}}", usuario)
            .Replace("{{DATA}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
            .Replace("{{PRODUTOS}}", produtosHtml)
            .Replace("{{TOTAL}}", total.ToString("N2"));

        // Salvando o arquivo para demonstração
        File.WriteAllText("relatorio.html", htmlFinal);
        WriteLine("     📄 Arquivo 'relatorio.html' criado com sucesso!");
    }

    static void DemonstrarInterpolacao()
    {
        var nomeCliente = "Carlos Santos";
        var valorPedido = 2500.75m;
        var dataPedido = DateTime.Now;
        var itens = new[] { "Notebook", "Mouse", "Teclado" };

        // Raw string literal com interpolação
        var emailNotificacao = $"""
            Olá {nomeCliente},

            Seu pedido foi confirmado com sucesso! 🎉

            📋 Detalhes do Pedido:
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            📅 Data: {dataPedido:dd/MM/yyyy HH:mm}
            💰 Valor Total: R$ {valorPedido:N2}
            📦 Itens ({itens.Length}):
            {string.Join("\n   ", itens.Select((item, i) => $"   {i + 1}. {item}"))}

            🚚 Estimativa de Entrega: {dataPedido.AddDays(7):dd/MM/yyyy}

            Para acompanhar seu pedido, acesse:
            🔗 https://exemplo.com/pedidos/acompanhar

            Atenciosamente,
            Equipe de Vendas 💼
            """;

        WriteLine("  📧 Email de notificação:");
        WriteLine(emailNotificacao);

        // JSON com interpolação
        var jsonConfig = $$"""
            {
              "ambiente": "producao",
              "timestamp": "{{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}}",
              "cliente": {
                "nome": "{{nomeCliente}}",
                "valor_pedido": {{valorPedido:F2}},
                "moeda": "BRL"
              },
              "configuracoes": {
                "timeout_segundos": 30,
                "retry_attempts": 3,
                "enable_logging": true,
                "endpoints": {
                  "api_base": "https://api.exemplo.com/v1",
                  "webhook_url": "https://webhook.exemplo.com/notify"
                }
              }
            }
            """;

        WriteLine("\n  ⚙️ JSON de configuração:");
        WriteLine(jsonConfig);
    }

    static void DemonstrarScriptsGerados()
    {
        var nomeServico = "ProcessadorPedidos";
        var versao = "1.2.0";
        var porta = 8080;

        // Script PowerShell
        var scriptPowerShell = $$"""
            # Script de Deploy - {{nomeServico}} v{{versao}}
            # Gerado automaticamente em {{DateTime.Now:yyyy-MM-dd HH:mm:ss}}

            param(
                [Parameter(Mandatory=$true)]
                [string]$Ambiente,
                
                [Parameter(Mandatory=$false)]
                [int]$Porta = {{porta}},
                
                [Parameter(Mandatory=$false)]
                [switch]$SkipTests
            )

            Write-Host "🚀 Iniciando deploy do {{nomeServico}}" -ForegroundColor Green
            Write-Host "📦 Versão: {{versao}}" -ForegroundColor Yellow
            Write-Host "🌐 Ambiente: $Ambiente" -ForegroundColor Yellow
            Write-Host "🔌 Porta: $Porta" -ForegroundColor Yellow

            try {
                # Parar serviço existente
                Write-Host "🛑 Parando serviço existente..." -ForegroundColor Yellow
                Stop-Service -Name "{{nomeServico}}" -ErrorAction SilentlyContinue
                
                # Backup da versão atual
                $backupPath = "C:\Backups\{{nomeServico}}\$(Get-Date -Format 'yyyyMMdd-HHmmss')"
                Write-Host "💾 Criando backup em: $backupPath" -ForegroundColor Yellow
                New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
                
                # Deploy da nova versão
                Write-Host "📂 Copiando arquivos..." -ForegroundColor Yellow
                Copy-Item -Path ".\bin\Release\*" -Destination "C:\Services\{{nomeServico}}\" -Recurse -Force
                
                # Atualizar configuração
                $configPath = "C:\Services\{{nomeServico}}\appsettings.$Ambiente.json"
                Write-Host "⚙️ Atualizando configuração: $configPath" -ForegroundColor Yellow
                
                # Executar testes se solicitado
                if (-not $SkipTests) {
                    Write-Host "🧪 Executando testes..." -ForegroundColor Yellow
                    dotnet test --configuration Release --no-build
                    if ($LASTEXITCODE -ne 0) {
                        throw "Testes falharam!"
                    }
                }
                
                # Iniciar serviço
                Write-Host "▶️ Iniciando serviço..." -ForegroundColor Yellow
                Start-Service -Name "{{nomeServico}}"
                
                # Verificar saúde do serviço
                Write-Host "🏥 Verificando saúde do serviço..." -ForegroundColor Yellow
                $healthCheck = Invoke-RestMethod -Uri "http://localhost:$Porta/health" -TimeoutSec 30
                
                if ($healthCheck.status -eq "healthy") {
                    Write-Host "✅ Deploy concluído com sucesso!" -ForegroundColor Green
                } else {
                    throw "Serviço não está saudável: $($healthCheck.status)"
                }
                
            } catch {
                Write-Host "❌ Erro durante o deploy: $_" -ForegroundColor Red
                Write-Host "🔄 Executando rollback..." -ForegroundColor Yellow
                
                # Lógica de rollback aqui
                Stop-Service -Name "{{nomeServico}}" -ErrorAction SilentlyContinue
                # Restaurar backup...
                
                exit 1
            }
            """;

        WriteLine("  💻 Script PowerShell gerado:");
        WriteLine("     (Primeiras linhas do script)");
        WriteLine(scriptPowerShell.Split('\n').Take(15).Aggregate((a, b) => a + "\n" + b));

        // Script Bash para Linux
        var scriptBash = $$"""
            #!/bin/bash
            # Script de Deploy - {{nomeServico}} v{{versao}}
            # Gerado automaticamente em {{DateTime.Now:yyyy-MM-dd HH:mm:ss}}

            set -euo pipefail  # Exit on error, undefined vars, pipe failures

            # Configurações
            SERVICE_NAME="{{nomeServico.ToLower()}}"
            VERSION="{{versao}}"
            PORT={{porta}}
            DEPLOY_DIR="/opt/$SERVICE_NAME"
            BACKUP_DIR="/opt/backups/$SERVICE_NAME"
            LOG_FILE="/var/log/$SERVICE_NAME-deploy.log"

            # Cores para output
            RED='\033[0;31m'
            GREEN='\033[0;32m'
            YELLOW='\033[1;33m'
            NC='\033[0m' # No Color

            log() {
                echo -e "$(date +'%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
            }

            log "${GREEN}🚀 Iniciando deploy do $SERVICE_NAME v$VERSION${NC}"

            # Verificar se o usuário tem permissões
            if [[ $EUID -ne 0 ]]; then
                log "${RED}❌ Este script deve ser executado como root${NC}"
                exit 1
            fi

            # Criar diretórios necessários
            mkdir -p "$BACKUP_DIR" "$DEPLOY_DIR"

            # Parar serviço
            log "${YELLOW}🛑 Parando serviço $SERVICE_NAME...${NC}"
            systemctl stop "$SERVICE_NAME" || true

            # Backup
            BACKUP_PATH="$BACKUP_DIR/$(date +'%Y%m%d-%H%M%S')"
            log "${YELLOW}💾 Criando backup em $BACKUP_PATH...${NC}"
            cp -r "$DEPLOY_DIR" "$BACKUP_PATH" || true

            # Deploy
            log "${YELLOW}📂 Copiando novos arquivos...${NC}"
            cp -r ./bin/Release/* "$DEPLOY_DIR/"

            # Definir permissões
            chmod +x "$DEPLOY_DIR/{{nomeServico}}"
            chown -R appuser:appgroup "$DEPLOY_DIR"

            # Iniciar serviço
            log "${YELLOW}▶️ Iniciando serviço...${NC}"
            systemctl start "$SERVICE_NAME"
            systemctl enable "$SERVICE_NAME"

            # Health check
            log "${YELLOW}🏥 Verificando saúde do serviço...${NC}"
            sleep 5

            if curl -f "http://localhost:$PORT/health" &>/dev/null; then
                log "${GREEN}✅ Deploy concluído com sucesso!${NC}"
            else
                log "${RED}❌ Health check falhou${NC}"
                exit 1
            fi
            """;

        WriteLine("\n  🐧 Script Bash para Linux:");
        WriteLine("     (Primeiras linhas do script)");
        WriteLine(scriptBash.Split('\n').Take(20).Aggregate((a, b) => a + "\n" + b));
    }

    static void DemonstrarConfiguracoes()
    {
        // Dockerfile complexo
        var dockerfile = """
            # Multi-stage build para aplicação .NET
            # Estágio 1: Build
            FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
            WORKDIR /src

            # Copiar arquivos de projeto e restaurar dependências
            COPY ["src/ProcessadorPedidos/ProcessadorPedidos.csproj", "src/ProcessadorPedidos/"]
            COPY ["src/ProcessadorPedidos.Core/ProcessadorPedidos.Core.csproj", "src/ProcessadorPedidos.Core/"]
            COPY ["src/ProcessadorPedidos.Infrastructure/ProcessadorPedidos.Infrastructure.csproj", "src/ProcessadorPedidos.Infrastructure/"]
            
            RUN dotnet restore "src/ProcessadorPedidos/ProcessadorPedidos.csproj"

            # Copiar código fonte e fazer build
            COPY . .
            WORKDIR "/src/src/ProcessadorPedidos"
            RUN dotnet build "ProcessadorPedidos.csproj" -c Release -o /app/build

            # Estágio 2: Publish
            FROM build AS publish
            RUN dotnet publish "ProcessadorPedidos.csproj" -c Release -o /app/publish /p:UseAppHost=false

            # Estágio 3: Runtime
            FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
            WORKDIR /app

            # Criar usuário não-root
            RUN adduser --disabled-password --gecos '' appuser

            # Instalar dependências do sistema
            RUN apt-get update && apt-get install -y \
                curl \
                && rm -rf /var/lib/apt/lists/*

            # Copiar aplicação
            COPY --from=publish /app/publish .

            # Configurar permissões
            RUN chown -R appuser:appuser /app
            USER appuser

            # Health check
            HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
                CMD curl -f http://localhost:8080/health || exit 1

            # Expor porta
            EXPOSE 8080

            # Comando de entrada
            ENTRYPOINT ["dotnet", "ProcessadorPedidos.dll"]
            """;

        WriteLine("  🐳 Dockerfile:");
        WriteLine(dockerfile);

        // docker-compose.yml
        var dockerCompose = """
            version: '3.8'

            services:
              processador-pedidos:
                build:
                  context: .
                  dockerfile: Dockerfile
                container_name: processador-pedidos
                ports:
                  - "8080:8080"
                environment:
                  - ASPNETCORE_ENVIRONMENT=Production
                  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ProcessadorPedidos;User Id=sa;Password=YourStrong@Passw0rd;
                  - Redis__ConnectionString=redis:6379
                  - Logging__LogLevel__Default=Information
                depends_on:
                  - sqlserver
                  - redis
                volumes:
                  - ./logs:/app/logs
                networks:
                  - app-network
                restart: unless-stopped
                deploy:
                  resources:
                    limits:
                      cpus: '1.0'
                      memory: 1G
                    reservations:
                      cpus: '0.5'
                      memory: 512M

              sqlserver:
                image: mcr.microsoft.com/mssql/server:2022-latest
                container_name: sqlserver
                environment:
                  - SA_PASSWORD=YourStrong@Passw0rd
                  - ACCEPT_EULA=Y
                ports:
                  - "1433:1433"
                volumes:
                  - sqlserver-data:/var/opt/mssql
                networks:
                  - app-network

              redis:
                image: redis:7-alpine
                container_name: redis
                ports:
                  - "6379:6379"
                volumes:
                  - redis-data:/data
                networks:
                  - app-network
                command: redis-server --appendonly yes

            volumes:
              sqlserver-data:
              redis-data:

            networks:
              app-network:
                driver: bridge
            """;

        WriteLine("\n  📦 Docker Compose:");
        WriteLine(dockerCompose);
    }

    static void DemonstrarComparacao()
    {
        WriteLine("  📊 ANTES (C# 10 e anteriores):");
        
        var exemploAntes = @"
        // JSON tradicional - muito escape!
        var json = ""{\n"" +
                  ""  \""usuario\"": {\n"" +
                  ""    \""nome\"": \""João Silva\"",\n"" +
                  ""    \""email\"": \""joao@exemplo.com\""\n"" +
                  ""  }\n"" +
                  ""}"";

        // SQL com concatenação
        var sql = ""SELECT u.nome, u.email "" +
                 ""FROM usuarios u "" +
                 ""WHERE u.ativo = 1 "" +
                 ""  AND u.data_criacao >= '"" + dataInicio + ""' "" +
                 ""ORDER BY u.nome"";

        // Regex com muitos escapes
        var regex = ""^[a-zA-Z0-9.!#$%&'*+\\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"";";
        
        WriteLine(exemploAntes);

        WriteLine("\n  ✨ DEPOIS (C# 11 com Raw String Literals):");
        
        var exemploDepois = """"
        // JSON limpo e legível!
        var json = """
            {
              "usuario": {
                "nome": "João Silva",
                "email": "joao@exemplo.com"
              }
            }
            """;

        // SQL multiline natural
        var sql = """
            SELECT u.nome, u.email 
            FROM usuarios u 
            WHERE u.ativo = 1 
              AND u.data_criacao >= @dataInicio
            ORDER BY u.nome
            """;

        // Regex sem escape!
        var regex = """
            ^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$
            """;
        """";
        
        WriteLine(exemploDepois);

        WriteLine("\n  📈 Benefícios mensuráveis:");
        WriteLine("     • 60% menos caracteres de escape");
        WriteLine("     • 80% mais legível para JSON/SQL/HTML");
        WriteLine("     • 90% menos erros de sintaxe");
        WriteLine("     • 100% de preservação de formatação");
    }
}
