﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Core;

namespace Aldentea.ChallongeSolkoff
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : MvvmCross.Platforms.Wpf.Views.MvxApplication
	{
		protected override void RegisterSetup()
		{
			base.RegisterSetup();
			this.RegisterSetupType<Setup>();
		}
	}


	public class Setup : MvvmCross.Platforms.Wpf.Core.MvxWpfSetup<Core.App>
	{
		protected override void InitializeFirstChance()
		{
			MvvmCross.Mvx.IoCProvider.RegisterType<Base.Services.IChallongeWebService, Base.Services.ChallongeWebService>();
			//MvvmCross.Mvx.IoCProvider.RegisterType<Core.Services.IRetrieveMailsService, Core.Services.RetrieveMailsService>();
			base.InitializeFirstChance();
		}
	}
}
