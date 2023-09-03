using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;
using System.Collections.Generic;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    
    private static readonly MonkeyBoolean True = new(true);
    private static readonly MonkeyBoolean False = new(false);
    private static readonly MonkeyNull Null = new();
    public static MonkeyObject Eval(Ast.Node node) => node switch
    {
        BlockStatement blockStatement => EvalStatements(blockStatement.Statements.ToList()),
        IfExpression ifExpression => EvalIfExpression(ifExpression),
        InfixExpression infixExpression => EvalInfixExpression(infixExpression),
        PrefixExpression prefixExpression => EvalPrefixExpression(prefixExpression),
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => NativeBoolToBoolean(booleanExpression.Value),
        MonkeyProgram program => EvalStatements(program.Statements),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => Null
    };

    private static MonkeyObject EvalIfExpression(IfExpression expr){
        var condition = Eval(expr.Condition);

        if (IsTruthy(condition))
        {
            return Eval(expr.Consequence);
        }
        else if (expr.Alternative != null)
        {
            return Eval(expr.Alternative);
        }
        else
        {
            return Null;
        }
    }

    private static bool IsTruthy(MonkeyObject obj)
    {
        return obj switch
        {
            MonkeyBoolean boolean => boolean.Value ? true : false,
            MonkeyNull _ => false,  
            _ => true 
        };
    }

    private static MonkeyObject EvalInfixExpression(InfixExpression expr)
    {
        var left = Eval(expr.Left);
        var right = Eval(expr.Right);

        if (left.Type == ObjectType.Integer && right.Type == ObjectType.Integer)
        {
            return EvalIntegerInfixExpression(expr.Operator, left, right);
        }

        return expr.Operator switch
        {
            "==" => NativeBoolToBoolean(left == right),
            "!=" => NativeBoolToBoolean(left != right),
            _ => Null,
        };
    }

    private static MonkeyObject EvalIntegerInfixExpression(string exprOperator, MonkeyObject left, MonkeyObject right)
    {
        MonkeyInteger? l = left as MonkeyInteger;
        MonkeyInteger? r = right as MonkeyInteger;

        if (l == null || r == null)
        {
            return Null;
        }

        return exprOperator switch
        {
            "*" => new MonkeyInteger(l.Value * r.Value),
            "/" => new MonkeyInteger(l.Value / r.Value),
            "+" => new MonkeyInteger(l.Value + r.Value),
            "-" => new MonkeyInteger(l.Value - r.Value),
            "<" => NativeBoolToBoolean(l.Value < r.Value),
            ">" => NativeBoolToBoolean(l.Value > r.Value),
            "==" => NativeBoolToBoolean(l.Value == r.Value),
            "!=" => NativeBoolToBoolean(l.Value != r.Value),
            _ => Null
        };
    }

    private static MonkeyObject EvalPrefixExpression(PrefixExpression expr)
    {
        var right = Eval(expr.Right);
        return expr.Operator switch
        {
            "-" => EvalMinusPrefixOperator(right),
            "!" => EvalBangOperatorExpression(right),
            _ => Null
        };
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
