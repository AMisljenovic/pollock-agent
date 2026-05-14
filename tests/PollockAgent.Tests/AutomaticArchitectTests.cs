using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Tests;

public class AutomaticArchitectTests : IDisposable
{
    readonly string _root;

    public AutomaticArchitectTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "pollock-arch-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    AutomaticArchitect Build(FakeLlmClient llm, Constraints constraints, out CodeCanvas canvas)
    {
        canvas = new CodeCanvas(_root, constraints.TotalLineBudget);
        return new AutomaticArchitect(llm, canvas, new DripValidator(constraints), constraints, TextWriter.Null);
    }

    [Fact]
    public async Task stops_when_a_drip_signals_feels_done()
    {
        var llm = new FakeLlmClient().Script(
            new Drip("a.txt", "one\n"),
            new Drip("a.txt", "two\n", FeelsDone: true));
        var architect = Build(llm, Constraints.Default, out var canvas);

        await architect.RunAsync(new IntentionSeed("seed"), CancellationToken.None);

        Assert.Equal(2, llm.Calls);
        Assert.Equal("one\ntwo\n", File.ReadAllText(Path.Combine(_root, "a.txt")));
    }

    [Fact]
    public async Task stops_once_the_total_line_budget_is_exhausted()
    {
        var constraints = Constraints.Default with { TotalLineBudget = 4 };
        var llm = new FakeLlmClient().Script(
            new Drip("a.txt", "one\ntwo\n"),
            new Drip("a.txt", "three\n"));
        var architect = Build(llm, constraints, out var canvas);

        await architect.RunAsync(new IntentionSeed("seed"), CancellationToken.None);

        Assert.Equal(2, llm.Calls);
        Assert.True(canvas.BudgetExhausted);
    }

    [Fact]
    public async Task skips_invalid_drips_and_keeps_dripping()
    {
        var llm = new FakeLlmClient().Script(
            new Drip("../escape.cs", "x\n"),
            new Drip("a.txt", "ok\n", FeelsDone: true));
        var architect = Build(llm, Constraints.Default, out _);

        await architect.RunAsync(new IntentionSeed("seed"), CancellationToken.None);

        Assert.Equal(2, llm.Calls);
        Assert.False(File.Exists(Path.Combine(_root, "..", "escape.cs")));
        Assert.True(File.Exists(Path.Combine(_root, "a.txt")));
    }

    [Fact]
    public async Task a_drip_that_overruns_the_step_timeout_is_skipped()
    {
        var constraints = Constraints.Default with { StepTimeout = TimeSpan.FromMilliseconds(50) };
        var llm = new FakeLlmClient()
            .ScriptDelay(TimeSpan.FromMilliseconds(500), new Drip("late.txt", "x\n"))
            .Script(new Drip("ok.txt", "y\n", FeelsDone: true));
        var architect = Build(llm, constraints, out _);

        await architect.RunAsync(new IntentionSeed("seed"), CancellationToken.None);

        Assert.False(File.Exists(Path.Combine(_root, "late.txt")));
        Assert.True(File.Exists(Path.Combine(_root, "ok.txt")));
    }
}
