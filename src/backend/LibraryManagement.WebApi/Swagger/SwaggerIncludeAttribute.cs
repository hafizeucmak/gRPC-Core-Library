// <copyright file="SwaggerIncludeAttribute.cs" company="Oplog">
// Copyright (c) Oplog. All rights reserved.
// </copyright>

using System;

namespace LibraryManagement.WebApi.Swagger
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SwaggerIncludeAttribute : Attribute { }
}