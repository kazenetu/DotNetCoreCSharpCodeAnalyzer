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
    private List<TextLine> textList = null;

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

      var root = tree.GetRoot();

      // エラー時表示用ソースコードを取得
      textList = root.GetText().Lines.ToList();

      // 直近のエラー行
      var errorLine = -1;

      // エラーチェック
      foreach (CSharpSyntaxNode item in root.DescendantNodes())
      {
        switch (item.Kind())
        {
          case SyntaxKind.ExpressionStatement:
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
          case SyntaxKind.VariableDeclarator:
          case SyntaxKind.EqualsValueClause:

            if (item is ExpressionSyntax)
            {
              var tokens = item.ChildTokens();

              if (!tokens.Any())
              {
                continue;
              }

              var token = tokens.First();
              var tokenSpan = token.Span;
              var nodes = item.ChildNodes().ToList();

              if (nodes.Count < 2)
              {
                continue;
              }

              var leftNode = nodes[0];
              var RightNode = nodes[1];

              switch (token.Kind())
              {
                case SyntaxKind.DotToken:
                  break;
                default:
                  if(leftNode.Span.End == tokenSpan.Start || tokenSpan.End == leftNode.Span.Start)
                  {
                    var startLinePos = item.GetLocation().GetLineSpan().StartLinePosition;
                    if(errorLine >= startLinePos.Line)
                    {
                      continue;
                    }

                    Console.Write($"[{startLinePos.Line + 1,3},{startLinePos.Character + 1,3}]");
                    Console.Write(":空白をいれてください");
                    Console.Write($":{GetSource(item)}");
                    Console.WriteLine();
                    errorLine = startLinePos.Line;
                  }
                  break;
              }
            }

            if(item is VariableDeclaratorSyntax v)
            {
              var token = v.Initializer.EqualsToken;
              var tokenSpan = token.Span;
              var leftSpan = v.Identifier.Span;
              var rightSpan = v.Initializer.Value.Span;

              if (leftSpan.End == tokenSpan.Start || tokenSpan.End == rightSpan.Start)
              {
                var startLinePos = item.GetLocation().GetLineSpan().StartLinePosition;
                if (errorLine >= startLinePos.Line)
                {
                  continue;
                }

                Console.Write($"[{startLinePos.Line + 1,3},{startLinePos.Character + 1,3}]");
                Console.Write(":空白をいれてください");
                Console.Write($":{GetSource(item)}");
                Console.WriteLine();
                errorLine = startLinePos.Line;
              }
            }

            if(item is ArgumentListSyntax a)
            {
              if (!CheckArguments(a))
              {
                continue;
              }

              bool CheckArguments(ArgumentListSyntax args)
              {
                var commaTokens = args.ChildTokens().Where(token => token.ToString() == ",");
                var leftSpanEnd = -1;

                foreach (var token in args.Arguments)
                {
                  if (leftSpanEnd < 0)
                  {
                    leftSpanEnd = token.Span.End;
                    continue;
                  }

                  var rightSpanStart = token.Span.Start;
                  if (leftSpanEnd + 1 == rightSpanStart)
                  {
                    // 空白をいれてください
                    var startLinePos = item.GetLocation().GetLineSpan().StartLinePosition;
                    if (errorLine >= startLinePos.Line)
                    {
                      continue;
                    }

                    Console.Write($"[{startLinePos.Line + 1,3},{startLinePos.Character + 1,3}]");
                    Console.Write(":空白をいれてください");
                    Console.Write($":{GetSource(item)}");
                    Console.WriteLine();
                    errorLine = startLinePos.Line;
                    return false;
                  }
                  
                  if (!commaTokens.Any(commaToken => commaToken.SpanStart == leftSpanEnd))
                  {
                    // カンマは同じ行につけてください。
                    var startLinePos = item.GetLocation().GetLineSpan().StartLinePosition;
                    if (errorLine >= startLinePos.Line)
                    {
                      continue;
                    }

                    Console.Write($"[{startLinePos.Line + 1,3},{startLinePos.Character + 1,3}]");
                    Console.Write(":カンマは同じ行につけてください。");
                    Console.Write($":{GetSource(item)}");
                    Console.WriteLine();
                    errorLine = startLinePos.Line;
                    return false;
                  }

                  leftSpanEnd = token.Span.End;
                }
                return true;
              }
            }

            break;
        }
      }
    }

    /// <summary>
    /// 対象ソースを取得
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <returns>対象のソースコード</returns>
    private string GetSource(CSharpSyntaxNode node)
    {
      var pos = node.GetLocation().GetLineSpan();
      var result = new StringBuilder();

      for(var index = pos.StartLinePosition.Line;index <= pos.EndLinePosition.Line; index++)
      {
        result.Append($" {textList[index].ToString().TrimStart()}");
      }

      return result.ToString();
    }
  }
}
