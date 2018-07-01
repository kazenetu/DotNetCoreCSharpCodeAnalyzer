using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace DotNetCoreConsole
{
  public class CodeAnalyzer
  {

    public void Analyze(string targetCode)
    {
      var tree = CSharpSyntaxTree.ParseText(targetCode) as CSharpSyntaxTree;

      // 構文エラーチェック
      foreach(var item in tree.GetDiagnostics())
      {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("構文エラーがあります");
        Console.WriteLine($"エラー内容：「{item.GetMessage()}」");
        var location = item.Location.GetMappedLineSpan();
        Console.WriteLine($"位置：({location.StartLinePosition.Line+1},{location.StartLinePosition.Character + 1}) - ({location.EndLinePosition.Line + 1},{location.EndLinePosition.Character + 1})");
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("");
      }

      var textList = tree.GetText();
      foreach (var item in textList.Lines)
      {
//        Console.WriteLine($"{item.LineNumber+1,-5}:{item.ToString()}");
      }

      var root = tree.GetRoot();
      //foreach (var item in root.DescendantTokens())
      //{
      //  switch (item.Kind())
      //  {
      //    case SyntaxKind.UsingKeyword:
      //    case SyntaxKind.UsingDirective:
      //    case SyntaxKind.UsingStatement:
      //      break;
      //    default:
      //      var kindName = Enum.GetName(typeof(SyntaxKind), item.Kind());
      //      Console.WriteLine($"{kindName,-10}:{item.ValueText}");
      //      break;
      //  }
      //}

      foreach (CSharpSyntaxNode item in root.DescendantNodes())
      {
        switch (item.Kind())
        {
          //case SyntaxKind.ExpressionStatement:
          //  var leftSpan = 0;
          //  var rightSpan = 0;
          //  foreach (var exItem in item.ChildNodes())
          //  {
          //    rightSpan = exItem.Span.End;
          //    if(leftSpan > 0 && leftSpan == rightSpan)
          //    {
          //      Console.Write($"{"ExpressionStatement",-10}:{item.ToString()}");
          //      Console.Write($"  //NG!!!!!!");
          //      Console.WriteLine();
          //      break;
          //    }
          //    leftSpan = rightSpan;
          //  }
          //  Console.WriteLine();

          //  break;

          case SyntaxKind.SimpleMemberAccessExpression:
          case SyntaxKind.AddAssignmentExpression:
          case SyntaxKind.AndAssignmentExpression:
          case SyntaxKind.SimpleAssignmentExpression:
          case SyntaxKind.SubtractAssignmentExpression:
          case SyntaxKind.MultiplyAssignmentExpression:
          case SyntaxKind.DivideAssignmentExpression:
          case SyntaxKind.ModuloAssignmentExpression:
          case SyntaxKind.ExclusiveOrAssignmentExpression:
          case SyntaxKind.OrAssignmentExpression:
          case SyntaxKind.LeftShiftAssignmentExpression:
          case SyntaxKind.RightShiftAssignmentExpression:

          case SyntaxKind.AddExpression:
          case SyntaxKind.SubtractExpression:
          case SyntaxKind.MultiplyExpression:
          case SyntaxKind.DivideExpression:
          case SyntaxKind.ModuloExpression:
          case SyntaxKind.ExclusiveOrExpression:
          case SyntaxKind.LeftShiftExpression:
          case SyntaxKind.RightShiftExpression:

          case SyntaxKind.ParameterList:
          case SyntaxKind.ArgumentList:
          case SyntaxKind.IfStatement:
          case SyntaxKind.ElseClause:
          case SyntaxKind.LocalDeclarationStatement:
            var kindName = Enum.GetName(typeof(SyntaxKind), item.Kind());
            Console.Write($"{kindName,-10}:{item.ToString()}");

            if (item is AssignmentExpressionSyntax a)
            {
              var operatorSpan = a.OperatorToken.Span;
              var leftSpanEnd = a.Left.Span.End;
              var rightSpanStart = a.Right.SpanStart;

              if(leftSpanEnd == operatorSpan.Start || rightSpanStart == operatorSpan.End)
              {
                Console.Write($"  //NG!!!!!!");
              }
            }

            if (item is BinaryExpressionSyntax b)
            {
              var operatorSpan = b.OperatorToken.Span;
              var leftSpanEnd = b.Left.Span.End;
              var rightSpanStart = b.Right.SpanStart;

              if (leftSpanEnd == operatorSpan.Start || rightSpanStart == operatorSpan.End)
              {
                Console.Write($"  //NG!!!!!!");
              }
            }


            Console.WriteLine();

            break;
        }
      }

    }
  }
}
