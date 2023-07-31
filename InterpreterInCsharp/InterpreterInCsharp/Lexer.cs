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

    private void ReadChar()
    {
        _ch = _readPosition >= _input.Length ? '\0' : _input[_readPosition];
        _position = _readPosition;
        _readPosition += 1;
    }

    public Token NextToken()
    {
        Token token;
        switch (_ch)
        {
            case '=':
                token = new Token(TokenType.Assign, "=");
                break;
            case ';':
                token = new Token(TokenType.Semicolon, ";");
                break;
            case '(':
                token = new Token(TokenType.Lparen, "(");
                break;
            case ')':
                token = new Token(TokenType.Rparen, ")");
                break;
            case ',':
                token = new Token(TokenType.Comma, ",");
                break;
            case '+':
                token = new Token(TokenType.Plus, "+");
                break;
            case '{':
                token = new Token(TokenType.Lbrace, "{");
                break;
            case '}':
                token = new Token(TokenType.Rbrace, "}");
                break;
            case '\0':
                token = new Token(TokenType.Eof, "");
                break;
            default:
                return null;
            
        } 
        ReadChar();
        return token;
    }
}