namespace zealot.interpreter.Ast.Types;
internal class TypeInfo
{
    public string Name { get; set; }
    public object Value { get; set; }

    public TypeInfo(string name, object value) => (Name, Value) = (name, value);
}
