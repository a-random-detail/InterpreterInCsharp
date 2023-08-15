namespace InterpreterInCsharp.Ast;

public record BlockStatement(Token Token, Statement[] Statements) : Statement(Token)
{
    public override string String => string.Join(" ", Statements.Select(s => s.String));
}