namespace InterpreterInCsharp.Ast;

public record CallExpression(Token Token, Expression Function, Expression[] Arguments) : Expression(Token)
{
    public override string String => $"{Function.String}({string.Join(", ", Arguments.Select(a => a.String))})";
}