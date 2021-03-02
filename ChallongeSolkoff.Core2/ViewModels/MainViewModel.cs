using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;


namespace Aldentea.ChallongeSolkoff.Core2
{
	using Services;
	namespace ViewModels
	{
		public class MainViewModel : MvvmCross.ViewModels.MvxViewModel
		{
			readonly IChallongeWebService _challongeWebService;

			#region プロパティ

			public string UserName
			{
				get => _userName;
				set
				{
					SetProperty(ref _userName, value);
				}
			}
			string _userName = string.Empty;

			public string ApiKey
			{
				get => _apiKey;
				set
				{
					SetProperty(ref _apiKey, value);
				}
			}
			string _apiKey = string.Empty;

			public string TournamentID
			{
				get => _tournamentID;
				set
				{
					SetProperty(ref _tournamentID, value);
				}

			}
			string _tournamentID;

			public string ErrorMessage
			{
				get => _errorMessage;
				private set
				{
					SetProperty(ref _errorMessage, value);
				}
			}
			string _errorMessage;


			public ObservableCollection<Match> Matches { get; } = new ObservableCollection<Match>();

			#endregion

			#region *コンストラクタ(MainViewModel)
			public MainViewModel(IChallongeWebService webService)
			{
				_challongeWebService = webService;
				// ★起動時にここでなぜかRetrieveMatchesが実行される。
				//RetrieveMatchesTaskNotifier =	MvxNotifyTask.Create(RetrieveMatches, onException: ex => OnException(ex));
				//RetrieveMatchesCommand = new MvxAsyncCommand(() => RetrieveMatchesTaskNotifier.Task);
				RetrieveMatchesCommand = new MvxAsyncCommand(RetrieveMatches);
			}
			#endregion


			public async Task RetrieveMatches()
			{
				Matches.Clear();
				ErrorMessage = string.Empty;
				// ★ので、ここにこんなチェックを入れる。
				if (!string.IsNullOrEmpty(TournamentID))
				{
					foreach (var match in await _challongeWebService.GetMatches(TournamentID, UserName, ApiKey))
					{
						Matches.Add(match.Match);
					}
				}
			}

			public IMvxAsyncCommand RetrieveMatchesCommand { get; private set; }

			public MvxNotifyTask RetrieveMatchesTaskNotifier
			{
				get => _retrieveMatchesTaskNotifier;
				private set => SetProperty(ref _retrieveMatchesTaskNotifier, value);
			}
			private MvxNotifyTask _retrieveMatchesTaskNotifier;

			private void OnException(Exception ex)
			{
				ErrorMessage = ex.Message;
			}

		}
	}
}