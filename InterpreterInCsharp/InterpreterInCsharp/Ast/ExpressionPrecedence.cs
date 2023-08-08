namespace InterpreterInCsharp.Ast;

public enum ExpressionPrecedence
{
    Lowest,
    Comparison, // == LESSGREATER // > or <
    Sum, // +
    Product, // *
    Prefix, // -X or !X
    Call // myFunction(X)
}
