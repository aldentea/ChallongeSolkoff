using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System.Linq;


namespace Aldentea.ChallongeSolkoff.Core
{
	using Base;
	using Base.Services;
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

			// これらは、IMvxAsyncCommandではなく、IMvxCommandを使う。
			public IMvxCommand RetrieveMatchesCommand { get; private set; }
			public IMvxCommand RetrieveParticipantsCommand { get; private set; }

			#region *RetrieveParticipantsTaskNotifierプロパティ
			public MvxNotifyTask RetrieveParticipantsTaskNotifier
			{
				get => _retrieveParticipantsTaskNotifier;
				private set => SetProperty(ref _retrieveParticipantsTaskNotifier, value);
			}
			private MvxNotifyTask _retrieveParticipantsTaskNotifier;
			#endregion

			#region *RetrieveMatchesTaskNotifierプロパティ
			public MvxNotifyTask RetrieveMatchesTaskNotifier
			{
				get => _retrieveMatchesTaskNotifier;
				private set => SetProperty(ref _retrieveMatchesTaskNotifier, value);
			}
			private MvxNotifyTask _retrieveMatchesTaskNotifier;
			#endregion

			private void OnException(Exception ex)
			{
				ErrorMessage = ex.Message;
			}


			#region *コンストラクタ(MainViewModel)
			public MainViewModel(IChallongeWebService webService)
			{
				_challongeWebService = webService;

				RetrieveParticipantsCommand
					= new MvxCommand(() => RetrieveParticipantsTaskNotifier = MvxNotifyTask.Create(
						() => RetrieveParticipants(), onException: ex => OnException(ex)));
				RetrieveMatchesCommand
					= new MvxCommand(() => RetrieveMatchesTaskNotifier = MvxNotifyTask.Create(
						() => RetrieveMatches(), onException: ex => OnException(ex)));
			}
			#endregion


			#region *マッチ情報を取得(RetrieveMatches)
			public async Task RetrieveMatches()
			{
				Matches.Clear();
				ErrorMessage = string.Empty;

				if (!string.IsNullOrEmpty(TournamentID))
				{
					foreach (var match in await _challongeWebService.GetMatches(TournamentID, UserName, ApiKey))
					{
						// プレイヤー名を取得する。
						var participant1 = Participants.FirstOrDefault(p => p.ID == match.Player1);
						if ((participant1?.ID).HasValue)
						{
							match.Match.Player1Name = participant1.Name;
						}
						var participant2 = Participants.FirstOrDefault(p => p.ID == match.Player2);
						if ((participant2?.ID).HasValue)
						{
							match.Match.Player2Name = participant2.Name;
						}

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
			#endregion

			#region *参加者情報を取得(RetrieveParticipants)
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
			#endregion


			void ExportParticipants()
			{
				var request = new SelectSaveFileQuestion
				{
					Callback = (filename) =>
					{
						ExportPartcipantsTo(filename);
					}
				};

				_selectSaveFileInteraction.Raise(request);
			}

			// ※名前だけをクリップボードにコピーするか、成績を含めてファイルに出力するかの2択でよい？
			// PresentationCore.dllのSystem.Windows.Clipboardを参照？

			private void ExportPartcipantsTo(string filename)
			{
				
				if (!string.IsNullOrEmpty(filename))
				{
					using (var writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
					{
						foreach (var participant in Participants)
						{
							writer.WriteLine(participant.Name);
						}
					}
				}
			}


			public IMvxInteraction<SelectSaveFileQuestion> SelectSaveFileInteraction => _selectSaveFileInteraction;
			readonly MvxInteraction<SelectSaveFileQuestion> _selectSaveFileInteraction;

		}
	}
}