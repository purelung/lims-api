using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ZeeReportingApi.Model
{
    public partial class ODSContext : DbContext
    {
        public ODSContext()
        {
        }

        public ODSContext(DbContextOptions<ODSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Franchisee> Franchisee { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserXSalon> UserXSalon { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:midnight-data.database.windows.net,1433;Initial Catalog=ODS;Persist Security Info=False;User ID=midnightData;Password=KRCxb24jLgzat279TgtYKPf4q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Franchisee>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Franchisee", "app");

                entity.Property(e => e.FranchiseId).HasColumnName("FranchiseID");

                entity.Property(e => e.FranchiseeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.OwnerName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "app");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FranchiseId).HasColumnName("FranchiseID");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserRoles", "app");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoleDescription)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserXSalon>(entity =>
            {
                entity.ToTable("User_x_Salon", "app");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SalonId).HasColumnName("SalonID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
