namespace InterpreterInCsharp.Object;

public enum ObjectType
{
    Integer,
    Boolean,
    Null,
}
public interface Object
{
    ObjectType Type { get; }
    string Inspect();
}

public record Integer(Int64 Value) : Object
{
    public ObjectType Type => ObjectType.Integer;
    public string Inspect() => Value.ToString("D");
}

public record Boolean(bool Value) : Object
{
    public ObjectType Type => ObjectType.Boolean;
    public string Inspect() => Value.ToString();
}

public record Null : Object
{
    public ObjectType Type => ObjectType.Null;
    public string Inspect() => "null";
}

