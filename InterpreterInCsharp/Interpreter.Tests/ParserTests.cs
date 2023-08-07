using System.Linq.Expressions;
using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Parser;
using NuGet.Frameworks;

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
        CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(3));


        var expectedIdentifiers = new[]
        {
            "x", "y", "foobar"
        };

        foreach (var (statement, expectedIdentifier) in program.Statements.Zip(expectedIdentifiers))
        {
            TestLetStatement(statement, expectedIdentifier);
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

        CheckParserErrors(parser);
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

        CheckParserErrors(parser);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<Identifier>(expressionStatement.Expression);
        var identifier = (Identifier)expressionStatement.Expression;
        Assert.That(expressionStatement.TokenLiteral, Is.EqualTo("foobar"));
        Assert.That(identifier.Value, Is.EqualTo("foobar"));
    }

    [Test]
    public void TestIntegerLiteralExpressions()
    {
        var input = @"5;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();

        CheckParserErrors(parser);
        Assert.NotNull(program);
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStatement = statement as ExpressionStatement;
        Assert.IsInstanceOf<IntegerLiteral>(expressionStatement.Expression);
        var integerLiteral = expressionStatement.Expression as IntegerLiteral;
        Assert.That(integerLiteral.Value, Is.EqualTo(5));
        Assert.That(integerLiteral.TokenLiteral, Is.EqualTo("5"));
    }

    [TestCase("!5", "!", 5)]
    [TestCase("-15", "-", 15)]
    public void TestParsingPrefixExpressions(string input, string op, Int64 value)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        
        CheckParserErrors(parser);
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        var statement = program.Statements[0];
        Assert.IsInstanceOf<ExpressionStatement>(statement);
        var expressionStmt = statement as ExpressionStatement;
        Assert.IsInstanceOf<PrefixExpression>(expressionStmt.Expression);
        var prefixExpression = expressionStmt.Expression as PrefixExpression;
        Assert.That(prefixExpression.Operator, Is.EqualTo(op));
        TestIntegerLiteral(prefixExpression, value);

    }

    private void TestIntegerLiteral(PrefixExpression exp, Int64 expectedValue)
    {
        Assert.IsInstanceOf<IntegerLiteral>(exp.Right);
        var integerLiteral = exp.Right as IntegerLiteral;

        Assert.That(integerLiteral.Value, Is.EqualTo(expectedValue));
    }


    private void CheckParserErrors(Parser parser)
    {
        string errors = string.Join(",", parser.Errors);
        Assert.That(parser.Errors.Count, Is.EqualTo(0), $"Parser had errors: {errors}");
    }

    private void TestLetStatement(Statement statement, string expectedIdentifier)
    {
        Assert.That(statement.TokenLiteral, Is.EqualTo("let"));
        Assert.IsInstanceOf<LetStatement>(statement);
        var letStatement = (LetStatement) statement;
        Assert.That(letStatement.Identifier.Value, Is.EqualTo(expectedIdentifier));
        Assert.That(letStatement.Identifier.TokenLiteral, Is.EqualTo(expectedIdentifier));
    }
}