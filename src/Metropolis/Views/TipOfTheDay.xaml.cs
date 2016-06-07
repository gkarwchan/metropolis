﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Metropolis.Common.Extensions;
using Metropolis.TipOfTheDay;
using Metropolis.ViewModels;

namespace Metropolis.Views
{
    /// <summary>
    /// Interaction logic for TipOfTheDay.xaml
    /// </summary>
    public partial class TipOfTheDay : Window
    {
        private readonly ITipOfTheDayFactory tipTheDayFactory;
        private readonly TipOfTheDayViewModel tipOfTheDayViewModel;

        public TipOfTheDay()
        {
            InitializeComponent();

            //TODO:  change this when we plug in IOC
            tipTheDayFactory = new TipOfTheDayFactory();

            tipOfTheDayViewModel = new TipOfTheDayViewModel();
            DataContext= tipOfTheDayViewModel;
            NextTip(this, new RoutedEventArgs());
            tipOfTheDayViewModel.ShowTips = WorkSpaceProvider.ShowTips;
        }
        private static IWorkspaceProvider WorkSpaceProvider => App.WorkspaceProvider;
        
        private void OpenIssue(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/dahood/metropolis/issues");
        }

        private void OpenBeginnerGuide(object sender, RoutedEventArgs routedEventArgs)
        {
            Process.Start("https://github.com/dahood/metropolis/wiki/Beginner-Guide");
        }
        
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NextTip(object sender, RoutedEventArgs e)
        {
            tipOfTheDayViewModel.TipOfTheDay = tipTheDayFactory.Next;
        }
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }

        private void ShowTipsUnchecked(object sender, RoutedEventArgs e)
        {
            WorkSpaceProvider.ShowTips = false;
        }

        private void ShowTipsChecked(object sender, RoutedEventArgs e)
        {
            WorkSpaceProvider.ShowTips = true;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (tipOfTheDayViewModel.TipOfTheDay.ForMoreInfoUrl.IsEmpty()) return;
            Process.Start(tipOfTheDayViewModel.TipOfTheDay.ForMoreInfoUrl);
        }
    }
}