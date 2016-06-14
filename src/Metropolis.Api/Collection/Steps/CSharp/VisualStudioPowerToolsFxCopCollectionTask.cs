using System.IO;
using Metropolis.Api.Collection.PowerShell;
using Metropolis.Api.Utilities;
using Metropolis.Common.Extensions;
using Metropolis.Common.Models;

namespace Metropolis.Api.Collection.Steps.CSharp
{
    public class FxCopCollectionTask : IFxCopCollectionTask
    {
        public const string CommandTemplate =
            @"&'{0}'/f:'{1}' /o:'{2}' ";

        private readonly IFileSystem fileSystem;
        private readonly IDotNetEnvironment dotNetEnvironment;

        private readonly IRunPowerShell powerShell;

        public FxCopCollectionTask() : this(new RunPowerShell(), new FileSystem(), new DotNetEnvironment())
        {
        }

        public FxCopCollectionTask(IRunPowerShell powerShell, IFileSystem fileSystem, IDotNetEnvironment dotNetEnvironment)
        {
            this.powerShell = powerShell;
            this.fileSystem = fileSystem;
            this.dotNetEnvironment = dotNetEnvironment;
        }

        public MetricsResult Run(MetricsCommandArguments args, string targetdll)
        {
            var result = new MetricsResult {ParseType = ParseType.FxCop, MetricsFile = GetMetricsOutputFileName(args, targetdll)};
            var command = CommandTemplate.FormatWith(dotNetEnvironment.FxCopMetricsToolPath, targetdll, result.MetricsFile);

            powerShell.Invoke(command);
            return result;
        }

        private string GetMetricsOutputFileName(MetricsCommandArguments args, string targetdll)
        {
            var assemblyName = fileSystem.GetFileName(targetdll).Replace(".dll", string.Empty);
            return Path.Combine(args.MetricsOutputFolder, $"{args.ProjectName}_{assemblyName}_metrics.xml");
        }
    }
}