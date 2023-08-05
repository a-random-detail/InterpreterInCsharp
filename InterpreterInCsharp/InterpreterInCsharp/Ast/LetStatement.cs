namespace InterpreterInCsharp.Ast;

public record LetStatement(Token Token, Identifier Identifier, Expression? Value) : Statement(Token)
{ 
    public override string String => $"{TokenLiteral} {Identifier.String} = {Value?.String};";
}
