using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Enums;
using Workmanager.Api.Core.Misc;

namespace Workmanager.ApiTest;

public class Seed {

   #region fields
   public Person Person1{ get; set; }
   public Person Person2{ get; set; }
   public Person Person3{ get; set; }
   public Person Person4{ get; set; }
   public Person Person5{ get; set; }
   public Person Person6{ get; set; }
   public Person Person7{ get; set; }
   public Person Person8{ get; set; }
   public Person Person9{ get; set; }
   public Person Person10{ get; set; }
   
   public Workorder Workorder01{ get; set; }
   public Workorder Workorder02{ get; set; }
   public Workorder Workorder03{ get; set; }
   public Workorder Workorder04{ get; set; }
   public Workorder Workorder05{ get; set; }
   public Workorder Workorder06{ get; set; }
   public Workorder Workorder07{ get; set; }
   public Workorder Workorder08{ get; set; }
   public Workorder Workorder09{ get; set; }
   public Workorder Workorder10{ get; set; }
   
   // not serialized
   public List<Person> People{ get; private set; } 
   public List<Workorder> Workorders{ get; private set; } 
   #endregion

   public Seed(){

      #region People
      Person1 = new(){
         Id = new Guid("01000000-0000-0000-0000-000000000000"),
         FirstName ="Arne",
         LastName = "Arndt",
         Email = "a.arndt@t-online.de",
         Phone = "05123 1234 5678"
      };
      Person2 = new(){
         Id = new Guid("02000000-0000-0000-0000-000000000000"),
         FirstName ="Berta",
         LastName = "Bauer",
         Email = "b.bauer@gmx.de",
         Phone = "05234 2345 6789" 
      };
      Person3 = new(){
         Id = new Guid("03000000-0000-0000-0000-000000000000"),
         FirstName ="Cord",
         LastName = "Conrad",
         Email = "c.conrad@icloud.com",
         Phone = "05826 3456 7890"
      };
      Person4 = new(){
         Id = new Guid("04000000-0000-0000-0000-000000000000"),
         FirstName ="Dagmar",
         LastName = "Diehl",
         Email = "d.diehl@gmail.com",
         Phone = "05456 4567 8901"
      };
      Person5 = new(){
         Id = new Guid("05000000-0000-0000-0000-000000000000"),
         FirstName ="Ernst",
         LastName = "Engel",
         Email = "e.engel@freenet.de",
         Phone = "05567 5678 9012"
      };
      Person6 = new(){
         Id = new Guid("06000000-0000-0000-0000-000000000000"),
         FirstName ="Frieda",
         LastName = "Fischer",
         Email = "f.fischer@t-online.de",
         Phone = "05789 6789 0123"
      };
      Person7 = new(){
         Id = new Guid("07000000-0000-0000-0000-000000000000"),
         FirstName ="Günter",
         LastName = "Grabe",
         Email = "g.grabe@gmx.de",
         Phone = "05826 7890 1234"
      };
      Person8 = new(){
         Id = new Guid("08000000-0000-0000-0000-000000000000"),
         FirstName ="Hanna",
         LastName = "Hoffmann",
         Email = "h.hoffmann@icloud.com",
         Phone = "05890 8901 2345"
      };
      Person9 = new(){
         Id = new Guid("09000000-0000-0000-0000-000000000000"),
         FirstName ="Ingo",
         LastName = "Imhof",
         Email = "i.imhof@gmail.de",
         Phone = "05826 9012 3456"
      };
      Person10 = new(){
         Id = new Guid("10000000-0000-0000-0000-000000000000"),
         FirstName ="Johanna",
         LastName = "Jung",
         Email = "j.jung@gmx.de",
         Phone = "05826 0123 4567"
      };
      #endregion
      
      #region Workorders
      Workorder01 = new(){
         Id = new Guid("01000000-0000-0000-0000-000000000000"),
         Title = "Rasenmähen, 500 m2",
         Description = "Mähen, entsoregen, düngen",
         Created = DateTimeExt.FromIso8601String("2023-12-01T08:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-01T08:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-01T12:00:00.000Z"),
         Duration = new TimeSpan(),
      };
      Workorder02 = new(){
         Id = new Guid("02000000-0000-0000-0000-000000000000"),
         Title = "6 Büsche schneiden und entsorgen",
         Description = "ggf. Büsche neu pflanzen",
         Created = DateTimeExt.FromIso8601String("2023-12-02T09:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-02T09:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-02T09:00:00.000Z"),
      };
      Workorder03 = new(){
         Id = new Guid("03000000-0000-0000-0000-000000000000"),
         Title = "1 Baum fällen und entsorgen",
         Description = "Kirschbaum, Höhe ca. 8 m",
         Created = DateTimeExt.FromIso8601String("2023-12-03T10:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-03T10:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-03T10:00:00.000Z"),
      };
      Workorder04 = new(){
         Id = new Guid("04000000-0000-0000-0000-000000000000"),
         Title = "Rasenmähen 1200 m2",
         Description = "Am Kindergarten. 1, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-04T11:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-04T11:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-04T11:00:00.000Z")
      };
      Workorder05 = new(){
         Id = new Guid("05000000-0000-0000-0000-000000000000"),
         Title = "5 Büsche schneiden, Unkraut entfernen",
         Description = "Lerchenweg 1, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-05T12:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-05T12:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-05T12:00:00.000Z")
      };
      Workorder06 = new(){
         Id = new Guid("06000000-0000-0000-0000-000000000000"),
         Title = "Tulpen und Narzissen pflanzen",
         Description = "Spechtstr. 1, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-06T13:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-06T13:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-06T13:00:00.000Z")
      };
      Workorder07 = new(){
         Id = new Guid("07000000-0000-0000-0000-000000000000"),
         Title = "Wegpflaster aufnehmen und neu verlegen, 30 m2",
         Description = "Hauptstr. 1, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-07T14:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-07T14:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-07T14:00:00.000Z")
      };
      Workorder08 = new(){
         Id = new Guid("08000000-0000-0000-0000-000000000000"),
         Title = "4 Baume schneiden, Unkraut entfernen",
         Description = "Lindenstr. 1, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-08T15:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-08T15:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-08T15:00:00.000Z")
      };
      Workorder09 = new(){
         Id = new Guid("09000000-0000-0000-0000-000000000000"),
         Title = "4 Baume schneiden, Unkraut entfernen",
         Description = "Waldstarße. 25, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-09T16:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-09T16:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-09T16:00:00.000Z")
      };
      Workorder10 = new(){
         Id = new Guid("10000000-0000-0000-0000-000000000000"),
         Title = "Teich reinigen und neu bepflanzen, ca. 25 m2",
         Description = "Olderdorferstr. 13, 29556 Suderburg",
         Created = DateTimeExt.FromIso8601String("2023-12-10T17:00:00.000Z"),
         Started = DateTimeExt.FromIso8601String("2023-12-10T17:00:00.000Z"),
         Completed = DateTimeExt.FromIso8601String("2023-12-10T17:00:00.000Z")
      };

      #endregion

      People = new List<Person>{ Person1, Person2, Person3, Person4, Person5, Person6, 
         Person7, Person8, Person9, Person10 };

      Workorders = new List<Workorder>{ Workorder01, Workorder02, Workorder03, 
         Workorder04, Workorder05, Workorder06, Workorder07, Workorder08, Workorder09,Workorder10 };

   }

   public Seed InitPeopleWithWordorders(){
      Person1.AndOrUpdateWorkorder(Workorder01); // Person 1 with workorders 1+2
      Person1.AndOrUpdateWorkorder(Workorder02); 
      Person2.AndOrUpdateWorkorder(Workorder03); // Person 2 witn workorder 3
                                                 // Person 3 without workorders  
      Person4.AndOrUpdateWorkorder(Workorder04); // Person 4 with workorder 4
      Person5.AndOrUpdateWorkorder(Workorder05); // Person 5 with workorder 5
      Person6.AndOrUpdateWorkorder(Workorder06); // Person 6 with workorders 6+7
      Person6.AndOrUpdateWorkorder(Workorder07); 
                                                 // Person 7 without workorders
      Person8.AndOrUpdateWorkorder(Workorder08); // Person 8 with workorder 8+9+10 
      Person8.AndOrUpdateWorkorder(Workorder09); 
      Person8.AndOrUpdateWorkorder(Workorder10); 
      return this;
   }
}