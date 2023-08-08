namespace InterpreterInCsharp.Ast;

public record ReturnStatement(Token Token, Expression? Value) : Statement(Token)
{
    public override string String => $"{TokenLiteral} {Value?.String};";
}
