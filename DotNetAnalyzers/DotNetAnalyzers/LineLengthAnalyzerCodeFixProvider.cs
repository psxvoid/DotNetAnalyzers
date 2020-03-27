using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace DotNetAnalyzers
{
    /// <summary>
    /// This class is responsible for fixing an issue reported by <see cref="LineLengthAnalyzer"/>.
    /// </summary>
    // TODO: implement line length code fix
    // [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LineLengthAnalyzerCodeFixProvider))]
    // [Shared]
    public class LineLengthAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Make uppercase";

        /// <inheritdoc/>
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LineLengthAnalyzer.DiagnosticId); }
        }

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            Diagnostic diagnostic = context.Diagnostics[0];
            Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            TypeDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedSolution: c => this.MakeUppercaseAsync(context.Document, declaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            SyntaxToken identifierToken = typeDecl.Identifier;
            string newName = identifierToken.Text.ToUpperInvariant();

            // Get the symbol representing the type to be renamed.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            // Produce a new solution that has all references to that type renamed, including the declaration.
            Solution originalSolution = document.Project.Solution;
            Microsoft.CodeAnalysis.Options.OptionSet optionSet = originalSolution.Workspace.Options;
            Solution newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }
    }
}
