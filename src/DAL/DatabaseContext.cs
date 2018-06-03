using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<FacebookPage> FacebookPage { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Publication> Publication { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Vacancy> Vacancy { get; set; }

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

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasMaxLength(500);

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

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Logo).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(100);

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

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

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

            modelBuilder.Entity<FacebookPage>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("FacebookPage_Category_Id_fk");

                entity.HasIndex(e => e.Id)
                    .HasName("FacebookPages_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Token)
                    .HasName("FacebookPages_Token_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.FacebookPage)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FacebookPage_Category_Id_fk");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("Language_Id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Code).HasMaxLength(2);

                entity.Property(e => e.Name).HasMaxLength(25);
            });

            modelBuilder.Entity<Publication>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("Publication_Category_Id_fk");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("Publication_Language_Id_fk");

                entity.HasIndex(e => e.Link)
                    .HasName("Publication_Link_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("Publication_User_Id_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Comment).HasMaxLength(1000);

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.EmbededPlayerCode).HasMaxLength(1000);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.LanguageId).HasColumnType("int(11)");

                entity.Property(e => e.Link).HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Type).HasMaxLength(25);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Views)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Publication)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Publication_Category_Id_fk");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Publication)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("Publication_Language_Id_fk");

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

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("Vacancy_Category_Id_fk");

                entity.HasIndex(e => e.Id)
                    .HasName(" Vacancy_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Image)
                    .HasName(" Vacancy_Image_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.LanguageId)
                    .HasName("Vacancy_Language_Id_fk");

                entity.HasIndex(e => e.UserId)
                    .HasName("Vacancy_User_Id_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Active).HasColumnType("bit(1)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.Contact)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Location).HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Url).HasMaxLength(5000);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Views)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Vacancy)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vacancy_Category_Id_fk");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Vacancy)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("Vacancy_Language_Id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Vacancy)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vacancy_User_Id_fk");
            });
        }
    }
}
