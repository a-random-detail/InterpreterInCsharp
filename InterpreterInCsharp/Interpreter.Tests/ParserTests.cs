using InterpreterInCsharp;
using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Parser;

namespace Interpreter.Tests;

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
        Assert.NotNull(program);
        Assert.AreEqual(3, program.Statements.Count);


        var expectedIdentifiers = new[]
        {
            "x", "y", "foobar"
        };
        
        foreach (var (statement, expectedIdentifier) in program.Statements.Zip(expectedIdentifiers))
        {
            TestLetStatement(statement, expectedIdentifier);
        }

    }
    
    private void TestLetStatement(Statement statement, string expectedIdentifier)
    {
        Assert.AreEqual("let", statement.TokenLiteral);
        Assert.IsInstanceOf<LetStatement>(statement);
        var letStatement = (LetStatement) statement;
        Assert.AreEqual(expectedIdentifier, letStatement.Identifier.Value);
        Assert.AreEqual(expectedIdentifier, letStatement.Identifier.TokenLiteral);
    }
}