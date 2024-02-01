namespace LibraryManagement.WebApi.Swagger
{
    public class SwashbuckleSchemaHelper
    {
        private string DefaultSchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType)
            {
                return modelType.Name;
            }

            string prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }

        public string GetSchemaId(Type modelType)
        {
            var defaultId = DefaultSchemaIdSelector(modelType);
            var prefix = modelType.Namespace?.Split(".").Last();
            return defaultId;
        }
    }
}
