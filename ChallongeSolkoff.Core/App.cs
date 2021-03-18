using System;
using System.Collections.Generic;
using System.Text;

namespace Aldentea.ChallongeSolkoff.Core
{
	public class App : MvvmCross.ViewModels.MvxApplication
	{
		public override void Initialize()
		{
			//base.Initialize();
			MvvmCross.Mvx.IoCProvider.RegisterType<Services.IChallongeWebService, Services.ChallongeWebService>();
			//RegisterAppStart<ViewModels.MainViewModel>();
			RegisterAppStart<ViewModels.Idaten11ViewModel>();
		}
	}
}
