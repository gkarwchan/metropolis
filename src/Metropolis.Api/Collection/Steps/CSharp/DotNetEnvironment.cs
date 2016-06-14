using Metropolis.Api.Utilities;

namespace Metropolis.Api.Collection.Steps.CSharp
{
    public class DotNetEnvironment : IDotNetEnvironment
    {
        public const string ExpectedFxCopToolPath = @"\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\FxCop\metrics.exe";
        public const string ExpectedMsBuildPath = @"\Program Files (x86)\MSBuild\14.0\Bin\\MSBuild.exe";

        private readonly IFileSystem fileSystem;

        public DotNetEnvironment() : this(new FileSystem())
        {
        }

        public DotNetEnvironment(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string FxCopMetricsToolPath => LocateTool(ExpectedFxCopToolPath);
        public string MsBuildPath => LocateTool(ExpectedMsBuildPath);

        private string LocateTool(string expectedPath)
        {
            foreach (var driveInfo in fileSystem.AllDrives)
            {
                var potentialPath = $"{driveInfo.Name}{expectedPath}";
                if (fileSystem.FileExists(potentialPath))
                    return potentialPath;
            }
            return string.Empty;
        }
    }
}