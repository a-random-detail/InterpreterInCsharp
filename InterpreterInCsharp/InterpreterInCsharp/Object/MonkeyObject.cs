namespace InterpreterInCsharp.Object;

public enum ObjectType
{
    Integer,
    Boolean,
    Null,
    ReturnValue
}
public interface MonkeyObject
{
    ObjectType Type { get; }
    string Inspect();
}

public record MonkeyInteger(Int64 Value) : MonkeyObject
{
    public ObjectType Type => ObjectType.Integer;
    public string Inspect() => Value.ToString("D");
}

public record MonkeyBoolean(bool Value) : MonkeyObject
{
    public ObjectType Type => ObjectType.Boolean;
    public string Inspect() => Value.ToString();
}

public record MonkeyNull : MonkeyObject
{
    public ObjectType Type => ObjectType.Null;
    public string Inspect() => "null";
}

public record MonkeyReturnValue(MonkeyObject Value) : MonkeyObject
{
    public ObjectType Type => ObjectType.ReturnValue;
    public string Inspect() => Value.Inspect();
}

