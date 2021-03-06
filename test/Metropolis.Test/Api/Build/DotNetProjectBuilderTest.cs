﻿using FluentAssertions;
using Metropolis.Api.Build;
using Metropolis.Api.Collection.PowerShell;
using Metropolis.Api.IO;
using Metropolis.Common.Extensions;
using Metropolis.Common.Models;
using Metropolis.Test.Api.Services;
using Moq;
using NUnit.Framework;

namespace Metropolis.Test.Api.Build
{
    [TestFixture]
    public class DotNetProjectBuilderTest : StrictMockBaseTest
    {
        private DotNetProjectBuilder builder;
        private Mock<IUserPreferences> userPreferences;
        private Mock<IRunPowerShell> runPowerShell;
        private Mock<IFileSystem> fileSystem;
        private ProjectBuildArguments args;

        [SetUp]
        public void SetUp()
        {userPreferences = CreateMock<IUserPreferences>();
            runPowerShell = CreateMock<IRunPowerShell>();
            fileSystem = CreateMock<IFileSystem>();
            args = new ProjectBuildArguments("foo", @"c:\project.sln", RepositorySourceType.CSharp, @"c:\buildfolder");

            builder = new DotNetProjectBuilder(userPreferences.Object, runPowerShell.Object, fileSystem.Object);
        }

        [Test]
        public void Build()
        {
            var buildArtifacts = new[] { new FileDto { Name = "app.exe" } };

            fileSystem.Setup(x => x.CleanFolder(args.BuildOutputFolder));
            userPreferences.Setup(x => x.MsBuildPath).Returns(@"c:\build.exe");
            runPowerShell.Setup(x => x.Invoke(DotNetProjectBuilder.MsBuildCommand.FormatWith(@"c:\build.exe", args.ProjectFile, args.BuildOutputFolder)));
            fileSystem.Setup(x => x.FindAllBinaries(args.BuildOutputFolder)).Returns(buildArtifacts);

            var result = builder.Build(args);

            result.Should().NotBeNull();
            result.Artifacts.Should().Contain(buildArtifacts);
        }
    }
}