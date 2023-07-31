// See https://aka.ms/new-console-template for more information


using InterpreterInCsharp;

var user = Environment.UserName;
Console.WriteLine($"Hello, {user}! This is the Monkey programming language!");
Console.WriteLine("Feel free to type in commands");
Repl.Start();