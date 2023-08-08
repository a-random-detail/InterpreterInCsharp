using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Parser;

using PrefixParseFn = Func<Expression?>;

public struct Parser
{
    private readonly List<string> _errors = new();
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private readonly Dictionary<TokenType, PrefixParseFn> _prefixParseFunctions;
    private readonly Dictionary<TokenType, Func<Expression, Expression?>> _infixParseFunctions;
    private int _traceLevel = 0;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;

        _prefixParseFunctions = new Dictionary<TokenType, PrefixParseFn>();
        RegisterPrefix(TokenType.Ident, ParseIdentifier);
        RegisterPrefix(TokenType.Int, ParseIntegerLiteral);
        RegisterPrefix(TokenType.Bang, ParsePrefixExpression);
        RegisterPrefix(TokenType.Minus, ParsePrefixExpression);
        
        NextToken();
        NextToken();
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

    private string TraceIndention => new('\t', _traceLevel);

    private void StartTrace(string msg)
    {
        Console.WriteLine($"{TraceIndention}{_traceLevel} START {msg}");
        ++_traceLevel;
    }

    private void EndTrace(string msg)
    {
        Console.WriteLine($"{TraceIndention}{_traceLevel} END {msg}");
        --_traceLevel;
    } 

    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
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
    
    private void PeekError(TokenType type) =>_errors.Add($"expected next token to be {type}, got {_peekToken.Type} instead");
    
    public List<string> Errors => _errors;

    private bool PeekTokenIs(TokenType type) => _peekToken.Type == type;

    private bool CurrentTokenIs(TokenType type) => _curToken.Type == type;

    private void RegisterPrefix(TokenType type, Func<Expression?> fn) =>_prefixParseFunctions.Add(type, fn);
    
    
    private void RegisterInfix(TokenType type, Func<Expression, Expression?> fn) => _infixParseFunctions.Add(type, fn);
    

    private void NoPrefixParseFnError(TokenType type) =>  _errors.Add($"No prefix parse function for {type} found.");
    
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
        StartTrace(nameof(ParseExpressionStatement));
        var initialToken = _curToken;
        var parsed = ParseExpression(ExpressionPrecedence.Lowest);
        if (parsed == null) return null;
        
        var result = new ExpressionStatement(initialToken, parsed);

        if (PeekTokenIs(TokenType.Semicolon))
            NextToken();
        
        EndTrace(nameof(ParseExpressionStatement));
        return result;
    }

    private Expression? ParseExpression(ExpressionPrecedence precedence)
    {
        StartTrace(nameof(ParseExpression));
        var fetchPrefixSuccess = _prefixParseFunctions.TryGetValue(_curToken.Type, out var prefix);
        if (!fetchPrefixSuccess)
        {
            NoPrefixParseFnError(_curToken.Type);
            return null;
        }

        var leftExpression = prefix!();
        if (leftExpression == null)
            return null;
        
        EndTrace(nameof(ParseExpression));
        return leftExpression;
    }

    private ReturnStatement? ParseReturnStatement()
    {
        StartTrace(nameof(ParseReturnStatement));
        var token = _curToken;
        NextToken();
        
        SkipToSemicolon();
        EndTrace(nameof(ParseReturnStatement));
        return new ReturnStatement(token, null);
    }

    private LetStatement? ParseLetStatement()
    {
        StartTrace(nameof(ParseLetStatement));
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
        EndTrace(nameof(ParseLetStatement));
        return new LetStatement(statementToken, name, new Expression(_curToken));
    }
    
    private Expression ParseIdentifier() =>  new Identifier(_curToken, _curToken.Literal);

    private Expression? ParseIntegerLiteral()
    {
        StartTrace(nameof(ParseIntegerLiteral));
        if (!Int64.TryParse(_curToken.Literal, out Int64 result))
        {
            _errors.Add($"Could not parse {_curToken.Literal} as Integer.");
            return null;
        }
        EndTrace(nameof(ParseIntegerLiteral));
        return new IntegerLiteral(_curToken, result);
    }
    
    private Expression? ParsePrefixExpression()
    {
        StartTrace(nameof(ParsePrefixExpression));
        var token = _curToken;
        NextToken();
        var right = ParseExpression(ExpressionPrecedence.Prefix);
        if (right == null) return null;
        EndTrace(nameof(ParsePrefixExpression));
        return new PrefixExpression(token, token.Literal, right);
    }

    private void SkipToSemicolon()
    {
        StartTrace(nameof(SkipToSemicolon));
        while (!CurrentTokenIs(TokenType.Semicolon))
        {
            NextToken();
        }
        EndTrace(nameof(SkipToSemicolon));
    }

}