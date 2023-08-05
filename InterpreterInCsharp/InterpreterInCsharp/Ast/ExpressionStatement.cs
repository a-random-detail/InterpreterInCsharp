namespace InterpreterInCsharp.Ast;

public record ExpressionStatement(Token Token, Expression Expression) : Statement(Token)
{
    public string String => Expression.String;
    
}