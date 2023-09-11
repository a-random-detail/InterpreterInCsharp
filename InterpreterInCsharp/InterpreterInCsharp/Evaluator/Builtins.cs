using InterpreterInCsharp.Object;

namespace InterpreterInCsharp.Evaluator;

public class Builtins {
    public static readonly Dictionary<string, MonkeyBuiltin> BuiltinsMap = new() {
        {"len", new MonkeyBuiltin(Len)}
    };

    private static MonkeyObject Len(MonkeyObject[] arg)
    {
        if (arg.Length != 1)
        {
            return new MonkeyError($"wrong number of arguments. got={arg.Length}, want=1");
        }

        if (arg[0].Type == ObjectType.String)
        {
            return new MonkeyInteger(arg[0].Inspect().Length);
        }

        return new MonkeyError($"argument to `len` not supported, got {arg[0].Type}");
    }
}
