namespace EduHub.Ch02;

public interface IAppLogger
{
    void Log(string message);
}

public class FileAppLogger : IAppLogger
{
    private readonly string _path;
    public FileAppLogger(string path = "club-log.txt") => _path = path;

    public void Log(string message) =>
        File.AppendAllText(_path, $"[{DateTime.Now}] {message}\n");
}