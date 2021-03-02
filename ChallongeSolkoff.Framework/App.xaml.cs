using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MvvmCross.Core;


namespace Aldentea.ChallongeSolkoff.Framework
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : MvvmCross.Platforms.Wpf.Views.MvxApplication
	{
		App()
		{
			this.RegisterSetupType<MvvmCross.Platforms.Wpf.Core.MvxWpfSetup<Aldentea.ChallongeSolkoff.Core2.App>>();
		}
	}
}
