using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DotNetAnalyzers.Test
{
    /// <summary>
    /// Unit tests for <see cref="LineLengthAnalyzer"/>
    /// and <see cref="LineLengthAnalyzerCodeFixProvider"/>.
    /// </summary>
    [TestClass]
    public class LineAnalyzerUnitTests : CodeFixVerifier
    {
        private const string ClassContainingOneLineExceededLineLengthLimit = @"
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            class TypeName
            {   
                void Test()
                {
                    Console.WriteLine(""This line is fine."");
                    Console.WriteLine(""This line has a very long message and should be marked as exceeding max length.\"");
                }
            }
        }";

        /// <summary>
        /// No diagnostics expected to show up.
        /// </summary>
        [TestMethod]
        public void NoMessagesShouldBeGeneratedWhenTheSourceFileIsEmpty()
        {
            this.VerifyCSharpDiagnostic(string.Empty);
        }

        /// <summary>
        /// No diagnostics expected to show up.
        /// </summary>
        [TestMethod]
        public void NoMessagesShouldBeGeneratedWhenLineLengthIsNotExceedTheMaximumValue()
        {
            const string test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication1
            {
                class TypeName
                {   
                    void Test()
                    {
                        Console.WriteLine(""This line is fine."");
                    }
                }
            }";

            this.VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Diagnostic should be triggered and checked.
        /// </summary>
        [TestMethod]
        public void ShouldReturnCorrectWarningMessageWhenLineLengthLimitExceeded()
        {
            var expected = new DiagnosticResult
            {
                Id = "LineLengthAnalyzer",
                Message = "Line '16' exceeded the configured maximum length by '22' characters",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 16, 22) },
            };

            this.VerifyCSharpDiagnostic(ClassContainingOneLineExceededLineLengthLimit, expected);
        }

        /// <summary>
        /// CodeFix both should fix the error.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void ShouldReduceLineLengthWhenLineLengthLimitExceeded()
        {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            string fixedVersion = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication1
            {
                class TYPENAME
                {   
                }
            }";
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            // TODO: to fix the fix
            // VerifyCSharpFix(test, fixedVersion);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new LineLengthAnalyzerCodeFixProvider();
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new LineLengthAnalyzer();
        }
    }
}
