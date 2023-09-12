namespace InterpreterInCsharp;

public enum TokenType
{
    Illegal,
    Eof,
    //Identifiers + literals
    Ident,
    Int,
    //Operators
    Assign,
    Plus,
    Minus,
    Slash,
    Star,
    //Delimiters
    Comma,
    Semicolon,
    Lparen,
    Rparen,
    Lbrace,
    Rbrace,
    LBracket,
    RBracket,
    //Keywords
    Function,
    Let,
    If,
    Else,
    Return,
    True,
    False,
    //Comparison
    Bang,
    LessThan,
    GreaterThan,
    IsEqual,
    NotEqual,
    //Placeholder for program
    Program,
    String
}
