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
using MvvmCross.Platforms.Wpf.Views;

namespace Aldentea.ChallongeSolkoff
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[MvvmCross.Platforms.Wpf.Presenters.Attributes.MvxWindowPresentation]
	public partial class MainWindow : MvxWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
