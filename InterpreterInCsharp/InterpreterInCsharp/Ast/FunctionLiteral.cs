namespace InterpreterInCsharp.Ast;

public record FunctionLiteral(Token Token, Identifier[] Parameters, BlockStatement Body) : Expression(Token)
{
    public override string String => $"{TokenLiteral}({string.Join(", ", Parameters.Select(p => p.String))}) {Body.String}";
}