namespace InterpreterInCsharp.Ast;

public record BooleanExpression(Token Token, bool Value) : Expression(Token);