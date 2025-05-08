using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public partial class ToDoListWebContext : DbContext
{
    public ToDoListWebContext()
    {
    }

    public ToDoListWebContext(DbContextOptions<ToDoListWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // Add DbSet for TaskTag
    public virtual DbSet<TaskTag> TaskTags { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure TaskTag entity
        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.TagId });

            entity.ToTable("TaskTags");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Task)
                .WithMany(p => p.TaskTags)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_TaskTags_Task");

            entity.HasOne(d => d.Tag)
                .WithMany(p => p.TaskTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskTags_Tag");
        });

        // Existing entity configurations...
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__442C64DE9D9D664A");

            entity.Property(e => e.AttachmentId).HasColumnName("AttachmentID");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Task).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Attachments_Task");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BCBDB4E3E");

            entity.HasIndex(e => new { e.UserId, e.Name }, "UQ_UserCategory").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Categories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Categories_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA96545A2E");

            entity.HasIndex(e => e.TaskId, "IDX_Comments_TaskID");

            entity.HasIndex(e => e.UserId, "IDX_Comments_UserID");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Comments_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_User");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A830A7B79BF2BC");

            entity.HasIndex(e => e.TaskId, "IDX_Reminders_TaskID");

            entity.HasIndex(e => new { e.TaskId, e.ReminderTime }, "UQ_TaskReminder").IsUnique();

            entity.Property(e => e.ReminderId).HasColumnName("ReminderID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsSent).HasDefaultValue(false);
            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Task).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Reminders_Task");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E08717B6DEB15DDF");

            entity.HasIndex(e => e.TaskId, "IDX_Subtasks_TaskID");

            entity.Property(e => e.SubtaskId).HasColumnName("SubtaskID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Subtasks_Task");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CFA4C374A34C2");

            entity.HasIndex(e => new { e.UserId, e.Name }, "UQ_UserTag").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Tags)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Tags_User");
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949D1F254F997");

            entity.HasIndex(e => e.CategoryId, "IDX_Tasks_CategoryID");

            entity.HasIndex(e => e.DueDate, "IDX_Tasks_DueDate");

            entity.HasIndex(e => e.Status, "IDX_Tasks_Status");

            entity.HasIndex(e => e.UserId, "IDX_Tasks_UserID");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("medium");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Tasks_Category");

            entity.HasOne(d => d.User).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Tasks_User");

            // Configure the many-to-many relationship between Task and Tag through TaskTag
            entity.HasMany(t => t.Tags)
                .WithMany(t => t.Tasks)
                .UsingEntity<TaskTag>(
                    j => j
                        .HasOne(tt => tt.Tag)
                        .WithMany(t => t.TaskTags)
                        .HasForeignKey(tt => tt.TagId),
                    j => j
                        .HasOne(tt => tt.Task)
                        .WithMany(t => t.TaskTags)
                        .HasForeignKey(tt => tt.TaskId),
                    j =>
                    {
                        j.HasKey(tt => new { tt.TaskId, tt.TagId });
                        j.ToTable("TaskTags");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC74B2BBAD");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4DEC522D4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342D2BBC67").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}