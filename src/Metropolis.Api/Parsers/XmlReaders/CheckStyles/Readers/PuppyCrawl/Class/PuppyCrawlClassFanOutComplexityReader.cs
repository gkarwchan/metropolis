using Metropolis.Api.Extensions;

namespace Metropolis.Api.Parsers.XmlReaders.CheckStyles.Readers.PuppyCrawl.Class
{
    public class PuppyCrawlClassFanOutComplexityReader : CheckStyleBaseReader, ICheckStylesClassParser
    {
        public override string Source => PuppyCrawlSources.ClassFanOutComplexity;
        
        public void Parse(Domain.Instance type, CheckStylesItem item)
        {
            type.ClassFanOutComplexity= IntParser.Match(item.Message).Value.AsInt();
        }
    }
}