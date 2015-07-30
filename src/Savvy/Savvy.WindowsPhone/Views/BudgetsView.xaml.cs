﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Savvy.ViewModels;

namespace Savvy.Views
{
    public partial class BudgetsView
    {
        public BudgetsViewModel ViewModel => this.DataContext as BudgetsViewModel;

        public BudgetsView()
        {
            this.InitializeComponent();
        }

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //await this.ViewModel.OpenBudget.ExecuteAsyncTask();
        }
    }
}
