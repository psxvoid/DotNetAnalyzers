using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace TestHelper
{
    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    public struct DiagnosticResult : IEquatable<DiagnosticResult>
    {
        private DiagnosticResultLocation[] locations;

        /// <summary>
        /// Gets or sets the collection of locations where code issues detected.
        /// </summary>
        public DiagnosticResultLocation[] Locations
        {
            get
            {
                return this.locations ?? (this.locations = Array.Empty<DiagnosticResultLocation>());
            }

            set
            {
                this.locations = value;
            }
        }

        /// <summary>
        /// Gets or sets the severity of the found issues.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the id of the diagnostic.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message of the diagnostic issue.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the path to a file where issues are found.
        /// </summary>
        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : string.Empty;
            }
        }

        /// <summary>
        /// Gets the line number where the issue is found.
        /// </summary>
        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        /// <summary>
        /// Gets the column number where the issue is found.
        /// </summary>
        public int Column
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Column : -1;
            }
        }

        /// <summary>
        /// Compares to <see cref="DiagnosticResult"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="DiagnosticResult"/>.</param>
        /// <param name="right">The second <see cref="DiagnosticResult"/>.</param>
        /// <returns>
        /// <see langword="true"/> when both results are equal, else <see langword="false"/>.
        /// </returns>
        public static bool operator ==(DiagnosticResult left, DiagnosticResult right) =>
            left.Equals(right);

        /// <summary>
        /// Compares to <see cref="DiagnosticResult"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="DiagnosticResult"/>.</param>
        /// <param name="right">The second <see cref="DiagnosticResult"/>.</param>
        /// <returns>
        /// <see langword="false"/> when both results are equal, else <see langword="true"/>.
        /// </returns>
        public static bool operator !=(DiagnosticResult left, DiagnosticResult right) =>
            !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is DiagnosticResult other)
            {
                return
                    this.Id == other.Id &&
                    this.Severity == other.Severity &&
                    this.Line == other.Line &&
                    this.Column == other.Column &&
                    this.Message == other.Message &&
                    this.Path == other.Path &&
                    this.Locations.Length == other.Locations.Length &&
                    this.Locations.SequenceEqual(other.Locations);
            }

            return object.Equals(this, obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + this.Id.GetHashCode(StringComparison.InvariantCulture);
                hash = (hash * 23) + this.Severity.GetHashCode();
                hash = (hash * 23) + this.Line.GetHashCode();
                hash = (hash * 23) + this.Column.GetHashCode();
                hash = (hash * 23) + this.Message.GetHashCode(StringComparison.InvariantCulture);
                hash = (hash * 23) + this.Path.GetHashCode(StringComparison.InvariantCulture);
                hash = (hash * 23) + this.Locations.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc/>
        public bool Equals(DiagnosticResult other)
        {
            return this.Equals((object)other);
        }
    }

    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public struct DiagnosticResultLocation : IEquatable<DiagnosticResultLocation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticResultLocation"/> struct.
        /// </summary>
        /// <param name="path">See <see cref="Path"/>.</param>
        /// <param name="line">See <see cref="Line"/>.</param>
        /// <param name="column">See <see cref="Column"/>.</param>
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets the path to a file where issues are found.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the line number where the issue is found.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column number where the issue is found.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Compares to <see cref="DiagnosticResultLocation"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="DiagnosticResultLocation"/>.</param>
        /// <param name="right">The second <see cref="DiagnosticResultLocation"/>.</param>
        /// <returns>
        /// <see langword="true"/> when both results are equal, else <see langword="false"/>.
        /// </returns>
        public static bool operator ==(DiagnosticResultLocation left, DiagnosticResultLocation right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares to <see cref="DiagnosticResultLocation"/>s.
        /// </summary>
        /// <param name="left">The first <see cref="DiagnosticResultLocation"/>.</param>
        /// <param name="right">The second <see cref="DiagnosticResultLocation"/>.</param>
        /// <returns>
        /// <see langword="false"/> when both results are equal, else <see langword="true"/>.
        /// </returns>
        public static bool operator !=(DiagnosticResultLocation left, DiagnosticResultLocation right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is DiagnosticResultLocation other)
            {
                return
                    this.Line == other.Line &&
                    this.Column == other.Column &&
                    this.Path == other.Path;
            }

            return object.ReferenceEquals(this, obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + this.Line.GetHashCode();
                hash = (hash * 23) + this.Column.GetHashCode();
                hash = (hash * 23) + this.Path.GetHashCode(StringComparison.InvariantCulture);
                return hash;
            }
        }

        /// <inheritdoc/>
        public bool Equals(DiagnosticResultLocation other)
        {
            return this.Equals((object)other);
        }
    }
}
