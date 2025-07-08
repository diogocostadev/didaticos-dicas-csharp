# Script para criar TODOS os 100 projetos das dicas de C#
# Este script cria projetos com top-level statements e adiciona à solution principal

$ErrorActionPreference = "Continue"
$baseDir = "f:\Projetos\Serviço de Pagamento\dicas-csharp"
$dicasDir = "$baseDir\Dicas"

# Array com todas as 100 dicas
$allDicas = @(
    @{Numero=1; Nome="RetornandoColecoesVazias"; Titulo="Retornando Coleções Vazias"; TemBenchmark=$true},
    @{Numero=2; Nome="RelancandoExcecoesCorretamente"; Titulo="Relançando Exceções Corretamente"; TemBenchmark=$false},
    @{Numero=3; Nome="TravamentoComAsyncAwait"; Titulo="Travamento com Async/Await"; TemBenchmark=$false},
    @{Numero=4; Nome="ArmadilhasDesempenhoLINQ"; Titulo="Armadilhas de Desempenho do LINQ"; TemBenchmark=$true},
    @{Numero=5; Nome="CSharpREPL"; Titulo="C# REPL (Crebel)"; TemBenchmark=$false},
    @{Numero=6; Nome="AcessandoSpanDeLista"; Titulo="Acessando Span de uma Lista"; TemBenchmark=$true},
    @{Numero=7; Nome="LoggingCorretoNET"; Titulo="Logging Correto no .NET"; TemBenchmark=$false},
    @{Numero=8; Nome="TiposVaziosCSharp12"; Titulo="Tipos Vazios no C# 12"; TemBenchmark=$false},
    @{Numero=9; Nome="ToListVsToArray"; Titulo="ToList() vs ToArray()"; TemBenchmark=$true},
    @{Numero=10; Nome="MarcadoresAssemblyDI"; Titulo="Marcadores de Assembly para DI"; TemBenchmark=$false},
    @{Numero=11; Nome="AtributoStringSyntax"; Titulo="Atributo StringSyntax"; TemBenchmark=$false},
    @{Numero=12; Nome="ConstrutoresPrimariosReadonly"; Titulo="Construtores Primários e Readonly"; TemBenchmark=$false},
    @{Numero=13; Nome="UUIDv7NET9"; Titulo="UUID v7 no .NET 9"; TemBenchmark=$false},
    @{Numero=14; Nome="MenorProgramaCSharp"; Titulo="O Menor Programa C# Válido"; TemBenchmark=$false},
    @{Numero=15; Nome="CancellationTokensASPNET"; Titulo="Cancellation Tokens em ASP.NET Core"; TemBenchmark=$false},
    @{Numero=16; Nome="InicializadoresColecoesCS12"; Titulo="Inicializadores de Coleção C# 12"; TemBenchmark=$false},
    @{Numero=17; Nome="PacotesNuGetDesatualizados"; Titulo="Verificando Pacotes NuGet Desatualizados"; TemBenchmark=$false},
    @{Numero=18; Nome="GeracaoTextoWaffle"; Titulo="Geração de Texto Waffle"; TemBenchmark=$false},
    @{Numero=19; Nome="MetodosWebApplication"; Titulo="Métodos WebApplication"; TemBenchmark=$false},
    @{Numero=20; Nome="ValidandoNaughtyStrings"; Titulo="Validando Naughty Strings"; TemBenchmark=$false},
    @{Numero=21; Nome="InterpolatedParser"; Titulo="Interpolated Parser"; TemBenchmark=$false},
    @{Numero=22; Nome="AliasQualquerTipo"; Titulo="Alias para Qualquer Tipo"; TemBenchmark=$false},
    @{Numero=23; Nome="DateTimeOffsetVsDateTime"; Titulo="DateTimeOffset vs DateTime"; TemBenchmark=$false},
    @{Numero=24; Nome="TestesArquitetura"; Titulo="Testes de Arquitetura"; TemBenchmark=$false},
    @{Numero=25; Nome="AlternativasFluentAssertions"; Titulo="Alternativas ao Fluent Assertions"; TemBenchmark=$false},
    @{Numero=26; Nome="ExportandoJsonSchemaNET9"; Titulo="Exportando Json Schema .NET 9"; TemBenchmark=$false},
    @{Numero=27; Nome="ParalelismoAssincrono"; Titulo="Paralelismo Assíncrono"; TemBenchmark=$true},
    @{Numero=28; Nome="RetestTestes"; Titulo="Retestando Testes Falhos"; TemBenchmark=$false},
    @{Numero=29; Nome="ParamsEnumerableCS13"; Titulo="Params com Tipos Enumerable C# 13"; TemBenchmark=$true},
    @{Numero=30; Nome="MonitorMaligno"; Titulo="O Monitor Maligno (Piada)"; TemBenchmark=$false},
    @{Numero=31; Nome="ConvencaoUnderscoreCampos"; Titulo="Convenção Underscore para Campos"; TemBenchmark=$false},
    @{Numero=32; Nome="UsandoHttpClientCorreto"; Titulo="Usando HttpClient Corretamente"; TemBenchmark=$false},
    @{Numero=33; Nome="TestesSnapshotVerify"; Titulo="Testes de Snapshot com Verify"; TemBenchmark=$false},
    @{Numero=34; Nome="ChamandoAPIsRefit"; Titulo="Chamando APIs com Refit"; TemBenchmark=$false},
    @{Numero=35; Nome="TravamentoSemaphoreSlim"; Titulo="Travamento com SemaphoreSlim"; TemBenchmark=$false},
    @{Numero=36; Nome="ULIDsIdentificadores"; Titulo="ULIDs Sortable Unique Identifiers"; TemBenchmark=$true},
    @{Numero=37; Nome="OperacoesAssincronasParalelo"; Titulo="Operações Assíncronas em Paralelo"; TemBenchmark=$true},
    @{Numero=38; Nome="ScopedLifetimesPersonalizados"; Titulo="Scoped Lifetimes Personalizados"; TemBenchmark=$false},
    @{Numero=39; Nome="ConstrutoresPrimariosReadonly2"; Titulo="Construtores Primários e Readonly (Repetição)"; TemBenchmark=$false},
    @{Numero=40; Nome="UnidadesMedidaUnitsNet"; Titulo="Unidades de Medida UnitsNet"; TemBenchmark=$false},
    @{Numero=41; Nome="ValidandoContainerDI"; Titulo="Validando Container DI"; TemBenchmark=$false},
    @{Numero=42; Nome="NullConditionalAssignment"; Titulo="Null Conditional Assignment C# 14"; TemBenchmark=$false},
    @{Numero=43; Nome="InicializadoresDicionario"; Titulo="Inicializadores de Dicionário"; TemBenchmark=$false},
    @{Numero=44; Nome="ConstrutoresPrimariosClassesRecords"; Titulo="Construtores Primários Classes vs Records"; TemBenchmark=$false},
    @{Numero=45; Nome="RefStructs"; Titulo="Ref Structs Alto Desempenho"; TemBenchmark=$true},
    @{Numero=46; Nome="PalavraChaveIn"; Titulo="Palavra-chave 'in'"; TemBenchmark=$true},
    @{Numero=47; Nome="ExecucaoAdiadaLINQ"; Titulo="Execução Adiada do LINQ"; TemBenchmark=$false},
    @{Numero=48; Nome="Stackalloc"; Titulo="Stackalloc Alocação na Stack"; TemBenchmark=$true},
    @{Numero=49; Nome="TiposDelegateIntegrados"; Titulo="Tipos de Delegate Integrados"; TemBenchmark=$false},
    @{Numero=50; Nome="SobrescritaComportamentoClasseBase"; Titulo="Sobrescrita de Comportamento da Classe Base"; TemBenchmark=$false},
    @{Numero=51; Nome="ReutilizacaoArraysArrayPool"; Titulo="Reutilização de Arrays com ArrayPool"; TemBenchmark=$true},
    @{Numero=52; Nome="EvitandoAsyncVoid"; Titulo="Evitando Async Void"; TemBenchmark=$false},
    @{Numero=53; Nome="OperadorNullForgiving"; Titulo="Operador Null Forgiving"; TemBenchmark=$false},
    @{Numero=54; Nome="PalavraChaveUsing"; Titulo="Palavra-chave using"; TemBenchmark=$false},
    @{Numero=55; Nome="PalavraChaveWith"; Titulo="Palavra-chave with"; TemBenchmark=$false},
    @{Numero=56; Nome="MembrosExtensao"; Titulo="Membros de Extensão C# 14"; TemBenchmark=$false},
    @{Numero=57; Nome="ExpressoesColecoesCS12"; Titulo="Expressões de Coleção C# 12"; TemBenchmark=$false},
    @{Numero=58; Nome="SuporteSpanParams"; Titulo="Suporte a Span para Params C# 13"; TemBenchmark=$true},
    @{Numero=59; Nome="TargetTypedNew"; Titulo="Target-Typed New C# 9"; TemBenchmark=$false},
    @{Numero=60; Nome="TopLevelStatements"; Titulo="Top-Level Statements C# 9"; TemBenchmark=$false},
    @{Numero=61; Nome="PadroesNotAndOr"; Titulo="Padrões not, and, or"; TemBenchmark=$false},
    @{Numero=62; Nome="UsandoNameof"; Titulo="Usando nameof"; TemBenchmark=$false},
    @{Numero=63; Nome="MetodosDeconstruct"; Titulo="Métodos Deconstruct"; TemBenchmark=$false},
    @{Numero=64; Nome="AtributosExpressoesLambda"; Titulo="Atributos em Expressões Lambda"; TemBenchmark=$false},
    @{Numero=65; Nome="PatternMatchingRanges"; Titulo="Pattern Matching com Ranges"; TemBenchmark=$false},
    @{Numero=66; Nome="ArgumentNullExceptionThrowIfNull"; Titulo="ArgumentNullException.ThrowIfNull"; TemBenchmark=$false},
    @{Numero=67; Nome="ConstrutoresCorpoExpressao"; Titulo="Construtores com Corpo de Expressão"; TemBenchmark=$false},
    @{Numero=68; Nome="ValueTuplesVsTuple"; Titulo="Value Tuples vs Tuple"; TemBenchmark=$true},
    @{Numero=69; Nome="PalavrasChavesCuriosas"; Titulo="Palavras-chave Curiosas"; TemBenchmark=$false},
    @{Numero=70; Nome="PassandoRefParaIn"; Titulo="Passando ref para in"; TemBenchmark=$false},
    @{Numero=71; Nome="ComparandoTuplas"; Titulo="Comparando Tuplas"; TemBenchmark=$false},
    @{Numero=72; Nome="AtributosCaller"; Titulo="Atributos CallerMemberName"; TemBenchmark=$false},
    @{Numero=73; Nome="AtributosSemanticos"; Titulo="Atributos Semânticos"; TemBenchmark=$false},
    @{Numero=74; Nome="UsoVar"; Titulo="O Uso de var"; TemBenchmark=$false},
    @{Numero=75; Nome="EvitandoDynamic"; Titulo="Evitando a palavra-chave dynamic"; TemBenchmark=$true},
    @{Numero=76; Nome="ExcecoesCasosExcepcionais"; Titulo="Exceções para Casos Excepcionais"; TemBenchmark=$false},
    @{Numero=77; Nome="TestesUnitariosAssincronos"; Titulo="Testes Unitários Assíncronos"; TemBenchmark=$false},
    @{Numero=78; Nome="EvitandoRegion"; Titulo="Evitando region"; TemBenchmark=$false},
    @{Numero=79; Nome="AtributoObsolete"; Titulo="Usando Atributo Obsolete"; TemBenchmark=$false},
    @{Numero=80; Nome="RodandoArquivoCSharpConsole"; Titulo="Rodando Arquivo C# do Console"; TemBenchmark=$false},
    @{Numero=81; Nome="GlobalUsings"; Titulo="Global Usings C# 10"; TemBenchmark=$false},
    @{Numero=82; Nome="NameofNaoReflexao"; Titulo="nameof Não é Reflexão"; TemBenchmark=$true},
    @{Numero=83; Nome="InternalsVisibleTo"; Titulo="InternalsVisibleTo"; TemBenchmark=$false},
    @{Numero=84; Nome="FiltrosWhenCatch"; Titulo="Filtros when em Blocos catch"; TemBenchmark=$false},
    @{Numero=85; Nome="RestricaoTipoGenericoNotnull"; Titulo="Restrição de Tipo Genérico notnull"; TemBenchmark=$false},
    @{Numero=86; Nome="EscopoValoresScoped"; Titulo="Escopo de Valores com scoped"; TemBenchmark=$false},
    @{Numero=87; Nome="DestrutoresIDisposable"; Titulo="Destrutores e IDisposable"; TemBenchmark=$false},
    @{Numero=88; Nome="PropertyPatterns"; Titulo="Property Patterns"; TemBenchmark=$false},
    @{Numero=89; Nome="ListPatterns"; Titulo="List Patterns C# 11"; TemBenchmark=$false},
    @{Numero=90; Nome="SealedOverride"; Titulo="Sealed Override"; TemBenchmark=$false},
    @{Numero=91; Nome="MetodosTryParse"; Titulo="Métodos TryParse"; TemBenchmark=$false},
    @{Numero=92; Nome="ExperimentandoRecursosFuturos"; Titulo="Experimentando Recursos C# Futuros"; TemBenchmark=$false},
    @{Numero=93; Nome="IndicesPartirFim"; Titulo="Índices a Partir do Fim"; TemBenchmark=$false},
    @{Numero=94; Nome="LiteraisArrayVazios"; Titulo="Literais de Array Vazios C# 12"; TemBenchmark=$false},
    @{Numero=95; Nome="AwaitForeach"; Titulo="await foreach"; TemBenchmark=$false},
    @{Numero=96; Nome="PalavraChaveChecked"; Titulo="Palavra-chave checked"; TemBenchmark=$false},
    @{Numero=97; Nome="IgualdadeObjetosAnonimos"; Titulo="Igualdade de Objetos Anônimos"; TemBenchmark=$false},
    @{Numero=98; Nome="TaskYield"; Titulo="Task.Yield()"; TemBenchmark=$false},
    @{Numero=99; Nome="InliningMetodos"; Titulo="Inlining de Métodos"; TemBenchmark=$true},
    @{Numero=100; Nome="ExpressoesRegularesCompiladasTempo"; Titulo="Expressões Regulares Compiladas"; TemBenchmark=$true}
)

# Função para criar um projeto
function New-DicaProject {
    param(
        [int]$Numero,
        [string]$Nome,
        [string]$Titulo,
        [bool]$TemBenchmark = $false
    )
    
    $dicaNum = $Numero.ToString("D2")
    $projectName = "Dica$dicaNum"
    $folderName = "Dica$dicaNum-$Nome"
    $projectPath = "$dicasDir\$folderName"
    
    Write-Host "Criando Dica $dicaNum - $Titulo" -ForegroundColor Green
    
    # Criar pasta se não existir
    if (-not (Test-Path $projectPath)) {
        New-Item -ItemType Directory -Force -Path $projectPath | Out-Null
    }
    
    # Criar projeto console principal
    Push-Location $projectPath
    try {
        if (-not (Test-Path "$projectName\$projectName.csproj")) {
            dotnet new console -n $projectName --force | Out-Null
            Write-Host "  ✓ Projeto principal criado" -ForegroundColor Gray
        }
    }
    catch {
        Write-Warning "Erro ao criar projeto $projectName"
    }
    finally {
        Pop-Location
    }
    
    # Criar projeto de benchmark se necessário
    if ($TemBenchmark) {
        Push-Location $projectPath
        try {
            $benchmarkName = "$projectName.Benchmark"
            if (-not (Test-Path "$benchmarkName\$benchmarkName.csproj")) {
                dotnet new console -n $benchmarkName --force | Out-Null
                dotnet add "$benchmarkName\$benchmarkName.csproj" package BenchmarkDotNet | Out-Null
                Write-Host "  ✓ Projeto benchmark criado" -ForegroundColor Gray
            }
        }
        catch {
            Write-Warning "Erro ao criar benchmark para $projectName"
        }
        finally {
            Pop-Location
        }
    }
    
    # Adicionar à solution
    Push-Location $baseDir
    try {
        $projectFile = "Dicas\$folderName\$projectName\$projectName.csproj"
        if (Test-Path $projectFile) {
            dotnet sln add $projectFile 2>$null | Out-Null
        }
        
        if ($TemBenchmark) {
            $benchmarkFile = "Dicas\$folderName\$projectName.Benchmark\$projectName.Benchmark.csproj"
            if (Test-Path $benchmarkFile) {
                dotnet sln add $benchmarkFile 2>$null | Out-Null
            }
        }
    }
    catch {
        Write-Warning "Erro ao adicionar projetos à solution"
    }
    finally {
        Pop-Location
    }
}

# Perguntar quantos projetos criar
$input = Read-Host "Quantas dicas criar? (1-100, padrão=10)"
$quantidadeDicas = 10
if ($input -match '^\d+$' -and [int]$input -ge 1 -and [int]$input -le 100) {
    $quantidadeDicas = [int]$input
}

Write-Host "`nCriando $quantidadeDicas projetos das dicas de C#..." -ForegroundColor Yellow
Write-Host "Projetos com benchmark serão marcados com [BENCHMARK]" -ForegroundColor Cyan
Write-Host ""

for ($i = 0; $i -lt $quantidadeDicas -and $i -lt $allDicas.Count; $i++) {
    $dica = $allDicas[$i]
    if ($dica.TemBenchmark) {
        Write-Host "  [BENCHMARK] " -NoNewline -ForegroundColor Cyan
    }
    New-DicaProject -Numero $dica.Numero -Nome $dica.Nome -Titulo $dica.Titulo -TemBenchmark $dica.TemBenchmark
}

Write-Host "`n✅ $quantidadeDicas projetos criados com sucesso!" -ForegroundColor Green
Write-Host "`n📝 Próximos passos:" -ForegroundColor Yellow
Write-Host "   1. Implemente o código das dicas nos arquivos Program.cs" -ForegroundColor White
Write-Host "   2. Para dicas com benchmark, execute: dotnet run -c Release --project [projeto].Benchmark" -ForegroundColor White
Write-Host "   3. Para rodar uma dica: dotnet run --project [projeto]" -ForegroundColor White
