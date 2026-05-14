using PollockAgent.Agent;
using PollockAgent.Canvas;
using PollockAgent.Config;
using PollockAgent.Llm;

DotEnv.Load(".env");

var seed = IntentionSeed.Parse(args);

var budget = int.TryParse(Environment.GetEnvironmentVariable("POLLOCK_LINE_BUDGET"), out var b)
    ? b
    : Constraints.Default.TotalLineBudget;
var constraints = Constraints.Default with { TotalLineBudget = budget };

var canvasDir = Environment.GetEnvironmentVariable("POLLOCK_CANVAS_DIR");
if (string.IsNullOrWhiteSpace(canvasDir)) canvasDir = "./canvas";

var canvas = new CodeCanvas(canvasDir, constraints.TotalLineBudget);
var validator = new DripValidator(constraints);

using var http = new HttpClient();
var llm = LlmClientFactory.Create(constraints, http);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var provider = Environment.GetEnvironmentVariable("LLM_PROVIDER") ?? "anthropic";
Console.WriteLine($"seed: {seed.Phrase}");
Console.WriteLine($"canvas: {Path.GetFullPath(canvasDir)}");
Console.WriteLine($"provider: {provider}");
Console.WriteLine($"budget: {constraints.TotalLineBudget} lines, {constraints.MaxLinesPerDrip}/drip, {constraints.StepTimeout.TotalSeconds:0}s step timeout");
Console.WriteLine();

var architect = new AutomaticArchitect(llm, canvas, validator, constraints, Console.Out);
await architect.RunAsync(seed, cts.Token);
