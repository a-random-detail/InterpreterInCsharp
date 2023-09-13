namespace InterpreterInCsharp.Ast;

public record HashLiteral(Token Token, Dictionary<Expression, Expression> Pairs) : Expression(Token) 
{
    public override string String => $"{{{string.Join(", ", Pairs.Select(pair => $"{pair.Key.String}: {pair.Value.String}"))}}}";
}
