using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workmanager.Api;
using Workmanager.Api.Core;
using Workmanager.Api.Di;
using Workmanager.Api.Persistence;
using Workmanager.Api.Persistence.Repositories;
using Workmanager.ApiTest.Persistence;
using Workmanager.ApiTest.Persistence.Repositories;

namespace Workmanager.ApiTest.Di;

public static class DiTestPersistence {

   public static IServiceCollection AddPersistenceTest(
      this IServiceCollection services
   ) {

      // Configuration
      // Nuget:  Microsoft.Extensions.Configuration
      //       + Microsoft.Extensions.Configuration.Json
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettingsTest.json", false)
         .Build();
      services.AddSingleton<IConfiguration>(configuration);
      
      // Logging
      // Nuget:  Microsoft.Extensions.Logging
      //       + Microsoft.Extensions.Logging.Configuration
      //       + Microsoft.Extensions.Logging.Debug
      var logging = configuration.GetSection("Logging");
      services.AddLogging(builder => {
         builder.ClearProviders();
         builder.AddConfiguration(logging);
         builder.AddDebug();
      });
      
      
      
      
      
      
      // UseCases, Mapper ...
      services.AddCore();
      
      // Repository, Database ...

      services.AddSingleton<IPeopleRepository, PeopleRepository>();
      services.AddSingleton<IWorkordersRepository, WorkordersRepository>();
      services.AddSingleton<ImagesRepository, ImagesRepositoryImpl>();
      services.AddSingleton<IDataContext, DataContext>();
      
      // Add DbContext (Database) to DI-Container
      var (useDatabase, dataSource) = AppDbContext.EvalDatabaseConfiguration(configuration);
      
      switch (useDatabase) {
         case "LocalDb":
         case "SqlServer":
            services.AddDbContext<AppDbContext>(options => 
               options.UseSqlServer(dataSource)
            );
            break;
         case "MariaDb":
//          var databaseVersion = MariaDbServerVersion.AutoDetect(dataSource)
//          var version = new MariaDbServerVersion(databaseVersion);
            // services.AddDbContext<CDbContext>(options => 
            //    options.UseMySql(dataSource, version)
            // );
            break;
         case "Postgres":
            services.AddDbContext<AppDbContext>((provider, options) =>
                  options.UseNpgsql(dataSource) //, b => b.UseNodaTime())
            );
            break;
         case "Sqlite":
            services.AddDbContext<AppDbContext>(options => 
               options.UseSqlite(dataSource)
            );
            break;
         default:
            throw new Exception($"appsettings.json UseDatabase not available");
      }

//    services.AddPersistence(configuration);
      services.AddScoped<ArrangeWorkmanagerTest>();
      services.AddScoped<Seed>();
      
      return services;
   }
}