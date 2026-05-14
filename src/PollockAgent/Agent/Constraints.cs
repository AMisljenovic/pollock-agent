namespace PollockAgent.Agent;

record Constraints(
    int MaxLinesPerDrip = 100,
    TimeSpan StepTimeout = default,
    int TotalLineBudget = 2000,
    string? Language = null)
{
    public static readonly Constraints Default = new(
        MaxLinesPerDrip: 100,
        StepTimeout: TimeSpan.FromSeconds(30),
        TotalLineBudget: 2000,
        Language: null);

    public static readonly string[] ForbiddenPhrases =
    [
        "// TODO",
        "// FIXME",
        "// first I'll",
        "// step 1",
        "// step 2",
        "// step 3",
        "// next,",
        "// then,",
        "// plan:",
        "// roadmap"
    ];
}
