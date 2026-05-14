namespace PollockAgent.Agent;

record Constraints(
    int MaxLinesPerDrip = 12,
    TimeSpan StepTimeout = default,
    int TotalLineBudget = 2000)
{
    public static readonly Constraints Default = new(
        MaxLinesPerDrip: 12,
        StepTimeout: TimeSpan.FromSeconds(30),
        TotalLineBudget: 2000);

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
