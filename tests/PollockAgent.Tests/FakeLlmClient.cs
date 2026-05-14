using PollockAgent.Agent;
using PollockAgent.Canvas;
using PollockAgent.Llm;

namespace PollockAgent.Tests;

class FakeLlmClient : ILlmClient
{
    readonly Queue<Func<CancellationToken, Task<Drip>>> _script = new();

    public int Calls { get; private set; }

    public FakeLlmClient Script(params Drip[] drips)
    {
        foreach (var d in drips)
            _script.Enqueue(_ => Task.FromResult(d));
        return this;
    }

    public FakeLlmClient ScriptDelay(TimeSpan delay, Drip drip)
    {
        _script.Enqueue(async ct =>
        {
            await Task.Delay(delay, ct);
            return drip;
        });
        return this;
    }

    public Task<Drip> NextDripAsync(IntentionSeed seed, CodeCanvas canvas, CancellationToken ct)
    {
        Calls++;
        if (_script.Count == 0)
            throw new InvalidOperationException("FakeLlmClient ran out of scripted drips.");
        return _script.Dequeue()(ct);
    }
}
