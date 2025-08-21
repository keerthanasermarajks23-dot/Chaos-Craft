namespace ChaosCraft.Services;

public interface IWireMockLogger
{
    void Debug(string formatString, params object[] args);
    void Info(string formatString, params object[] args);
    void Warn(string formatString, params object[] args);
    void Error(string formatString, params object[] args);
    void Error(string formatString, Exception exception, params object[] args);
}

public class WireMockConsoleLogger : IWireMockLogger
{
    public void Debug(string formatString, params object[] args) { }
    public void Info(string formatString, params object[] args) { }
    public void Warn(string formatString, params object[] args) { }
    public void Error(string formatString, params object[] args) { }
    public void Error(string formatString, Exception exception, params object[] args) { }
}