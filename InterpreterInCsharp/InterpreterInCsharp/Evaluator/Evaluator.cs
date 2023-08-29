using System.Runtime.InteropServices.JavaScript;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    
    private static MonkeyBoolean True = new(true);
    private static MonkeyBoolean False = new(false);
    private static MonkeyNull Null = new();
    public static MonkeyObject Eval(Ast.Node node) => node switch
    {
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => NativeBoolToBoolean(booleanExpression.Value),
        MonkeyProgram program => EvalStatements(program.Statements),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => Null
    };

    private static MonkeyObject NativeBoolToBoolean(bool value)
    {
        return value ? True : False;
    }   

    private static MonkeyObject EvalStatements(List<Statement> stmts)
    {
        MonkeyObject result = null;
        foreach (var stmt in stmts)
        {
            result = Eval(stmt);
        }

        return result;
    }
}