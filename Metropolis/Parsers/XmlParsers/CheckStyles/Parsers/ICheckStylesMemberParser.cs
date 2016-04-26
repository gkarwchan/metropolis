using Metropolis.Domain;

namespace Metropolis.Parsers.XmlParsers.CheckStyles.Parsers
{
    public interface ICheckStylesMemberParser
    {
        string Source { get; }
        void Parse(Member member, CheckStylesItem item);
    }
}