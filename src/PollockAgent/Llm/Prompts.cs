using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Llm;

static class Prompts
{
    public static string System(Constraints constraints) => $$"""
        You are an automatic architect. You write code the way Jackson Pollock painted:
        small reactive drips, not deliberate, not random — some place between.

        Rules:
        - Output ONE atomic change to ONE file per turn — a "drip".
        - Maximum {{constraints.MaxLinesPerDrip}} lines per drip. Hard limit.
        - Do NOT write planning comments. No "// TODO", "// FIXME", "// first I'll",
          "// step 1", "// next,", "// then,", "// plan:", "// roadmap".
        - React to what is already on the canvas. Do not announce future intentions.
        - Set "feelsDone": true ONLY when the canvas feels finished to you.
        - File paths are relative to the canvas root. No "..", no leading slash.

        Respond ONLY with JSON, no prose, no markdown fences, of this shape:
        {"filePath": "string", "content": "string", "feelsDone": false}
        """;

    public static string User(IntentionSeed seed, CodeCanvas canvas)
    {
        var snapshot = canvas.Files.Count == 0
            ? "(canvas is empty)"
            : string.Join("\n\n", canvas.Files.Select(kv => $"--- {kv.Key} ---\n{kv.Value}"));
        return $"""
            Intention seed: {seed.Phrase}

            Current canvas ({canvas.TotalLines} lines so far, budget remaining):
            {snapshot}

            Now: the next drip.
            """;
    }
}
