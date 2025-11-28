

namespace kms.Util
{

    /// <summary>
    /// Parser for AUTHOR assembly attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class AuthorAttribute(string author) : Attribute
    {

        public string Author { get; } = author.Replace("\"", "");

    }

    /// <summary>
    /// Parser for SOURCE-URL assembly attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class SourceUrlAttribute(string sourceUrl) : Attribute
    {

        public string SourceUrl { get; } = sourceUrl.Replace("\"", "");

    }

    /// <summary>
    /// Parser for STUDENT-ID assembly attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class StudentIdAttribute(string studentId) : Attribute
    {

        public string StudentId { get; } = studentId.Replace("\"", "");

    }

}
