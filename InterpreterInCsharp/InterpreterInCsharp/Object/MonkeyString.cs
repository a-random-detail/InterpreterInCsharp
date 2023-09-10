namespace InterpreterInCsharp.Object;

public record MonkeyString(string Value) : MonkeyObject 
{
    public ObjectType Type => ObjectType.String;
    public string Inspect() => Value;
}
