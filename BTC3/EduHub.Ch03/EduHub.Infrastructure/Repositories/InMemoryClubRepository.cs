using EduHub.Domain.Entities;
using EduHub.Domain.Interfaces;

namespace EduHub.Infrastructure.Repositories;

public class InMemoryClubRepository : IClubRepository
{
    private static readonly List<Club> _clubs = new();

    public Task<IEnumerable<Club>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<Club>>(_clubs.ToList());
    }

    public Task<Club?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return Task.FromResult(_clubs.FirstOrDefault(c => c.Id == id));
    }

    public Task AddAsync(Club club, CancellationToken ct = default)
    {
        _clubs.Add(club);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Club club, CancellationToken ct = default)
    {
        var index = _clubs.FindIndex(c => c.Id == club.Id);
        if (index < 0)
            throw new KeyNotFoundException($"Không tìm thấy club {club.Id}");
        _clubs[index] = club;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var club = _clubs.FirstOrDefault(c => c.Id == id)
            ?? throw new KeyNotFoundException($"Không tìm thấy club {id}");
        _clubs.Remove(club);
        return Task.CompletedTask;
    }
}