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

let result = add(five, ten);

!-/*5;
5 < 10 > 5;

if (5 < 10) {
       return true;
   } else {
       return false;
}

19 == 19;
19 != 20;
";
        
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
            
            new(TokenType.Bang, "!"),
            new(TokenType.Minus, "-"),
            new(TokenType.Slash, "/"),
            new(TokenType.Star, "*"),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Int, "5"),
            new(TokenType.LessThan, "<"),
            new(TokenType.Int, "10"),
            new(TokenType.GreaterThan, ">"),
            new(TokenType.Int, "5"),
            new(TokenType.Semicolon, ";"),
            
            new(TokenType.If, "if"),
            new(TokenType.Lparen, "("),
            new(TokenType.Int, "5"),
            new(TokenType.LessThan, "<"),
            new(TokenType.Int, "10"),
            new(TokenType.Rparen, ")"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Return, "return"),
            new(TokenType.True, "true"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            new(TokenType.Else, "else"),
            new(TokenType.Lbrace, "{"),
            new(TokenType.Return, "return"),
            new(TokenType.False, "false"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Rbrace, "}"),
            
            new(TokenType.Int, "19"),
            new(TokenType.IsEqual, "=="),
            new(TokenType.Int, "19"),
            new(TokenType.Semicolon, ";"),
            new(TokenType.Int, "19"),
            new(TokenType.NotEqual, "!="),
            new(TokenType.Int, "20"),
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