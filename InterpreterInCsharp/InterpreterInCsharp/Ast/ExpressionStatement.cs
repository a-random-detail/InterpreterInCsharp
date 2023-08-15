namespace InterpreterInCsharp.Ast;

public record ExpressionStatement(Token Token, Expression Expression) : Statement(Token)
{
    public override string String => Expression.String;
}
