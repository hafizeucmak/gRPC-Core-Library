namespace LibraryManagement.Common.Utils
{
    public static class Guard
    {
        /// <summary>
        /// Checks for null or default objects and throws NullReferenceException on first occurrence.
        /// </summary>
        /// <param name="objects">
        /// Array of object to check and parameter name to add into the exception message
        /// string section of this parameter will be added into exception message like (myObject, objName): "objName cannot be null or empty !"
        /// </param>
        /// <returns>True if validation errors occur.</returns>
        public static bool NullOrDefault(params object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                // Null is not acceptable in any way
                if (objects[i] == null)
                {
                    return true;
                }

                // If object is a guid, Default value is not acceptable
                if (objects[i] is Guid && Guid.Parse(objects[i].ToString()) == default)
                {
                    return true;
                }

                // If object is a string, we ask for if empty strings are allowed
                if (objects[i] is string && string.IsNullOrEmpty(objects[i].ToString()))
                {
                    return true;
                }

                if (objects[i] is int && (int.Parse(objects[i].ToString()) == default))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
