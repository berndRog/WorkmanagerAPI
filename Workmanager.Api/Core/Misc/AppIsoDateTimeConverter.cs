using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Workmanager.Api.Core.Misc;

public class AppIsoDateTimeConverter() : JsonConverter<DateTime> {
   
   // private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
   // public override DateTime Read(
   //    ref Utf8JsonReader reader, 
   //    Type typeToConvert, 
   //    JsonSerializerOptions options
   // ) => DateTime.ParseExact(reader.GetString(), DateFormat, 
   //       CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind
   //    );
   
   private readonly string[] _dateFormats = new[] {
      "yyyy-MM-ddTHH:mm:ssZ",    // Without milliseconds
      "yyyy-MM-ddTHH:mm:ss.fffZ",   // With milliseconds
      "yyyy-MM-ddTHH:mm:ss.ffffffZ" // With milliseconds
   };

   public override DateTime Read(
      ref Utf8JsonReader reader, 
      Type typeToConvert, 
      JsonSerializerOptions options
   ) {
      string dateString = reader.GetString();
      foreach (var format in _dateFormats) {
         if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, 
             DateTimeStyles.RoundtripKind, out DateTime date)) {
            return date;
         }
      }
      Debug.WriteLine("Invalid date format: {0}", dateString);
      throw new JsonException("Invalid date format.");
   }
   
   public override void Write(
      Utf8JsonWriter writer, 
      DateTime value, 
      JsonSerializerOptions options
   ) {
      writer.WriteStringValue(
         value.ToUniversalTime().ToString(_dateFormats[0], CultureInfo.InvariantCulture));
   }
}