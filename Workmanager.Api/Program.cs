using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore.Design;
using Workmanager.Api.Core.Misc;

namespace Workmanager.Api;
/*
 builder.Services.AddCors siehe ExtensionMethods
 */
public class Programm {
   
   static void Main(string[] args){

      // path for WebApi images: ~/WorkmanagerApi/images
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var wwwroot = Path.Combine(home, "WorkmanagerApi");
      
      //
      // Configure Web-App/Api
      //
      WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
      builder.ConfigureServices();
      builder.Environment.WebRootPath = wwwroot; // path to images;
      
      var app = builder.Build();
      
      //
      // Configure Http Pipeline
      //

      // Set the path base
      // app.UsePathBase("/workmanagerapi");
      
      // use middleware to handle errors
      if (app.Environment.IsDevelopment())
         app.UseExceptionHandler("/workmanagerapi/error-development");
      else
         app.UseExceptionHandler("/workmanagerapi/error");

      // API Versioning, OpenAPI/Swagger documentation
      IApiVersionDescriptionProvider provider =
         app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
      if(app.Environment.IsDevelopment()){
         app.UseSwagger();
         app.UseSwaggerUI(options => {
            foreach(var description in provider.ApiVersionDescriptions){
               options.SwaggerEndpoint(
                  $"/swagger/{description.GroupName}/swagger.json",
                  description.GroupName.ToUpperInvariant());
            }
         });
      }

      app.UseCors();
      
      app.UseRouting();
      // app.UseAuthentication();
      // app.UseAuthorization();
      app.MapControllers();
      app.Run();
   }
}