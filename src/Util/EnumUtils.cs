
namespace kms.Util
{

    using NStack;

    /// <summary>
    /// Enum utilities.
    /// </summary>
    public static class EnumUtils
    {

        public static TEnum FromString<TEnum>(string value) where TEnum : struct, Enum =>
            Enum.Parse<TEnum>(value, true);

        public static ustring[] GetNamesList(Type value) =>
            [.. Enum.GetNames(value).ToList().Select(name => ustring.Make(name))];

    }
}