using System.Runtime.InteropServices.JavaScript;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    public static Object.MonkeyObject Eval(Ast.Node node) => node switch
    {
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => new MonkeyBoolean(booleanExpression.Value),
        MonkeyProgram program => EvalStatements(program.Statements),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => throw new ArgumentOutOfRangeException(nameof(node))
    };

    private static Object.MonkeyObject EvalStatements(List<Statement> stmts)
    {
        Object.MonkeyObject result = null;
        foreach (var stmt in stmts)
        {
            result = Eval(stmt);
        }

        return result;
    }
}