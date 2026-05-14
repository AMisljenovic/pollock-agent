using PollockAgent.Canvas;

namespace PollockAgent.Agent;

record ValidationResult(bool IsValid, string? Reason = null)
{
    public static readonly ValidationResult Ok = new(true);
    public static ValidationResult Fail(string reason) => new(false, reason);
}

class DripValidator(Constraints constraints)
{
    public ValidationResult Validate(Drip drip)
    {
        if (string.IsNullOrWhiteSpace(drip.FilePath))
            return ValidationResult.Fail("Drip has no file path.");

        if (Path.IsPathRooted(drip.FilePath) || drip.FilePath.StartsWith('/') || drip.FilePath.StartsWith('\\'))
            return ValidationResult.Fail($"Drip path \"{drip.FilePath}\" is rooted; must be relative.");

        var segments = drip.FilePath.Split('/', '\\');
        if (segments.Any(s => s == ".."))
            return ValidationResult.Fail($"Drip path \"{drip.FilePath}\" escapes the canvas with \"..\".");

        if (drip.LineCount > constraints.MaxLinesPerDrip)
            return ValidationResult.Fail(
                $"Drip is {drip.LineCount} lines; max is {constraints.MaxLinesPerDrip}.");

        foreach (var phrase in Constraints.ForbiddenPhrases)
        {
            if (drip.Content.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Fail($"Forbidden phrase detected: \"{phrase}\".");
        }

        return ValidationResult.Ok;
    }
}
