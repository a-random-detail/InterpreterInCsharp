namespace InterpreterInCsharp.Ast;

public record IndexExpression(Token Token, Expression Left, Expression Index): Expression(Token) 
{
    public override string String => $"({Left.String}[{Index.String}])";
}
