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

    [TestCase("return 10;", 10)]
    [TestCase("return 10; 9;", 10)]
    [TestCase("return 2 * 5; 9;", 10)]
    [TestCase("9; return 2 * 5; 9;", 10)]
    [TestCase(@"
if (10 > 1) {
    if (10 > 1) {
        return 10;
    }
    return 1;
}", 10)]
    public void TestReturnStatements(string input, Int64 expected) {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [TestCase("5 + true;", "type mismatch: Integer + Boolean")]
    [TestCase("5 + true; 5;", "type mismatch: Integer + Boolean")]
    [TestCase("-true;", "unknown operator: -Boolean")]
    [TestCase("true + false;", "unknown operator: Boolean + Boolean")]
    [TestCase("5; true + false; 5;", "unknown operator: Boolean + Boolean")]
    [TestCase("if (10 > 1) { true + false; }", "unknown operator: Boolean + Boolean")]
    [TestCase(@"
if (10 > 1) {
    if (10 > 1) {
        return true + false;
    }
    return 1;
}", "unknown operator: Boolean + Boolean")]
    [TestCase("foobar", "identifier not found: foobar")]
    public void TestErrorHandling(string input, string expectedMessage) {
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyError>(evaluated);
        var err = evaluated as MonkeyError;
        Assert.That(err.Message, Is.EqualTo(expectedMessage));
    }
    
    [Test]
    public void TestFunctionObject()
    {
        var input = "fn(x) { x + 2; };";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyFunction>(evaluated);
        var fn = evaluated as MonkeyFunction;
        Assert.That(fn.Parameters.Count, Is.EqualTo(1));
        Assert.That(fn.Parameters[0].String, Is.EqualTo("x"));
        Assert.That(fn.Body.String, Is.EqualTo("(x + 2)"));
    }

    [TestCase("let identity = fn(x) { x; }; identity(5);", 5)]
    [TestCase("let identity = fn(x) { return x; }; identity(5);", 5)]
    [TestCase("let double = fn(x) { x * 2; }; double(5);", 10)]
    [TestCase("let add = fn(x, y) { x + y; }; add(5, 5);", 10)]
    [TestCase("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20)]
    [TestCase("fn(x) { x; }(5)", 5)]    
    public void TestFunctionApplication(string input, Int64 expectedValue)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expectedValue);
    }

    [Test]
    public void TestClosures() 
    {
       var input = @"
let newAdder = fn(x) {
    fn(y) { x + y };
};
let addTwo = newAdder(2);
addTwo(2);";
        
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, 4);
    }

    [Test]
    public void TestStringLiteral() 
    {
        var input = @"""Hello World!""";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyString>(evaluated);
        var str = evaluated as MonkeyString;
        Assert.That(str.Value, Is.EqualTo("Hello World!"));
    }

    [Test]
    public void TestStringConcatenation() 
    {
        var input = @"""Hello"" + "" "" + ""World!""";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyString>(evaluated);
        var str = evaluated as MonkeyString;
        Assert.That(str.Value, Is.EqualTo("Hello World!"));
    }

    [TestCase(@"""Hello"" == ""Hello""", true)]
    [TestCase(@"""Hello"" != ""Hello""", false)]
    [TestCase(@"""Hello"" == ""World""", false)]
    [TestCase(@"""Hello"" != ""World""", true)]
    public void TestStringComparison(string input, bool expected)
    {
        var evaluated = TestEval(input);
        if (evaluated.GetType() != typeof(MonkeyBoolean)){
            var error = evaluated as MonkeyError;
            Console.WriteLine(error.Message);
        }
        TestBooleanObject(evaluated, expected); 
    }

    [TestCase("-")]
    [TestCase("*")]
    [TestCase("/")]
    public void TestStringErrorHandling(string @operator) 
    {
        var input = $"\"Hello\" {@operator} \"World!\"";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyError>(evaluated);
        var err = evaluated as MonkeyError;
        Assert.That(err.Message, Is.EqualTo($"unknown operator: String {@operator} String"));
    }

    [TestCase("len(\"\")", 0)]
    [TestCase("len(\"four\")", 4)]
    [TestCase("len(\"hello world\")", 11)]
    [TestCase("len(1)", "argument to `len` not supported, got Integer")]
    [TestCase("len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1")]
    public void TestStringLengthBuiltinFunction(string input, object expected)
    {
        var evaluated = TestEval(input);
        switch (expected){
            case string:
                Assert.IsInstanceOf<MonkeyError>(evaluated);
                var err = evaluated as MonkeyError;
                Assert.That(err.Message, Is.EqualTo(expected));
                break;
            case int:
                Assert.IsInstanceOf<MonkeyInteger>(evaluated);
                var integer = evaluated as MonkeyInteger;
                Assert.That(integer.Value, Is.EqualTo(expected));
                break;
            default:
                throw new Exception("unexpected type");
        }
    }
   
    [Test]
    public void TestArrayLiterals() 
    {
        var input = "[1, 2 * 2, 3 + 3]";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyArray>(evaluated);
        var result = evaluated as MonkeyArray;
        Assert.That(result.Elements.Count, Is.EqualTo(3));
        TestIntegerObject(result.Elements[0], 1);
        TestIntegerObject(result.Elements[1], 4);
        TestIntegerObject(result.Elements[2], 6);
    }

    [TestCase("[1,2,3][0]", 1)]
    [TestCase("[1,2,3][1]", 2)]
    [TestCase("[1,2,3][2]", 3)]
    [TestCase("let i = 0; [1][i];", 1)]
    [TestCase("[1,2,3][1 + 1];", 3)]
    [TestCase("let myArray = [1, 2, 3]; myArray[2];", 3)]
    [TestCase("let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2];", 6)]
    [TestCase("let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]", 2)]
    public void TestArrayIndexExpressions(string input, Int64 expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [Test]
    public void TestHashLiterals()
    {
        var input = @"
let two = ""two"";
{
    ""one"": 10 - 9,
    two: 1 + 1,
    ""thr"" + ""ee"": 6 / 2,
    4:4,
    true: 5,
    false:6
};
";
        var evaluated = TestEval(input);       
        Assert.IsInstanceOf<MonkeyHash>(evaluated);
        var result = evaluated as MonkeyHash;
        Assert.That(result.Pairs.Count, Is.EqualTo(6));
        var expected = new Dictionary<MonkeyHashKey, Int64>
        {
            {new MonkeyHashKey(ObjectType.String, "one".GetHashCode()), 1},
            {new MonkeyHashKey(ObjectType.String, "two".GetHashCode()), 2},
            {new MonkeyHashKey(ObjectType.String, "three".GetHashCode()), 3},
            {new MonkeyHashKey(ObjectType.Integer, 4), 4},
            {new MonkeyHashKey(ObjectType.Boolean, 1), 5},
            {new MonkeyHashKey(ObjectType.Boolean, 0), 6}
        };

        foreach (var (key, value) in expected)
        {
            var actualValue = result.Pairs[key].Value;
            TestIntegerObject(actualValue, value);
        }
    }

    [TestCase("{\"foo\":5}[\"foo\"]", 5)]
    [TestCase("{\"foo\":5}[\"bar\"]", null)]
    [TestCase("let key = \"foo\"; {\"foo\":5}[key]", 5)]
    [TestCase("{}[\"foo\"]", null)]
    [TestCase("{5:5}[5]", 5)]
    [TestCase("{true:5}[true]", 5)]
    [TestCase("{false:5}[false]", 5)]
    public void TestHashIndexExpressions(string input, Int64? expected)
    {
        var evaluated = TestEval(input);
        if (expected == null)
            TestNullObject(evaluated);
        else
            TestIntegerObject(evaluated, (Int64)expected);
    }

    [Test]
    public void TestErrorHandling() 
    {
        var input = "{\"foo\":\"bar\"}[fn(x) { x }];";
        var evaluated = TestEval(input);
        Assert.IsInstanceOf<MonkeyError>(evaluated);
        var err = evaluated as MonkeyError;
        Assert.That(err.Message, Is.EqualTo("unusable as hash key: Function"));
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
        MonkeyEnvironment env = MonkeyEnvironment.NewEnvironment();
        return Evaluator.Eval(program, env);
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
