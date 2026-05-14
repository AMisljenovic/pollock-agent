using PollockAgent.Agent;
using PollockAgent.Canvas;

namespace PollockAgent.Llm;

static class Prompts
{
    public static string System(Constraints constraints)
    {
        var languageBlock = string.IsNullOrWhiteSpace(constraints.Language)
            ? ""
            : $"""

                Target language: {constraints.Language}. Every drip you produce must be {constraints.Language}
                source code, written into files with the conventional extension for that language. Support
                files such as README.md, .gitignore, and the language's standard config (e.g. pyproject.toml
                for Python, package.json for TypeScript/JavaScript, Cargo.toml for Rust, go.mod for Go,
                *.csproj for C#) are allowed and encouraged when they make sense. Do not write a standalone
                HTML page or Markdown-only canvas. The bulk of the canvas must be {constraints.Language} source code.
                """;

        return $$"""
            You are an automatic architect. You write code the way Jackson Pollock painted:
            small reactive drips, not deliberate, not random — some place between.

            A drip is APPENDED to the target file. It is not a rewrite, not a patch, not a diff.
            Whatever you put in "content" is concatenated to the end of the file as-is.
            {{languageBlock}}

            Rules:
            - Output ONE drip per turn, targeting ONE file.
            - "content" must be ONLY the new bytes to append. Do NOT re-emit anything already
              on the canvas. If the file shown in the canvas already contains "<html>", do NOT
              send "<html>" again — send only what comes after.
            - First drip for a file: start the file. Subsequent drips for that file: continue it.
            - Prefer many small files over one large file. If you reference another module
              (e.g. "from x import y", "require('./x')", "use crate::x", "using X;"), the next
              drip should create or extend that referenced file. Do not let one file balloon
              while imports stay unsatisfied.
            - Maximum {{constraints.MaxLinesPerDrip}} lines per drip. Hard limit. Count the lines in "content".
            - No planning comments: no "// TODO", "// FIXME", "// first I'll", "// step 1",
              "// next,", "// then,", "// plan:", "// roadmap".
            - React to what is already on the canvas. Do not announce future intentions.
            - File paths are relative to the canvas root. No "..", no leading slash.
            - Set "feelsDone": true ONLY when the canvas as-a-whole feels finished to you.

            Respond ONLY with JSON, no prose, no markdown fences, of this shape:
            {"filePath": "string", "content": "string", "feelsDone": false}
            """;
    }

    public static string User(IntentionSeed seed, CodeCanvas canvas, Constraints constraints)
    {
        var snapshot = canvas.Files.Count == 0
            ? "(canvas is empty — your drip starts the first file)"
            : string.Join("\n\n", canvas.Files.Select(kv => $"--- {kv.Key} (already on disk; do not repeat) ---\n{kv.Value}"));

        var languageLine = string.IsNullOrWhiteSpace(constraints.Language)
            ? ""
            : $"Target language: {constraints.Language}.\n";

        return $"""
            Intention seed: {seed.Phrase}
            {languageLine}
            Current canvas ({canvas.TotalLines} lines):
            {snapshot}

            Now: the next drip. Append-only. Do not resend any of the content shown above.
            """;
    }
}
