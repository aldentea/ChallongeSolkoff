using System;
using System.Collections.Generic;
using System.Text;

namespace Aldentea.Idaten11.Core
{
	public class App : MvvmCross.ViewModels.MvxApplication
	{
		public override void Initialize()
		{
			//base.Initialize();
			MvvmCross.Mvx.IoCProvider.RegisterType<ChallongeSolkoff.Base.Services.IChallongeWebService, ChallongeSolkoff.Base.Services.ChallongeWebService>();
			RegisterAppStart<ViewModels.Idaten11ViewModel>();
		}
	}
}
