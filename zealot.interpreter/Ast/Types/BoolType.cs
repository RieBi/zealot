namespace Zealot.Interpreter.Ast.Types;
internal readonly struct BoolType(bool value)
{
    public static readonly BoolType True = (BoolType)true;

    public static readonly BoolType False = (BoolType)false;

    public readonly bool Value = value;

    public override readonly string ToString() => Value ? "true" : "false";

    public BoolType Negate() => Value ? False : True;

    public static explicit operator bool(BoolType boolType) => boolType.Value;

    public static explicit operator BoolType(bool val) => new(val);
}
