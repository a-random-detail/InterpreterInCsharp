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
    [TestCase("5 + 5 + 5 + 5 - 10", 10)]
    [TestCase("2*2*2*2*2", 32)]
    [TestCase("-50 + 100 + -50", 0)]
    [TestCase("5*2+10", 20)]
    [TestCase("5+2*10", 25)]
    [TestCase("20 + 2 * -10", 0)]
    [TestCase("50 / 2 * 2 + 10", 60)]
    [TestCase("2 * (5 + 10)", 30)]
    [TestCase("3 * 3 * 3 + 10", 37)]
    [TestCase("3 * (3 * 3) + 10", 37)]
    [TestCase("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50)]
    
    public void TestEvalIntegerExpression(string input, Int64 expectedValue)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expectedValue);
    }
    
    [TestCase("true", true)]
    [TestCase("false", false)]
    [TestCase("1 < 2", true)]
    [TestCase("1 > 2", false)]
    [TestCase("1 < 1", false)]
    [TestCase("1 > 1", false)]
    [TestCase("1 == 1", true)]
    [TestCase("1 != 1", false)]
    [TestCase("1 == 2", false)]
    [TestCase("1 != 2", true)]
    [TestCase("(1 < 2) == true", true)]
    [TestCase("(1 < 2) == false", false)]
    [TestCase("(1 > 2) == true", false)]
    [TestCase("(1 > 2) == false", true)]
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

    [TestCase("if (true) { 10 }", 10)]
    [TestCase("if (false) { 10 }", null)]
    [TestCase("if (1) { 10 }", 10)]
    [TestCase("if (1 < 2) { 10 }", 10)]
    [TestCase("if (1 > 2) { 10 }", null)]
    [TestCase("if (1 > 2) { 10 } else { 20 }", 20)]
    [TestCase("if (1 < 2) { 10 } else { 20 }", 10)]
    public void TestIfElseExpressions(string input, Int64? expected){
        var evaluated = TestEval(input);
        if (expected == null)
            TestNullObject(evaluated);
        else
            TestIntegerObject(evaluated, (Int64)expected);
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

    private void TestNullObject(MonkeyObject obj)
    {
        Assert.IsInstanceOf<MonkeyNull>(obj);
    }
}
