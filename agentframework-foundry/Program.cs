using System.ComponentModel;
using Azure.AI.AgentServer.AgentFramework.Extensions;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

string openAiEndpoint = "https://jjaitestv2gwc-resource.openai.azure.com";
string modelDeploymentName = "gpt-5-mini";

DefaultAzureCredential credential = new();

IChatClient chatClient = new AzureOpenAIClient(new Uri(openAiEndpoint), credential)
    .GetChatClient(modelDeploymentName)
    .AsIChatClient();

AIAgent agent = new ChatClientAgent(
        chatClient,
        name: "weather-hosted-agent",
        instructions: """
            You are a helpful weather assistant.
            Use the GetWeather tool for all weather questions.
            Always make it clear that the forecast is simulated.
            Keep answers concise.
            """,
        tools: [AIFunctionFactory.Create(GetWeather, name: nameof(GetWeather))]);

await agent.RunAIAgentAsync(
    telemetrySourceName: "WeatherHostedAgent");

return;

[Description("Get a simulated weather forecast for a given location.")]
static string GetWeather([Description("The city or location to forecast.")] string location)
{
    string normalizedLocation = location.Trim();
    if (string.IsNullOrWhiteSpace(normalizedLocation))
    {
        throw new ArgumentException("Location is required.", nameof(location));
    }

    string[] conditions = ["sunny", "cloudy", "rainy", "windy"];
    int seed = Math.Abs(normalizedLocation.ToLowerInvariant().GetHashCode());
    string condition = conditions[seed % conditions.Length];
    int highCelsius = 12 + (seed % 15);
    int lowCelsius = Math.Max(4, highCelsius - 7);

    return $"Simulated forecast for {normalizedLocation}: {condition} with temperatures from {lowCelsius}C to {highCelsius}C.";
}
