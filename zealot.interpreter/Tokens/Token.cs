﻿namespace zealot.interpreter.Tokens;
internal class Token
{
    public Token() { }

    public Token(TokenKind kind, string value) => (Kind, Value) = (kind, value);

    public required TokenKind Kind { get; set; }
    public required string Value { get; set; }
}
