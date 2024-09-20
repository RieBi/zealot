namespace zealot.interpreter.Tokens;
internal class Token
{
    public Token(TokenKind kind, string value) => (Kind, Value) = (kind, value);

    public TokenKind Kind { get; set; }
    public string Value { get; set; }
}
