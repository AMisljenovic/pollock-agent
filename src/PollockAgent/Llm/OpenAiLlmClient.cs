using OpenAI.Chat;
using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Llm;

class OpenAiLlmClient(ChatClient chat, Constraints constraints) : ILlmClient
{
    public async Task<Drip> NextDripAsync(IntentionSeed seed, CodeCanvas canvas, CancellationToken ct)
    {
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        var completion = await chat.CompleteChatAsync(
            [
                new SystemChatMessage(Prompts.System(constraints)),
                new UserChatMessage(Prompts.User(seed, canvas))
            ],
            options,
            ct);

        var text = completion.Value.Content.FirstOrDefault()?.Text
            ?? throw new InvalidOperationException("OpenAI returned no content.");

        return DripJson.Parse(text);
    }
}
