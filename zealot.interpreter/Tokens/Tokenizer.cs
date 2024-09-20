using System.Text.RegularExpressions;

namespace zealot.interpreter.Tokens;
internal static partial class Tokenizer
{
    /// <summary>
    /// Tokenizes the provided line of code and returns a list of tokens.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static List<Token> Tokenize(string input)
    {
        var tokenRegex = TokenRegex();
        var tokens = tokenRegex.Matches(input).Select(m =>
        {
            var val = m.Value;
            TokenKind tokenKind = val switch
            {
                "def" => TokenKind.VariableDefinition,
                "define" => TokenKind.FunctionDefinition,
                "repeat" => TokenKind.RepeatDefinition,
                "=>" => TokenKind.BlockDefinitionOperator,
                "=" => TokenKind.AssignmentOperator,
                "false" => TokenKind.ConstantFalse,
                "true" => TokenKind.ConstantTrue,
                "+" => TokenKind.AdditionOperator,
                "+=" => TokenKind.ShortAdditionOperator,
                "-" => TokenKind.SubtractionOperator,
                "-=" => TokenKind.ShortSubtractionOperator,
                "*" => TokenKind.MultiplicationOperator,
                "*=" => TokenKind.ShortMultiplicationOperator,
                "/" => TokenKind.DivisionOperator,
                "/=" => TokenKind.ShortDivisionOperator,
                "$" => TokenKind.ExponentiationOperator,
                "$=" => TokenKind.ShortExponentiationOperator,
                "(" => TokenKind.ParantheseOpen,
                ")" => TokenKind.ParantheseClosed,
                "\"" => TokenKind.QuotationMarks,
                "," => TokenKind.CommaSeparator,
                "<" => TokenKind.LessThanOperator,
                ">" => TokenKind.GreaterThanOperator,
                _ when m.Name == "integer" => TokenKind.ConstantNumberInteger,
                _ when m.Name == "double" => TokenKind.ConstantNumberDouble,
                _ when m.Name == "identifier" => TokenKind.Identifier,
                _ => throw new InvalidOperationException("Unidentified token inspected.")
            };

            return new Token(tokenKind, val);
        });

        return tokens.ToList();
    }

    [GeneratedRegex(@"def|define|repeat|\+=|-=|\*=|/=|\$=|[+\-*/$=()"",<>]|(?<integer>[0-9]+)|(?<double>[0-9]+(\.[0-9])?(e[0-9]+)?)|[a-zA-Z_][a-zA-Z0-9_\-]*")]
    private static partial Regex TokenRegex();
}
