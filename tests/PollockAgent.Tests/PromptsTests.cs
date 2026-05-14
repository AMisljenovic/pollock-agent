using PollockAgent.Agent;
using PollockAgent.Canvas;
using PollockAgent.Llm;

namespace PollockAgent.Tests;

public class PromptsTests
{
    [Fact]
    public void system_prompt_omits_language_block_when_language_is_null()
    {
        var prompt = Prompts.System(Constraints.Default);

        Assert.DoesNotContain("Target language:", prompt);
    }

    [Fact]
    public void system_prompt_names_the_target_language_when_set()
    {
        var constraints = Constraints.Default with { Language = "python" };

        var prompt = Prompts.System(constraints);

        Assert.Contains("Target language: python", prompt);
        Assert.Contains("python source code", prompt);
    }

    [Fact]
    public void user_prompt_includes_a_target_language_line_when_set()
    {
        var constraints = Constraints.Default with { Language = "rust" };
        var canvas = new CodeCanvas(Path.GetTempPath(), 100);

        var prompt = Prompts.User(new IntentionSeed("a museum of broken machines"), canvas, constraints);

        Assert.Contains("Target language: rust", prompt);
    }

    [Fact]
    public void system_prompt_nudges_toward_many_small_files()
    {
        var prompt = Prompts.System(Constraints.Default);

        Assert.Contains("many small files", prompt);
    }

    [Fact]
    public void user_prompt_omits_target_language_when_unset()
    {
        var canvas = new CodeCanvas(Path.GetTempPath(), 100);

        var prompt = Prompts.User(new IntentionSeed("a tiny city"), canvas, Constraints.Default);

        Assert.DoesNotContain("Target language:", prompt);
    }
}
