using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    public static Object.Object Eval(Ast.Node node) => node switch
    {
        IntegerLiteral integerLiteral => new Integer(integerLiteral.Value),
        MonkeyProgram program => EvalStatements(program.Statements),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => throw new ArgumentOutOfRangeException(nameof(node))
    };

    private static Object.Object EvalStatements(List<Statement> stmts)
    {
        Object.Object result = null;
        foreach (var stmt in stmts)
        {
            result = Eval(stmt);
        }

        return result;
    }
}