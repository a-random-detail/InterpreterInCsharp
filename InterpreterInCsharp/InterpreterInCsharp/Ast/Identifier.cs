namespace InterpreterInCsharp.Ast;

public record Identifier(Token Token, string Value) : Expression(Token)
{
    public string TokenLiteral => Token.Literal;
    public string ExpressionNode => Token.Literal;
}