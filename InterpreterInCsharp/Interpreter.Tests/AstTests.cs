using InterpreterInCsharp;
using InterpreterInCsharp.Ast;

namespace Interpreter.Tests;

[TestFixture]
public class AstTests
{
    [Test]
    public void TestString()
    {
        var letStatement =
            new LetStatement(new Token(TokenType.Let, "let"),
                new Identifier(new Token(TokenType.Ident, "myVar"), "myVar"),
                new Identifier(new Token(TokenType.Ident, "anotherVar"), "anotherVar"));
        var statements = new List<Statement>
        {
            letStatement
        };
        var program = new MonkeyProgram(statements);

        Assert.That(program.String, Is.EqualTo("let myVar = anotherVar;"));
    }
}