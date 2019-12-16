namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using Xunit;
    using PublicApiGenerator;
    using System.IO;

    public class AssemblyEvolutionReporter
    {
        [Fact(Skip = "Only run this at the end of a refactoring session to see how the API has been affected.")]
        public void WriteLatestVersion()
        {
            var assembly = typeof(AnonymousDbaseRecord).Assembly;
            var report = assembly.GeneratePublicApi();
            var path =
                ".." + Path.DirectorySeparatorChar +
                ".." + Path.DirectorySeparatorChar +
                ".." + Path.DirectorySeparatorChar +
                ".." + Path.DirectorySeparatorChar +
                ".." + Path.DirectorySeparatorChar +
                "versions" + Path.DirectorySeparatorChar +
                "latest.txt";
            File.WriteAllText(path, report);
        }
    }
}
