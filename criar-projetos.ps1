# Script para criar todos os projetos das 100 dicas de C#

$baseDir = "f:\Projetos\Serviço de Pagamento\dicas-csharp"
$dicasDir = "$baseDir\Dicas"

# Array com informações das dicas
$dicas = @(
    @{Numero=1; Nome="RetornandoColecoesVazias"; Titulo="Retornando Coleções Vazias"},
    @{Numero=2; Nome="RelancandoExcecoesCorretamente"; Titulo="Relançando Exceções Corretamente"},
    @{Numero=3; Nome="TravamentoComAsyncAwait"; Titulo="Travamento (Locking) com Async/Await"},
    @{Numero=4; Nome="ArmadilhasDesempenhoLINQ"; Titulo="Armadilhas de Desempenho do LINQ"},
    @{Numero=5; Nome="CSharpREPL"; Titulo="C# REPL (Crebel)"},
    @{Numero=6; Nome="AcessandoSpanDeLista"; Titulo="Acessando Span de uma Lista (List)"},
    @{Numero=7; Nome="LoggingCorretoNET"; Titulo="Logging Correto no .NET"},
    @{Numero=8; Nome="TiposVaziosCSharp12"; Titulo="Tipos Vazios (Empty Types) no C# 12"},
    @{Numero=9; Nome="ToListVsToArray"; Titulo="ToList() vs ToArray()"},
    @{Numero=10; Nome="MarcadoresAssemblyDI"; Titulo="Marcadores de Assembly para Injeção de Dependência"}
)

foreach ($dica in $dicas) {
    $dicaNum = $dica.Numero.ToString("D2")
    $projectName = "Dica$dicaNum"
    $folderName = "Dica$dicaNum-$($dica.Nome)"
    $projectPath = "$dicasDir\$folderName"
    
    Write-Host "Criando projeto para Dica $dicaNum - $($dica.Titulo)"
    
    # Criar pasta
    New-Item -ItemType Directory -Force -Path $projectPath | Out-Null
    
    # Criar projeto console
    Push-Location $projectPath
    dotnet new console -n $projectName --force
    Pop-Location
    
    # Adicionar à solution
    Push-Location $baseDir
    dotnet sln add "Dicas\$folderName\$projectName\$projectName.csproj"
    Pop-Location
}

Write-Host "Projetos criados com sucesso!"
