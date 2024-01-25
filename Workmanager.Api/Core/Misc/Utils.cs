using System.Globalization;
namespace Workmanager.Api.Core.Misc; 
public static class Utils {
   
   public static DateTime EvalDateTime(string? date) {
      // 1234567890
      // YYYY-MM-dd
      if(date == null) return DateTime.MinValue;
      if(date.Length < 10) return DateTime.MinValue;
      
      DateTime dateTime;
      if (!DateTime.TryParseExact(
         s: date,
         format: "yyyy-MM-dd",
         provider: CultureInfo.InvariantCulture,
         style: DateTimeStyles.AdjustToUniversal,
         result: out dateTime)
      ) {
         throw new NotSupportedException($"Date format not accepted: {date}");
      }
      return dateTime.ToUniversalTime();
   }
   
   
   public static string As8(this Guid guid) => guid.ToString()[..8];
}