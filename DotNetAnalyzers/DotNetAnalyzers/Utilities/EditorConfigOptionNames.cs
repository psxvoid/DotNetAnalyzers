using DotNetAnalyzers;

namespace Analyzer.Utilities
{
    /// <summary>
    /// Option names to configure analyzer execution through an .editorconfig file.
    /// </summary>
    internal static partial class EditorConfigOptionNames
    {
        /// <summary>
        /// Configures line length limit. When this option isn't specified or set to 0 then
        /// a line length won't be verified by <see cref="LineLengthAnalyzer"/>. When this option
        /// is set to a value greater than 0 then the value will be used to ensure that all line
        /// length do not exceed that value.
        /// <para>Notice: the options should be set in .editorconfig using 'dotnet_code_quality.line_length_limit'."</para>
        /// </summary>
        /// <example>
        /// <code>dotnet_code_quality.line_length_limit: 100</code>
        /// </example>
        public const string LineLengthLimit = "line_length_limit";
    }
}
