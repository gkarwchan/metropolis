using System;
using Metropolis.Api.Extensions;
using Metropolis.Common.Models;

namespace Metropolis.Api.Services.Tasks.Commands
{
    /// <summary>
    ///     Java Checkstyle parser automation
    ///     TODO: On view post this link: https://java.com/en/download/manual.jsp and then check if Java is installed or not
    /// </summary>
    public class JavaMetricsCommand : BaseMetricsCommand
    {
        private const string CheckstyleCommand = @"java -cp {0} com.puppycrawl.tools.checkstyle.Main -c {1} -f xml -o {2} {3}";

        public JavaMetricsCommand() : base(false) {}

        public override string MetricsType => "Java Checkstyle";
        public override string Extension => ".xml";

        protected override string PrepareCommand(MetricsCommandArguments args, MetricsResult result)
        {
            var cmd = CheckstyleCommand.FormatWith(
                AppDomain.CurrentDomain.BaseDirectory + "*.jar", // include all jars into the class path
                AppDomain.CurrentDomain.BaseDirectory + "metropolis_checkstyle_metrics.xml", // metropolis collection settings for checkstyle
                result.MetricsFile, // output xml file
                args.SourceDirectory // source directory to scan
                );

            return cmd;
        }

        protected override MetricsResult MetricResultFor(MetricsCommandArguments args)
        {
            throw new NotImplementedException();
        }
    }
}