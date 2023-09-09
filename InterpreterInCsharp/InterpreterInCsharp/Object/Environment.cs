namespace InterpreterInCsharp.Object;

public class MonkeyEnvironment {
    private readonly Dictionary<string, MonkeyObject> _store = new();
    private MonkeyEnvironment _outer = null;

    public static MonkeyEnvironment NewEnvironment() => new();

    public static MonkeyEnvironment NewEnclosedEnvironment(MonkeyEnvironment outer) => new() {_outer = outer};

    public MonkeyObject Get(string name)
    {
        if (_store.TryGetValue(name, out var value))
        {
            return value;
        }

        if (_outer != null)
        {
            return _outer.Get(name);
        }

        return null;
    }

    public MonkeyObject Set(string name, MonkeyObject value)
    {
        _store.Add(name, value);
        return value;
    }
}
