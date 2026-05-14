using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Tests;

public class DripValidatorTests
{
    static readonly Constraints Small = Constraints.Default with { MaxLinesPerDrip = 5 };
    static readonly DripValidator Validator = new(Small);

    [Fact]
    public void accepts_a_drip_within_the_line_cap()
    {
        var drip = new Drip("src/Foo.cs", "one\ntwo\nthree\n");
        Assert.True(Validator.Validate(drip).IsValid);
    }

    [Fact]
    public void rejects_a_drip_over_the_line_cap()
    {
        var content = string.Join("\n", Enumerable.Range(0, 10).Select(i => $"line {i}"));
        var drip = new Drip("src/Foo.cs", content);
        var result = Validator.Validate(drip);

        Assert.False(result.IsValid);
        Assert.Contains("max is", result.Reason);
    }

    [Theory]
    [InlineData("// TODO finish later")]
    [InlineData("// FIXME broken")]
    [InlineData("// first I'll add scaffolding")]
    [InlineData("// step 1: define the model")]
    [InlineData("// plan: build the thing")]
    public void rejects_each_planning_phrase(string content)
    {
        var result = Validator.Validate(new Drip("src/Foo.cs", content));
        Assert.False(result.IsValid);
        Assert.Contains("Forbidden phrase", result.Reason);
    }

    [Fact]
    public void matches_forbidden_phrases_case_insensitively()
    {
        var result = Validator.Validate(new Drip("src/Foo.cs", "// todo capitalised differently"));
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("../escape.cs")]
    [InlineData("a/../../etc/passwd")]
    [InlineData("/etc/passwd")]
    [InlineData("\\windows\\system32")]
    public void rejects_paths_that_escape_or_root(string path)
    {
        var result = Validator.Validate(new Drip(path, "ok\n"));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void accepts_a_normal_nested_relative_path()
    {
        var result = Validator.Validate(new Drip("src/PollockAgent/Foo.cs", "x\n"));
        Assert.True(result.IsValid);
    }
}
