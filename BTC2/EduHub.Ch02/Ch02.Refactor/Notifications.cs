namespace EduHub.Ch02;

public enum NotificationChannel { Gmail, Yahoo, Email, Slack }

public interface INotificationService
{
    void Send(string recipient, string message);
}

public class GmailNotificationService : INotificationService
{
    public void Send(string recipient, string message) =>
        Console.WriteLine($"Sending Gmail notification to {recipient}: {message}");
}

public class YahooNotificationService : INotificationService
{
    public void Send(string recipient, string message) =>
        Console.WriteLine($"Sending Yahoo notification to {recipient}: {message}");
}

public class EmailNotificationService : INotificationService
{
    public void Send(string recipient, string message) =>
        Console.WriteLine($"Sending generic email to {recipient}: {message}");
}

public class SlackNotificationService : INotificationService
{
    public void Send(string recipient, string message) =>
        Console.WriteLine($"Slack notification sent: {message}");
}

public interface INotificationServiceFactory
{
    INotificationService Create(NotificationChannel channel);
}

// Factory: thêm kênh mới chỉ cần thêm một nhánh ở đây (hoặc đăng ký thêm), không sửa nghiệp vụ.
public class NotificationServiceFactory : INotificationServiceFactory
{
    public INotificationService Create(NotificationChannel channel) => channel switch
    {
        NotificationChannel.Gmail => new GmailNotificationService(),
        NotificationChannel.Yahoo => new YahooNotificationService(),
        NotificationChannel.Email => new EmailNotificationService(),
        NotificationChannel.Slack => new SlackNotificationService(),
        _ => throw new ArgumentOutOfRangeException(nameof(channel))
    };
}

public static class NotificationChannelResolver
{
    public static NotificationChannel FromEmail(string email) =>
        email.Contains("@gmail.com") ? NotificationChannel.Gmail
        : email.Contains("@yahoo.com") ? NotificationChannel.Yahoo
        : NotificationChannel.Email;
}