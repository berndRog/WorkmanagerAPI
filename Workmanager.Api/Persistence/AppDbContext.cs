using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core;

namespace Workmanager.Api.Persistence; 

public class AppDbContext: DbContext  {
   // https://docs.microsoft.com/en-us/ef/core/modeling/relationships

   #region fields
   private readonly ILogger<AppDbContext> _logger = default!;
   
   public DbSet<Person> People => Set<Person>();
   public DbSet<Workorder> Workorders => Set<Workorder>();
   public DbSet<Image> Images { get; set; } = null!;
   #endregion

   public AppDbContext Context { get; } = default!;
   
   #region ctor
   // ctor for migration only
   public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
   
   public AppDbContext(
      DbContextOptions<AppDbContext> options, 
      ILoggerFactory loggerFactory
   ) : base(options) {
      _logger = loggerFactory.CreateLogger<AppDbContext>();
      Context = this;
   }
   #endregion

   #region methods
   public async Task<bool> SaveAllChangesAsync(){
      _logger?.LogDebug("\n{output}", ChangeTracker.DebugView.LongView);
  
      var result = await SaveChangesAsync();

      _logger?.LogDebug("SaveChanges {result}", result);
      _logger?.LogDebug("\n{output}", ChangeTracker.DebugView.LongView);
      return result > 0;
   }
   
   public void ClearChangeTracker(){
      ChangeTracker.Clear();
   }

   public void LogChangeTracker(string text){
      _logger?.LogDebug("{Text}\n{Tracker}",
         text, ChangeTracker.DebugView.LongView);
   }


   protected override void OnModelCreating(ModelBuilder modelBuilder) {

      base.OnModelCreating(modelBuilder);
      
      if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite") {
         // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
         // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
         // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
         // use the DateTimeOffsetToBinaryConverter
         // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
         // This only supports millisecond precision, but should be sufficient for most use cases.
         foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
            var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
               || p.PropertyType == typeof(DateTimeOffset?));
            foreach (var property in properties) {
               modelBuilder
                  .Entity(entityType.Name)
                  .Property(property.Name)
                  .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
         }
      }
      
      //
      // RELATIONS
      //
      // https://www.learnentityframeworkcore.com/configuration/one-to-many-relationship-configuration

      // Table People
      modelBuilder.Entity<Person>( entity => {
         entity.ToTable("People");                 // tablename people
         entity.HasKey(person => person.Id);       // primary key
         entity.Property(person => person.Id)      // primary key has type Guid
            .ValueGeneratedNever();                // and should never be gerated by DB  
      });
      
      // Table Workorders
      modelBuilder.Entity<Workorder>(entity => {
         entity.ToTable("Wordorders");
         entity.HasKey(workorder => workorder.Id);
         entity.Property(workorder => workorder.Id).ValueGeneratedNever();
      });
      //
      // RELATIONS
      //
      // https://www.learnentityframeworkcore.com/configuration/one-to-many-relationship-configuration
      // Has-With pattern
      // either:  one-to-many Person [1] <--> Workorders [0..*]
      modelBuilder.Entity<Person>()
         .HasMany        (person => person.Workorders)     // Person    --> Wordorder [0..*]
         .WithOne        (workorder => workorder.Person)   // Workorder --> Person       [1]
         .HasForeignKey  (workorder => workorder.PersonId)
         .HasPrincipalKey(person => person.Id)
         .OnDelete(DeleteBehavior.ClientNoAction);
      
      // or: many-to-one Account [0..*] <--> Owner [1]
      modelBuilder.Entity<Workorder>()
         .HasOne         (workorder => workorder.Person)     // Workorder --> Person    [1]
         .WithMany       (person    => person.Workorders)    // Person    --> Workorder [0..*]
         .HasForeignKey  (workorder => workorder.PersonId)
         .HasPrincipalKey(person    => person.Id)       
         .OnDelete(DeleteBehavior.ClientNoAction);
   }
   #endregion

   #region static methods
// "UseDatabase": "Sqlite",
// "ConnectionStrings": {
//    "LocalDb": "WebApi04",
//    "SqlServer": "Server=localhost,2433; Database=WebApi04; User=sa; Password=P@ssword_geh1m;",
//    "Sqlite": "WebApi04"
// },
   public static (string useDatabase, string dataSource) EvalDatabaseConfiguration(
      IConfiguration configuration
   ) {

      string useDatabase = configuration.GetSection("UseDatabase").Value ??
         throw new Exception($"UseDatabase is not available");

      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var databasePath = Path.Combine(home, "databases");
      if(! Directory.Exists(databasePath)) Directory.CreateDirectory(databasePath);

      string connectionString = configuration.GetSection("ConnectionStrings")[useDatabase!]
         ?? throw new Exception($"ConnectionStrings is not available"); 
      

      switch (useDatabase) {
         case "LocalDb":
            FileInfo fileInfoLocalDb = new FileInfo(Path.Combine(databasePath, connectionString));
            if(!fileInfoLocalDb.Exists) 
               throw new Exception($"LocalDb database {fileInfoLocalDb.FullName} not found");
            
            var dbFile = $"{Path.Combine(databasePath, connectionString)}.mdf";
            var dataSourceLocalDb =
               $"Data Source = (LocalDB)\\MSSQLLocalDB; " +
               $"Initial Catalog = {connectionString}; Integrated Security = True; " +
               $"AttachDbFileName = {fileInfoLocalDb.FullName};";
            return (useDatabase, dataSourceLocalDb);

         case "SqlServer":
            return (useDatabase, connectionString);

         case "MariaDb":
            return (useDatabase, connectionString);
         
         case "Postgres":
            return (useDatabase, connectionString);
       
         case "Sqlite":
            FileInfo fileInfo = new FileInfo(Path.Combine(databasePath, connectionString+".db"));
            var dataSourceSqlite = "Data Source=" + fileInfo.FullName;
            return (useDatabase, dataSourceSqlite);
         default:
            throw new Exception($"appsettings.json Problems with database configuration");
      }
   }
   #endregion
}