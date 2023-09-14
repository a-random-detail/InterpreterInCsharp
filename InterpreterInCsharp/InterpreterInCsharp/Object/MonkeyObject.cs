using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Object;

public enum ObjectType
{
    Integer,
    Boolean,
    Null,
    ReturnValue,
    Error,
    Function,
    String,
    Builtin,
    Array,
    Hash,
}
public interface MonkeyObject
{
    ObjectType Type { get; }
    string Inspect();
}

public record MonkeyInteger(Int64 Value) : MonkeyObject, MonkeyHashable
{
    public ObjectType Type => ObjectType.Integer;
    public string Inspect() => Value.ToString("D");
    public MonkeyHashKey HashKey() => new(ObjectType.Integer, Value);
}

public record MonkeyBoolean(bool Value) : MonkeyObject, MonkeyHashable
{
    public static readonly MonkeyBoolean True = SharedInstances.True;
    public static readonly MonkeyBoolean False = SharedInstances.False;
    public ObjectType Type => ObjectType.Boolean;
    public string Inspect() => Value.ToString();
    public MonkeyHashKey HashKey() => new(ObjectType.Boolean, Value ? 1 : 0);
}

public record MonkeyString(string Value) : MonkeyObject, MonkeyHashable
{
    public ObjectType Type => ObjectType.String;
    public string Inspect() => Value;
    public MonkeyHashKey HashKey() => new(ObjectType.String, Value.GetHashCode());
}

public record MonkeyNull : MonkeyObject
{
    public static readonly MonkeyNull Instance = SharedInstances.Null;
    public ObjectType Type => ObjectType.Null;
    public string Inspect() => "null";
}

public record MonkeyReturnValue(MonkeyObject Value) : MonkeyObject
{
    public ObjectType Type => ObjectType.ReturnValue;
    public string Inspect() => Value.Inspect();
}

public record MonkeyError(string Message) : MonkeyObject
{
    public ObjectType Type => ObjectType.Error;
    public string Inspect() => $"ERROR: {Message}";
}

public record MonkeyFunction(Identifier[] Parameters, BlockStatement Body, MonkeyEnvironment Env) : MonkeyObject
{
    public ObjectType Type => ObjectType.Function;
    public string Inspect() => $"fn({string.Join(", ", Parameters.Select(p => p.String))}) {{\n{Body.String}\n}}";
}

public record MonkeyBuiltin(Func<MonkeyObject[], MonkeyObject> Fn) : MonkeyObject
{
    public ObjectType Type => ObjectType.Builtin;
    public string Inspect() => "builtin function";
}

public record MonkeyArray(MonkeyObject[] Elements) : MonkeyObject
{
    public ObjectType Type => ObjectType.Array;
    public string Inspect() => $"[{string.Join(", ", Elements.Select(e => e.Inspect()))}]";
}

public record MonkeyHashKey(ObjectType Type, Int64 Value){}

public record MonkeyHashPair(MonkeyObject Key, MonkeyObject Value){}

public record MonkeyHash(Dictionary<MonkeyHashKey, MonkeyHashPair> Pairs) : MonkeyObject
{
    public ObjectType Type => ObjectType.Hash;
    public string Inspect() => $"{{{string.Join(", ", Pairs.Select(p => $"{p.Value.Key.Inspect()}: {p.Value.Value.Inspect()}"))}}}";
}
