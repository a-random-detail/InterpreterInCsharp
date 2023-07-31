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
    //Delimiters
    Comma,
    Semicolon,
    Lparen,
    Rparen,
    Lbrace,
    Rbrace,
    //Keywords
    Function,
    Let
}