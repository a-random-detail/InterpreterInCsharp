using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Parser;

public struct Parser
{
    private readonly List<string> _errors = new();
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
            TokenType.Return => ParseReturnStatement(),
            _ => null,
        };
    }

    private ReturnStatement? ParseReturnStatement()
    {
        var token = _curToken;
        NextToken();
        
        while(CurrentTokenIs(TokenType.Semicolon)) {
            NextToken();
        }

        return new ReturnStatement(token, null);
    }

    private LetStatement? ParseLetStatement()
    {
        var statementToken = _curToken;
        if (!ExpectPeekTokenType(TokenType.Ident))
        {
            return null;
        }

        var name = new Identifier(_curToken, _curToken.Literal);
        
        if (!ExpectPeekTokenType(TokenType.Assign))
        {
            return null;
        }


        while (!CurrentTokenIs(TokenType.Semicolon)) {
            NextToken();
        }

        return new LetStatement(statementToken, name, new Expression(_curToken));
    }

    private bool ExpectPeekTokenType(TokenType type)
    {
        if (!PeekTokenIs(type))
        {
            PeekError(type);
            return false;
        }
        
        NextToken();
        return true;
    }
    
    private void PeekError(TokenType type)
    {
        var msg = $"expected next token to be {type}, got {_peekToken.Type} instead";
        _errors.Add(msg);
    }
    
    public List<string> Errors => _errors;

    private bool PeekTokenIs(TokenType type) => _peekToken.Type == type;

    private bool CurrentTokenIs(TokenType type) => _curToken.Type == type;
    
}