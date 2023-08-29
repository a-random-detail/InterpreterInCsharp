using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Evaluator;
using InterpreterInCsharp.Object;
using InterpreterInCsharp.Parser;
using Object = InterpreterInCsharp.Object.Object;

namespace Interpreter.Tests;

[TestFixture]
public class EvaluatorTests
{
    [TestCase("5", 5)]
    [TestCase("123", 123)]
    public void TestEvalIntegerExpression(string input, Int64 expectedValue)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expectedValue);
    }

    private Object TestEval(string input)
    {
        Lexer lexer = new(input);
        Parser parser = new(lexer);
        MonkeyProgram program = parser.ParseProgram();
        return Evaluator.Eval(program);
    }

    private void TestIntegerObject(Object obj, Int64 expected)
    {
        Assert.IsInstanceOf<Integer>(obj);
        var integer = obj as Integer;
        Assert.That(integer.Value, Is.EqualTo(expected));
    }
}