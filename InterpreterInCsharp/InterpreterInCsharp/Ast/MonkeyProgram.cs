namespace InterpreterInCsharp.Ast;

public record MonkeyProgram(List<Statement> Statements)
{
    public string TokenLiteral() => Statements.Count > 0 ? Statements[0].TokenLiteral : "";
    public string String => string.Join("", Statements.Select(x => x.String));
}
