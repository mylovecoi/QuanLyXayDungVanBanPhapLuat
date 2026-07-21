using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UI.Security
{
    public class SortPathsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var sortedPaths = swaggerDoc.Paths
                .OrderBy(path => path.Key.Contains("GenerateToken") ? 0 : 1)
                .ThenBy(path => path.Key)
                .ToList();

            var newPaths = new OpenApiPaths();
            foreach (var item in sortedPaths)
            {
                newPaths.Add(item.Key, item.Value);
            }
            swaggerDoc.Paths = newPaths;
        }
    }
}
