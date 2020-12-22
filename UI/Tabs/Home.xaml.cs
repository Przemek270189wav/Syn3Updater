﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Syn3Updater.UI.Tabs
{
    /// <summary>
    ///     Interaction logic for Install.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this)) (DataContext as HomeViewModel)?.Init();
        }

        private void Home_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue && (bool) e.OldValue == false) (DataContext as HomeViewModel)?.ReloadSettings();
        }
    }
}