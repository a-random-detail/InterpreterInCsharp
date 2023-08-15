using InterpreterInCsharp.Ast;
using InterpreterInCsharp.Parser;

namespace Interpreter.Tests;

public class TestHelpers
{
    
    public static void TestBooleanLiteral(Expression exp, bool value)
    {
        Assert.IsInstanceOf<BooleanExpression>(exp);
        var casted = exp as BooleanExpression;
        Assert.That(casted.Value, Is.EqualTo(value));
        Assert.That(casted.TokenLiteral, Is.EqualTo(value.ToString().ToLower()));
    }

    public static void TestInfixExpression<L, R>(Expression exp, L left, string op, R right)
    {
        Assert.IsInstanceOf<InfixExpression>(exp);
        var infixExp = exp as InfixExpression;
        
        TestLiteralExpression(infixExp.Left, left);
        Assert.That(infixExp.Operator, Is.EqualTo(op));
        
        TestLiteralExpression(infixExp.Right, right);
        
    }
    
    public static void TestBooleanExpression(Expression exp, bool expected)
    {
        Assert.IsInstanceOf<BooleanExpression>(exp);
        var booleanExpression = exp as BooleanExpression;
        Assert.That(booleanExpression.Value, Is.EqualTo(expected));
        Assert.That(booleanExpression.TokenLiteral, Is.EqualTo(expected.ToString().ToLower()));
    }

    public static void TestIdentifier(Expression exp, string value)
    {
        Assert.IsInstanceOf<Identifier>(exp);
        var ident = exp as Identifier;
        Assert.That(ident.Value, Is.EqualTo(value));
        Assert.That(ident.TokenLiteral, Is.EqualTo(value));
    }

    public static void TestIntegerLiteral(Expression exp, Int64 expectedValue)
    {
        Assert.IsInstanceOf<IntegerLiteral>(exp);
        var integerLiteral = exp as IntegerLiteral;

        Assert.That(integerLiteral.Value, Is.EqualTo(expectedValue));
        Assert.That(integerLiteral.TokenLiteral, Is.EqualTo(expectedValue.ToString()));
    }
    
    public static void CheckParserErrors(Parser parser)
    {
        string errors = string.Join(",", parser.Errors);
        Assert.That(parser.Errors.Count, Is.EqualTo(0), $"Parser had errors: {errors}");
    }

    public static void TestLetStatement(Statement statement, string expectedIdentifier)
    {
        Assert.That(statement.TokenLiteral, Is.EqualTo("let"));
        Assert.IsInstanceOf<LetStatement>(statement);
        var letStatement = (LetStatement) statement;
        TestIdentifier(letStatement.Identifier, expectedIdentifier);
    }
    
    public static void TestLiteralExpression<T>(Expression exp, T expected)
    {
        switch (expected)
        {
            case int i:
                TestIntegerLiteral(exp, i);
                break;
            case Int64 i:
                TestIntegerLiteral(exp, i);
                break;
            case string str:
                TestIdentifier(exp, str);
                break;
            case bool b:
                TestBooleanLiteral(exp, b);
                break;
            default:
                Assert.Fail($"No pattern matches for expression {exp.String}");
                break;
        }
    }
}