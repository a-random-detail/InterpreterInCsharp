namespace InterpreterInCsharp.Ast;

public record ExpressionStatement(Token token, Expression expression) : Statement(token)
{
    public string TokenLiteral => token.Literal;
    public string StatementNode => token.Literal;

    public string String()
    {
        return expression != null ? expression.String : "";
    }
}