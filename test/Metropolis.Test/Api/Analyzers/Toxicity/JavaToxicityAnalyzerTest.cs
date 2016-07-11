﻿using System;
using FluentAssertions;
using Metropolis.Api.Analyzers.Toxicity;
using Metropolis.Api.Domain;
using Metropolis.Api.Extensions;
using NUnit.Framework;

namespace Metropolis.Test.Api.Analyzers.Toxicity
{
    [TestFixture]
    public class JavaToxicityAnalyzerTest : AbstractToxicityAnalyzerTest<JavaToxicityAnalyzer>
    {
        protected override int ThresholdNumberOfMembers => JavaToxicityAnalyzer.ThresholdNumberOfMethods;
        protected override int ThresholdCyclomaticComplexity => JavaToxicityAnalyzer.ThresholdCyclomaticComplexity;
        protected override int ThresholdMethodLength => JavaToxicityAnalyzer.ThresholdMethodLength;

        protected override Instance HealthyInstance => AnalyzerFixture.HealthJavaInstance;

        protected override Instance CreateHealthyInstance(Action<Instance> initializer)
        {
            return AnalyzerFixture.Initialize(AnalyzerFixture.HealthJavaInstance, initializer);
        }

        [Test]
        public void Healthy_NumberOfMembers()
        {
            var toAnalyse = HealthyInstance;
            ThresholdNumberOfMembers.ForEach(x => toAnalyse.WithHealthyMember<JavaToxicityAnalyzer>($"Member{x}"));

            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(0);
        }

        [Test]
        public void ToxicOn_NumberOfMembers()
        {
            var toAnalyse = HealthyInstance;
            (ThresholdNumberOfMembers + 1).ForEach(x => toAnalyse.WithHealthyMember<JavaToxicityAnalyzer>($"Member{x}"));

            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(Math.Log(1));
        }

        [Test]
        public void ToxicOn_LinesOfCode_NoMembers()
        {
            var toAnalyse = CreateHealthyInstance(x => x.LinesOfCode += ThresholdExceeded);
            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(Math.Log(ThresholdExceeded));
        }

        [Test]
        public void NotToxic_WhenEverythingIsWithinTheThresholds_OneMember()
        {
            var toAnalyse = HealthyInstance.WithHealthyMember<JavaToxicityAnalyzer>("ToString");
            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(0);
        }

        [Test]
        public void NotToxic_Member_MethodLengthExceeded()
        {
            var exceededMethodLength = ThresholdMethodLength + ThresholdExceeded;
            var toAnalyse = HealthyInstance.WithHealthyMember<JavaToxicityAnalyzer>("ToString", exceededMethodLength);

            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(Math.Log(ThresholdExceeded));
        }

        [Test]
        public void NotToxic_Member_CylcomaticComplexityExceeded()
        {
            var exceededCyclomaticComplexity = ThresholdCyclomaticComplexity + ThresholdExceeded;
            var toAnalyse = HealthyInstance.WithHealthyMember<JavaToxicityAnalyzer>("ToString", ThresholdMethodLength, exceededCyclomaticComplexity);

            var score = Analyzer.CalculateToxicity(toAnalyse);
            score.Toxicity.Should().Be(Math.Log(ThresholdExceeded));
        }
    }
}