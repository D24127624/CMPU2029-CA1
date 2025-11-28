
namespace kms.Util
{

    /// <summary>
    /// String utilities.
    /// </summary>
    public static class StringUtils
    {

        public static readonly string APPLICATION_NAME = "Kennel Management System";

        public static string GetTitle(string label) => $"{APPLICATION_NAME} -- {label}";

        public static string RandomUuid() => $"{Guid.NewGuid()}";

        public static string RandomString(int length) => RandomUuid().Replace("-", "")[length..];

        public static bool AllNullOrWhiteSpace(string?[] values)
        {
            foreach (string? arg in values)
            {
                if (!String.IsNullOrWhiteSpace(arg)) return false;
            }
            return true;
        }

        public static bool AnyNullOrWhiteSpace(string?[] values)
        {
            foreach (string? arg in values)
            {
                if (String.IsNullOrWhiteSpace(arg)) return true;
            }
            return false;
        }

        public static string? ValueOrNull(string? value) =>
            AllNullOrWhiteSpace([value]) ? null : $"{value}".Trim();

    }

}
