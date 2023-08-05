namespace InterpreterInCsharp.Ast;

public record Identifier(Token Token, string Value) : Expression(Token)
{
    public string String => Value;
}