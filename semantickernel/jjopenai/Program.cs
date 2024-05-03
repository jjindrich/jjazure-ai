using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Net;
using System.ComponentModel;

// https://learn.microsoft.com/en-us/semantic-kernel/agents/

string modelId = "jjconsole";
string endpoint = "https://jjazscoai.openai.azure.com/";
string apiKey = "";

// Create the kernel
var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
builder.Plugins.AddFromType<WeatherPlugin>();
builder.Plugins.AddFromType<WeatherHistoryPlugin>();
builder.Plugins.AddFromType<FriendPlugin>();
Kernel kernel = builder.Build();

// Retrieve the chat completion service from the kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Create the chat history
ChatHistory chatMessages = new ChatHistory("""
You are a friendly assistant who likes to follow the rules. Your name is JJagent.
""");

// Start the conversation
while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Stream the results
    string fullMessage = "";
    await foreach (var content in result)
    {
        if (content.Role.HasValue)
        {
            System.Console.Write("Assistant > ");
        }
        System.Console.Write(content.Content);
        fullMessage += content.Content;
    }
    System.Console.WriteLine();

    // Add the message from the agent to the chat history
    chatMessages.AddAssistantMessage(fullMessage);
}

// samples:
// Jake bylo pocasi minuly rok v New York ?
// Jake bylo pocasi v New York v unoru 2020 ?
// Máš popis kamaráda Erika ?