﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawWinAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private DrawWindow drawWindow;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        #endregion

        #region Methods
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            drawWindow = new DrawWindow(ControlHostElement.ActualHeight, ControlHostElement.ActualWidth);
            ControlHostElement.Children.Add(drawWindow);
        }
        #endregion
    }
}
