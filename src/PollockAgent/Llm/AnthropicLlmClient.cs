using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Llm;

class AnthropicLlmClient(HttpClient http, Constraints constraints, string apiKey, string model) : ILlmClient
{
    const string Endpoint = "https://api.anthropic.com/v1/messages";

    public async Task<Drip> NextDripAsync(IntentionSeed seed, CodeCanvas canvas, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = JsonContent.Create(new AnthropicRequest(
                Model: model,
                MaxTokens: 1024,
                System: Prompts.System(constraints),
                Messages: [new AnthropicMessage("user", Prompts.User(seed, canvas))]))
        };
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AnthropicResponse>(ct)
            ?? throw new InvalidOperationException("Anthropic returned empty body.");

        var text = body.Content.FirstOrDefault()?.Text
            ?? throw new InvalidOperationException("Anthropic returned no content blocks.");

        return DripJson.Parse(text);
    }

    record AnthropicRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("max_tokens")] int MaxTokens,
        [property: JsonPropertyName("system")] string System,
        [property: JsonPropertyName("messages")] AnthropicMessage[] Messages);

    record AnthropicMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    record AnthropicResponse(
        [property: JsonPropertyName("content")] AnthropicContentBlock[] Content);

    record AnthropicContentBlock(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("text")] string Text);
}

static class DripJson
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static Drip Parse(string raw)
    {
        var json = Strip(raw);
        var dto = JsonSerializer.Deserialize<DripDto>(json, Options)
            ?? throw new InvalidOperationException("LLM returned non-JSON drip.");
        return new Drip(dto.FilePath ?? string.Empty, dto.Content ?? string.Empty, dto.FeelsDone);
    }

    static string Strip(string raw)
    {
        var s = raw.Trim();
        if (s.StartsWith("```"))
        {
            var firstNewline = s.IndexOf('\n');
            if (firstNewline > 0) s = s[(firstNewline + 1)..];
            if (s.EndsWith("```")) s = s[..^3];
        }
        return s.Trim();
    }

    record DripDto(string? FilePath, string? Content, bool FeelsDone);
}
