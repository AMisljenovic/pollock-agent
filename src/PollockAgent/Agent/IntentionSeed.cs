namespace PollockAgent.Agent;

record IntentionSeed(string Phrase)
{
    public static IntentionSeed Parse(string[] args)
    {
        var joined = args.Length > 0 ? string.Join(' ', args).Trim() : string.Empty;
        return new IntentionSeed(joined.Length > 0 ? joined : "a platform that grows");
    }
}
