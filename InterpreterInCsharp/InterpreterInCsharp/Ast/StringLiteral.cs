namespace InterpreterInCsharp.Ast;

public record StringLiteral(Token Token, string Value) : Expression(Token) 
{
    public override string String => Value;
}
