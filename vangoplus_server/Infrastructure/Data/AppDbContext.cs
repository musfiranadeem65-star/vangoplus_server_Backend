using Microsoft.EntityFrameworkCore;
using vangoplus_server.Domain.Entities;
using Route = vangoplus_server.Domain.Entities.Route;

namespace vangoplus_server.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Guardian> Guardians { get; set; } = null!;
        public DbSet<StudentGuardian> StudentGuardians { get; set; } = null!;
        public DbSet<Driver> Drivers { get; set; } = null!;
        public DbSet<Route> Routes { get; set; } = null!;
        public DbSet<RouteStop> RouteStops { get; set; } = null!;
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;
        public DbSet<StudentRouteAssignment> StudentRouteAssignments { get; set; } = null!;
        public DbSet<SchoolSetting> SchoolSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Name).IsRequired().HasMaxLength(200);
                b.Property(u => u.Email).IsRequired().HasMaxLength(200);
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.Phone).HasMaxLength(50);
                b.Property(u => u.City).HasMaxLength(100);
                b.Property(u => u.Role).HasMaxLength(50);
                b.Property(u => u.Status).HasMaxLength(50);
                b.Property(u => u.EmailAlerts).IsRequired().HasDefaultValue(true);
                b.Property(u => u.SmsAlerts).IsRequired().HasDefaultValue(true);
            });

            modelBuilder.Entity<Student>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Name).IsRequired().HasMaxLength(200);
                b.Property(s => s.Grade).IsRequired().HasMaxLength(50);
                b.Property(s => s.Section).HasMaxLength(50);
                b.Property(s => s.Status).HasMaxLength(50);

                b.HasOne(s => s.ParentUser)
                    .WithMany()
                    .HasForeignKey(s => s.ParentUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Guardian>(b =>
            {
                b.HasKey(g => g.Id);
                b.Property(g => g.Name).IsRequired().HasMaxLength(200);
                b.Property(g => g.Relation).IsRequired().HasMaxLength(100);
                b.Property(g => g.Phone).HasMaxLength(50);
                b.Property(g => g.Status).HasMaxLength(50);
                b.Property(g => g.Note).HasMaxLength(500);
                b.Property(g => g.IdentityDocumentPath).HasMaxLength(500);

                b.HasOne(g => g.User)
                    .WithMany()
                    .HasForeignKey(g => g.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StudentGuardian>(b =>
            {
                b.HasKey(sg => new { sg.StudentId, sg.GuardianId });
                b.Property(sg => sg.Status).HasMaxLength(50);

                b.HasOne(sg => sg.Student)
                    .WithMany()
                    .HasForeignKey(sg => sg.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(sg => sg.Guardian)
                    .WithMany(g => g.StudentGuardians)
                    .HasForeignKey(sg => sg.GuardianId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Driver>(b =>
            {
                b.HasKey(d => d.Id);
                b.Property(d => d.Name).IsRequired().HasMaxLength(200);
                b.Property(d => d.LicenseNo).IsRequired().HasMaxLength(100);
                b.Property(d => d.Phone).HasMaxLength(50);
                b.Property(d => d.Email).HasMaxLength(200);
                b.Property(d => d.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<Route>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Name).IsRequired().HasMaxLength(200);
                b.Property(r => r.Status).IsRequired().HasMaxLength(50);
                b.Property(r => r.Description).HasMaxLength(500);

                b.HasOne(r => r.Driver)
                    .WithMany(d => d.Routes)
                    .HasForeignKey(r => r.DriverId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RouteStop>(b =>
            {
                b.HasKey(rs => rs.Id);
                b.Property(rs => rs.StopName).IsRequired().HasMaxLength(200);
                b.Property(rs => rs.OrderIndex).IsRequired();

                b.HasOne(rs => rs.Route)
                    .WithMany(r => r.RouteStops)
                    .HasForeignKey(rs => rs.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SubscriptionPlan>(b =>
            {
                b.HasKey(sp => sp.Id);
                b.Property(sp => sp.Name).IsRequired().HasMaxLength(200);
                b.Property(sp => sp.Price).IsRequired();
                b.Property(sp => sp.MaxChildren).IsRequired();
                b.Property(sp => sp.Features).IsRequired().HasMaxLength(1000);
            });

            modelBuilder.Entity<SchoolSetting>(b =>
            {
                b.HasKey(ss => ss.Id);
                b.Property(ss => ss.SchoolName).IsRequired().HasMaxLength(200);
                b.Property(ss => ss.ContactPerson).HasMaxLength(200);
                b.Property(ss => ss.SchoolAddress).HasMaxLength(500);
                b.Property(ss => ss.MonthlyAmount).IsRequired();
                b.Property(ss => ss.SenderEmail).HasMaxLength(200);
            });

            modelBuilder.Entity<Subscription>(b =>
            {
                b.HasKey(s => s.Id);

                b.Property(s => s.UserId).IsRequired();

                b.Property(s => s.PlanId).IsRequired();

                b.Property(s => s.PlanName)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(s => s.Price)
                    .IsRequired();

                b.Property(s => s.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                b.Property(s => s.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(s => s.StartedAt)
                    .IsRequired();

                // JazzCash fields
                // Optional so existing subscription records remain valid.
                b.Property(s => s.JazzCashNumber)
                    .HasMaxLength(20)
                    .IsRequired(false);

                b.Property(s => s.TransactionId)
                    .HasMaxLength(200)
                    .IsRequired(false);

                b.Property(s => s.PaymentStatus)
                    .HasMaxLength(50)
                    .IsRequired(false);

                b.Property(s => s.PaidAt)
                    .IsRequired(false);

                b.HasOne(s => s.User)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(s => s.SubscriptionPlan)
                    .WithMany(sp => sp.Subscriptions)
                    .HasForeignKey(s => s.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Alert>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.StudentId).IsRequired();
                b.Property(a => a.Type).IsRequired().HasMaxLength(100);
                b.Property(a => a.Title).IsRequired().HasMaxLength(200);
                b.Property(a => a.Message).IsRequired().HasMaxLength(1000);
                b.Property(a => a.SentAt).IsRequired();
                b.Property(a => a.IsRead).IsRequired();

                b.HasOne(a => a.Student)
                    .WithMany(s => s.Alerts)
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StudentRouteAssignment>(b =>
            {
                b.HasKey(sra => sra.Id);

                b.Property(sra => sra.StudentId)
                    .IsRequired();

                b.Property(sra => sra.RouteId)
                    .IsRequired();

                b.Property(sra => sra.PickupTime);

                b.Property(sra => sra.DropoffTime);

                b.Property(sra => sra.AssignedAt)
                    .IsRequired();

                b.Property(sra => sra.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                b.HasOne(sra => sra.Student)
                    .WithMany()
                    .HasForeignKey(sra => sra.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(sra => sra.Route)
                    .WithMany()
                    .HasForeignKey(sra => sra.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}