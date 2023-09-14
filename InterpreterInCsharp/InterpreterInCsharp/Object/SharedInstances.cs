namespace InterpreterInCsharp.Object;

public class SharedInstances
{
    public static readonly MonkeyNull Null = new();
    public static readonly MonkeyBoolean True = new(true);
    public static readonly MonkeyBoolean False = new(false);
}
