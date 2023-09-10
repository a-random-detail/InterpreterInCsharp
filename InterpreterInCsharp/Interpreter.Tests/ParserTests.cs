using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Parser;

namespace Interpreter.Tests;

[TestFixture]
public class ParserTests
{
    [TestCase("let x = 5;", "x", 5)]
    [TestCase("let y = true;", "y", true)]
    [TestCase("let foobar = y;", "foobar", "y")]
    public void TestLetStatements<T>(string input, string expectedIdentifier, T expectedValue)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        TestHelpers.CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        TestHelpers.TestLetStatement(statement, expectedIdentifier);
        var letStatement = statement as LetStatement;
        TestHelpers.TestLiteralExpression(letStatement.Value, expectedValue);
    }

    [Test]
    public void TestLetStatementParseErrors()
    {
        var input = @"let x 5;
let = 10;
let 838383;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        parser.ParseProgram();
        Assert.That(parser.Errors.Count, Is.EqualTo(3));
    }

    [TestCase("return 5;", 5)]
    [TestCase("return true;", true)]
    [TestCase("return foobar;", "foobar")]
    public void TestReturnStatements<T>(string input, T expectedValue)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.That(statement.TokenLiteral, Is.EqualTo("return"));
        Assert.IsInstanceOf<ReturnStatement>(statement);
        var returnStatement = statement as ReturnStatement;
        TestHelpers.TestLiteralExpression(returnStatement.Value, expectedValue);
    }

    [Test]
    public void TestIdentifierExpression()
    {
        var input = "foobar;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        TestHelpers.TestIdentifier(expressionStatement.Expression, "foobar");
    }

    [Test]
    public void TestIntegerLiteralExpressions()
    {
        var input = @"5;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        TestHelpers.TestIntegerLiteral(expressionStatement.Expression, 5);
    }

    [TestCase("!5;", "!", 5)]
    [TestCase("-15;", "-", 15)]
    [TestCase("!true", "!", true)]
    [TestCase("!false", "!", false)]
    public void TestParsingPrefixExpressions<T>(string input, string op, T value)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        Assert.NotNull(program);
        TestHelpers.CheckParserErrors(parser);
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        Assert.IsInstanceOf<ExpressionStatement>(program.Statements[0]);
        var stmt = program.Statements[0] as ExpressionStatement;
        Assert.IsInstanceOf<PrefixExpression>(stmt.Expression);
        var expr = stmt.Expression as PrefixExpression;
        Assert.That(expr.Operator, Is.EqualTo(op));
        TestHelpers.TestLiteralExpression(expr.Right, value);
    }

    [TestCase("5 + 5;", 5, "+", 5)]
    [TestCase("5 - 5;", 5, "-", 5)]
    [TestCase("5 * 5;", 5, "*", 5)]
    [TestCase("5 / 5;", 5, "/", 5)]
    [TestCase("5 > 5;", 5, ">", 5)]
    [TestCase("5 < 5;", 5, "<", 5)]
    [TestCase("5 == 5;", 5, "==", 5)]
    [TestCase("5 != 5;", 5, "!=", 5)]
    [TestCase("true == true", true, "==", true)]
    [TestCase("true != false", true, "!=", false)]
    [TestCase("false == false", false, "==", false)]
    public void TestParsingInfixExpressions<L, R>(string input, L left, string operation, R right)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        Assert.NotNull(program);
        TestHelpers.CheckParserErrors(parser);
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements.First();
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expr = statement as ExpressionStatement;
        TestHelpers.TestInfixExpression(expr.Expression, left, operation, right);
    }

    [TestCase("-a * b", "((-a) * b)")]
    [TestCase("!-a", "(!(-a))")]
    [TestCase("a + b + c", "((a + b) + c)")]
    [TestCase("a + b - c", "((a + b) - c)")]
    [TestCase("a * b * c", "((a * b) * c)")]
    [TestCase("a * b / c", "((a * b) / c)")]
    [TestCase("a + b / c", "(a + (b / c))")]
    [TestCase("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)")]
    [TestCase("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)")]
    [TestCase("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))")]
    [TestCase("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))")]
    [TestCase("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))")]
    [TestCase("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)")]
    [TestCase("(5 + 5) * 2", "((5 + 5) * 2)")]
    [TestCase("2 / (5 + 5)", "(2 / (5 + 5))")]
    [TestCase("-(5 + 5)", "(-(5 + 5))")]
    [TestCase("!(true == true)", "(!(true == true))")]
    [TestCase("a + add(b * c) + d", "((a + add((b * c))) + d)")]
    [TestCase("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))")]
    [TestCase("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))")]
    public void TestOperatorPrecedenceParsing(string input, string expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        TestHelpers.CheckParserErrors(parser);
        
        Assert.That(program.String, Is.EqualTo(expected));
    }
    
    [TestCase("true;", true)]
    [TestCase("false;", false)]
    public void TestBooleanExpression(string input, bool expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        TestHelpers.TestBooleanLiteral(expressionStatement.Expression, expected);
    }
    
    [TestCase("true", "true")]
    [TestCase("false", "false")]
    [TestCase("3 > 5 == false", "((3 > 5) == false)")]
    [TestCase("3 < 5 == true", "((3 < 5) == true)")]
    public void TestBooleanOperatorPrecedence(string input, string expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.That(program.String, Is.EqualTo(expected));
    }

    [Test]
    public void TestIfExpression()
    {
        var input = "if (x < y) { x }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<IfExpression>(expressionStatement.Expression);
        var ifExpression = expressionStatement.Expression as IfExpression;
        TestHelpers.TestInfixExpression(ifExpression.Condition, "x", "<", "y");
        Assert.That(ifExpression.Consequence.Statements.Count, Is.EqualTo(1));
        var consequence = ifExpression.Consequence.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(consequence);
        var consequenceExpression = consequence as ExpressionStatement;
        TestHelpers.TestIdentifier(consequenceExpression.Expression, "x");
        Assert.IsNull(ifExpression.Alternative);
    }
    
    [Test]
    public void TestIfElseExpression()
    {
        var input = "if (x < y) { x } else { y }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<IfExpression>(expressionStatement.Expression);
        var ifExpression = expressionStatement.Expression as IfExpression;
        TestHelpers.TestInfixExpression(ifExpression.Condition, "x", "<", "y");
        Assert.That(ifExpression.Consequence.Statements.Count, Is.EqualTo(1));
        var consequence = ifExpression.Consequence.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(consequence);
        var consequenceExpression = consequence as ExpressionStatement;
        TestHelpers.TestIdentifier(consequenceExpression.Expression, "x");
        Assert.That(ifExpression.Alternative.Statements.Count, Is.EqualTo(1));
        var alternative = ifExpression.Alternative.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(alternative);
        var alternativeExpression = alternative as ExpressionStatement;
        TestHelpers.TestIdentifier(alternativeExpression.Expression, "y");
    }

    [Test]
    public void TestFunctionLiteralParsing()
    {
        var input = "fn(x, y) { x + y; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<FunctionLiteral>(expressionStatement.Expression);
        var functionLiteral = expressionStatement.Expression as FunctionLiteral;
        Assert.That(functionLiteral.Parameters.Count, Is.EqualTo(2));
        TestHelpers.TestLiteralExpression(functionLiteral.Parameters[0], "x");
        TestHelpers.TestLiteralExpression(functionLiteral.Parameters[1], "y");
        Assert.That(functionLiteral.Body.Statements.Count, Is.EqualTo(1));
        var bodyStatement = functionLiteral.Body.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(bodyStatement);
        var bodyExpressionStatement = bodyStatement as ExpressionStatement;
        TestHelpers.TestInfixExpression(bodyExpressionStatement.Expression, "x", "+", "y");
    }
    
    [TestCase("fn() {};", new string[] {})]
    [TestCase("fn(x) {};", new string[] {"x"})]
    [TestCase("fn(x, y, z) {};", new string[] {"x", "y", "z"})]
    public void TestFunctionParameterParsing(string input, string[] expectedParams)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<FunctionLiteral>(expressionStatement.Expression);
        var functionLiteral = expressionStatement.Expression as FunctionLiteral;
        Assert.That(functionLiteral.Parameters.Count, Is.EqualTo(expectedParams.Length));
        for (var i = 0; i < expectedParams.Length; i++)
        {
            TestHelpers.TestLiteralExpression(functionLiteral.Parameters[i], expectedParams[i]);
        }
    }

    [Test]
    public void TestCallExpressionParsing()
    {
        var input = "add(1, 2 * 3, 4 + 5);";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<CallExpression>(expressionStatement.Expression);
        var callExpression = expressionStatement.Expression as CallExpression;
        TestHelpers.TestIdentifier(callExpression.Function, "add");
        Assert.That(callExpression.Arguments.Count, Is.EqualTo(3));
        TestHelpers.TestLiteralExpression(callExpression.Arguments[0], 1);
        TestHelpers.TestInfixExpression(callExpression.Arguments[1], 2, "*", 3);
        TestHelpers.TestInfixExpression(callExpression.Arguments[2], 4, "+", 5);
    }
    
    [Test]
    public void TestStringLiteralExpression()
    {
        var input = "\"hello world\"";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<StringLiteral>(expressionStatement.Expression);
        var stringLiteral = expressionStatement.Expression as StringLiteral;
        Assert.That(stringLiteral.Value, Is.EqualTo("hello world"));
    }
}
