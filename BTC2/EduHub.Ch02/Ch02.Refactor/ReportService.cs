namespace EduHub.Ch02;

public record ClubReportItem(string ClubName, int MemberCount);

public interface IReportStrategy
{
    void Write(IReadOnlyList<ClubReportItem> items);
}

public class ConsoleReportStrategy : IReportStrategy
{
    public void Write(IReadOnlyList<ClubReportItem> items)
    {
        foreach (var i in items)
            Console.WriteLine($"Club: {i.ClubName} | Members: {i.MemberCount}");
    }
}

public class CsvReportStrategy : IReportStrategy
{
    private readonly string _path;
    public CsvReportStrategy(string path = "club-report.csv") => _path = path;

    public void Write(IReadOnlyList<ClubReportItem> items)
    {
        var lines = new List<string> { "Club,Members" };
        lines.AddRange(items.Select(i => $"\"{i.ClubName}\",{i.MemberCount}"));
        File.WriteAllLines(_path, lines);
    }
}

public class ReportService
{
    private readonly IClubRepository _clubs;
    private readonly IMemberRepository _members;
    private readonly IAppLogger _logger;

    public ReportService(IClubRepository clubs, IMemberRepository members, IAppLogger logger)
    {
        _clubs = clubs;
        _members = members;
        _logger = logger;
    }

    public void Generate(IReportStrategy strategy)
    {
        var items = _clubs.GetAll()
            .Select(c => new ClubReportItem(c.Name, _members.CountByClub(c.Id)))
            .ToList();
        strategy.Write(items);
        _logger.Log($"Report generated for {items.Count} clubs");
    }
}