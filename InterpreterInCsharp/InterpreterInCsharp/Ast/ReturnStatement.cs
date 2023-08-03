namespace InterpreterInCsharp.Ast;

public record ReturnStatement(Token Token, Expression? Value) : Statement(Token)
{
    public string TokenLiteral => Token.Literal;
    public string StatementNode => Token.Literal;
}