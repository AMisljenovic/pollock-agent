using PollockAgent.Canvas;

namespace PollockAgent.Tests;

public class CodeCanvasTests : IDisposable
{
    readonly string _root;

    public CodeCanvasTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "pollock-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void applying_a_drip_writes_the_file_to_disk()
    {
        var canvas = new CodeCanvas(_root, 100);
        canvas.Apply(new Drip("hello.txt", "hi\n"));

        Assert.Equal("hi\n", File.ReadAllText(Path.Combine(_root, "hello.txt")));
    }

    [Fact]
    public void two_drips_to_the_same_file_concatenate_on_disk()
    {
        var canvas = new CodeCanvas(_root, 100);
        canvas.Apply(new Drip("note.txt", "first\n"));
        canvas.Apply(new Drip("note.txt", "second\n"));

        Assert.Equal("first\nsecond\n", File.ReadAllText(Path.Combine(_root, "note.txt")));
    }

    [Fact]
    public void parent_directories_are_created_automatically()
    {
        var canvas = new CodeCanvas(_root, 100);
        canvas.Apply(new Drip("a/b/c/file.cs", "x\n"));

        Assert.True(File.Exists(Path.Combine(_root, "a", "b", "c", "file.cs")));
    }

    [Fact]
    public void total_lines_sums_across_drips()
    {
        var canvas = new CodeCanvas(_root, 100);
        canvas.Apply(new Drip("a.txt", "one\ntwo\n"));
        canvas.Apply(new Drip("b.txt", "three\n"));

        Assert.Equal(3, canvas.TotalLines);
    }

    [Fact]
    public void budget_trips_once_total_lines_reaches_the_limit()
    {
        var canvas = new CodeCanvas(_root, totalLineBudget: 3);
        canvas.Apply(new Drip("a.txt", "x"));
        Assert.False(canvas.BudgetExhausted);

        canvas.Apply(new Drip("a.txt", "y\nz\nw\n"));
        Assert.True(canvas.BudgetExhausted);
    }
}
