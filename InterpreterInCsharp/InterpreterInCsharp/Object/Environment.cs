namespace InterpreterInCsharp.Object;

public class MonkeyEnvironment {
    private readonly Dictionary<string, MonkeyObject> _store = new();
    private MonkeyEnvironment _outer = null;

    public static MonkeyEnvironment NewEnvironment() => new();

    public static MonkeyEnvironment NewEnclosedEnvironment(MonkeyEnvironment outer) => new() {_outer = outer};

    public bool TryGet(string name, out MonkeyObject result)
    {
        if (_store.TryGetValue(name, out result))
        {
            return true;
        }

        if (_outer != null)
        {
            return _outer.TryGet(name, out result);
        }

        return false;
    }

    public MonkeyObject Set(string name, MonkeyObject value)
    {
        _store.Add(name, value);
        return value;
    }
}
