using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL
{
    public partial class DatabaseContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Publication> Publication { get; set; }
        public virtual DbSet<User> User { get; set; }

        private string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("Category_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("Channel_Category_Id_fk");

                entity.HasIndex(e => e.Name)
                    .HasName("Channel_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Channel)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Channel_Category_Id_fk");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("Event_Category_Id_fk");

                entity.HasIndex(e => e.UserId)
                    .HasName("Event_User_Id_fk");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(100);

                entity.Property(e => e.Link).HasMaxLength(100);

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Event_Category_Id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Event_User_Id_fk");
            });

            modelBuilder.Entity<Publication>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("Publication_Category_Id_fk");

                entity.HasIndex(e => e.Link)
                    .HasName("Publication_Link_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("Publication_User_Id_fk");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Comment).HasMaxLength(1000);

                entity.Property(e => e.Content).HasMaxLength(5000);

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Type).HasMaxLength(25);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Publication)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Publication_Category_Id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Publication)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Publication_User_Id_fk");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Key)
                    .HasName("User_Key_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
