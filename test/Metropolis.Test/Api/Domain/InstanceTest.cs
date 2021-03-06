﻿using FluentAssertions;
using Metropolis.Api.Domain;
using Metropolis.Test.Utilities;
using NUnit.Framework;

namespace Metropolis.Test.Api.Domain
{
    [TestFixture]
    public class InstanceTest
    {
        public Instance canvas;

        [SetUp]
        public void Setup()
        {
            canvas = new Instance(CodeBag.Empty, "Canvas", new Location(@"fileA.cs"));
            canvas.LinesOfCode = 100;
        }

        [Test]
        public void Should_Report_DuplicateLines_And_Percentage_Simple_Case()
        {
            canvas.AddDuplicate(new Duplicate(1,10, new Location("fileA.cs"), new Duplicate(1, 10, new Location("fileB.cs"))));
            canvas.LinesOfCode.Should().Be(100);
            canvas.DuplicateLines.Should().Be(1);
            canvas.DuplicatePercentage.Should().Be(0.01d);
        }

        [Test]
        public void Should_Report_DuplicateLines_And_Percentage_But_Not_Add_Duplicates_Which_are_not_Mine()
        {
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileA.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileB.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileC.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileD.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileE.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileF.cs")));
            canvas.AddDuplicate(new Duplicate(20, 10, new Location("fileG.cs")));
            canvas.LinesOfCode.Should().Be(100);
            canvas.DuplicateLines.Should().Be(20);
            canvas.DuplicatePercentage.Should().Be(0.20d);
            canvas.Duplicates.Count.Should().Be(1);
        }

        [Test]
        public void Should_Report_DuplicateLines_And_Percentage_If_Over_100_Percent()
        {
            canvas.AddDuplicate(new Duplicate(20, 0, new Location("fileA.cs")));
            canvas.AddDuplicate(new Duplicate(20, 20, new Location("fileA.cs")));
            canvas.AddDuplicate(new Duplicate(20, 40, new Location("fileA.cs")));
            canvas.AddDuplicate(new Duplicate(20, 60, new Location("fileA.cs")));
            canvas.AddDuplicate(new Duplicate(20, 70, new Location("fileA.cs")));
            
            canvas.AddDuplicate(new Duplicate(10, 10, new Location("fileA.cs")));//another duplicate block at the start

            canvas.LinesOfCode.Should().Be(100);
            canvas.DuplicateLines.Should().Be(110);
            canvas.DuplicatePercentage.Should().Be(1.10d);
            canvas.Duplicates.Count.Should().Be(6);
        }



        [Test]
        public void ShouldApplyLargestMetricWhenPhysicalPathMatches()
        {
            var mbr = new Member("store", 1, 1, 1);
            var cls = InstanceBuilder.Build(new CodeBag("dev", CodeBagType.Namespace, @"C:\dev"), "name", @"C:\dev\name.cs", 1, 1, 3, 1, new[] { mbr });
            var toApply = InstanceBuilder.Build(new CodeBag("dev", CodeBagType.Namespace, @"C:\dev"), "name", @"C:\dev\name.cs", 2, 2, 2, 2, new[] { mbr });

            cls.Apply(toApply);

            Validate.Begin().IsNotNull(cls, "class")
                .IsEqual(cls.Members.Count, 1, "mbr count")
                .IsEqual(cls.NumberOfMethods, 1, "# Methods")
                .IsEqual(cls.LinesOfCode, 2, "loc")
                .IsEqual(cls.CyclomaticComplexity, 2, "cyclo")
                .IsEqual(cls.DepthOfInheritance, 3, "DIT")
                .IsEqual(cls.ClassCoupling, 2, "class coupling")
                .Check();
        }

        [Test]
        public void ShouldNotApplyWhenPhysicalPathDiffers()
        {
            var mbr = new Member("store", 1, 1, 1);
            var cls = InstanceBuilder.Build(new CodeBag("dev", CodeBagType.Namespace, @"C:\dev"), "name", @"C:\dev\name.cs", 1, 1, 3, 1, new[] {mbr});
            var toApply = InstanceBuilder.Build(new CodeBag("dev", CodeBagType.Namespace, @"C:\dev"), "namexxx", @"C:\dev\namexxx.cs", 2, 2, 2, 2, new[] { mbr });

           cls.Apply(toApply);

            Validate.Begin().IsNotNull(cls, "class")
                .IsEqual(cls.Members.Count, 1, "mbr count")
                .IsEqual(cls.NumberOfMethods, 1, "# Methods")
                .IsEqual(cls.LinesOfCode, 1, "loc")
                .IsEqual(cls.CyclomaticComplexity, 1, "cyclo")
                .IsEqual(cls.DepthOfInheritance, 3, "DIT")
                .IsEqual(cls.ClassCoupling, 1, "class coupling")
                .Check();
        }
    }
}