using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Llm;

interface ILlmClient
{
    Task<Drip> NextDripAsync(IntentionSeed seed, CodeCanvas canvas, CancellationToken ct);
}
