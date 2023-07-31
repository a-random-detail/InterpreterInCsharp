using InterpreterInCsharp;

namespace Interpreter.Tests;

public class LexerTests
{
    [Test]
    public void TestNextToken()
    {
        var input = "=+(){},;";
        
        var tokens = new List<Token>
        {
            new(TokenType.Assign, "="),
            new(TokenType.Plus, "+"),
            new(TokenType.Lparen, "("),
            new(TokenType.Rparen, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Comma, ","),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Eof, "")
        };

        var lexer = new Lexer(input);
        foreach (var expected in tokens)
        {
            var nextToken = lexer.NextToken();
            Assert.AreEqual(expected, nextToken);
        }
    }
}