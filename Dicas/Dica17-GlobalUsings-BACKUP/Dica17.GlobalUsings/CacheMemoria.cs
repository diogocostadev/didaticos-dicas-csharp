// =================== CACHE DE MEMÓRIA (FILE-SCOPED NAMESPACE) ===================
namespace Dica17.GlobalUsings;

// Demonstração de classe genérica com file-scoped namespace
public class CacheMemoria<T> where T : class
{
    private readonly ConcurrentDictionary<string, ItemCache<T>> _cache = new();

    public void Adicionar(string chave, T valor, TimeSpan expiracao)
    {
        var item = new ItemCache<T>(valor, DateTime.UtcNow.Add(expiracao));
        _cache.AddOrUpdate(chave, item, (_, _) => item);
    }

    public T? Obter(string chave)
    {
        if (!_cache.TryGetValue(chave, out var item))
            return null;

        if (DateTime.UtcNow > item.Expiracao)
        {
            _cache.TryRemove(chave, out _);
            return null;
        }

        return item.Valor;
    }

    public void Limpar() => _cache.Clear();

    public IntList ObterEstatisticas()
    {
        var agora = DateTime.UtcNow;
        var itensValidos = _cache.Values.Count(item => agora <= item.Expiracao);
        var itensExpirados = _cache.Count - itensValidos;

        return new IntList { _cache.Count, itensValidos, itensExpirados };
    }
}

// Record aninhado também funciona
public record ItemCache<T>(T Valor, DateTime Expiracao) where T : class;

// Interface com file-scoped namespace
public interface IRepositorio<T>
{
    Task<T?> ObterPorIdAsync(int id);
    Task<IntList> ListarIdsAsync();
    Task SalvarAsync(T item);
}

// Implementação da interface
public class RepositorioMemoria<T> : IRepositorio<T> where T : class
{
    private readonly ConcurrentDictionary<int, T> _dados = new();
    private int _proximoId = 1;

    public Task<T?> ObterPorIdAsync(int id)
    {
        _dados.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<IntList> ListarIdsAsync()
    {
        var ids = new IntList(_dados.Keys);
        return Task.FromResult(ids);
    }

    public Task SalvarAsync(T item)
    {
        var id = Interlocked.Increment(ref _proximoId);
        _dados.TryAdd(id, item);
        return Task.CompletedTask;
    }
}
