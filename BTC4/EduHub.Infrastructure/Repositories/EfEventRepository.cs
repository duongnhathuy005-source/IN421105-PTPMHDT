using EduHub.Domain.Abstractions;
using EduHub.Domain.Entities;
using EduHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduHub.Infrastructure.Repositories;

public class EfEventRepository(EduHubDbContext db) : IEventRepository
{
    public async Task<(IReadOnlyList<Event> Items, int TotalCount)> SearchAsync(
        Guid? clubId, string? search, int skip, int take, CancellationToken ct = default)
    {
        var query = db.Events.AsNoTracking();

        if (clubId.HasValue)
            query = query.Where(e => e.ClubId == clubId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{search.Trim()}%"));

        var total = await query.CountAsync(ct);   // đếm trước khi cắt trang

        var items = await query
            .OrderBy(e => e.StartTime).ThenBy(e => e.Id)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Events.FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Add(Event ev) => db.Events.Add(ev);
    public void Remove(Event ev) => db.Events.Remove(ev);
}