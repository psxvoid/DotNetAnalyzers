﻿using System;
using System.Collections.Immutable;
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

        private const string Category = "Readability";

        private const int MaxLineLength = 100;

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

            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.EnableConcurrentExecution();

            // the action after parsing a document
            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void SyntaxTreeAction(SyntaxTreeAnalysisContext context)
        {
            SourceText text = context.Tree.GetText();

            foreach (TextLine line in text.Lines)
            {
                int length = line.End - line.Start;

                if (length <= MaxLineLength) continue;

                var location = Location.Create(context.Tree, line.Span);

                var diagnostic =
                    Diagnostic.Create(
                        Rule,
                        location,
                        line.LineNumber + 1,
                        length - MaxLineLength);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}