namespace InterpreterInCsharp.Ast;

public record Node(Token Token)
{
    public string TokenLiteral => Token.Literal;
    public virtual string String => Token.Literal;
}

public record Statement(Token Token) : Node(Token);

public record Expression(Token Token) : Node(Token);
