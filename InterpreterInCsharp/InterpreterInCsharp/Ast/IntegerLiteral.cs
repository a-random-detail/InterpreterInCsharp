namespace InterpreterInCsharp.Ast;

public record IntegerLiteral(Token Token, Int64 Value) : Expression(Token);
