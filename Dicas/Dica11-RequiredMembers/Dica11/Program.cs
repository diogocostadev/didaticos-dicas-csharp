namespace Dica11;

/// <summary>
/// Demonstra o uso de Required Members introduzido no C# 11.
/// 
/// Required Members garantem que certas propriedades sejam inicializadas
/// durante a construção do objeto, oferecendo uma alternativa mais flexível
/// aos construtores obrigatórios.
/// 
/// Principais características:
/// 1. Palavra-chave 'required' para propriedades obrigatórias
/// 2. Erro de compilação se não inicializadas
/// 3. Compatível com init-only properties
/// 4. Funciona com herança
/// 5. Atributo [SetsRequiredMembers] para construtores
/// 6. Integração com serialização JSON
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Services.AddSingleton<IRequiredMembersService, RequiredMembersService>();
        builder.Services.AddSingleton<RequiredMembersDemonstration>();

        var host = builder.Build();
        
        var demonstration = host.Services.GetRequiredService<RequiredMembersDemonstration>();
        await demonstration.ExecuteAsync();
    }
}
