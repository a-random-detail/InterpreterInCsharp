namespace InterpreterInCsharp.Ast;

public record Identifier(Token Token, string Value) : Expression(Token)
{
    public override string String => Value;
}
