namespace InterpreterInCsharp.Object;

public enum ObjectType
{
    Integer,
    Boolean,
    Null,
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

