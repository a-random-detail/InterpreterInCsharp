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
        var input = @" return 5;
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