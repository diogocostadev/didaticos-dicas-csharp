using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dica80.CleanArchitecture.Domain.Entities;

namespace Dica80.CleanArchitecture.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TaskItem entity
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ProjectId)
            .IsRequired();

        builder.Property(x => x.AssignedToId)
            .IsRequired(false);

        builder.Property(x => x.DueDate)
            .IsRequired(false);

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Project)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AssignedTo)
            .WithMany(x => x.AssignedTasks)
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.Title)
            .HasDatabaseName("IX_Tasks_Title");

        builder.HasIndex(x => x.ProjectId)
            .HasDatabaseName("IX_Tasks_ProjectId");

        builder.HasIndex(x => x.AssignedToId)
            .HasDatabaseName("IX_Tasks_AssignedToId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Tasks_Status");

        builder.HasIndex(x => x.Priority)
            .HasDatabaseName("IX_Tasks_Priority");

        builder.HasIndex(x => x.DueDate)
            .HasDatabaseName("IX_Tasks_DueDate");

        // Ignore domain events
        builder.Ignore(x => x.DomainEvents);
    }
}

/// <summary>
/// Entity Framework configuration for Comment entity
/// </summary>
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.TaskId)
            .IsRequired();

        builder.Property(x => x.AuthorId)
            .IsRequired();

        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Task)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.TaskId)
            .HasDatabaseName("IX_Comments_TaskId");

        builder.HasIndex(x => x.AuthorId)
            .HasDatabaseName("IX_Comments_AuthorId");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Comments_CreatedAt");

        // Ignore domain events
        builder.Ignore(x => x.DomainEvents);
    }
}
