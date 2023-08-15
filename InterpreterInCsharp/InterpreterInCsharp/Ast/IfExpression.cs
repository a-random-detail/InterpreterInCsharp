namespace InterpreterInCsharp.Ast;

public record IfExpression(Token Token, Expression Condition, BlockStatement Consequence, BlockStatement? Alternative) : Expression(Token)
{
    public override string String => $"if {Condition.String} {Consequence.String} else {Alternative?.String ?? ""}";
}