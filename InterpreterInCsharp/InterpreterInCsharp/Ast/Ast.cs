namespace InterpreterInCsharp.Ast;

public record Node(Token Token)
{
    public string TokenLiteral => Token.Literal;
    public virtual string String => Token.Literal;
}

public record Statement(Token token) : Node(token);

public record Expression(Token token) : Node(token);
