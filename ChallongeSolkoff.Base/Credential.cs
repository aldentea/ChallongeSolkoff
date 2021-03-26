using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Aldentea.ChallongeSolkoff.Base
{

	public class Credential : INotifyPropertyChanged
	{

		#region *UserNameプロパティ
		[JsonPropertyName("user_name")]
		public string UserName
		{
			get => _userName;
			set
			{
				if (_userName != value)
				{
					_userName = value;
					NotifyPropertyChanged("UserName");
				}
			}
		}
		string _userName = string.Empty;
		#endregion

		#region *ApiKeyプロパティ
		[JsonPropertyName("api_key")]
		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (_apiKey != value)
				{
					_apiKey = value;
					NotifyPropertyChanged("ApiKey");
				}
			}
		}
		string _apiKey = string.Empty;
		#endregion


		#region INotifyPropertyChanged実装
		protected void NotifyPropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		#endregion

	}
}
