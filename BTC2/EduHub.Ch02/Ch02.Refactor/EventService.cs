using EduHub.Ch02.Domain;

namespace EduHub.Ch02;

public interface IEventObserver
{
    void OnEventCreated(Event ev, Club club);
}

public class EmailNotifier : IEventObserver
{
    private readonly IMemberRepository _members;
    private readonly INotificationServiceFactory _notifications;

    public EmailNotifier(IMemberRepository members, INotificationServiceFactory notifications)
    {
        _members = members;
        _notifications = notifications;
    }

    public void OnEventCreated(Event ev, Club club)
    {
        var email = _notifications.Create(NotificationChannel.Email);
        foreach (var m in _members.GetByClub(club.Id))
            email.Send(m.Email, $"new event '{ev.Title}' at {club.Name}");
    }
}

public class LogNotifier : IEventObserver
{
    private readonly IAppLogger _logger;
    public LogNotifier(IAppLogger logger) => _logger = logger;

    public void OnEventCreated(Event ev, Club club) =>
        _logger.Log($"Event created: {ev.Title}");
}

public class EventService
{
    private readonly IClubRepository _clubs;
    private readonly List<IEventObserver> _observers = new();

    public EventService(IClubRepository clubs) => _clubs = clubs;

    public void Subscribe(IEventObserver observer) => _observers.Add(observer);

    public Event CreateEvent(Guid clubId, string title, DateTime date)
    {
        var club = _clubs.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found");

        var ev = new Event { Id = Guid.NewGuid(), ClubId = clubId, Title = title, Date = date };
        foreach (var o in _observers)
            o.OnEventCreated(ev, club);
        return ev;
    }
}