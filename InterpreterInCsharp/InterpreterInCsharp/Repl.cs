using System;

namespace InterpreterInCsharp;

public class Repl
{
    private const string Prompt = ">> ";

    public static void Start()
    {
        Token nextToken;
        Console.Write(Prompt);
        while(true)
        {
            string? nextLine = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nextLine))
            {
                return;
            }
            
            var lexer = new Lexer(nextLine);
            
            do
            {
                nextToken = lexer.NextToken();
                Console.WriteLine(nextToken);
            } while (nextToken.Type != TokenType.Eof);
        } 
    }
    
}