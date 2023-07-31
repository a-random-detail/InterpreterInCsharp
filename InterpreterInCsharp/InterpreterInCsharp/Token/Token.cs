namespace InterpreterInCsharp;

public record Token(TokenType Type, string Literal)
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "fn", TokenType.Function },
        { "let", TokenType.Let },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "return", TokenType.Return },
        { "true", TokenType.True },
        { "false", TokenType.False }
    };
    
    public Token(TokenType type, char literal) : this(type, literal.ToString())
    {
    }

    public static TokenType LookupIdent(string ident)
    {
        return Keywords.TryGetValue(ident, out var keyword) ? keyword : TokenType.Ident;
    }
    
}
