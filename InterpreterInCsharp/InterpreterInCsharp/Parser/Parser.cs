using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Parser;

public struct Parser
{
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        NextToken();
        NextToken();
    }

    public void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    public MonkeyProgram? ParseProgram()
    {
        var program = new MonkeyProgram(new List<Statement>());
        while (!CurrentTokenIs(TokenType.Eof))
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                program.Statements.Add(statement);
            }

            NextToken();
        }

        return program;
    }

    private Statement? ParseStatement()
    {
        return _curToken.Type switch
        {
            TokenType.Let => ParseLetStatement(),
            _ => null,

        };
    }

private LetStatement? ParseLetStatement()
    {
        var statementToken = _curToken;
        if (!ExpectPeek(TokenType.Ident))
        {
            return null;
        }

        var name = new Identifier(_curToken, _curToken.Literal);
        
        if (!ExpectPeek(TokenType.Assign))
        {
            return null;
        }


        while (!CurrentTokenIs(TokenType.Semicolon)) {
            NextToken();
        }

        return new LetStatement(statementToken, name, new Expression(_curToken));
    }

    private bool ExpectPeek(TokenType type)
    {
        if (!PeekTokenIs(type)) return false;
        
        NextToken();
        return true;

    }

    private bool PeekTokenIs(TokenType type) => _peekToken.Type == type;

    private bool CurrentTokenIs(TokenType type) => _curToken.Type == type;
    
}