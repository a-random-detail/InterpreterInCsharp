using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Evaluator;
using InterpreterInCsharp.Object;
using InterpreterInCsharp.Parser;

namespace Interpreter.Tests;

[TestFixture]
public class EvaluatorTests
{
    [TestCase("5", 5)]
    [TestCase("123", 123)]
    [TestCase("5", 5)]
    [TestCase("10", 10)]
    [TestCase("-5", -5)]
    [TestCase("-10", -10)]
    public void TestEvalIntegerExpression(string input, Int64 expectedValue)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expectedValue);
    }
    
    [TestCase("true", true)]
    [TestCase("false", false)]
    public void TestEvalBooleanExpression(string input, bool expectedValue)
    {
        var evaluated = TestEval(input);
        TestBooleanObject(evaluated, expectedValue);
    }
    
    [Test]
    public void TestNullObject()
    {
        var evaluated = TestEval("null");
        Assert.IsInstanceOf<MonkeyNull>(evaluated);
    }

    [TestCase("!true", false)]
    [TestCase("!false", true)]
    [TestCase("!5", false)]
    [TestCase("!!true", true)]
    [TestCase("!!false", false)]
    [TestCase("!!5", true)]
    public void TestBangOperator(string input, bool expected)
    {
        var evaluated = TestEval(input);
        TestBooleanObject(evaluated, expected);
    }

    private void TestBooleanObject(MonkeyObject evaluated, bool expectedValue)
    {
        Assert.IsInstanceOf<MonkeyBoolean>(evaluated);
        var boolean = evaluated as MonkeyBoolean;
        Assert.That(boolean.Value, Is.EqualTo(expectedValue));
    }

    private MonkeyObject TestEval(string input)
    {
        Lexer lexer = new(input);
        Parser parser = new(lexer);
        MonkeyProgram program = parser.ParseProgram();
        return Evaluator.Eval(program);
    }

    private void TestIntegerObject(MonkeyObject obj, Int64 expected)
    {
        Assert.IsInstanceOf<MonkeyInteger>(obj);
        var integer = obj as MonkeyInteger;
        Assert.That(integer.Value, Is.EqualTo(expected));
    }
}