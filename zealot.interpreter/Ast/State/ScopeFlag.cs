namespace Zealot.Interpreter.Ast.State;

[Flags]
internal enum ScopeFlag
{
    None = 0,
    Break = 1,
    Continue = 2,
    Return = 4
}