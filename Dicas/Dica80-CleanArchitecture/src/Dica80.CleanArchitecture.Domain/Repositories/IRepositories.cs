using Dica80.CleanArchitecture.Domain.Entities;

namespace Dica80.CleanArchitecture.Domain.Repositories;

/// <summary>
/// Base repository interface with common operations
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// User repository interface
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetProjectMembersAsync(int projectId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Project repository interface
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetWithTasksAsync(int id, CancellationToken cancellationToken = default);
    Task<Project?> GetWithMembersAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetUserProjectsAsync(int userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Task repository interface
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(int assigneeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskItem?> GetWithCommentsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetTasksDueSoonAsync(int days, CancellationToken cancellationToken = default);
}

/// <summary>
/// Comment repository interface
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of Work pattern interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    ITaskRepository Tasks { get; }
    ICommentRepository Comments { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
