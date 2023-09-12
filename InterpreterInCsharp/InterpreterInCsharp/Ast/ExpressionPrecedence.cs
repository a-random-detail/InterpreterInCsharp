namespace InterpreterInCsharp.Ast;

public enum ExpressionPrecedence
{
    Lowest,
    Equal, // ==
    LessGreater,// > or <
    Sum, // +
    Product, // *
    Prefix, // -X or !X
    Call, // myFunction(X)
    Index // array[index]
}
