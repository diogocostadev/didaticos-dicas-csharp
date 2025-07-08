// =================== PROCESSADOR DE TEXTO (FILE-SCOPED NAMESPACE) ===================
namespace Dica17.GlobalUsings;

// Esta classe demonstra como usar file-scoped namespace
// Note que não há chaves após o namespace - toda a declaração fica no mesmo nível

public class ProcessadorTexto
{
    public string ProcessarComContadores(string texto)
    {
        var palavras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var caracteres = texto.Length;
        var vogais = texto.Count(c => "aeiouAEIOU".Contains(c));
        
        return $"'{texto}' → {palavras.Length} palavras, {caracteres} caracteres, {vogais} vogais";
    }

    public StringDict AnalisarFrequencia(string texto)
    {
        return texto.ToLowerInvariant()
            .Where(char.IsLetter)
            .GroupBy(c => c)
            .ToDictionary(g => g.Key.ToString(), g => g.Count().ToString());
    }

    public async Task<string> ProcessarAssincrono(string texto)
    {
        await Task.Delay(50); // Simula processamento
        return texto.ToUpperInvariant().Replace(" ", "_");
    }
}

// Demonstração de record com file-scoped namespace
public record Pessoa(string Nome, int Idade, string Profissao)
{
    public bool EhAdulto => Idade >= 18;
    public string Categoria => Idade switch
    {
        < 13 => "Criança",
        < 18 => "Adolescente",
        < 60 => "Adulto",
        _ => "Idoso"
    };
}

// Enum também funciona com file-scoped namespace
public enum StatusProcessamento
{
    Pendente,
    EmAndamento,
    Concluido,
    Erro
}
