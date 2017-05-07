namespace Gu.Reactive.Analyzers.Tests.Verifiers
{
    using System.Threading.Tasks;

    public interface IHappyPathVerifier
    {
        Task VerifyHappyPathAsync(params string[] testCode);
    }
}