﻿using System;
using System.Collections.Generic;
using Metropolis.Api.Readers.CsvReaders;
using Metropolis.Api.Readers.XmlReaders.CheckStyles;
using Metropolis.Common.Models;

namespace Metropolis.Api.Readers
{
    public class MetricsReaderFactory : IMetricsReaderFactory
    {
        private readonly Dictionary<ParseType, Func<IInstanceReader>> readerMap = new Dictionary<ParseType, Func<IInstanceReader>>
        {
            {ParseType.VisualStudio, () => new VisualStudioMetricsReader()},
            {ParseType.RichardToxicity, () => new ToxicityReader()},
            {ParseType.PuppyCrawler, () => CheckStylesReader.PuppyCrawlReader},
            {ParseType.EsLint, () => CheckStylesReader.EslintReader},
            {ParseType.SlocEcma, () => new SourceLinesOfCodeReader(FileInclusion.Js)},
            {ParseType.SlocCSharp, () => new SourceLinesOfCodeReader(FileInclusion.CSharp)},
            {ParseType.SlocJava, () => new SourceLinesOfCodeReader(FileInclusion.Java)},
        };

        public IInstanceReader GetReader(ParseType parseType)
        {
            if (!readerMap.ContainsKey(parseType))
                throw new ApplicationException($"{parseType} is not a known metrics parser type");
            return readerMap[parseType]();
        }
    }
}