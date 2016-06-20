﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Metropolis.Common.Models;
using Metropolis.Controllers;
using Metropolis.Test.Api.Services;
using Metropolis.ViewModels;
using Metropolis.Views.UserControls.StepPanels;
using Moq;
using NUnit.Framework;

namespace Metropolis.Test.Metropolis.Controllers
{
    [TestFixture]
    public class CsharpCollectionControllerTest : StrictMockBaseTest
    {
        private CSharpCollectionViewStub view;
        private Mock<IWorkspaceProvider> wsProvider;
        private CsharpCollectionController controller;
        private ProjectDetailsViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            wsProvider = CreateMock<IWorkspaceProvider>();

            viewModel = new ProjectDetailsViewModel
            {
                ProjectFile = "myproject.sln",
                ProjectName = "myProject",
                RepositorySourceType = RepositorySourceType.CSharp,
                FilesToIgnore = new [] {new FileDto {Name = "ignore.me"} }
            };
            view = new CSharpCollectionViewStub(viewModel);

            controller = new CsharpCollectionController(view, wsProvider.Object);
        }

        [Test]
        public void RunBuild()
        {
            var expectedArgs = new ProjectBuildArguments
                                { SourceType = viewModel.RepositorySourceType, ProjectName = viewModel.ProjectName, ProjetFile = viewModel.ProjectFile };
            var buildResult = new ProjectBuildResult
                                { BuildFolder = @"C:\folder", Artifacts = new[] {new FileDto {Name = "me.exe"}, new FileDto {Name = "ignore.me"}} };

            wsProvider.Setup(x => x.BuildSolution(expectedArgs)).Returns(buildResult);

            view.RaiseBuildRequestEvent();
            view.ShowBuildArtifactsCalled.Should().BeTrue();
            view.ActualArtifacts.Count().Should().Be(2);
        }

        [Test]
        public void SolutionFileSelected()
        {
            wsProvider.Setup(x => x.SetUpDotNetBuild(viewModel, @"C:\solution.sln"));
            view.RaiseSolutionFileSelectedEvent(@"C:\solution.sln");
        }

        [Test]
        public void RunAnalysis()
        {
            wsProvider.Setup(x => x.CreateIgnoreFile(It.Is<ProjectDetailsViewModel>(a => ReferenceEquals(a, viewModel))));
            wsProvider.Setup(x => x.Analyze(It.Is<ProjectDetailsViewModel>(a => ReferenceEquals(a, viewModel))));

            view.RaiseRunAnalysisRequestEvent(viewModel.FilesToIgnore);
        }
    }

    public class CSharpCollectionViewStub : ICSharpCollectionView
    {
        public ProjectDetailsViewModel ProjectDetails { get; private set; }
        public bool ShowBuildArtifactsCalled { get; private set; }
        public IEnumerable<FileDto> ActualArtifacts { get; private set; }

        public CSharpCollectionViewStub(ProjectDetailsViewModel details)
        {
            ProjectDetails = details;
        }

        public void RunAnalysis() { }

        public event EventHandler BuildRequested;
        public event EventHandler<SolutionFileArgs> SolutionFileSelected;
        public event EventHandler<IgnoreFileArgs> RunAnalysisRequest;
        public void ShowBuildArtifacts(IEnumerable<FileDto> artifacts)
        {
            ShowBuildArtifactsCalled = true;
            ActualArtifacts = artifacts;
        }
        
        public void RaiseBuildRequestEvent()
        {
            BuildRequested.Should().NotBeNull();            //ensure the controller is dialed in
            BuildRequested?.Invoke(this, EventArgs.Empty);   //invoke the controller method
        }
        
        public void RaiseSolutionFileSelectedEvent(string file)
        {
            SolutionFileSelected.Should().NotBeNull();                                      //ensure the controller is dialed in
            SolutionFileSelected?.Invoke(this, new SolutionFileArgs {SolutionFile = file});  //invoke the controller method
        }
        
        public void RaiseRunAnalysisRequestEvent(IEnumerable<FileDto> filesToIgnore)
        {
            RunAnalysisRequest.Should().NotBeNull();                                                    //ensure the controller is dialed in
            RunAnalysisRequest?.Invoke(this, new IgnoreFileArgs{ IngoreFiles = filesToIgnore });  //invoke the controller method
        }
    }
}