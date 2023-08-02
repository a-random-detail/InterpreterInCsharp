namespace InterpreterInCsharp.Ast;

public record LetStatement(Token Token, Identifier Identifier, Expression Value) : Statement(Token)
{
    public string TokenLiteral => Token.Literal;
    public string StatementNode => Token.Literal;
}