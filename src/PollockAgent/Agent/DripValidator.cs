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
