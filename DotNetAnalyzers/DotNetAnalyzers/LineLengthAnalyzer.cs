using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Analyzer.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace DotNetAnalyzers
{
    /// <summary>
    /// <para>To Do:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>configure line length</description>
    /// </item>
    /// <item>
    /// <description>configure "ignore comments"</description>
    /// </item>
    /// <item>
    /// <description>configure "ignore http/https links in comments" (default: true)</description>
    /// </item>
    /// <item>
    /// <description>configure "ignore string constants"</description>
    /// </item>
    /// <item>
    /// <description>configure "ignore line breaks" (default: true)</description>
    /// </item>
    /// </list>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LineLengthAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The id that uniquely identifies this analyzer.
        /// </summary>
        public const string DiagnosticId = "LineLengthAnalyzer";

        private const string EditorConfigName = "line_length_limit";

        private const string Category = "Readability";

        private const uint DefaultMaxLineLength = 100U;

        // You can change these strings in the Resources.resx file.
        // If you do not want your analyzer to be localize-able,
        // you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly LocalizableString Title =
            new LocalizableResourceString(
                nameof(Resources.LineLengthAnalyzerTitle),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(
                nameof(Resources.LineLengthAnalyzerMessageFormat),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(
                nameof(Resources.LineAnalyzerDescription),
                Resources.ResourceManager,
                typeof(Resources));

        private static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(
                DiagnosticId,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // the action after parsing a document
            context.RegisterSyntaxTreeAction((ctx) =>
            {
                uint limit = ctx.Options.GetUnsignedIntegralOptionValue(
                    optionName: EditorConfigOptionNames.LineLengthLimit,
                    rule: Rule,
                    defaultValue: 0U,
                    cancellationToken: ctx.CancellationToken);

                SyntaxTreeAction(ctx, limit);
            });
        }

        private static void SyntaxTreeAction(SyntaxTreeAnalysisContext context, uint maxLineLength)
        {
            if (maxLineLength == 0
                || !Regex.Match(context.Tree.FilePath, @"\.cs$").Success
                || Regex.Match(context.Tree.FilePath, @"GlobalSuppressions\.cs$").Success) return;

            SourceText text = context.Tree.GetText();

            foreach (TextLine line in text.Lines)
            {
                int length = line.End - line.Start;

                if (length <= maxLineLength) continue;

                var location = Location.Create(context.Tree, line.Span);

                var diagnostic =
                    Diagnostic.Create(
                        Rule,
                        location,
                        line.LineNumber + 1,
                        maxLineLength,
                        length - maxLineLength);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
