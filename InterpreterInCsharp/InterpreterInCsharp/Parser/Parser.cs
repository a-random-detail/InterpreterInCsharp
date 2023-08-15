using InterpreterInCsharp.Ast;

namespace InterpreterInCsharp.Parser;

using PrefixParseFn = Func<Expression?>;
using InfixParseFn = Func<Expression, Expression?>;

public class Parser
{
    private readonly List<string> _errors = new();
    private readonly Lexer _lexer;
    private Token _curToken;
    private Token _peekToken;
    private readonly Dictionary<TokenType, PrefixParseFn> _prefixParseFunctions;
    private readonly Dictionary<TokenType, InfixParseFn> _infixParseFunctions;
    private int _traceLevel = 0;
    private bool _traceEnabled;

    private readonly Dictionary<TokenType, ExpressionPrecedence> _precedences =
        new()
        {
            { TokenType.IsEqual, ExpressionPrecedence.Equal },
            { TokenType.NotEqual, ExpressionPrecedence.Equal },
            { TokenType.LessThan, ExpressionPrecedence.LessGreater },
            { TokenType.GreaterThan, ExpressionPrecedence.LessGreater },
            { TokenType.Plus, ExpressionPrecedence.Sum },
            { TokenType.Minus, ExpressionPrecedence.Sum },
            { TokenType.Star, ExpressionPrecedence.Product },
            { TokenType.Slash, ExpressionPrecedence.Product },
        };

    public Parser(Lexer lexer, bool traceEnabled = false)
    {
        _lexer = lexer;
        _traceEnabled = traceEnabled;

        _prefixParseFunctions = new Dictionary<TokenType, PrefixParseFn>();
        RegisterPrefix(TokenType.Ident, ParseIdentifier);
        RegisterPrefix(TokenType.Int, ParseIntegerLiteral);
        RegisterPrefix(TokenType.Bang, ParsePrefixExpression);
        RegisterPrefix(TokenType.Minus, ParsePrefixExpression);
        RegisterPrefix(TokenType.True, ParseBoolean);
        RegisterPrefix(TokenType.False, ParseBoolean);
        RegisterPrefix(TokenType.Lparen, ParseGroupedExpression);
        RegisterPrefix(TokenType.If, ParseIfExpression);
        
        _infixParseFunctions = new Dictionary<TokenType, InfixParseFn>();
        RegisterInfix(TokenType.Plus, ParseInfixExpression);
        RegisterInfix(TokenType.Minus, ParseInfixExpression);
        RegisterInfix(TokenType.Slash, ParseInfixExpression);
        RegisterInfix(TokenType.Star, ParseInfixExpression);
        RegisterInfix(TokenType.IsEqual, ParseInfixExpression);
        RegisterInfix(TokenType.NotEqual, ParseInfixExpression);
        RegisterInfix(TokenType.LessThan, ParseInfixExpression);
        RegisterInfix(TokenType.GreaterThan, ParseInfixExpression);
        
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
        if (_traceEnabled)
            Console.WriteLine($"{TraceIndention}{_traceLevel} START {msg}");
        ++_traceLevel;
    }

    private void EndTrace(string msg)
    {
        if (_traceEnabled)
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

    private void RegisterPrefix(TokenType type, PrefixParseFn fn) =>_prefixParseFunctions.Add(type, fn);
    
    
    private void RegisterInfix(TokenType type, Func<Expression, Expression?> fn) => _infixParseFunctions.Add(type, fn);
    

    private void NoPrefixParseFnError(TokenType type) =>  _errors.Add($"No prefix parse function for {type} found.");

    private ExpressionPrecedence PeekPrecedence()
    {
        if (_precedences.TryGetValue(_peekToken.Type, out var precedence))
            return precedence;
        return ExpressionPrecedence.Lowest;
    }

    private ExpressionPrecedence CurrentPrecedence()
    {
        if (_precedences.TryGetValue(_curToken.Type, out var precedence))
        {
            return precedence;
        }

        return ExpressionPrecedence.Lowest;
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
        if (!_prefixParseFunctions.TryGetValue(_curToken.Type, out var prefix))
        {
            NoPrefixParseFnError(_curToken.Type);
            return null;
        }

        var leftExpression = prefix!();
        if (leftExpression == null)
            return null;

        while (!PeekTokenIs(TokenType.Semicolon) && precedence < PeekPrecedence())
        {
            if (!_infixParseFunctions.TryGetValue(_peekToken.Type, out var infix))
            {
                EndTrace(nameof(ParseExpression));
                return leftExpression;
            }

            NextToken();
            leftExpression = infix!(leftExpression);
        }

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
    
    private Expression? ParseInfixExpression(Expression left)
    {
        StartTrace(nameof(ParseInfixExpression));
        
        var initialToken = _curToken;
        var precedence = CurrentPrecedence();
        
        NextToken();
        
        var right = ParseExpression(precedence);
        EndTrace(nameof(ParseInfixExpression));
        
        return new InfixExpression(initialToken, left, initialToken.Literal, right);
    }
    
    private Expression? ParseGroupedExpression()
    {
        NextToken();
        var exp = ParseExpression(ExpressionPrecedence.Lowest);
        if (!ExpectPeekTokenType(TokenType.Rparen))
            return null;
        
        return exp;
    }
    
    private IfExpression? ParseIfExpression()
    {
        var initialToken = _curToken;
        if (!ExpectPeekTokenType(TokenType.Lparen))
            return null;
        NextToken();
        var condition = ParseExpression(ExpressionPrecedence.Lowest);
        if (!ExpectPeekTokenType(TokenType.Rparen))
            return null;
        if (!ExpectPeekTokenType(TokenType.Lbrace))
            return null;

        var consequence = ParseBlockStatement();
        if (PeekTokenIs(TokenType.Else))
        {
            NextToken();
            if (!ExpectPeekTokenType(TokenType.Lbrace))
                return null;
            var alternative = ParseBlockStatement();
            return new IfExpression(initialToken, condition, consequence, alternative);
        }
        
        return new IfExpression(initialToken, condition, consequence, null);
    }

    private BlockStatement ParseBlockStatement()
    {
        var initialToken = _curToken;
        var statements = new List<Statement>();
        NextToken();
        while (!CurrentTokenIs(TokenType.Rbrace))
        {
            var statement = ParseStatement();
            if (statement != null)
                statements.Add(statement);
            NextToken();
        }

        return new BlockStatement(initialToken, statements.ToArray());
    }

    private Expression? ParseBoolean() => new BooleanExpression(_curToken, CurrentTokenIs(TokenType.True));

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