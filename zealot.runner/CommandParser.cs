namespace zealot.runner;
internal class CommandParser
{
    /// <summary>
    /// Parses command-line arguments and returns an object containing information about them.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Invalid filepath or argument.</exception>
    public CommandDetails ParseInput(string[] args)
    {
        if (args.Length == 0 || args.Length > 2)
            throw new InvalidOperationException($"Excepted number of arguments: 1-2. Got: {args.Length}");

        string? fileName = null;
        bool interactiveMode = false;

        var invalidCharsSet = Path.GetInvalidFileNameChars().ToHashSet();
        if (!args[0].Any(f => invalidCharsSet.Contains(f)))
            fileName = args[0];
        else if (args[0] != "-i")
            throw new InvalidOperationException($"Unknown argument: {args[0]}");

        var optionInd = fileName is null ? 0 : 1;
        if (optionInd < args.Length)
        {
            if (args[optionInd] == "-i")
                interactiveMode = true;
            else
                throw new InvalidOperationException($"Unknown argument: {args[1]}");
        }

        return new(fileName, interactiveMode);
    }
}
