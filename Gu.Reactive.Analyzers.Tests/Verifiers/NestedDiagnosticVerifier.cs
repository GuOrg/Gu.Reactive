namespace Gu.Reactive.Analyzers.Tests.Verifiers
{
    using System.Threading.Tasks;
    using Gu.Reactive.Analyzers.Tests.Verifiers.Helpers;

    public abstract class NestedDiagnosticVerifier<T>
        where T : DiagnosticVerifier, new()
    {
        private readonly T parent = new T();

        public DiagnosticResult CSharpDiagnostic()
        {
            return this.parent.CSharpDiagnostic();
        }

        public Task VerifyCSharpDiagnosticAsync(string testCode, DiagnosticResult expected)
        {
            return this.parent.VerifyCSharpDiagnosticAsync(testCode, expected);
        }

        public Task VerifyCSharpDiagnosticAsync(string[] testCode, DiagnosticResult expected)
        {
            return this.parent.VerifyCSharpDiagnosticAsync(testCode, expected);
        }
    }
}