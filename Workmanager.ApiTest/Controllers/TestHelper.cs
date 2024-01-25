using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Workmanager.ApiTest.Controllers;
public static class TestHelper {
   
   public static T ResultFromResponse<T, TS>(
      ActionResult<TS> response
   ) where T : class 
      where TS : class {
      response.Should().NotBeNull();
      response.Result.Should().NotBeNull().And.BeOfType<T>();
      return response.Result as T 
         ?? throw new InvalidOperationException("Unexpected null result");
   }
}