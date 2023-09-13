using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Builtins {
    public static readonly Dictionary<string, MonkeyBuiltin> BuiltinsMap = new() {
        {"len", new MonkeyBuiltin(Len)},
        {"first", new MonkeyBuiltin(First)},
        {"last", new MonkeyBuiltin(Last)},
        {"rest", new MonkeyBuiltin(Rest)},
        {"push", new MonkeyBuiltin(Push)},
    };

    private static MonkeyObject Push(MonkeyObject[] arg)
    {
        if (arg.Length != 2)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=2");
        }

        if (arg[0].Type != ObjectType.Array)
        {
            return new MonkeyError($"argument to `push` must be ARRAY, got {arg[0].Type}");
        }

        var arr = arg[0] as MonkeyArray;
        var newElements = arr?.Elements.Append(arg[1]).ToArray();
        return new MonkeyArray(newElements);
    }

    private static MonkeyObject Rest(MonkeyObject[] arg)
    {
        if (arg.Length != 1)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=1");
        }

        if(arg[0].Type != ObjectType.Array)
        {
            return new MonkeyError($"argument to `rest` must be ARRAY, got {arg[0].Type}");
        }
        var arr = arg[0] as MonkeyArray;
        var length = Len(arg) as MonkeyInteger;
        if (length?.Value > 0)
        {
            var newElements = arr?.Elements.Skip(1).ToArray();
            return new MonkeyArray(newElements);
        }
        return new MonkeyNull();
    }

    private static MonkeyObject Last(MonkeyObject[] arg)
    {
        if (arg.Length != 1)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=1");
        }
        if (arg[0].Type != ObjectType.Array)
        {
            return new MonkeyError($"argument to `last` must be ARRAY, got {arg[0].Type}");
        }
        var arr = arg[0] as MonkeyArray;
        var length = Len(arg) as MonkeyInteger;
        if (length?.Value > 0)
        {
            return arr?.Elements[^1] ?? new MonkeyNull();
        }
        return new MonkeyNull();
    }

    private static MonkeyObject First(MonkeyObject[] arg)
    {
        if (arg.Length != 1)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=1");
        }
        if (arg[0].Type != ObjectType.Array)
        {
            return new MonkeyError($"argument to `first` must be ARRAY, got {arg[0].Type}");
        }
        var arr = arg[0] as MonkeyArray;
        var length = Len(arg) as MonkeyInteger;
        if (length?.Value > 0)
        {
            return arr?.Elements[0] ?? new MonkeyNull();
        }
        return new MonkeyNull();
    }

    private static MonkeyObject Len(MonkeyObject[] arg)
    {
        if (arg.Length != 1)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=1");
        }

        var arr = arg[0] as MonkeyArray;
        return arg[0].Type switch
        {
            ObjectType.Array => new MonkeyInteger((arg[0] as MonkeyArray)?.Elements.Count() ?? 0),
            ObjectType.String => new MonkeyInteger(arg[0].Inspect().Length),
            _ => new MonkeyError($"argument to `len` not supported, got {arg[0].Type}")
        };
    }
}
