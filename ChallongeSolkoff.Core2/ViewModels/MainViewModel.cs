using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MvvmCross.Commands;

namespace Aldentea.ChallongeSolkoff.Core2
{
	using Services;
	namespace ViewModels
	{
		public class MainViewModel : MvvmCross.ViewModels.MvxViewModel
		{
			readonly IChallongeWebService _challongeWebService;

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

			public ObservableCollection<Match> Matches { get; } = new ObservableCollection<Match>();

			public MainViewModel(IChallongeWebService webService)
			{
				_challongeWebService = webService;
			}


			public async Task RetrieveMatches()
			{
				Matches.Clear();
				foreach (var match in await _challongeWebService.GetMatches(TournamentID, UserName, ApiKey))
				{
					Matches.Add(match);
				}
			}

			public IMvxAsyncCommand RetrieveMatchesCommand => new MvxAsyncCommand(RetrieveMatches);

		}
	}
}