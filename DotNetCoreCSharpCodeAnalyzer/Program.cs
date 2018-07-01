using System;

namespace DotNetCoreConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      
      var srcCode =
@"using DataTransferObjects.Request.OrderEdit;
using DataTransferObjects.Response.OrderEdit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WebAPI.Transactions.OrderEdit;
using Framework.WebAPI.BaseClasses;
using Framework.WebAPI.Interfaces;

namespace WebAPI.Controllers.V1.OrderEdit
{
  public partial class OrderEditController
  {

    /// <summary>
    /// FindUserName
    /// </summary>
    /// <returns>結果JSON</returns>
    public virtual IActionResult FindUserName([FromBody]FindUserNameRequest request)
    {
      // システムエラーチェック
      if (systenErrorResult is IActionResult b) return systenErrorResult;

      // 入力チェック
      if(!request.Validate())
      {
        logger.LogError(string.Empty,request.ValidateNGPropertyName);
        return Json(new FindUserNameResponse(FindUserNameResponse.Results.NG, string.Empty, null));
      }

      var status = FindUserNameResponse.Results.OK;
      var message = string.Empty;
      FindUserNameResponse.FindUserNameResponseParam resultParam=null;

      var transaction = new OrderEditTransaction(repository, logger);
      resultParam = transaction.FindUserName(request);

      // 注文者名が存在しない場合はエラー
      if (string.IsNullOrEmpty(resultParam.OrderUserName))
      {
        status = FindUserNameResponse.Results.NG;
        message = string.Empty;
      }
      else{
      var a=
123;
        a = 1*2+3;
      }

      return Json(new FindUserNameResponse(status, message, resultParam));
    }
  }
}";

      var codeAnalyzer = new CodeAnalyzer();
      codeAnalyzer.Analyze(srcCode);

      Console.WriteLine("End");
      Console.ReadKey();
    }
  }
}
