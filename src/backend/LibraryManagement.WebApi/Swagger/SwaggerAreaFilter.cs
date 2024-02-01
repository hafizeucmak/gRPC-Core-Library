// <copyright file="SwaggerAreaFilter.cs" company="Oplog">
// Copyright (c) Oplog. All rights reserved.
// </copyright>

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace LibraryManagement.WebApi.Swagger
{
    public class SwaggerAreaFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        private readonly string[] _enabledAreas;

        public SwaggerAreaFilter(params string[] enabledAreas)
        {
            _enabledAreas = enabledAreas;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var disabledPaths = new List<string>();
            foreach (var item in swaggerDoc.Paths)
            {
                string path = GetRootPath(item.Key).ToLower();
                if (!_enabledAreas.Contains(path))
                {
                    disabledPaths.Add(item.Key);
                }
            }

            foreach (string disabledPath in disabledPaths)
            {
                swaggerDoc.Paths.Remove(disabledPath);
            }
        }

        private string GetRootPath(string apiPath)
        {
            if (string.IsNullOrEmpty(apiPath))
            {
                return string.Empty;
            }

            string[] splittedPath = apiPath.Split('/');
            if (splittedPath.Length <= 1)
            {
                return splittedPath[0];
            }

            return splittedPath[1];
        }
    }
}