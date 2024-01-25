using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workmanager.Api.Core;
using Workmanager.Api.Persistence;
using Workmanager.Api.Persistence.Repositories;
using Workmanager.Api.Controllers;

namespace Workmanager.Api.Di;

public static class DiPersistence {
   public static IServiceCollection AddPersistence(
      this IServiceCollection services,
      IConfiguration configuration,
      bool isTest = false
   ){
      
      services.AddScoped<IPeopleRepository, PeopleRepository>();
      services.AddScoped<IWorkordersRepository, WorkordersRepository>();
      services.AddScoped<ImagesRepository, ImagesRepositoryImpl>();
      
      services.AddScoped<IDataContext, DataContext>();
      
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
      
      
      return services;

   }
}