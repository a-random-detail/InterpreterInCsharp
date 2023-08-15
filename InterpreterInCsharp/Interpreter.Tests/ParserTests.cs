using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Parser;

namespace Interpreter.Tests;

[TestFixture]
public class ParserTests
{
    [Test]
    public void TestLetStatements()
    {
        var input = @"let x = 5;
let y = 10;
let foobar = 838383;
";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        TestHelpers.CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(3));


        var expectedIdentifiers = new[]
        {
            "x", "y", "foobar"
        };

        foreach (var (statement, expectedIdentifier) in program.Statements.Zip(expectedIdentifiers))
        {
            TestHelpers.TestLetStatement(statement, expectedIdentifier);
        }

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

    [Test]
    public void TestReturnStatements()
    {
        var input = @"return 5;
return 18;
return 839838;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();

        TestHelpers.CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(3));

        foreach (var statement in program.Statements)
        {
            Assert.That(statement.TokenLiteral, Is.EqualTo("return"));
            Assert.IsInstanceOf<ReturnStatement>(statement);
        }

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

}