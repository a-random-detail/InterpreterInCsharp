namespace InterpreterInCsharp.Ast;

public record Node(Token Token)
{
    public string TokenLiteral => Token.Literal;
}

public record Statement(Token token) : Node(token)
{
    public string TokenLiteral => Token.Literal;
    public string StatementNode => Token.Literal;
}

public record Expression(Token token) : Node(token)
{
    public string TokenLiteral => Token.Literal;
    public string ExpressionNode => Token.Literal;
}
