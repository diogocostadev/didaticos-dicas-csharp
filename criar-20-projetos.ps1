# Script para criar todos os 100 projetos das dicas de C#

$baseDir = "f:\Projetos\Serviço de Pagamento\dicas-csharp"
$dicasDir = "$baseDir\Dicas"

# Array com todas as 100 dicas
$allDicas = @(
    @{Numero=1; Nome="RetornandoColecoesVazias"; Titulo="Retornando Coleções Vazias"},
    @{Numero=2; Nome="RelancandoExcecoesCorretamente"; Titulo="Relançando Exceções Corretamente"},
    @{Numero=3; Nome="TravamentoComAsyncAwait"; Titulo="Travamento com Async/Await"},
    @{Numero=4; Nome="ArmadilhasDesempenhoLINQ"; Titulo="Armadilhas de Desempenho do LINQ"},
    @{Numero=5; Nome="CSharpREPL"; Titulo="C# REPL (Crebel)"},
    @{Numero=6; Nome="AcessandoSpanDeLista"; Titulo="Acessando Span de uma Lista"},
    @{Numero=7; Nome="LoggingCorretoNET"; Titulo="Logging Correto no .NET"},
    @{Numero=8; Nome="TiposVaziosCSharp12"; Titulo="Tipos Vazios no C# 12"},
    @{Numero=9; Nome="ToListVsToArray"; Titulo="ToList() vs ToArray()"},
    @{Numero=10; Nome="MarcadoresAssemblyDI"; Titulo="Marcadores de Assembly para DI"},
    @{Numero=11; Nome="AtributoStringSyntax"; Titulo="Atributo StringSyntax"},
    @{Numero=12; Nome="ConstrutoresPrimariosReadonly"; Titulo="Construtores Primários e Readonly"},
    @{Numero=13; Nome="UUIDv7NET9"; Titulo="UUID v7 no .NET 9"},
    @{Numero=14; Nome="MenorProgramaCSharp"; Titulo="O Menor Programa C# Válido"},
    @{Numero=15; Nome="CancellationTokensASPNET"; Titulo="Cancellation Tokens em ASP.NET Core"},
    @{Numero=16; Nome="InicializadoresColecoesCS12"; Titulo="Inicializadores de Coleção C# 12"},
    @{Numero=17; Nome="PacotesNuGetDesatualizados"; Titulo="Verificando Pacotes NuGet Desatualizados"},
    @{Numero=18; Nome="GeracaoTextoWaffle"; Titulo="Geração de Texto Waffle"},
    @{Numero=19; Nome="MetodosWebApplication"; Titulo="Métodos WebApplication"},
    @{Numero=20; Nome="ValidandoNaughtyStrings"; Titulo="Validando Naughty Strings"},
    @{Numero=21; Nome="InterpolatedParser"; Titulo="Interpolated Parser"},
    @{Numero=22; Nome="AliasQualquerTipo"; Titulo="Alias para Qualquer Tipo"},
    @{Numero=23; Nome="DateTimeOffsetVsDateTime"; Titulo="DateTimeOffset vs DateTime"},
    @{Numero=24; Nome="TestesArquitetura"; Titulo="Testes de Arquitetura"},
    @{Numero=25; Nome="AlternativasFluentAssertions"; Titulo="Alternativas ao Fluent Assertions"},
    @{Numero=26; Nome="ExportandoJsonSchemaNET9"; Titulo="Exportando Json Schema .NET 9"},
    @{Numero=27; Nome="ParalelismoAssincrono"; Titulo="Paralelismo Assíncrono"},
    @{Numero=28; Nome="RetestTestes"; Titulo="Retestando Testes Falhos"},
    @{Numero=29; Nome="ParamsEnumerableCS13"; Titulo="Params com Tipos Enumerable C# 13"},
    @{Numero=30; Nome="MonitorMaligno"; Titulo="O Monitor Maligno (Piada)"},
    @{Numero=31; Nome="ConvencaoUnderscoreCampos"; Titulo="Convenção Underscore para Campos"},
    @{Numero=32; Nome="UsandoHttpClientCorreto"; Titulo="Usando HttpClient Corretamente"},
    @{Numero=33; Nome="TestesSnapshotVerify"; Titulo="Testes de Snapshot com Verify"},
    @{Numero=34; Nome="ChamandoAPIsRefit"; Titulo="Chamando APIs com Refit"},
    @{Numero=35; Nome="TravamentoSemaphoreSlim"; Titulo="Travamento com SemaphoreSlim"},
    @{Numero=36; Nome="ULIDsIdentificadores"; Titulo="ULIDs Sortable Unique Identifiers"},
    @{Numero=37; Nome="OperacoesAssincronasParalelo"; Titulo="Operações Assíncronas em Paralelo"},
    @{Numero=38; Nome="ScopedLifetimesPersonalizados"; Titulo="Scoped Lifetimes Personalizados"},
    @{Numero=39; Nome="ConstrutoresPrimariosReadonly2"; Titulo="Construtores Primários e Readonly (Repetição)"},
    @{Numero=40; Nome="UnidadesMedidaUnitsNet"; Titulo="Unidades de Medida UnitsNet"},
    @{Numero=41; Nome="ValidandoContainerDI"; Titulo="Validando Container DI"},
    @{Numero=42; Nome="NullConditionalAssignment"; Titulo="Null Conditional Assignment C# 14"},
    @{Numero=43; Nome="InicializadoresDicionario"; Titulo="Inicializadores de Dicionário"},
    @{Numero=44; Nome="ConstrutoresPrimariosClassesRecords"; Titulo="Construtores Primários Classes vs Records"},
    @{Numero=45; Nome="RefStructs"; Titulo="Ref Structs Alto Desempenho"},
    @{Numero=46; Nome="PalavraChaveIn"; Titulo="Palavra-chave 'in'"},
    @{Numero=47; Nome="ExecucaoAdiadaLINQ"; Titulo="Execução Adiada do LINQ"},
    @{Numero=48; Nome="Stackalloc"; Titulo="Stackalloc Alocação na Stack"},
    @{Numero=49; Nome="TiposDelegateIntegrados"; Titulo="Tipos de Delegate Integrados"},
    @{Numero=50; Nome="SobrescritaComportamentoClasseBase"; Titulo="Sobrescrita de Comportamento da Classe Base"},
    @{Numero=51; Nome="ReutilizacaoArraysArrayPool"; Titulo="Reutilização de Arrays com ArrayPool"},
    @{Numero=52; Nome="EvitandoAsyncVoid"; Titulo="Evitando Async Void"},
    @{Numero=53; Nome="OperadorNullForgiving"; Titulo="Operador Null Forgiving"},
    @{Numero=54; Nome="PalavraChaveUsing"; Titulo="Palavra-chave using"},
    @{Numero=55; Nome="PalavraChaveWith"; Titulo="Palavra-chave with"},
    @{Numero=56; Nome="MembrosExtensao"; Titulo="Membros de Extensão C# 14"},
    @{Numero=57; Nome="ExpressoesColecoesCS12"; Titulo="Expressões de Coleção C# 12"},
    @{Numero=58; Nome="SuporteSpanParams"; Titulo="Suporte a Span para Params C# 13"},
    @{Numero=59; Nome="TargetTypedNew"; Titulo="Target-Typed New C# 9"},
    @{Numero=60; Nome="TopLevelStatements"; Titulo="Top-Level Statements C# 9"},
    @{Numero=61; Nome="PadroesNotAndOr"; Titulo="Padrões not, and, or"},
    @{Numero=62; Nome="UsandoNameof"; Titulo="Usando nameof"},
    @{Numero=63; Nome="MetodosDeconstruct"; Titulo="Métodos Deconstruct"},
    @{Numero=64; Nome="AtributosExpressoesLambda"; Titulo="Atributos em Expressões Lambda"},
    @{Numero=65; Nome="PatternMatchingRanges"; Titulo="Pattern Matching com Ranges"},
    @{Numero=66; Nome="ArgumentNullExceptionThrowIfNull"; Titulo="ArgumentNullException.ThrowIfNull"},
    @{Numero=67; Nome="ConstrutoresCorpoExpressao"; Titulo="Construtores com Corpo de Expressão"},
    @{Numero=68; Nome="ValueTuplesVsTuple"; Titulo="Value Tuples vs Tuple"},
    @{Numero=69; Nome="PalavrasChavesCuriosas"; Titulo="Palavras-chave Curiosas"},
    @{Numero=70; Nome="PassandoRefParaIn"; Titulo="Passando ref para in"},
    @{Numero=71; Nome="ComparandoTuplas"; Titulo="Comparando Tuplas"},
    @{Numero=72; Nome="AtributosCaller"; Titulo="Atributos CallerMemberName"},
    @{Numero=73; Nome="AtributosSemanticos"; Titulo="Atributos Semânticos"},
    @{Numero=74; Nome="UsoVar"; Titulo="O Uso de var"},
    @{Numero=75; Nome="EvitandoDynamic"; Titulo="Evitando a palavra-chave dynamic"},
    @{Numero=76; Nome="ExcecoesCasosExcepcionais"; Titulo="Exceções para Casos Excepcionais"},
    @{Numero=77; Nome="TestesUnitariosAssincronos"; Titulo="Testes Unitários Assíncronos"},
    @{Numero=78; Nome="EvitandoRegion"; Titulo="Evitando region"},
    @{Numero=79; Nome="AtributoObsolete"; Titulo="Usando Atributo Obsolete"},
    @{Numero=80; Nome="RodandoArquivoCSharpConsole"; Titulo="Rodando Arquivo C# do Console"},
    @{Numero=81; Nome="GlobalUsings"; Titulo="Global Usings C# 10"},
    @{Numero=82; Nome="NameofNaoReflexao"; Titulo="nameof Não é Reflexão"},
    @{Numero=83; Nome="InternalsVisibleTo"; Titulo="InternalsVisibleTo"},
    @{Numero=84; Nome="FiltrosWhenCatch"; Titulo="Filtros when em Blocos catch"},
    @{Numero=85; Nome="RestricaoTipoGenericoNotnull"; Titulo="Restrição de Tipo Genérico notnull"},
    @{Numero=86; Nome="EscopoValoresScoped"; Titulo="Escopo de Valores com scoped"},
    @{Numero=87; Nome="DestrutoresIDisposable"; Titulo="Destrutores e IDisposable"},
    @{Numero=88; Nome="PropertyPatterns"; Titulo="Property Patterns"},
    @{Numero=89; Nome="ListPatterns"; Titulo="List Patterns C# 11"},
    @{Numero=90; Nome="SealedOverride"; Titulo="Sealed Override"},
    @{Numero=91; Nome="MetodosTryParse"; Titulo="Métodos TryParse"},
    @{Numero=92; Nome="ExperimentandoRecursosFuturos"; Titulo="Experimentando Recursos C# Futuros"},
    @{Numero=93; Nome="IndicesPartirFim"; Titulo="Índices a Partir do Fim"},
    @{Numero=94; Nome="LiteraisArrayVazios"; Titulo="Literais de Array Vazios C# 12"},
    @{Numero=95; Nome="AwaitForeach"; Titulo="await foreach"},
    @{Numero=96; Nome="PalavraChaveChecked"; Titulo="Palavra-chave checked"},
    @{Numero=97; Nome="IgualdadeObjetosAnonimos"; Titulo="Igualdade de Objetos Anônimos"},
    @{Numero=98; Nome="TaskYield"; Titulo="Task.Yield()"},
    @{Numero=99; Nome="InliningMetodos"; Titulo="Inlining de Métodos"},
    @{Numero=100; Nome="ExpressoesRegularesCompiladasTempo"; Titulo="Expressões Regulares Compiladas"}
)

# Função para criar um projeto
function New-DicaProject {
    param(
        [int]$Numero,
        [string]$Nome,
        [string]$Titulo
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
    
    # Criar projeto console
    Push-Location $projectPath
    try {
        if (-not (Test-Path "$projectName\$projectName.csproj")) {
            dotnet new console -n $projectName --force | Out-Null
        }
    }
    catch {
        Write-Warning "Erro ao criar projeto $projectName"
    }
    finally {
        Pop-Location
    }
    
    # Adicionar à solution
    Push-Location $baseDir
    try {
        $projectFile = "Dicas\$folderName\$projectName\$projectName.csproj"
        if (Test-Path $projectFile) {
            dotnet sln add $projectFile 2>$null | Out-Null
        }
    }
    catch {
        Write-Warning "Erro ao adicionar $projectName à solution"
    }
    finally {
        Pop-Location
    }
}

# Criar primeiros 20 projetos
Write-Host "Criando primeiros 20 projetos das dicas de C#..." -ForegroundColor Yellow

for ($i = 0; $i -lt 20 -and $i -lt $allDicas.Count; $i++) {
    $dica = $allDicas[$i]
    New-DicaProject -Numero $dica.Numero -Nome $dica.Nome -Titulo $dica.Titulo
}

Write-Host "`nPrimeiros 20 projetos criados com sucesso!" -ForegroundColor Green
Write-Host "Para criar todos os 100 projetos, execute: .\criar-todos-projetos.ps1" -ForegroundColor Cyan
