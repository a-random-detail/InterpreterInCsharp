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
        MonkeyProgram program => EvalProgram(program),
        ReturnStatement returnStatement => HandleReturnStatement(returnStatement), 
        BlockStatement blockStatement => EvalBlockStatement(blockStatement.Statements.ToList()),
        IfExpression ifExpression => EvalIfExpression(ifExpression),
        InfixExpression infixExpression => HandleInfixExpression(infixExpression), 
        PrefixExpression prefixExpression => HandlePrefixExpression(prefixExpression), 
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => NativeBoolToBoolean(booleanExpression.Value),
        ExpressionStatement expr => Eval(expr.Expression),
        _ => Null
    };

    private static MonkeyObject HandleInfixExpression(InfixExpression expr) {
        var left = Eval(expr.Left);
        if (IsError(left))
        {
            return left;
        }
        var right = Eval(expr.Right);
        if (IsError(right))
        {
            return right;
        }

        return EvalInfixExpression(expr);
    }

    private static MonkeyObject HandlePrefixExpression(PrefixExpression expr) {
        var right = Eval(expr.Right);
        if (IsError(right))
        {
            return right;
        }
        
        return EvalPrefixExpression(expr);
    }
    
    private static MonkeyObject HandleReturnStatement(ReturnStatement stmt)
    {
        var val = Eval(stmt.Value);
        if (val.Type == ObjectType.Error)
        {
            return val;
        }
        return new MonkeyReturnValue(val);
    }

    private static MonkeyObject NewError(string format, params string[] args)
    {
        return new MonkeyError(string.Format(format, args));
    }

    private static MonkeyObject EvalBlockStatement(List<Statement> statements)
    {
        MonkeyObject result = Null;
        foreach (var stmt in statements)
        {
            result = Eval(stmt);
            if (result != null) {
                if (result.Type == ObjectType.ReturnValue || result.Type == ObjectType.Error)
                {
                    return result;
                }
            }
        }

        return result;
    }

    private static MonkeyObject EvalIfExpression(IfExpression expr){
        var condition = Eval(expr.Condition);
        if (IsError(condition))
        {
            return condition;
        }

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

    private static bool IsError(MonkeyObject obj)
    {
        return obj != null && obj.Type == ObjectType.Error;
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

        if (left.Type != right.Type) {
            return NewError("type mismatch: {0} {1} {2}", left.Type.ToString(), expr.Operator, right.Type.ToString());
        }
        
        return expr.Operator switch
        {
            "==" => NativeBoolToBoolean(left == right),
            "!=" => NativeBoolToBoolean(left != right),
            _ => NewError("unknown operator: {0} {1} {2}", left.Type.ToString(), expr.Operator, right.Type.ToString())
        };
    }

    private static MonkeyObject EvalIntegerInfixExpression(string exprOperator, MonkeyObject left, MonkeyObject right)
    {
        MonkeyInteger? l = left as MonkeyInteger;
        MonkeyInteger? r = right as MonkeyInteger;

        if (l == null || r == null)
        {
            return NewError("type mismatch: {0} {1} {2}", left.Type.ToString(), exprOperator, right.Type.ToString());
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
            _ => NewError("unknown operator: {0} {1} {2}", left.Type.ToString(), exprOperator, right.Type.ToString()) 
        };
    }

    private static MonkeyObject EvalPrefixExpression(PrefixExpression expr)
    {
        var right = Eval(expr.Right);
        return expr.Operator switch
        {
            "-" => EvalMinusPrefixOperator(right),
            "!" => EvalBangOperatorExpression(right),
            _ => NewError("unknown operator: {0}{1}", expr.Operator, right.Type.ToString())
        };
    }

    private static MonkeyObject EvalMinusPrefixOperator(MonkeyObject right)
    {
        if (right.Type != ObjectType.Integer)
        {
            return NewError("unknown operator: -{0}", right.Type.ToString()); 
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

    private static MonkeyObject EvalProgram(MonkeyProgram program)
    {
        MonkeyObject result = null;
        foreach (var stmt in program.Statements)
        {
            result = Eval(stmt);
            switch (result.Type)
            {
                case ObjectType.ReturnValue:
                    return (result as MonkeyReturnValue)!.Value;
                case ObjectType.Error:
                    return result;
            }
        }

        return result;
    }
}
