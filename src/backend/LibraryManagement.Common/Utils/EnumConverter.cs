using System.Linq.Expressions;

namespace LibraryManagement.Common.Utils
{
    public static class EnumConverter<TEnum>
    {
        public static Expression<Func<TEnum, string>> EnumToString => value => Enum.GetName(typeof(TEnum), value);

        public static Expression<Func<string, TEnum>> StringToEnum => value => (TEnum)Enum.Parse(typeof(TEnum), value, true);
    }
}
