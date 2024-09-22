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
                _ when m.HasNonEmptyGroup("indentation") => TokenKind.Indentation,
                _ when m.HasNonEmptyGroup("integer") => TokenKind.ConstantNumberInteger,
                _ when m.HasNonEmptyGroup(groupName: "double") => TokenKind.ConstantNumberDouble,
                _ when m.HasNonEmptyGroup("identifier") => TokenKind.Identifier,
                _ => throw new InvalidOperationException("Unidentified token inspected.")
            };

            return new Token(tokenKind, val);
        });

        return tokens.ToList();
    }

    public static bool HasNonEmptyGroup(this Match match, string groupName) => match.Groups.TryGetValue(groupName, out var group) && group.Length > 0;

    [GeneratedRegex(@"def|define|repeat|\+=|-=|\*=|/=|\$=|[+\-*/$=()"",<>]|(?<integer>[0-9]+)|(?<double>[0-9]+(\.[0-9])?(e[0-9]+)?)|[a-zA-Z_][a-zA-Z0-9_\-]*")]
    private static partial Regex TokenRegex();
}
