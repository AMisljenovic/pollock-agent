using PollockAgent.Agent;

namespace PollockAgent.Tests;

public class IntentionSeedTests
{
    [Fact]
    public void empty_args_falls_back_to_the_default_phrase()
    {
        var seed = IntentionSeed.Parse([]);
        Assert.Equal("a platform that grows", seed.Phrase);
    }

    [Fact]
    public void multiple_args_are_joined_with_spaces()
    {
        var seed = IntentionSeed.Parse(["a", "tiny", "city"]);
        Assert.Equal("a tiny city", seed.Phrase);
    }

    [Fact]
    public void whitespace_only_args_fall_back_to_the_default()
    {
        var seed = IntentionSeed.Parse(["   "]);
        Assert.Equal("a platform that grows", seed.Phrase);
    }
}
