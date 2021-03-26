using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MvvmCross.Platforms.Wpf.Views;

namespace Aldentea.Idaten11.Views
{
	/// <summary>
	/// MatchScoreDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class MatchScoreDialog : MvxWindow
	{
		public MatchScoreDialog()
		{
			InitializeComponent();
		}

		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
	}
}
