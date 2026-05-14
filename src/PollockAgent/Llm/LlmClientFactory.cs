using System.ClientModel;
using OpenAI;
using PollockAgent.Agent;

namespace PollockAgent.Llm;

static class LlmClientFactory
{
    public static ILlmClient Create(Constraints constraints, HttpClient http)
    {
        var provider = (Environment.GetEnvironmentVariable("LLM_PROVIDER") ?? "anthropic").Trim().ToLowerInvariant();

        return provider switch
        {
            "openai" => CreateOpenAi(constraints),
            "anthropic" => CreateAnthropic(constraints, http),
            _ => throw new InvalidOperationException($"Unknown LLM_PROVIDER \"{provider}\". Use \"anthropic\" or \"openai\".")
        };
    }

    static AnthropicLlmClient CreateAnthropic(Constraints constraints, HttpClient http)
    {
        var key = Require("ANTHROPIC_API_KEY");
        var model = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL") ?? "claude-opus-4-7";
        return new AnthropicLlmClient(http, constraints, key, model);
    }

    static OpenAiLlmClient CreateOpenAi(Constraints constraints)
    {
        var key = Require("OPENAI_API_KEY");
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o";
        var baseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL");

        var options = new OpenAIClientOptions();
        if (!string.IsNullOrWhiteSpace(baseUrl))
            options.Endpoint = new Uri(baseUrl);

        var client = new OpenAIClient(new ApiKeyCredential(key), options);
        return new OpenAiLlmClient(client.GetChatClient(model), constraints);
    }

    static string Require(string key) =>
        Environment.GetEnvironmentVariable(key)
        ?? throw new InvalidOperationException($"Missing required env var: {key}. Copy .env.example to .env and fill it in.");
}
