using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Evaluator
{
    
    private static readonly MonkeyBoolean True = new(true);
    private static readonly MonkeyBoolean False = new(false);
    private static readonly MonkeyNull Null = new();

    public static MonkeyObject Eval(Ast.Node node, MonkeyEnvironment environment) => node switch
    {
        
        StringLiteral stringLiteral => new MonkeyString(stringLiteral.Value),
        CallExpression callExpression => EvalCallExpression(callExpression, environment),
        FunctionLiteral functionLiteral => new MonkeyFunction(functionLiteral.Parameters, functionLiteral.Body, environment),
        Identifier identifier => EvalIdentifier(identifier, environment),
        LetStatement letStatement => EvalLetStatement(letStatement, environment),
        MonkeyProgram program => EvalProgram(program, environment),
        ReturnStatement returnStatement => HandleReturnStatement(returnStatement, environment),
        BlockStatement blockStatement => EvalBlockStatement(blockStatement.Statements.ToList(), environment),
        IfExpression ifExpression => EvalIfExpression(ifExpression, environment),
        InfixExpression infixExpression => HandleInfixExpression(infixExpression, environment), 
        PrefixExpression prefixExpression => HandlePrefixExpression(prefixExpression, environment), 
        IntegerLiteral integerLiteral => new MonkeyInteger(integerLiteral.Value),
        BooleanExpression booleanExpression => NativeBoolToBoolean(booleanExpression.Value),
        ExpressionStatement expr => Eval(expr.Expression, environment),
        _ => Null
    };

    private static MonkeyObject HandleInfixExpression(InfixExpression expr, MonkeyEnvironment env) 
    {
        var left = Eval(expr.Left, env);
        if (IsError(left))
        {
            return left;
        }
        var right = Eval(expr.Right, env);
        if (IsError(right))
        {
            return right;
        }

        return EvalInfixExpression(expr, env);
    }

    private static MonkeyObject HandlePrefixExpression(PrefixExpression expr, MonkeyEnvironment env) 
    {
        var right = Eval(expr.Right, env);
        if (IsError(right))
        {
            return right;
        }
        
        return EvalPrefixExpression(expr, env);
    }
    
    private static MonkeyObject HandleReturnStatement(ReturnStatement stmt, MonkeyEnvironment env) 
    {
        var val = Eval(stmt.Value, env);
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
    
    private static MonkeyObject EvalCallExpression(CallExpression expr, MonkeyEnvironment env) 
    {
        var func = Eval(expr.Function, env);
        if (IsError(func))
        {
            return func;
        }
        var args = EvalExpressions(expr.Arguments, env);
        if (args.Count == 1 && IsError(args[0]))
        {
            return args[0];
        }

        return ApplyFunction(func, args);
    }

    private static MonkeyObject ApplyFunction(MonkeyObject func, List<MonkeyObject> args)
    {
        var fn = func as MonkeyFunction;
        if (fn == null)
        {
            return NewError("not a function: {0}", func.Type.ToString());
        }
        var extendedEnv = ExtendFunctionEnvironment(fn, args);
        var evaluated = Eval(fn.Body, extendedEnv);
        return UnwrapReturnValue(evaluated);
    }

    private static MonkeyEnvironment ExtendFunctionEnvironment(MonkeyFunction fn, List<MonkeyObject> args)
    {
        var env = MonkeyEnvironment.NewEnclosedEnvironment(fn.Env);
        for (int i = 0; i < fn.Parameters.Count(); i++)
        {
            env.Set(fn.Parameters[i].Value, args[i]);
        }

        return env;
    }

    private static MonkeyObject UnwrapReturnValue(MonkeyObject obj) => obj switch
    {
        MonkeyReturnValue returnValue => returnValue.Value,
        _ => obj
    };

    private static List<MonkeyObject> EvalExpressions(Expression[] args, MonkeyEnvironment env)
    {
        var result = new List<MonkeyObject>();
        foreach (var arg in args)
        {
            var evaluated = Eval(arg, env);
            if (IsError(evaluated))
            {
                return new List<MonkeyObject> { evaluated };
            }
            result.Add(evaluated);
        }

        return result;
    } 

    private static MonkeyObject EvalIdentifier(Identifier node, MonkeyEnvironment env)
    {
        var ok = env.TryGet(node.Value, out MonkeyObject val);
        if (ok)
        {
            return val;
        }

        return NewError("identifier not found: {0}", node.Value);
    }

    private static MonkeyObject EvalLetStatement(LetStatement stmt, MonkeyEnvironment env)
    {
        var val = Eval(stmt.Value, env);
        if (val.Type == ObjectType.Error)
        {
            return val;
        }
        env.Set(stmt.Identifier.Value, val);
        return val;
    }

    private static MonkeyObject EvalBlockStatement(List<Statement> statements, MonkeyEnvironment env)
    {
        MonkeyObject result = Null;
        foreach (var stmt in statements)
        {
            result = Eval(stmt, env);
            if (result != null) 
            {
                if (result.Type == ObjectType.ReturnValue || result.Type == ObjectType.Error)
                {
                    return result;
                }
            }
        }

        return result;
    }

    private static MonkeyObject EvalIfExpression(IfExpression expr, MonkeyEnvironment env){
        var condition = Eval(expr.Condition, env);
        if (IsError(condition))
        {
            return condition;
        }

        if (IsTruthy(condition))
        {
            return Eval(expr.Consequence, env);
        }
        else if (expr.Alternative != null)
        {
            return Eval(expr.Alternative, env);
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

    private static MonkeyObject EvalInfixExpression(InfixExpression expr, MonkeyEnvironment env)
    {
        var left = Eval(expr.Left, env);
        var right = Eval(expr.Right, env);
        if (left.Type == ObjectType.Integer && right.Type == ObjectType.Integer)
        {
            return EvalIntegerInfixExpression(expr.Operator, left, right);
        }

        if (left.Type == ObjectType.String && right.Type == ObjectType.String)
        {
            return EvalStringInfixExpression(expr.Operator, left, right);
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

    private static MonkeyObject EvalStringInfixExpression(string @operator, MonkeyObject left, MonkeyObject right)
    {
       MonkeyString? l = left as MonkeyString; 
       MonkeyString? r = right as MonkeyString;

       if (l == null || r == null)
       {
           return NewError("type mismatch: {0} {1} {2}", left.Type.ToString(), @operator, right.Type.ToString());
       }

       return @operator switch
       {
           "+" => new MonkeyString(l.Value + r.Value),
           _ => NewError("unknown operator: {0} {1} {2}", left.Type.ToString(), @operator, right.Type.ToString())
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

    private static MonkeyObject EvalPrefixExpression(PrefixExpression expr, MonkeyEnvironment env)
    {
        var right = Eval(expr.Right, env);
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

    private static MonkeyObject EvalProgram(MonkeyProgram program, MonkeyEnvironment env)
    {
        MonkeyObject result = null;
        foreach (var stmt in program.Statements)
        {
            result = Eval(stmt, env);
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
