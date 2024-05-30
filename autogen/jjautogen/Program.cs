// Copyright (c) Microsoft Corporation. All rights reserved.
// Example02_TwoAgent_MathChat.cs

using AutoGen.Core;
using AutoGen;
using FluentAssertions;
using AutoGen.OpenAI;

// get gpt-3.5-turbo config
var azureOpenAIKey = "KEY";
var endpoint = "https://jjazscoai.openai.azure.com/";
var deployName = "jjconsole";
var gpt35 = new AzureOpenAIConfig(endpoint, deployName, azureOpenAIKey);

// create teacher agent
// teacher agent will create math questions
var teacher = new AssistantAgent(
    name: "teacher",
    systemMessage: @"You are a teacher that create pre-school math question for student and check answer.
        If the answer is correct, you terminate conversation by saying [TERMINATE].
        If the answer is wrong, you ask student to fix it.",
    llmConfig: new ConversableAgentConfig
    {
        Temperature = 0,
        ConfigList = [gpt35],
    })
    .RegisterPostProcess(async (_, reply, _) =>
    {
        if (reply.GetContent()?.ToLower().Contains("terminate") is true)
        {
            return new TextMessage(Role.Assistant, GroupChatExtension.TERMINATE, from: reply.From);
        }

        return reply;
    })
    .RegisterPrintMessage();

// create student agent
// student agent will answer the math questions
var student = new AssistantAgent(
    name: "student",
    systemMessage: "You are a student that answer question from teacher",
    llmConfig: new ConversableAgentConfig
    {
        Temperature = 0,
        ConfigList = [gpt35],
    })
    .RegisterPrintMessage();

// start the conversation
var conversation = await student.InitiateChatAsync(
    receiver: teacher,
    message: "Hey teacher, please create math question for me.",
    maxRound: 10);

conversation.Count().Should().BeLessThan(10);
conversation.Last().IsGroupChatTerminateMessage().Should().BeTrue();
