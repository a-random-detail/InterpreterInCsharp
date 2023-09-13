using InterpreterInCsharp.Object;

namespace Interpreter.Tests;

[TestFixture]
public class ObjectTests
{
    [Test]
    public void TestStringHashKey()
    {
        var hello1 = new MonkeyString("Hello World");
        var hello2 = new MonkeyString("Hello World");
        var diff1 = new MonkeyString("My name is johnny");
        var diff2 = new MonkeyString("My name is johnny");
        Assert.AreEqual(hello1.HashKey(), hello2.HashKey());
        Assert.AreEqual(diff1.HashKey(), diff2.HashKey());
        Assert.AreNotEqual(hello1.HashKey(), diff1.HashKey());
    }
}

