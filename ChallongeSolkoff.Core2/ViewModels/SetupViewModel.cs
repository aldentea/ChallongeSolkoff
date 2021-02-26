using System;
using System.Collections.Generic;
using System.Text;

namespace Aldentea.ChallongeSolkoff.Core.ViewModels
{
	public class SetupViewModel : MvvmCross.ViewModels.MvxViewModel
	{

		#region *UserNameプロパティ
		public string UserName {
			get => _username;
			set
			{
				string new_value = string.IsNullOrEmpty(value) ? string.Empty : value;
				SetProperty(ref _username, value);
			}
		}
		string _username = string.Empty;
		#endregion


		#region *APIKeyプロパティ
		public string APIKey
		{
			get => _apikey;
			set
			{
				string new_value = string.IsNullOrEmpty(value) ? string.Empty : value;
				SetProperty(ref _apikey, value);
			}
		}
		string _apikey = string.Empty;
		#endregion


	}
}
