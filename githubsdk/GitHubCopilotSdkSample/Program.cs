using GitHub.Copilot;
using Microsoft.Extensions.AI;
using System.ComponentModel;

Console.WriteLine("Starting GitHub Copilot SDK sample...");

await using var client = new CopilotClient();
await client.StartAsync();

static string GetWeatherForecast([Description("City name to forecast for.")] string city)
    => $"The weather in {city} is mild, with a light breeze and a 70% chance of sunshine.";

var weatherTool = CopilotTool.DefineTool(
    GetWeatherForecast,
    factoryOptions: new AIFunctionFactoryOptions
    {
        Name = "get_weather_forecast",
        Description = "Return a short weather forecast for a city."
    });

await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5-mini",
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Tools = [weatherTool],
    SystemMessage = new SystemMessageConfig
    {
        Mode = SystemMessageMode.Append,
        Content = "You are a concise assistant. Use the weather tool whenever the user asks for a weather forecast."
    }
});

var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

session.On<SessionEvent>(evt =>
{
    switch (evt)
    {
        case ToolExecutionCompleteEvent toolComplete:
            Console.WriteLine($"\n[tool] result: {toolComplete.Data.Result?.Content}");
            break;
        case AssistantMessageEvent assistant when (assistant.Data.ToolRequests is null || assistant.Data.ToolRequests.Length == 0):
            completion.TrySetResult(assistant.Data.Content);
            break;
        case SessionErrorEvent error:
            completion.TrySetException(new InvalidOperationException(error.Data.Message));
            break;
    }
});

await session.SendAsync(new MessageOptions
{
    Prompt = "What is the weather forecast for the main city in Czechia?"
});
Console.WriteLine("\nPrompt sent, waiting for Copilot response...");

var response = await completion.Task;
Console.WriteLine("\nCopilot response:");
Console.WriteLine(response);
