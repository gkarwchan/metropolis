﻿using System.Text.RegularExpressions;
using Metropolis.Domain;
using Metropolis.Extensions;
using Metropolis.Parsers.XmlParsers.CheckStyles.CheckStylesMemberParsers;
using Metropolis.Parsers.XmlParsers.CheckStyles.CheckStylesMemberParsers.EsLint;

namespace Metropolis.Parsers.XmlParsers.CheckStyles.Parsers.EsLint
{
    public class EsLintComplexityParser : CheckStyleBaseParser, ICheckStylesMemberParser
    {
        private readonly Regex nameRegex = new Regex("'",RegexOptions.IgnorePatternWhitespace|RegexOptions.Compiled);

        public override string Source => EslintSources.Complexity;

        public EsLintComplexityParser() : base(IntRegex)
        {
        }

        public void Parse(Member member, CheckStylesItem item)
        {
            member.Name = nameRegex.Split(item.Message)[1];
            member.CylomaticComplexity = Parser.Match(item.Message).Value.AsInt();
        }
    }
}