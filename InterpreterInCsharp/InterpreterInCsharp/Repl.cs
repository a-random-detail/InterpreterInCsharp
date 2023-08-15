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
            var parser = new Parser.Parser(lexer);
            var program = parser.ParseProgram();
            if (parser.Errors.Count != 0)
            {
                PrintParserErrors(parser.Errors);
                continue;
            }
            
            Console.WriteLine(program.String);
            Console.Write(Prompt);
        } 
    }

    private static void PrintParserErrors(List<string> errors)
    {
        Console.WriteLine(" parser errors:");
        foreach (var error in errors)
        {
            Console.WriteLine($"\t{error}");
        }
    }
}