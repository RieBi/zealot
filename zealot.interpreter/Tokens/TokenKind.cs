namespace Zealot.Interpreter.Tokens;
internal enum TokenKind
{
    /// <summary>
    /// Represents 'def' token
    /// </summary>
    VariableDefinition,

    /// <summary>
    /// Represents 'define' token
    /// </summary>
    FunctionDefinition,

    /// <summary>
    /// Represents 'repeat' token
    /// </summary>
    RepeatDefinition,

    /// <summary>
    /// Represents 'break' token
    /// </summary>
    BreakStatement,

    /// <summary>
    /// Represents 'continue' token
    /// </summary>
    ContinueStatement,

    /// <summary>
    /// Represents '=>' token
    /// </summary>
    BlockDefinitionOperator,

    /// <summary>
    /// Represents '=' token
    /// </summary>
    AssignmentOperator,

    /// <summary>
    /// Represents a valid integer value without a decimal part, with possible leading zeroes, such as '123', '00048', '1000'
    /// </summary>
    ConstantNumberInteger,

    /// <summary>
    /// Represents a valid number containing a decimal part after a dot, possibly written as a scientific notation, such as: '0.01', '100.3', '50.77e3'
    /// </summary>
    ConstantNumberDouble,

    /// <summary>
    /// Represents 'false' token
    /// </summary>
    ConstantFalse,

    /// <summary>
    /// Represents 'true' token
    /// </summary>
    ConstantTrue,

    /// <summary>
    /// Represents '+' token
    /// </summary>
    AdditionOperator,

    /// <summary>
    /// Represents '+=' token
    /// </summary>
    ShortAdditionOperator,

    /// <summary>
    /// Represents '-' token
    /// </summary>
    SubtractionOperator,

    /// <summary>
    /// Represents '-=' token
    /// </summary>
    ShortSubtractionOperator,

    /// <summary>
    /// Represents '*' token
    /// </summary>
    MultiplicationOperator,

    /// <summary>
    /// Represents '*=' token
    /// </summary>
    ShortMultiplicationOperator,

    /// <summary>
    /// Represents '/' token
    /// </summary>
    DivisionOperator,

    /// <summary>
    /// Represents '/=' token
    /// </summary>
    ShortDivisionOperator,

    /// <summary>
    /// Represents '%' token
    /// </summary>
    ModuloOperator,

    /// <summary>
    /// Represents '%=' token
    /// </summary>
    ShortModuloOperator,

    /// <summary>
    /// Represents '$' token
    /// </summary>
    ExponentiationOperator,

    /// <summary>
    /// Represents '$=' token
    /// </summary>
    ShortExponentiationOperator,

    /// <summary>
    /// Represents '(' token
    /// </summary>
    ParentheseOpen,

    /// <summary>
    /// Represents ')' token
    /// </summary>
    ParentheseClosed,

    /// <summary>
    /// Represents '"' token
    /// </summary>
    QuotationMarks,

    /// <summary>
    /// Represents an arbitraly long string of characters, delimited by " symbol at both ends
    /// </summary>
    String,

    /// <summary>
    /// Represents ',' token
    /// </summary>
    CommaSeparator,

    /// <summary>
    /// Represents '<' token
    /// </summary>
    LessThanOperator,

    /// <summary>
    /// Represents '<=' token
    /// </summary>
    LessThanOrEqualToOperator,

    /// <summary>
    /// Represents '>' token
    /// </summary>
    GreaterThanOperator,

    /// <summary>
    /// Represents '>=' token
    /// </summary>
    GreaterThanOrEqualToOperator,

    /// <summary>
    /// Represents '==' token
    /// </summary>
    EqualOperator,

    /// <summary>
    /// Represents '!=' token
    /// </summary>
    NotEqualOperator,

    /// <summary>
    /// Represents '!' token
    /// </summary>
    LogicalNotOperator,

    /// <summary>
    /// Represents '&&' token
    /// </summary>
    LogicalAndOperator,

    /// <summary>
    /// Represents '^' token
    /// </summary>
    LogicalExclusiveOrOperator,

    /// <summary>
    /// Represents '||' token
    /// </summary>
    LogicalOrOperator,

    /// <summary>
    /// Represents '?' token
    /// </summary>
    QuestionMark,

    /// <summary>
    /// Represents ':' token
    /// </summary>
    Colon,

    /// <summary>
    /// Represents 'if' token
    /// </summary>
    IfStatement,

    /// <summary>
    /// Represents 'elseif' token
    /// </summary>
    ElseIfStatement,

    /// <summary>
    /// Represents 'else' token
    /// </summary>
    ElseStatement,

    /// <summary>
    /// Represents an identifier for a variable or function, consisting of A-Za-z0-9_- symbols. Can only start with a letter or underscore
    /// </summary>
    Identifier,

    /// <summary>
    /// Represents a sequence of one or more tab \t characters
    /// </summary>
    Indentation,

    /// <summary>
    /// Represents end of line. Does not represent \n (new line) symbol
    /// </summary>
    EndOfLine,
}
