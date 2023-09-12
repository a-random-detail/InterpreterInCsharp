namespace InterpreterInCsharp.Ast;

public record ArrayLiteral(Token Token, Expression[] Elements): Expression(Token) 
{
    public override string String => $"[{string.Join(", ", Elements.Select(e => e.String))}]";
}
