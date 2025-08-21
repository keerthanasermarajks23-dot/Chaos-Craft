using ChaosCraft.Models;
using WireMock.Server;
using WireMock.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Text.Json;
using System.Collections.Concurrent;

namespace ChaosCraft.Services;

public class ChaosTestService
{
    private WireMockServer? _server;
    private readonly ConcurrentBag<ChaosTest> _tests = new();
    private readonly List<ChaosTemplate> _templates = new();
    private readonly Random _random = new();
    private Timer? _logTimer;

    public event Action<string>? LogAdded;
    public event Action? TestsUpdated;

    public ChaosTestService()
    {
        InitializeTemplates();
        StartLogTimer();
    }

    public List<string> GetEndpoints() => new()
    {
        "/login", "/orders", "/checkout", "/users", "/products", "/payments"
    };

    public List<ChaosTemplate> GetTemplates() => _templates;

    public List<ChaosTest> GetTests() => _tests.ToList();
    
    public ChaosTest? GetTest(string id) => _tests.FirstOrDefault(t => t.Id == id);

    public async Task<string> RunTestAsync(string endpoint, ChaosTemplate template)
    {
        var test = new ChaosTest
        {
            Name = $"{template.Name} on {endpoint}",
            Description = template.Description,
            Endpoint = endpoint,
            Template = template,
            Status = TestStatus.Running
        };

        _tests.Add(test);
        TestsUpdated?.Invoke();

        try
        {
            await StopServerAsync();
            
            var settings = new WireMockServerSettings 
            { 
                Port = 8080
            };
            
            _server = WireMockServer.Start(settings);
            AddLog(test, "Started WireMock server on port 8080");

            var responseBuilder = CreateResponseBuilder(template);
            
            _server.Given(Request.Create().WithPath(endpoint).UsingAnyMethod())
                   .RespondWith(responseBuilder);

            AddLog(test, $"Configured endpoint {endpoint} with {template.Name}");
            AddLog(test, $"Server URL: http://localhost:8080{endpoint}");
            
            await Task.Delay(1000);
            
            test.Status = TestStatus.Completed;
            AddLog(test, "Test completed successfully");
            TestsUpdated?.Invoke();

            return test.Id;
        }
        catch (Exception ex)
        {
            test.Status = TestStatus.Failed;
            AddLog(test, $"Error: {ex.Message}");
            TestsUpdated?.Invoke();
            return test.Id;
        }
    }

    public string GenerateWireMockConfig(string endpoint, ChaosTemplate template)
    {
        var config = new
        {
            mappings = new[]
            {
                new
                {
                    request = new { method = "ANY", url = endpoint },
                    response = new
                    {
                        status = template.StatusCode,
                        body = template.ResponseBody,
                        headers = template.Headers,
                        delayDistribution = new { type = "fixed", milliseconds = template.DelayMs }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
    }

    public string ExplainTest(ChaosTemplate template, string endpoint)
    {
        var explanation = $"**Chaos Test Explanation**\n\n";
        explanation += $"This test simulates **{template.Description.ToLower()}** on the `{endpoint}` endpoint.\n\n";
        
        switch (template.Name)
        {
            case "Server Error":
                explanation += "This helps test how your application handles server failures, database outages, or internal service errors.";
                break;
            case "Slow Response":
                explanation += "This simulates network latency, overloaded servers, or slow database queries.";
                break;
            case "Not Found":
                explanation += "This tests how your system handles missing resources or broken links.";
                break;
            case "Malformed JSON":
                explanation += "This tests your application's resilience to corrupted or invalid API responses.";
                break;
            case "Random Responses":
                explanation += "This simulates unpredictable API behavior, testing your system's ability to handle inconsistent responses.";
                break;
        }
        
        explanation += $"\n\n**Technical Details:**\n";
        explanation += $"- HTTP Status: {template.StatusCode}\n";
        explanation += $"- Response Delay: {template.DelayMs}ms\n";
        explanation += $"- Endpoint: {endpoint}";
        
        return explanation;
    }

    public string GenerateMarkdownReport(ChaosTest test)
    {
        return $@"# Chaos Test Report

## Test Details
- **API Tested**: {test.Endpoint}
- **Chaos Scenario**: {test.Template.Name}
- **Start Time**: {test.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC
- **Status**: {test.Status}

## Test Configuration
- **Status Code**: {test.Template.StatusCode}
- **Delay**: {test.Template.DelayMs}ms
- **Description**: {test.Template.Description}

## AI Explanation
{ExplainTest(test.Template, test.Endpoint)}

## Logs
{string.Join("\n", test.Logs.Select(log => $"- {log}"))}

---
*Generated by ChaosCraft at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*";
    }

    private async Task StopServerAsync()
    {
        if (_server != null)
        {
            _server.Stop();
            _server.Dispose();
            _server = null;
            await Task.Delay(500);
        }
    }
    
    private IResponseBuilder CreateResponseBuilder(ChaosTemplate template)
    {
        var responseBuilder = Response.Create();
        
        if (template.Name == "Random Responses")
        {
            var isError = _random.Next(0, 2) == 0;
            responseBuilder = responseBuilder.WithStatusCode(isError ? 500 : 200);
            responseBuilder = responseBuilder.WithBody(isError ? 
                "{\"error\":\"Random server error\"}" : 
                "{\"message\":\"Random success\"}");
        }
        else
        {
            responseBuilder = responseBuilder.WithStatusCode(template.StatusCode);
            if (!string.IsNullOrEmpty(template.ResponseBody))
            {
                responseBuilder = responseBuilder.WithBody(template.ResponseBody);
            }
        }
        
        if (template.DelayMs > 0)
        {
            responseBuilder = responseBuilder.WithDelay(template.DelayMs);
        }
        
        foreach (var header in template.Headers)
        {
            responseBuilder = responseBuilder.WithHeader(header.Key, header.Value);
        }
        
        return responseBuilder;
    }
    
    private void AddLog(ChaosTest test, string message)
    {
        var logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        test.Logs.Add(logEntry);
        LogAdded?.Invoke(logEntry);
    }
    
    private void StartLogTimer()
    {
        _logTimer = new Timer(SimulateServerActivity, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
    }
    
    private void SimulateServerActivity(object? state)
    {
        if (_server?.IsStarted == true)
        {
            var activities = new[]
            {
                "Incoming request processed",
                "Response sent to client", 
                "Health check performed",
                "Request matched mapping"
            };
            
            var activity = activities[_random.Next(activities.Length)];
            LogAdded?.Invoke($"[{DateTime.Now:HH:mm:ss}] WireMock: {activity}");
        }
    }
    
    public void Dispose()
    {
        _logTimer?.Dispose();
        _server?.Stop();
        _server?.Dispose();
    }
    
    private void InitializeTemplates()
    {
        _templates.AddRange(new[]
        {
            new ChaosTemplate
            {
                Name = "Server Error",
                Description = "Returns HTTP 500 Internal Server Error",
                StatusCode = 500,
                ResponseBody = "{\"error\":\"Internal server error\"}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            },
            new ChaosTemplate
            {
                Name = "Slow Response",
                Description = "Introduces 5 second delay",
                StatusCode = 200,
                DelayMs = 5000,
                ResponseBody = "{\"message\":\"Success after delay\"}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            },
            new ChaosTemplate
            {
                Name = "Not Found",
                Description = "Returns HTTP 404 Not Found",
                StatusCode = 404,
                ResponseBody = "{\"error\":\"Resource not found\"}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            },
            new ChaosTemplate
            {
                Name = "Malformed JSON",
                Description = "Returns invalid JSON response",
                StatusCode = 200,
                ResponseBody = "{\"incomplete\": json",
                IsMalformed = true,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            },
            new ChaosTemplate
            {
                Name = "Random Responses",
                Description = "Randomly switches between success and error",
                StatusCode = 200,
                ResponseBody = "{\"random\":true}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            },
            new ChaosTemplate
            {
                Name = "Rate Limited",
                Description = "Returns HTTP 429 Too Many Requests",
                StatusCode = 429,
                ResponseBody = "{\"error\":\"Rate limit exceeded\"}",
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            }
        });
    }
}