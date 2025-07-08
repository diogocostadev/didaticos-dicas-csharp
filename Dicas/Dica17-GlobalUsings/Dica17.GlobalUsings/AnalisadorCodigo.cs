// =================== ANALISADOR DE CÓDIGO (FILE-SCOPED NAMESPACE) ===================
namespace Dica17.GlobalUsings;

// Esta classe demonstra a diferença entre código tradicional e com Global Usings
public static class AnalisadorCodigo
{
    public static void CompararAntesDepois()
    {
        WriteLine("\n   📊 ANTES (C# 9 e anteriores):");
        ExibirCodigoTradicional();
        
        WriteLine("\n   ✨ DEPOIS (C# 10+ com Global Usings e File-Scoped Namespace):");
        ExibirCodigoModerno();
        
        WriteLine("\n   📈 Benefícios:");
        WriteLine("     • 15-20 linhas menos de boilerplate por arquivo");
        WriteLine("     • Redução de 25-30% na indentação");
        WriteLine("     • Foco na lógica, não na configuração");
        WriteLine("     • Consistency across entire project");
    }

    private static void ExibirCodigoTradicional()
    {
        WriteLine(@"
     using System;
     using System.Collections.Generic;
     using System.Linq;
     using System.Threading.Tasks;
     using System.IO;
     using System.Text.Json;
     using System.Collections.Concurrent;
     using System.Diagnostics;
     
     namespace MeuProjeto.Services
     {
         public class ProcessadorTexto
         {
             public string ProcessarComContadores(string texto)
             {
                 var palavras = texto.Split(' ');
                 var caracteres = texto.Length;
                 Console.WriteLine($""Processados {palavras.Length} palavras"");
                 return resultado;
             }
         }
     }");
    }

    private static void ExibirCodigoModerno()
    {
        WriteLine(@"
     // GlobalUsings.cs (uma vez no projeto)
     global using System;
     global using System.Collections.Generic;
     global using System.Linq;
     global using System.Threading.Tasks;
     global using static System.Console;
     
     // ProcessadorTexto.cs
     namespace MeuProjeto.Services;
     
     public class ProcessadorTexto
     {
         public string ProcessarComContadores(string texto)
         {
             var palavras = texto.Split(' ');
             var caracteres = texto.Length;
             WriteLine($""Processados {palavras.Length} palavras"");
             return resultado;
         }
     }");
    }

    public static void AnaliseEstatistica()
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Simula análise de projeto
        var arquivos = GerarEstatisticasProjeto();
        
        stopwatch.Stop();
        
        WriteLine($"\n   📋 Análise de Projeto Concluída em {stopwatch.ElapsedMilliseconds}ms:");
        WriteLine($"     • Total de arquivos: {arquivos.Count}");
        WriteLine($"     • Média de linhas por arquivo: {arquivos.Average():F1}");
        WriteLine($"     • Economia estimada: {arquivos.Sum() * 0.15:F0} linhas de boilerplate");
    }

    private static IntList GerarEstatisticasProjeto()
    {
        // Simula contagem de linhas em diferentes arquivos
        var random = new Random();
        return Enumerable.Range(1, 20)
            .Select(_ => random.Next(50, 300))
            .ToList();
    }

    public static async Task TestarPerformanceAsync()
    {
        WriteLine("\n   ⚡ Teste de Performance:");
        
        var tasks = new List<Task<TimeSpan>>();
        
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(MedirTempoOperacaoAsync($"Operação {i + 1}"));
        }
        
        var tempos = await Task.WhenAll(tasks);
        var tempoMedio = new TimeSpan((long)tempos.Average(t => t.Ticks));
        
        WriteLine($"     • Tempo médio por operação: {tempoMedio.TotalMilliseconds:F2}ms");
        WriteLine($"     • Operações mais rápida: {tempos.Min().TotalMilliseconds:F2}ms");
        WriteLine($"     • Operações mais lenta: {tempos.Max().TotalMilliseconds:F2}ms");
    }

    private static async Task<TimeSpan> MedirTempoOperacaoAsync(string nome)
    {
        var sw = Stopwatch.StartNew();
        
        // Simula operação complexa usando tipos dos global usings
        var dados = new ConcurrentDictionary<string, IntList>();
        
        await Task.Run(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                var lista = new IntList { i, i * 2, i * 3 };
                dados.TryAdd($"item_{i}", lista);
            }
        });
        
        sw.Stop();
        WriteLine($"     • {nome}: {sw.ElapsedMilliseconds}ms");
        
        return sw.Elapsed;
    }
}
