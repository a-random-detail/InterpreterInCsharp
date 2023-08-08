namespace InterpreterInCsharp.Ast;

public record PrefixExpression(Token Token, string Operator, Expression Right) : Expression(Token)
{
    public override string String => $"({Operator}{Right.String})";
}
