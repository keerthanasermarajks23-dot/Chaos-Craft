namespace ChaosCraft.Models;

public class ChaosTest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public ChaosTemplate Template { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TestStatus Status { get; set; } = TestStatus.NotStarted;
    public List<string> Logs { get; set; } = new();
}

public class ChaosTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    public int DelayMs { get; set; } = 0;
    public string ResponseBody { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool IsMalformed { get; set; } = false;
}

public enum TestStatus
{
    NotStarted,
    Running,
    Completed,
    Failed
}

public class TestReport
{
    public string TestId { get; set; } = string.Empty;
    public string ApiTested { get; set; } = string.Empty;
    public string ChaosScenario { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string AiExplanation { get; set; } = string.Empty;
    public List<string> Logs { get; set; } = new();
}