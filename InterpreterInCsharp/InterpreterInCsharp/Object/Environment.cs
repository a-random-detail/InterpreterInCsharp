namespace InterpreterInCsharp.Object;

public class MonkeyEnvironment {
    private readonly Dictionary<string, MonkeyObject> _store = new();

    public static MonkeyEnvironment NewEnvironment() => new();

    public MonkeyObject Get(string name)
    {
        if (_store.TryGetValue(name, out var value))
        {
            return value;
        }

        return null;
    }

    public MonkeyObject Set(string name, MonkeyObject value)
    {
        _store.Add(name, value);
        return value;
    }
}
