# JJAgent

Minimal C# Microsoft Agent Framework sample prepared for Azure AI Foundry hosted-agent deployment.

## What the app does

- Creates a `ChatClientAgent` using Microsoft Agent Framework and Azure OpenAI.
- Registers one deterministic simulated weather tool named `GetWeather`.
- Starts the Foundry hosted-agent adapter via `RunAIAgentAsync(...)`.

## Required environment variables

Set these before running locally or configure them for the hosted-agent deployment:

- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_DEPLOYMENT_NAME`

Authentication uses `DefaultAzureCredential`, so local development works with a signed-in Azure CLI session.

## Run locally

```powershell
$env:AZURE_OPENAI_ENDPOINT="https://<your-openai-resource>.openai.azure.com/"
$env:AZURE_OPENAI_DEPLOYMENT_NAME="<your-model-deployment-name>"
dotnet run
```

## Deploy to Azure AI Foundry as a hosted agent

Use the **Microsoft Foundry for Visual Studio Code** extension for this C# project.

### Prerequisites

- Azure subscription
- Visual Studio Code
- Microsoft Foundry for Visual Studio Code extension
- .NET 9 SDK
- Permission to create Foundry resources
- The Foundry project managed identity must have:
  - **Azure AI User**
  - **AcrPull**

### 1. Create a Foundry project

In VS Code:

1. Open the Command Palette.
2. Run `Microsoft Foundry: Create Project`.
3. Select your Azure subscription.
4. Create or select a resource group.
5. Enter a project name.

### 2. Deploy a model

In VS Code:

1. Open the Command Palette.
2. Run `Microsoft Foundry: Open Model Catalog`.
3. Pick a model you want to use.
4. Deploy it to the Foundry project.

After deployment, collect the model settings that the app uses:

- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_DEPLOYMENT_NAME`

If you use a separate Azure OpenAI resource for inference, configure those values from that resource instead.

### 3. Test the agent locally

Authenticate first:

```powershell
az login
```

Then run the app:

```powershell
$env:AZURE_OPENAI_ENDPOINT="https://<your-openai-resource>.openai.azure.com/"
$env:AZURE_OPENAI_DEPLOYMENT_NAME="<your-model-deployment-name>"
dotnet restore
dotnet build
dotnet run
```

The app should start as the hosted-agent HTTP server.

### 4. Deploy the hosted agent

In VS Code:

1. Open the Command Palette.
2. Run `Microsoft Foundry: Deploy Hosted Agent`.
3. Select your Foundry workspace.
4. Make sure the deployment has these environment variables configured:

   - `AZURE_OPENAI_ENDPOINT`
   - `AZURE_OPENAI_DEPLOYMENT_NAME`

5. When asked for the container agent file, choose:

   `C:\Users\jajindri\source\repos\jjazure-ai\agentframework\jjagent.csproj`

6. Choose the CPU and memory size.
7. Confirm deployment.

### 5. Verify the deployment

After deployment:

1. Open the **Hosted Agents (Preview)** view in the Foundry extension.
2. Open the deployed agent version.
3. Verify the status is **Started**.
4. Open the playground and test prompts such as:
   - `What's the weather in Prague?`
   - `Give me a weather forecast for Seattle.`

The response should clearly indicate that the forecast is simulated.

## Troubleshooting

- If local startup fails with `AZURE_OPENAI_ENDPOINT is not set`, make sure both required environment variables are defined.
- If authentication fails, rerun `az login`.
- If deployment succeeds but the agent does not start, verify the project managed identity has **AcrPull**.
- If deployment fails with `Could not load type 'Microsoft.Extensions.AI.UserInputRequestContent'`, make sure the project stays on the AgentServer-compatible package set: target `.NET 9` and keep `Microsoft.Extensions.AI.OpenAI` aligned to `10.3.0`. A newer direct `Microsoft.Extensions.AI.*` reference can force `Microsoft.Extensions.AI.Abstractions` to `10.5.0`, which is incompatible with `Azure.AI.AgentServer.AgentFramework` `1.0.0-beta.11`.
- Hosted agents are currently in preview, so deployment behavior and portal UX can change.
