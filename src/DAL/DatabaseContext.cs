using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL
{
    public partial class DatabaseContext : DbContext
    {
        private readonly string _connectionString;
        
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }


        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<FacebookPage> FacebookPages { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Slack> Slacks { get; set; }
        public virtual DbSet<TwitterAccount> TwitterAccounts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.35-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_unicode_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasIndex(e => e.Name, "Category_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Tags).HasMaxLength(500);
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.ToTable("Channel");

                entity.HasIndex(e => e.CategoryId, "Channel_Category_Id_fk");

                entity.HasIndex(e => e.Name, "Channel_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Logo).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Channels)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Channel_Category_Id_fk");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.HasIndex(e => e.CategoryId, "Event_Category_Id_fk");

                entity.HasIndex(e => e.UserId, "Event_User_Id_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Link)
                    .HasMaxLength(100)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Event_Category_Id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Event_User_Id_fk");
            });

            modelBuilder.Entity<FacebookPage>(entity =>
            {
                entity.ToTable("FacebookPage");

                entity.HasIndex(e => e.CategoryId, "FacebookPage_Category_Id_fk");

                entity.HasIndex(e => e.Id, "FacebookPages_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Token, "FacebookPages_Token_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Logo).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(500)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasDefaultValueSql("''")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.FacebookPages)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FacebookPage_Category_Id_fk");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.HasIndex(e => e.Id, "Language_Id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasMaxLength(2)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Name).HasMaxLength(25);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Page");

                entity.HasIndex(e => e.Name, "Page_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.HasIndex(e => e.CategoryId, "Publication_Category_Id_fk");

                entity.HasIndex(e => e.LanguageId, "Publication_Language_Id_fk");

                entity.HasIndex(e => e.Link, "Publication_Link_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UserId, "Publication_User_Id_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Comment)
                    .HasMaxLength(1000)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.CommentUa)
                    .HasMaxLength(1000)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.EmbededPlayerCode).HasMaxLength(1000);

                entity.Property(e => e.Header).HasMaxLength(1000);

                entity.Property(e => e.HeaderUa).HasMaxLength(1000);

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.LanguageId).HasColumnType("int(11)");

                entity.Property(e => e.Link).HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Views)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Visible).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<Slack>(entity =>
            {
                entity.ToTable("Slack");

                entity.HasIndex(e => e.Name, "Slack_Name_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.WebHookUrl, "Slack_WebHookUrl_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.WebHookUrl)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<TwitterAccount>(entity =>
            {
                entity.ToTable("TwitterAccount");

                entity.HasIndex(e => e.AccessTokenSecret, "TwitterAccount_AccessTokenSecret_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.AccessToken, "TwitterAccount_AccessToken_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ConsumerKey, "TwitterAccount_ConsumerKey_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ConsumerSecret, "TwitterAccount_ConsumerSecret_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "TwitterAccount_Name_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Url, "TwitterAccount_Url_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.AccessTokenSecret)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ConsumerKey)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ConsumerSecret)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Logo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Key, "User_Key_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.ToTable("Vacancy");

                entity.HasIndex(e => e.Id, " Vacancy_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Image, " Vacancy_Image_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.CategoryId, "Vacancy_Category_Id_fk");

                entity.HasIndex(e => e.LanguageId, "Vacancy_Language_Id_fk");

                entity.HasIndex(e => e.UserId, "Vacancy_User_Id_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Active).HasColumnType("bit(1)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.Contact)
                    .IsRequired()
                    .HasMaxLength(500)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Image)
                    .HasMaxLength(500)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Location).HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Url)
                    .HasMaxLength(5000)
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Views)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vacancy_Category_Id_fk");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("Vacancy_Language_Id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Vacancy_User_Id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
