using InterpreterInCsharp;

namespace Interpreter.Tests;

public class LexerTests
{
    [Test]
    public void TestNextToken()
    {
        var input = @"let five = 5;
let ten = 10;
let add = fn(x, y) {
    x + y;
}

let result = add(five, ten);";
        
        var tokens = new List<Token>
        {
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "five"),
            new(TokenType.Assign, "="),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "ten"),
            new(TokenType.Assign, "="),
            new(TokenType.Int, "10"),
            new(TokenType.Semicolon, ";"),
            
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "add"),
            new(TokenType.Assign, "="),
            new(TokenType.Function, "fn"),
            new(TokenType.Lparen, "("),
            new(TokenType.Ident, "x"),
            new(TokenType.Comma, ","),
            new(TokenType.Ident, "y"),
            new(TokenType.Rparen, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Ident, "x"),
            new(TokenType.Plus, "+"),
            new(TokenType.Ident, "y"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            
            new(TokenType.Let, "let"),
            new(TokenType.Ident, "result"),
            new(TokenType.Assign, "="),
            new(TokenType.Ident, "add"),
            new(TokenType.Lparen, "("),
            new(TokenType.Ident, "five"),
            new(TokenType.Comma, ","),
            new(TokenType.Ident, "ten"),
            new(TokenType.Rparen, ")"),
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