using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Hosting.Infrastructure;
using Workmanager.Api.Di;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Workmanager.Api.Core.Misc;

namespace Workmanager.Api;

public static class ExtensionMethods {

   // Extension method
   public static WebApplicationBuilder ConfigureServices(
      this WebApplicationBuilder builder
   ){
      // Builder Pattern
      //
      // LOGGING
      //
      // Add Logging Services to DI-Container
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
      // builder.Logging.AddEventLog();
      // builder.Logging.AddEventSourceLogger();

      // Write Logging to Debug into a file
      // Windows C:\users\<username>\appdata\local
      // Mac       /users/<username>\.local/share
      // var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); 
      // var tracePath = Path.Join(path, $"Log_WebApi_{DateTime.Now.ToString("yyyy_MM_dd-HHmmss")}.txt");
      // Trace.Listeners.Add(new TextWriterTraceListener(File.Create(tracePath)));
      // Trace.AutoFlush = true;

      //
      // CONTROLLERS, CORE, PERSISTENCE
      //
      // Add the Hosting Enviroment to DI-Container,( from .NET 8 this is already part of the Di Container)
      
      // Add Controllers to DI-Container
      builder.Services.AddControllers()
         .AddJsonOptions(opt => {
            opt.JsonSerializerOptions.Converters.Add(new AppIsoDateTimeConverter());
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
         });
      // Add core objects to Di-Container
      builder.Services.AddCore();
      // Add Persistence DI-Container
      builder.Services.AddPersistence(builder.Configuration);
      
      // Add Error handling DI-Container
      builder.Services.AddProblemDetails();

      // Add Cors 
      
      
      // builder.Services.AddCors(options => {
      //    options.AddDefaultPolicy(policy => {
      //       policy.WithOrigins(
      //          "http://localhost", 
      //          "http://localhost:5010",
      //          "http://localhost:5042",
      //          "https://localhost:7003", 
      //          "http://localhost:63343"
      //       ).AllowAnyMethod().AllowAnyHeader();
      //    });
      // });

      builder.Services.AddCors(options => {
         options.AddDefaultPolicy(
            policy => {
               policy.WithOrigins(
                     "http://localhost:5010", 
                     "http://localhost:5042"
               ).AllowAnyHeader()
                .AllowAnyMethod();
            });
      });

      
      builder.Services.Configure<FormOptions>(options => {
         // Set the limit to 256 MB
         options.MultipartBodyLengthLimit = 268435456;
      });

      //
      // SWAGGER/OPEN API
      //
      // Add API Versioning to DI-Container
      //  
      builder.Services.AddApiVersioning(opt => {
         opt.DefaultApiVersion = new ApiVersion(1, 0);
         opt.AssumeDefaultVersionWhenUnspecified = true;
         opt.ReportApiVersions = true;
         opt.ApiVersionReader = new UrlSegmentApiVersionReader();
         //   ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
         //                            new HeaderApiVersionReader("x-api-version"),
         //                            new MediaTypeApiVersionReader("x-api-version"),
         //                            new QueryStringApiVersionReader("api-version"));
      })
      // Add MVC to DI-Container   
      .AddMvc()
      // Add Swagger with different versions
      .AddApiExplorer(options => {
         options.GroupNameFormat = "'v'VVV";
         options.SubstituteApiVersionInUrl = true;
      });
      
      // Add Swagger to DI-Container
      builder.Services.AddSwaggerGen( options => {
         //  var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
         //  var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
         //  setupActions.IncludeXmlComments(filePath);
         var dir = new DirectoryInfo(AppContext.BaseDirectory);
         // combine WebApi.Controllers.xml and WebApi.Core.xml
         foreach (var file in dir.EnumerateFiles("*.xml")) {
            options.IncludeXmlComments(file.FullName);
         }
      });
      builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
      
      return builder;
   }

   public static void IsDbConfigurationAvalible(IConfiguration configuration){
      // /users/rogallab/documents
      string useDatabase = configuration.GetSection("UseDatabase").Value ??
         throw new Exception($"UseDatabase is not available");
      _ = configuration.GetSection("ConnectionStrings")[useDatabase]
         ?? throw new Exception($"ConnectionStrings is not available");

   }
}