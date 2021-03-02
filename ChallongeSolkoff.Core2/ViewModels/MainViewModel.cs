using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Linq;


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
			public ObservableCollection<Participant> Participants { get; } = new ObservableCollection<Participant>();

			#endregion

			#region *コンストラクタ(MainViewModel)
			public MainViewModel(IChallongeWebService webService)
			{
				_challongeWebService = webService;
				// ★起動時にここでなぜかRetrieveMatchesが実行される。
				//RetrieveMatchesTaskNotifier =	MvxNotifyTask.Create(RetrieveMatches, onException: ex => OnException(ex));
				//RetrieveMatchesCommand = new MvxAsyncCommand(() => RetrieveMatchesTaskNotifier.Task);
				RetrieveMatchesCommand = new MvxAsyncCommand(RetrieveMatches);
				RetrieveParticipantsCommand = new MvxAsyncCommand(RetrieveParticipants);

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

				// 集計を行う。
				var match_list = new List<Match>(Matches);
				foreach (var player in Participants)
				{
					player.Wins = match_list.Count(m => m.Winner == player.ID);
					player.Loses = match_list.Count(m => m.Loser == player.ID);
					player.Plus = match_list.Where(m => m.Player1 == player.ID).Sum(m => m.Player1Score) + match_list.Where(m => m.Player2 == player.ID).Sum(m => m.Player2Score);
					player.Minus = match_list.Where(m => m.Player1 == player.ID).Sum(m => m.Player2Score) + match_list.Where(m => m.Player2 == player.ID).Sum(m => m.Player1Score);
				}

				// ソルコフ類の集計を行う。
				foreach (var player in Participants)
				{
					// playerが勝った相手のID。
					var wons = match_list.Where(m => m.Winner == player.ID).Select(m => m.Loser);
					player.SbScore = Participants.Where(p => wons.Contains(p.ID)).Sum(p => p.Wins);
					var losts = match_list.Where(m => m.Loser == player.ID).Select(m => m.Winner);
					player.Solkoff = player.SbScore + Participants.Where(p => losts.Contains(p.ID)).Sum(p => p.Wins);
				}

			}

			private async Task RetrieveParticipants()
			{
				Participants.Clear();
				ErrorMessage = string.Empty;
				if (!string.IsNullOrEmpty(TournamentID))
				{
					foreach (var participant in await _challongeWebService.GetParticipants(TournamentID, UserName, ApiKey))
					{
						Participants.Add(participant.Participant);
					}
				}
			}



			public IMvxAsyncCommand RetrieveMatchesCommand { get; private set; }
			public IMvxAsyncCommand RetrieveParticipantsCommand { get; private set; }

			//public MvxNotifyTask RetrieveMatchesTaskNotifier
			//{
			//	get => _retrieveMatchesTaskNotifier;
			//	private set => SetProperty(ref _retrieveMatchesTaskNotifier, value);
			//}
			//private MvxNotifyTask _retrieveMatchesTaskNotifier;

			//private void OnException(Exception ex)
			//{
			//	ErrorMessage = ex.Message;
			//}

		}
	}
}