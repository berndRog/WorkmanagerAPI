using AutoMapper.Internal.Mappers;

namespace Workmanager.Api.Core.Misc;





// https://stackoverflow.com/questions/246498/creating-a-datetime-in-a-specific-time-zone-in-c-sharp/246529#246529
// https://codeblog.jonskeet.uk/2019/03/27/storing-utc-is-not-a-silver-bullet/



public static class DateTimeExt {
   
   private static TimeZoneInfo _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
   private static TimeZoneInfo _localTimeZone = TimeZoneInfo.Local;
   private static TimeZoneInfo _utcTimeZone = TimeZoneInfo.Utc;
   
   static TimeSpan _offset  = _timeZoneInfo.GetUtcOffset(DateTime.Now);
   
   private static string _iso8601Format = "yyyy-MM-dd\\THH:mm:ss"; //ISO-8601 used by Javascript (ALWAYS UTC)
   
   public static DateTime AdjustMillis(this DateTime dateTime) =>
      new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
         dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

   public static DateTime SetKindUtc(this DateTime dateTime) {
      return dateTime.Kind == DateTimeKind.Utc ? dateTime 
         : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
   }

   public static DateTime SetKindLocal(this DateTime dateTime) {
      return dateTime.Kind == DateTimeKind.Local ? dateTime 
         : DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
   }
   
   public static DateTime FromEpochInMillis(long epoch) {
      return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
   }

   public static long ToEpochInMillis(DateTime dateTime) {
      if (dateTime.Kind == DateTimeKind.Unspecified) {
         throw new Exception("Cannot convert DateTimeKind.Unspecified to epoch");
      }
      else if (dateTime.Kind == DateTimeKind.Local) {
         dateTime = dateTime.ToUniversalTime();
      }
      else if (dateTime.Kind == DateTimeKind.Utc) {
         // nothing to do 
      }  
      return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
   }

   /// <summary>
   /// Convert ISO8610 Stirng in UTC DateTime
   /// </summary>
   /// <param name="stringIso"></param>
   /// <returns>DateTime in UTC Time Zone</returns>
   /// <exception cref="ArgumentException"></exception>
   public static DateTime FromIso8601String(
      string stringIso
   ) {
      
      if (stringIso.Length < 20)
         throw new ArgumentException("Invalid length $stringIso");
      else if (stringIso.Length > 28) {
         stringIso = stringIso.Substring(0, 27) + "Z";
      }
      var format = stringIso.Length switch {
         24 => _iso8601Format + ".fffK",
         20 => _iso8601Format + "K",           //20 -> 30
         21 => _iso8601Format + ".K",          //21 -> 30         
         22 => _iso8601Format + ".fK",         //22 -> 30  // milliseconds .0
         23 => _iso8601Format + ".ffK",        //23 -> 30  // milliseconds .00
//       24 => _iso8601Format + ".fffK",                                   .000
         25 => _iso8601Format + ".ffffK",      //25 -> 30  // microseconds .000 0
         26 => _iso8601Format + ".fffffK",     //26 -> 30  // microseconds .000 00
         27 => _iso8601Format + ".ffffffK",    //27 -> 30  // microseconds .000 000
         28 => _iso8601Format + ".fffffffK",   //28 -> 30  // nanoseconds  .000 000 0
         _ => throw new ArgumentException("Invalid length $stringIso")
      };
      
      // localDateTime is Kind=Unspecified is ZonedDatetime in local TimeZone
      DateTime localDateTime = DateTime.ParseExact(stringIso, format, 
         System.Globalization.CultureInfo.InvariantCulture);
      _iso8601Format = "yyyy-MM-dd\\THH:mm:ssK";
      
      // useLocal == true -> returned localDateTime else UTC DateTime
      return localDateTime.ToUniversalTime(); 
   }

   
   
   public static DateTime FromDateAndTimeString(string date, string time, bool useLocal = false) {
      //Return a new DateTime buiding an ISOFROMAT string from date, time params expressed in UTC (by default) or in LT if you set useLocal=true 
      var sb = new System.Text.StringBuilder(30);
      if (!string.IsNullOrEmpty(date)) { sb.Append(date); sb.Replace('-', '/'); }
      if (!string.IsNullOrEmpty(time)) { sb.Append(' '); sb.Append(time); }
      var s = sb.ToString();
      if (!useLocal) { //Always return DateTime Kind=UTC, if you don't pass +/-TIMEOFFSET or 'Z' postfix I'll add it by default (as needed for UTC)
         if (!(s.Contains('Z') || s.Contains('+') || s.Contains('-'))) s += "Z";
         return DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
      } else { //Return DateTime Kind=Local and do necessary conversion to LT if you pass time with +/-TIMEOFFSET or referred as UTC with 'Z' postfix
         return DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
      }
   }
   

   
   public static string ToIso8601String(
      DateTime dateTime, 
      bool useLocal = false
   ) {
      if (!useLocal && dateTime.Kind == DateTimeKind.Local) { 
         //If d is LT or you don't want LocalTime -> convert to UTC and always add K format always add 'Z' postfix
         return dateTime.ToUniversalTime().ToString(_iso8601Format);
      } else { //If d is already UTC K format add 'Z' postfix, if d is LT K format add +/-TIMEOFFSET
         return dateTime.ToString(_iso8601Format);
      }
   }
}


public static class Iso8601FormatExtensions {
   
    const string Iso8601Format = "yyyy-MM-dd\\THH:mm:ss.fffK"; //ISO-8601 used by Javascript (ALWAYS UTC)
    
    public static string ToIso8601String(this DateTime dateTime, bool useLocal = false) {
        if (!useLocal && dateTime.Kind == DateTimeKind.Local) { 
            //If d is LT or you don't want LocalTime -> convert to UTC and always add K format always add 'Z' postfix
            return dateTime.ToUniversalTime().ToString(Iso8601Format);
        } else { //If d is already UTC K format add 'Z' postfix, if d is LT K format add +/-TIMEOFFSET
            return dateTime.ToString(Iso8601Format);
        }
    }
    public static DateTime FromIso8601String(
       this DateTime dateTime, 
       string stringIso, 
       bool useLocal = false
    ) {
        var localDateTime = DateTime.ParseExact(stringIso, Iso8601Format, System.Globalization.CultureInfo.InvariantCulture);
        // useLocal == true -> returned localDateTime else UTC DateTime
        return useLocal ? localDateTime : localDateTime.ToUniversalTime(); 
    }
    public static DateTime FromIso8601String(this DateTime dateTime, string date, string time, bool useLocal = false) {
        //Return a new DateTime buiding an ISOFROMAT string from date, time params expressed in UTC (by default) or in LT if you set useLocal=true 
        var sb = new System.Text.StringBuilder(30);
        if (!string.IsNullOrEmpty(date)) { sb.Append(date); sb.Replace('-', '/'); }
        if (!string.IsNullOrEmpty(time)) { sb.Append(' '); sb.Append(time); }
        var s = sb.ToString();
        if (!useLocal) { //Always return DateTime Kind=UTC, if you don't pass +/-TIMEOFFSET or 'Z' postfix I'll add it by default (as needed for UTC)
            if (!(s.Contains('Z') || s.Contains('+') || s.Contains('-'))) s += "Z";
            return dateTime = DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
        } else { //Return DateTime Kind=Local and do necessary conversion to LT if you pass time with +/-TIMEOFFSET or referred as UTC with 'Z' postfix
            return dateTime = DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
        }
    }
}


public readonly struct xZonedDateTime {
   
   private readonly DateTime _utcDateTime;
   private readonly TimeZoneInfo _timeZone;

   public DateTime LocalTime     => TimeZoneInfo.ConvertTime(_utcDateTime, _timeZone);
   public DateTime UniversalTime => _utcDateTime;
   public TimeZoneInfo TimeZone  => _timeZone;

   public xZonedDateTime(DateTime localDateTime, TimeZoneInfo timeZone) {
      var dateTimeUnspec = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
      _utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone); 
      _timeZone = timeZone;

   }
}

class ZonedTimeProvider : TimeProvider  {
   private TimeZoneInfo _zoneInfo;

   public ZonedTimeProvider(TimeZoneInfo zoneInfo) : base()
   {
      _zoneInfo = zoneInfo ?? TimeZoneInfo.Local;
   }

   public override TimeZoneInfo LocalTimeZone => _zoneInfo;

   public static TimeProvider FromLocalTimeZone(TimeZoneInfo zoneInfo) =>
      new ZonedTimeProvider(zoneInfo);
}
