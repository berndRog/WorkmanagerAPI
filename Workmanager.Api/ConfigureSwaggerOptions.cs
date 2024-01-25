
using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Workmanager.Api; 

public class ConfigureSwaggerOptions: IConfigureNamedOptions<SwaggerGenOptions> {
   
   #region fields
   private readonly IApiVersionDescriptionProvider _provider;
   #endregion
   
   #region ctor
   public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
      _provider = provider;
   #endregion

   #region methods
   public void Configure(SwaggerGenOptions options){
      // add swagger document for every API version discovered
      foreach(var description in _provider.ApiVersionDescriptions){
         options.SwaggerDoc(
            description.GroupName,
            CreateVersionInfo(description));
      }
   }
   public void Configure(string? name, SwaggerGenOptions options)
      => Configure(options);
   
   private static OpenApiInfo CreateVersionInfo(
      ApiVersionDescription description
   ){
      var info = new OpenApiInfo(){
         Title = "WorkmanagerApi",
         Description = "Verwaltung von Arbeitsaufträgen",
         Version = description.ApiVersion.ToString()
      };
      if(description.IsDeprecated)
         info.Description += " This API version has been deprecated.";

      return info;
   }
   #endregion
}