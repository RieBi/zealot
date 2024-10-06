using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.State;
internal static class BuiltinFunctions
{
    public readonly static List<(string name, List<string> parameters, Func<IList<TypeInfo>, TypeInfo> function)> Functions =
    [
        ("print", ["value"], Print),
        ("printn", ["value"], PrintN),
        ("read", [], Read),
    ];

    public static TypeInfo Print(IList<TypeInfo> arguments)
    {
        Console.Write(arguments[0].Value);
        return new("empty", new());
    }

    public static TypeInfo PrintN(IList<TypeInfo> arguments)
    {
        Console.WriteLine(arguments[0].Value);
        return new("empty", new());
    }

    public static TypeInfo Read(IList<TypeInfo> _)
    {
        var value = Console.ReadLine() ?? throw new InvalidOperationException("Input stream is closed.");
        return new("string", value);
    }
}
