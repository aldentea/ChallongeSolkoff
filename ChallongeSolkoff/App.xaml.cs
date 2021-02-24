using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using MvvmCross.Core;

namespace Aldentea.ChallongeSolkoff
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : MvvmCross.Platforms.Wpf.Views.MvxApplication
	{
		App()
		{
			
			this.RegisterSetupType<MvvmCross.Platforms.Wpf.Core.MvxWpfSetup<Core.App>>();
		}
	}
}
