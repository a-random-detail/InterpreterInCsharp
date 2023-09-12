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
        SkipWhitespace();
        switch (_ch)
        {
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    token = new Token(TokenType.IsEqual, $"{ch}{_ch}");
                }
                else
                {
                    token = new Token(TokenType.Assign, _ch);
                }
                break;
            case ';':
                token = new Token(TokenType.Semicolon, _ch);
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    token = new Token(TokenType.NotEqual, $"{ch}{_ch}");
                }
                else
                {
                    token = new Token(TokenType.Bang, _ch);
                }
                break;
            case '*':
                token = new Token(TokenType.Star, _ch);
                break;
            case '/':
                token = new Token(TokenType.Slash, _ch);
                break;
            case '+':
                token = new Token(TokenType.Plus, _ch);
                break;
            case '-':
                token = new Token(TokenType.Minus, _ch);
                break;
            case '<':
                token = new Token(TokenType.LessThan, _ch);
                break;
            case '>':
                token = new Token(TokenType.GreaterThan, _ch);
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
            case '{':
                token = new Token(TokenType.Lbrace, _ch);
                break;
            case '}':
                token = new Token(TokenType.Rbrace, _ch);
                break;
            case '"':
                token = new Token(TokenType.String, ReadString());
                break;
            case '[':
                token = new Token(TokenType.LBracket, _ch);
                break;
            case ']':
                token = new Token(TokenType.RBracket, _ch);
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
        ReadChar();
        return token;
    }

    private string ReadString()
    {
        var initialPosition = _position + 1;
        string stringResult = "";
        while (true)
        {
            ReadChar();
            if (_ch == '"' || _ch == '\0')
            {
                break;
            }
            var withPeekChar = _ch + PeekChar().ToString();
            switch (withPeekChar)
            {
                case "\\t":
                    stringResult += "\\t";
                    ReadChar();
                    break;
                case "\\n":
                    stringResult += "\\n";
                    ReadChar();
                    break;
                case "\\\"":
                    stringResult += "\\\"";
                    ReadChar();
                    break;
                default:
                    stringResult += _ch;
                    break;
            }
        }
        return stringResult;    
    }

    private char PeekChar()
    {
        return _readPosition >= _input.Length ? '\0' : _input[_readPosition];
    }

    private void ReadChar()
    {
        _ch = _readPosition >= _input.Length ? '\0' : _input[_readPosition];
        _position = _readPosition;
        _readPosition += 1;
    }
    
    private string ReadNumber()
    {
        var initialPosition = _position;
        while (IsDigit(_ch))
        {
            ReadChar();
        }
        var numberResult = _input[initialPosition.._position];
        return numberResult;
    }
    
    private string ReadIdentifier()
    {
        var initialPosition = _position;
        while (IsLegalIdentifierCharacter(_ch))
        {
            ReadChar();
        }
        var identifierResult = _input[initialPosition.._position];
        return identifierResult;
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(_ch))
        {
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
