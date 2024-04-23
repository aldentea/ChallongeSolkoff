using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Core;
using MvvmCross.IoC;
using MvvmCross.Platforms.Wpf.Core;


namespace Aldentea.ChallongeSolkoff
{
	public class Setup : MvvmCross.Platforms.Wpf.Core.MvxWpfSetup<Core.App>
	{
		protected override ILoggerFactory CreateLogFactory()
		{
			Serilog.Log.Logger = new Serilog.LoggerConfiguration()
				.MinimumLevel.Debug().CreateLogger();

			return new Serilog.Extensions.Logging.SerilogLoggerFactory();
		}

		protected override ILoggerProvider CreateLogProvider()
		{
			return new Serilog.Extensions.Logging.SerilogLoggerProvider();
		}

		protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
		{
			//iocProvider.RegisterType<Base.Services.IChallongeWebService, Base.Services.ChallongeWebService>();
			MvvmCross.Mvx.IoCProvider.RegisterType<Base.Services.IChallongeWebService, Base.Services.ChallongeWebService>();
			//MvvmCross.Mvx.IoCProvider.RegisterType<Core.Services.IRetrieveMailsService, Core.Services.RetrieveMailsService>();
			base.InitializeFirstChance(iocProvider);
		}
	}
}
