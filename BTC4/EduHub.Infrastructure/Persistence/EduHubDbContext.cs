using EduHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Infrastructure.Persistence;

public class EduHubDbContext(DbContextOptions<EduHubDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(b =>
        {
            b.ToTable("members");
            b.HasKey(m => m.Id);
            b.Property(m => m.StudentId).IsRequired().HasMaxLength(20);
            b.HasIndex(m => m.StudentId).IsUnique();
            b.Property(m => m.FullName).IsRequired().HasMaxLength(150);
            b.Property(m => m.Email).IsRequired().HasMaxLength(200);
            b.HasIndex(m => m.Email).IsUnique();
        });

        modelBuilder.Entity<Club>(b =>
        {
            b.ToTable("clubs");
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).IsRequired().HasMaxLength(150);
            b.Property(c => c.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<Event>(b =>
        {
            b.ToTable("events");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).IsRequired().HasMaxLength(200);
            b.Property(e => e.Capacity).HasDefaultValue(100);
            b.HasOne<Club>().WithMany().HasForeignKey(e => e.ClubId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Registration>(b =>
        {
            b.ToTable("registrations");
            b.HasKey(r => r.Id);
            b.Property(r => r.Status).IsRequired().HasMaxLength(20);
            b.HasOne<Member>().WithMany().HasForeignKey(r => r.MemberId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Event>().WithMany().HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(r => new { r.MemberId, r.EventId }).IsUnique();
        });
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await Clubs.AnyAsync(ct)) return;   // chạy lại nhiều lần không bị nhân đôi

        var clubs = new[]
        {
            Club.Create("CLB Lập trình", "Sinh hoạt lập trình và thi đấu"),
            Club.Create("CLB Tiếng Anh", "Luyện giao tiếp tiếng Anh"),
            Club.Create("CLB Nhiếp ảnh", "Chụp ảnh và chỉnh sửa ảnh"),
        };
        Clubs.AddRange(clubs);

        // StartTime phải là UTC, nếu không Npgsql sẽ báo lỗi khi lưu vào cột timestamptz
        Events.AddRange(
            Event.Create("Hackathon EduHub", clubs[0].Id, DateTime.UtcNow.AddDays(14), 100,
                "Cuộc thi lập trình 24 giờ", "Hội trường A"),
            Event.Create("Workshop Git (demo đầy)", clubs[0].Id, DateTime.UtcNow.AddDays(7), 2,
                "Buổi thực hành Git cơ bản", "Phòng Lab 2"));

        Members.AddRange(
            Member.Create("SV001", "Nguyễn Văn A", "a@example.com"),
            Member.Create("SV002", "Trần Thị B", "b@example.com"),
            Member.Create("SV003", "Lê Văn C", "c@example.com"));

        await SaveChangesAsync(ct);
    }
}