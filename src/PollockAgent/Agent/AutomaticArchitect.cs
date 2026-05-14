using PollockAgent.Canvas;
using PollockAgent.Llm;

namespace PollockAgent.Agent;

class AutomaticArchitect(
    ILlmClient llm,
    CodeCanvas canvas,
    DripValidator validator,
    Constraints constraints,
    TextWriter log)
{
    public async Task RunAsync(IntentionSeed seed, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (canvas.BudgetExhausted)
            {
                await log.WriteLineAsync($"budget exhausted at {canvas.TotalLines} lines; stopping.");
                return;
            }

            using var step = CancellationTokenSource.CreateLinkedTokenSource(ct);
            step.CancelAfter(constraints.StepTimeout);

            Drip drip;
            try
            {
                drip = await llm.NextDripAsync(seed, canvas, step.Token);
            }
            catch (OperationCanceledException) when (step.IsCancellationRequested && !ct.IsCancellationRequested)
            {
                await log.WriteLineAsync("step timed out; dripping on.");
                continue;
            }
            catch (Exception ex)
            {
                await log.WriteLineAsync($"llm error: {ex.Message}; dripping on.");
                continue;
            }

            var result = validator.Validate(drip);
            if (!result.IsValid)
            {
                await log.WriteLineAsync($"rejected: {result.Reason}");
                continue;
            }

            canvas.Apply(drip);
            await log.WriteLineAsync($"{drip.FilePath} +{drip.LineCount}");

            if (drip.FeelsDone)
            {
                await log.WriteLineAsync("feels done; stopping.");
                return;
            }
        }
    }
}
