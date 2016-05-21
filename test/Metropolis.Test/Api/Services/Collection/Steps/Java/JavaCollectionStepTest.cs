﻿using System;
using FluentAssertions;
using Metropolis.Api.Collection.Steps.Java;
using Metropolis.Common.Extensions;
using Metropolis.Common.Models;
using NUnit.Framework;

namespace Metropolis.Test.Api.Services.Collection.Steps.Java
{
    [TestFixture]
    public class JavaCollectionStepTest : CollectionBaseTest
    {
        private PuppyCrawlerCheckstyleCollectionStep step;

        [SetUp]
        public void BeforeEachTest()
        {
            step = new PuppyCrawlerCheckstyleCollectionStep();
        }

        [Test]
        public void HasCorrectSettings()
        {
            step.Extension.Should().Be(".xml");
            step.MetricsType.Should().Be("Java Checkstyle");
            step.ParseType.Should().Be(ParseType.PuppyCrawler);
        }

        [Test]
        public void CanParseCommand()
        {
            var expected = PuppyCrawlerCheckstyleCollectionStep.CheckstyleCommand
                                              .FormatWith(AppDomain.CurrentDomain.BaseDirectory + "*.jar", // include all jars into the class path
                                                          AppDomain.CurrentDomain.BaseDirectory + "metropolis_checkstyle_metrics.xml",
                                                          Result.MetricsFile, Args.SourceDirectory);
            
            var command = step.PrepareCommand(Args, Result);

            command.Should().Be(expected);
        }
    }
}