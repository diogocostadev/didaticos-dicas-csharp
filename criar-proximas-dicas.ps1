# Script para criar mais projetos das dicas de C#

$baseDir = "f:\Projetos\Serviço de Pagamento\dicas-csharp"
$dicasDir = "$baseDir\Dicas"

# Dicas 3-10
$proximasDicas = @(
    @{Numero=3; Nome="TravamentoComAsyncAwait"; Titulo="Travamento com Async/Await"},
    @{Numero=4; Nome="ArmadilhasDesempenhoLINQ"; Titulo="Armadilhas de Desempenho do LINQ"; Benchmark=$true},
    @{Numero=5; Nome="CSharpREPL"; Titulo="C# REPL (Crebel)"},
    @{Numero=6; Nome="AcessandoSpanDeLista"; Titulo="Acessando Span de uma Lista"; Benchmark=$true},
    @{Numero=7; Nome="LoggingCorretoNET"; Titulo="Logging Correto no .NET"},
    @{Numero=8; Nome="TiposVaziosCSharp12"; Titulo="Tipos Vazios no C# 12"},
    @{Numero=9; Nome="ToListVsToArray"; Titulo="ToList() vs ToArray()"; Benchmark=$true},
    @{Numero=10; Nome="MarcadoresAssemblyDI"; Titulo="Marcadores de Assembly para DI"}
)

Write-Host "Criando próximas 8 dicas (3-10)..." -ForegroundColor Green

foreach ($dica in $proximasDicas) {
    $dicaNum = $dica.Numero.ToString("D2")
    $projectName = "Dica$dicaNum"
    $folderName = "Dica$dicaNum-$($dica.Nome)"
    $projectPath = "$dicasDir\$folderName"
    
    Write-Host "Criando Dica $dicaNum - $($dica.Titulo)" -ForegroundColor Yellow
    
    # Criar pasta
    if (-not (Test-Path $projectPath)) {
        New-Item -ItemType Directory -Force -Path $projectPath | Out-Null
    }
    
    # Criar projeto principal
    if (-not (Test-Path "$projectPath\$projectName\$projectName.csproj")) {
        Push-Location $projectPath
        dotnet new console -n $projectName --force | Out-Null
        Pop-Location
        Write-Host "  ✓ Projeto criado" -ForegroundColor Gray
    }
    
    # Criar benchmark se necessário
    if ($dica.Benchmark) {
        $benchmarkName = "$projectName.Benchmark"
        if (-not (Test-Path "$projectPath\$benchmarkName\$benchmarkName.csproj")) {
            Push-Location $projectPath
            dotnet new console -n $benchmarkName --force | Out-Null
            dotnet add "$benchmarkName\$benchmarkName.csproj" package BenchmarkDotNet | Out-Null
            Pop-Location
            Write-Host "  ✓ Benchmark criado" -ForegroundColor Gray
        }
    }
    
    # Adicionar à solution
    Push-Location $baseDir
    $projectFile = "Dicas\$folderName\$projectName\$projectName.csproj"
    if (Test-Path $projectFile) {
        dotnet sln add $projectFile | Out-Null
    }
    
    if ($dica.Benchmark) {
        $benchmarkFile = "Dicas\$folderName\$projectName.Benchmark\$projectName.Benchmark.csproj"
        if (Test-Path $benchmarkFile) {
            dotnet sln add $benchmarkFile | Out-Null
        }
    }
    Pop-Location
}

Write-Host "`nProjetos criados com sucesso!" -ForegroundColor Green
