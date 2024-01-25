namespace Workmanager.Api.Core; 
public abstract class ResultData<T> where T : class? {
   public string Message { get; }
   public T?     Data    { get; }

   protected ResultData(
      string message = "",
      T?     data    = null
   ) {
      Message = message;
      Data    = data;
   }
}

public class Success<T> : ResultData<T> where T : class? {
   public Success(T? data) : base("", data) { }
}

public class Error<T> : ResultData<T> where T : class? {
   public Error(string error) : base(error) { }
}

public class Loading<T> : ResultData<T> where T : class? {
   public Loading(string message) : base(message) { }
}