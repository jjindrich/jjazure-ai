using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Net;
using System.ComponentModel;

// https://learn.microsoft.com/en-us/semantic-kernel/prompts/templatizing-prompts?tabs=Csharp

public class WeatherPlugin
{
    [KernelFunction]
    [Description("Returns the current weather for a given location.")]
    [return: Description("The current weather for the given location")]
    public async Task<string> GetCurrentWeatherAsync(
               Kernel kernel,
               [Description("The location to get the weather for")] string location
           )
    {
        // Add logic to get the current weather for the given location
        // For now, we'll just return a static string
        return "The current weather is 72°F and sunny.";
    }
}

public class WeatherHistoryPlugin
{
    [KernelFunction]
    [Description("Returns the weather history for a given location.")]
    [return: Description("The weather history for the given location")]
    public async Task<string> GetWeatherHistoryAsync(
                      Kernel kernel,
                [Description("The location to get the weather history for")] string location
                  )
    {
        // Add logic to get the weather history for the given location
        // For now, we'll just return a static string
        return "The weather history is 22°F and sunny.";
    }
}

public class FriendPlugin
{
    [KernelFunction]
    [Description("Returns the description of friend. ")]
    [return: Description("Returns the description of friend.")]
    public async Task<string> GetFriendAsync(
                      Kernel kernel,
                [Description("Name of friend")] string name
                  )
    {
        // Add logic to get the weather history for the given location
        // For now, we'll just return a static string

        // Prompt the LLM to generate a list of steps to complete the task
        var result = await kernel.InvokePromptAsync($"""
        I'm going to write description of friend {name}.
        Is very friendly but is coming late on meetings.
        """, new() {
            { "name", name }
        });

        // Return the plan back to the agent
        return result.ToString();
    }
}