namespace InterpreterInCsharp;

public class Lexer
{
    public readonly string _input;
    public int _position;
    public int _readPosition;
    public char _ch;
    
    public Lexer(string input)
    {
        _input = input;
        ReadChar();
    }

    public Token NextToken()
    {
        Token token;
        Console.WriteLine($"checking character: {_ch}");
        SkipWhitespace();
        switch (_ch)
        {
            case '=':
                token = new Token(TokenType.Assign, _ch);
                break;
            case ';':
                Console.WriteLine("semicolon found");
                token = new Token(TokenType.Semicolon, _ch);
                break;
            case '(':
                token = new Token(TokenType.Lparen, _ch);
                break;
            case ')':
                token = new Token(TokenType.Rparen, _ch);
                break;
            case ',':
                token = new Token(TokenType.Comma, _ch);
                break;
            case '+':
                token = new Token(TokenType.Plus, _ch);
                break;
            case '{':
                token = new Token(TokenType.Lbrace, _ch);
                break;
            case '}':
                token = new Token(TokenType.Rbrace, _ch);
                break;
            case '\0':
                token = new Token(TokenType.Eof, "");
                break;
            default:
                if (IsLegalIdentifierCharacter(_ch))
                {
                    var literal = ReadIdentifier();
                    var type = Token.LookupIdent(literal);
                    return new Token(type, literal);
                }

                if (IsDigit(_ch))
                {
                    var literal = ReadNumber();
                    return new Token(TokenType.Int, literal);
                }
                
                token = new Token(TokenType.Illegal, _ch);
                
                break;

        } 
        Console.WriteLine("Reading character after switch");
        ReadChar();
        return token;
    }

    private void ReadChar()
    {
        _ch = _readPosition >= _input.Length ? '\0' : _input[_readPosition];
        _position = _readPosition;
        _readPosition += 1;
        
        Console.WriteLine($"_ch: {_ch}");
        Console.WriteLine($"_position: {_position}");
        Console.WriteLine($"_readPosition: {_readPosition}");
    }
    
    private string ReadNumber()
    {
        var initialPosition = _position;
        while (IsDigit(_ch))
        {
            Console.WriteLine($"{_ch} is digit");
            ReadChar();
        }
        var numberResult = _input[initialPosition.._position];
        Console.WriteLine($"numberResult: {numberResult}");
        return numberResult;
    }
    
    private string ReadIdentifier()
    {
        var initialPosition = _position;
        while (IsLegalIdentifierCharacter(_ch))
        {
            Console.WriteLine($"{_ch} is legal identifier character");
            ReadChar();
        }
        var identifierResult = _input[initialPosition.._position];
        Console.WriteLine($"identifierResult: {identifierResult}");
        return identifierResult;
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(_ch))
        {
            Console.WriteLine($"{_ch} is whitespace");
            ReadChar();
        }
    }

    private bool IsDigit(char ch)
    {
        return ch is >= '0' and <= '9';
    }
    
    private bool IsLegalIdentifierCharacter(char ch)
    {
        return ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }
}
