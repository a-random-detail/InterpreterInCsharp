using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Parser;

public struct Parser
{
    private readonly List<string> _errors = new();
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private Dictionary<TokenType, Func<Expression>> _prefixParseFunctions;
    private Dictionary<TokenType, Func<Expression, Expression?>> _infixParseFunctions;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        NextToken();
        NextToken();

        _prefixParseFunctions = new Dictionary<TokenType, Func<Expression>>();
        RegisterPrefix(TokenType.Ident, ParseIdentifier);
        RegisterPrefix(TokenType.Int, ParseIntegerLiteral);
        RegisterPrefix(TokenType.Bang, ParsePrefixExpression);
        RegisterPrefix(TokenType.Minus, ParsePrefixExpression);
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
            _ => ParseExpressionStatement(),
        };
    }

    private Statement? ParseExpressionStatement()
    {
        var initialToken = _curToken;
        var parsed = ParseExpression(ExpressionPrecedence.LOWEST);
        
        if (_peekToken.Type == TokenType.Semicolon)
        {
            NextToken();
        }
        return new ExpressionStatement(initialToken, parsed);
    }

    private Expression? ParseExpression(ExpressionPrecedence precedence)
    {
        if (precedence is ExpressionPrecedence.PREFIX)
        {
            Console.WriteLine($"Parsing prefix expression with token {_curToken.Literal} and type {_curToken.Type}");
            Console.WriteLine($"Does prefix exist? {_prefixParseFunctions[_curToken.Type] != null}");
        }
        var prefix = _prefixParseFunctions[_curToken.Type];
        if (prefix == null)
        {
            NoPrefixParseFnError(_curToken.Type);
            return null;
        }
        var leftExpression = prefix();
        return leftExpression;
    }

    private ReturnStatement? ParseReturnStatement()
    {
        var token = _curToken;
        NextToken();
        
        SkipToSemicolon();

        return new ReturnStatement(token, null);
    }

    private LetStatement? ParseLetStatement()
    {
        var statementToken = _curToken;
        if (!ExpectPeekTokenType(TokenType.Ident))
        {
            SkipToSemicolon();
            return null;
        }

        var name = new Identifier(_curToken, _curToken.Literal);
        
        if (!ExpectPeekTokenType(TokenType.Assign))
        {
            SkipToSemicolon();
            return null;
        }


        SkipToSemicolon();

        return new LetStatement(statementToken, name, new Expression(_curToken));
    }
    
    private Expression ParseIdentifier()
    {
        return new Identifier(_curToken, _curToken.Literal);
    }

    private Expression ParseIntegerLiteral()
    {
        Console.WriteLine($"Parsing {_curToken.Literal} in integer literal parsing function");
        if (!Int64.TryParse(_curToken.Literal, out Int64 result))
        {
            _errors.Add($"Could not parse {_curToken.Literal} as Integer.");
            return null;
        }

        return new IntegerLiteral(_curToken, result);
    }
    
    private Expression ParsePrefixExpression()
    {
        var token = _curToken;
        NextToken();
        var right = ParseExpression(ExpressionPrecedence.PREFIX);
        Console.WriteLine($"old current token {token.Literal} -- new current token {_curToken.Literal}");
        Console.WriteLine($"right -> {right?.String}");
        return new PrefixExpression(token, token.Literal, right)
            
            
    }

    private void SkipToSemicolon()
    {
        while (!CurrentTokenIs(TokenType.Semicolon))
        {
            NextToken();
        }
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

    private Func<Expression> ParsePrefixFunction()
    {
        return null;
    }

    private Func<Expression, Expression?> ParseInfixFunction(Expression ex)
    {
        return null;
    }

    private void RegisterPrefix(TokenType type, Func<Expression> fn)
    {
        _prefixParseFunctions.Add(type, fn);
    }
    
    private void RegisterInfix(TokenType type, Func<Expression, Expression?> fn)
    {
        _infixParseFunctions.Add(type, fn);
    }

    private void NoPrefixParseFnError(TokenType type)
    {
        _errors.Add($"No prefix parse function for {type} found.");
    }

}