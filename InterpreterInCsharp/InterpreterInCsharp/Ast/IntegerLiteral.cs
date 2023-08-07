namespace InterpreterInCsharp.Ast;

public record IntegerLiteral(Token Token, Int64 Value) : Expression(Token)
{
    public string String => Token.Literal;
    public string TokenLiteral => Token.Literal;
}