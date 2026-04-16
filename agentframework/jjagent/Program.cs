using System;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ClientModel;
using OpenAI;
using System.ComponentModel;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_API_KEY");

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";
var client = new OpenAIClient(
    new ApiKeyCredential(apiKey),
    new OpenAIClientOptions
    {
        Endpoint = new Uri(endpoint)
    });

AIAgent agent = client
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "HelloAgent",
        ChatOptions = new ChatOptions
        {
            Instructions = "You are a friendly assistant. Keep your answers brief.",
            ModelId = deploymentName,
            Tools = [AIFunctionFactory.Create(GetWeather)]
        },
        ChatHistoryProvider = new InMemoryChatHistoryProvider()
    });

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?", session));
Console.WriteLine(await agent.RunAsync("Describe weather conditions in more details.", session));
Console.WriteLine(await agent.RunAsync("Which country is city we are discussing.", session));

