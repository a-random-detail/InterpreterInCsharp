using System.Runtime.InteropServices.JavaScript;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    
    private static readonly MonkeyBoolean True = new(true);
    private static readonly MonkeyBoolean False = new(false);
    private static readonly MonkeyNull Null = new();
    public static MonkeyObject Eval(Ast.Node node) => node switch
    {
        PrefixExpression prefixExpression => EvalPrefixExpression(prefixExpression),
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => NativeBoolToBoolean(booleanExpression.Value),
        MonkeyProgram program => EvalStatements(program.Statements),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => Null
    };

    private static MonkeyObject EvalPrefixExpression(PrefixExpression expr)
    {
        var right = Eval(expr.Right);
        switch (expr.Operator)
        {
            case "-":
                return EvalMinusPrefixOperator(right);
            case "!":
                return EvalBangOperatorExpression(right);
            default:
                return Null;
        }
    }

    private static MonkeyObject EvalMinusPrefixOperator(MonkeyObject right)
    {
        if (right.Type != ObjectType.Integer)
        {
            return Null;
        }

        var objInt = right as MonkeyInteger;
        return new MonkeyInteger(-objInt!.Value);

    }

    private static MonkeyObject EvalBangOperatorExpression(MonkeyObject right)
    {
        return right switch
        {
            MonkeyBoolean boolean => boolean.Value ? False : True,
            MonkeyNull _ => True,
            _ => False
        };
    }

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