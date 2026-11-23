using EduHub.Domain.Abstractions;
using EduHub.Infrastructure.Persistence;

namespace EduHub.Infrastructure.Repositories;

// DbContext đã là Unit of Work. Lớp này chỉ che nó sau IUnitOfWork để lớp Application không phụ thuộc EF Core.
public class EfUnitOfWork(EduHubDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}