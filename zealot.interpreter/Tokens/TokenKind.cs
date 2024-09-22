namespace Zealot.Interpreter.Tokens;
internal enum TokenKind
{
    /// <summary>
    /// Represents keyword 'def'
    /// </summary>
    VariableDefinition,

    /// <summary>
    /// Represents keyword 'define'
    /// </summary>
    FunctionDefinition,

    /// <summary>
    /// Represents keyword 'repeat'
    /// </summary>
    RepeatDefinition,

    /// <summary>
    /// Represents keyword '=>'
    /// </summary>
    BlockDefinitionOperator,

    /// <summary>
    /// Represents '=' symbol
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
    /// Represents a keyword 'false'
    /// </summary>
    ConstantFalse,

    /// <summary>
    /// Represents a keyword 'true'
    /// </summary>
    ConstantTrue,

    /// <summary>
    /// Represents '+' symbol
    /// </summary>
    AdditionOperator,

    /// <summary>
    /// Represents '+=' keyword
    /// </summary>
    ShortAdditionOperator,

    /// <summary>
    /// Represents '-' symbol
    /// </summary>
    SubtractionOperator,

    /// <summary>
    /// Represents '-=' keyword
    /// </summary>
    ShortSubtractionOperator,

    /// <summary>
    /// Represents '*' symbol
    /// </summary>
    MultiplicationOperator,

    /// <summary>
    /// Represents '*=' keyword
    /// </summary>
    ShortMultiplicationOperator,

    /// <summary>
    /// Represents '/' symbol
    /// </summary>
    DivisionOperator,

    /// <summary>
    /// Represents '/=' keyword
    /// </summary>
    ShortDivisionOperator,

    /// <summary>
    /// Represents '$' symbol
    /// </summary>
    ExponentiationOperator,

    /// <summary>
    /// Represents '$=' keyword
    /// </summary>
    ShortExponentiationOperator,

    /// <summary>
    /// Represents '(' symbol
    /// </summary>
    ParantheseOpen,

    /// <summary>
    /// Represents ')' symbol
    /// </summary>
    ParantheseClosed,

    /// <summary>
    /// Represents '"' symbol
    /// </summary>
    QuotationMarks,

    /// <summary>
    /// Represents ',' symbol
    /// </summary>
    CommaSeparator,

    /// <summary>
    /// Represents '<' symbol
    /// </summary>
    LessThanOperator,

    /// <summary>
    /// Represents '>' symbol
    /// </summary>
    GreaterThanOperator,

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
