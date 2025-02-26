using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TodoApi
{
    public partial class ToDoDbContext : DbContext
    {
        private readonly ILogger<ToDoDbContext> _logger;

        public ToDoDbContext()
        {
        }
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options, ILogger<ToDoDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }
        public virtual DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("ToDoDB");

                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Connection string 'ToDoDB' not found in environment variables.");
                    // Handle the error appropriately, e.g., throw an exception or use a default connection string.
                }
                else
                {
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    optionsBuilder.LogTo(message => _logger.LogInformation(message));
                    _logger.LogInformation($"Connection string used: {connectionString}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while configuring the DbContext.");
                // Handle the exception appropriately.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("Items")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Id).HasColumnName("Id"); // שינוי שם העמודה ל-"Id"
                entity.Property(e => e.IsComplete).HasColumnName("IsComplete"); // שינוי שם העמודה ל-"IsComplite"
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("Name"); // שינוי שם העמודה ל-"Name"
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
// using System;
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;
// using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

// namespace TodoApi;

// public partial class ToDoDbContext : DbContext
// {
//     public ToDoDbContext()
//     {
//     }

//     public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
//         : base(options)
//     {
//     }

//     public virtual DbSet<Item> Items { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         => optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder
//             .UseCollation("utf8mb4_0900_ai_ci")
//             .HasCharSet("utf8mb4");

//         modelBuilder.Entity<Item>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("PRIMARY");

//             entity
//                 .ToTable("items")
//                 .UseCollation("utf8mb4_unicode_ci");

//             entity.Property(e => e.Id).HasColumnName("id");
//             entity.Property(e => e.Iscomplete).HasColumnName("iscomplete");
//             entity.Property(e => e.Name)
//                 .HasMaxLength(100)
//                 .HasColumnName("name");
//         });

//         OnModelCreatingPartial(modelBuilder);
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// }

