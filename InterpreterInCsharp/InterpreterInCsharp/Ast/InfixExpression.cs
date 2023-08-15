namespace InterpreterInCsharp.Ast;

public record InfixExpression(Token Token, Expression Left, string Operator, Expression Right) : Expression(Token)
{
    public override string String => $"({Left.String} {Operator} {Right.String})";
}