using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Microsoft.Extensions.Logging;
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

			#region *UserNameプロパティ
			/// <summary>
			/// challonge.comのAPIユーザ名を取得／設定します。
			/// </summary>
			public string UserName
			{
				get => _userName;
				set
				{
					SetProperty(ref _userName, value);
				}
			}
			string _userName = string.Empty;
			#endregion

			#region *ApiKeyプロパティ
			/// <summary>
			/// challonge.comのAPIキーを取得／設定します。
			/// </summary>
			public string ApiKey
			{
				get => _apiKey;
				set
				{
					SetProperty(ref _apiKey, value);
				}
			}
			string _apiKey = string.Empty;

			#endregion

			#region *TournamentIDプロパティ
			/// <summary>
			/// トーナメントIDを取得／設定します。
			/// </summary>
			public string TournamentID
			{
				get => _tournamentID;
				set
				{
					SetProperty(ref _tournamentID, value);
				}

			}
			string _tournamentID;

			#endregion

			#region *ErrorMessageプロパティ

			/// <summary>
			/// エラーメッセージを取得（／設定）します。
			/// </summary>
			public string ErrorMessage
			{
				get => _errorMessage;
				private set
				{
					SetProperty(ref _errorMessage, value);
				}
			}
			string _errorMessage;
			#endregion

			#region *UseScoreSolkoffプロパティ
			/// <summary>
			/// 得点ソルコフを使うかどうかの値を取得／設定します。
			/// falseの場合は勝ち数ソルコフを使います。
			/// </summary>
			public bool UseScoreSolkoff
			{
				get => _useScoreSolkoff;
				set
				{
					SetProperty(ref _useScoreSolkoff, value);
				}
			}
			bool _useScoreSolkoff = false;
			#endregion

			public ObservableCollection<Match> Matches { get; } = new ObservableCollection<Match>();
			public ObservableCollection<Participant> Participants { get; } = new ObservableCollection<Participant>();

			#endregion

			// これらは、IMvxAsyncCommandではなく、IMvxCommandを使う。
			public IMvxCommand RetrieveMatchesCommand { get; private set; }
			public IMvxCommand RetrieveParticipantsCommand { get; private set; }
			public IMvxCommand CopyParticipantsListCommand { get; private set; }
			public IMvxCommand ExportParticipantsCommand { get; private set; }
			// (0.4.2)
			public IMvxCommand ExportStandingsCommand { get; private set; }
			public IMvxCommand ExportMatchesCommand { get; private set; }

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

			#region *ExportParticipantsTaskNotifierプロパティ
			public MvxNotifyTask ExportParticipantsTaskNotifier
			{
				get => _exportParticipantsTaskNotifier;
				private set => SetProperty(ref _exportParticipantsTaskNotifier, value);
			}
			private MvxNotifyTask _exportParticipantsTaskNotifier;
			#endregion


			private void OnException(Exception ex)
			{
				ErrorMessage = ex.Message;
			}


			#region *コンストラクタ(MainViewModel)
			public MainViewModel(IChallongeWebService webService, ILogger<MainViewModel> logger)
			{
				_challongeWebService = webService;
				_logger = logger;

				RetrieveParticipantsCommand
					= new MvxCommand(() => RetrieveParticipantsTaskNotifier = MvxNotifyTask.Create(
						() => RetrieveParticipants(), onException: ex => OnException(ex)));
				RetrieveMatchesCommand
					= new MvxCommand(() => RetrieveMatchesTaskNotifier = MvxNotifyTask.Create(
						() => RetrieveMatches(), onException: ex => OnException(ex)));

				CopyParticipantsListCommand
					= new MvxCommand(() => CopyParticipantsList());
				ExportParticipantsCommand
					= new MvxCommand(() => ExportParticipants());
				ExportStandingsCommand
					= new MvxCommand(() => ExportStandings());
				ExportMatchesCommand
					= new MvxCommand(() => ExportMatches());
				//ExportParticipantsCommand
				//	= new MvxCommand(() => ExportParticipantsTaskNotifier = MvxNotifyTask.Create(
				//		() => ExportParticipants(), onException: ex => OnException(ex)));

				_selectSaveFileInteraction = new MvxInteraction<SelectSaveFileQuestion>();
				_copyToClipboardInteraction = new MvxInteraction<string>();
			}

			#endregion


			readonly ILogger<MainViewModel> _logger;

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
				if (UseScoreSolkoff)
				{
					foreach (var player in Participants)
					{
						// playerが勝った相手のID。
						var wons = match_list.Where(m => m.Winner == player.ID).Select(m => m.Loser);
						player.Solkoff = Participants.Where(p => wons.Contains(p.ID)).Sum(p => p.Plus);
						var losts = match_list.Where(m => m.Loser == player.ID).Select(m => m.Winner);
						player.Solkoff += Participants.Where(p => losts.Contains(p.ID)).Sum(p => p.Plus);
						// SBスコアはややこしくなりそうなので、とりあえず計算しないでおく。
						player.SbScore = 0;
					}
				}
				else
				{
					foreach (var player in Participants)
					{
						// playerが勝った相手のID。
						var wons = match_list.Where(m => m.Winner == player.ID).Select(m => m.Loser);
						player.SbScore = Participants.Where(p => wons.Contains(p.ID)).Sum(p => p.Wins);
						var losts = match_list.Where(m => m.Loser == player.ID).Select(m => m.Winner);
						player.Solkoff = player.SbScore + Participants.Where(p => losts.Contains(p.ID)).Sum(p => p.Wins);
					}
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
				_logger.Log(LogLevel.Information, "Participants information is retrieved.");
			}
			#endregion


			#region ソルコフ付順位表をエクスポート(ExportParticpants)

			void ExportParticipants()
			{
				var request = new SelectSaveFileQuestion
				{
					Callback = async (filename) =>
					{
						await ExportPartcipantsTo(filename);
					}
				};
				_selectSaveFileInteraction.Raise(request);

				// RaiseしたあとのCallbackでエラーが発生した場合、ここで受け取ることはできない？
			}

			private async Task ExportPartcipantsTo(string filename)
			{
				
				if (!string.IsNullOrEmpty(filename))
				{
					using (var writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
					{
						var header = "名前,勝,負,ソルコフ,SB,得,失,ID";
						await writer.WriteLineAsync(header);
						foreach (var participant in Participants)
						{
							var line = $"{participant.Name},{participant.Wins},{participant.Loses},{participant.Solkoff},{participant.SbScore},{participant.Plus},{participant.Minus},{participant.ID}";
							await writer.WriteLineAsync(line);
						}
					}
				}
			}

			#endregion

			// (0.4.2)
			#region 得点表をエクスポート(ExportStandings)

			void ExportStandings()
			{
				var request = new SelectSaveFileQuestion
				{
					Callback = async (filename) =>
					{
						await ExportStandingsTo(filename);
					}
				};
				_selectSaveFileInteraction.Raise(request);

				// RaiseしたあとのCallbackでエラーが発生した場合、ここで受け取ることはできない？
			}

			private async Task ExportStandingsTo(string filename)
			{

				if (!string.IsNullOrEmpty(filename))
				{
					using (var writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
					{
						var header = "名前,得失差,得,失,勝,負,ID";
						await writer.WriteLineAsync(header);
						foreach (var participant in Participants)
						{
							var line = $"{participant.Name},{participant.Delta},{participant.Plus},{participant.Minus},{participant.Wins},{participant.Loses},{participant.ID}";
							await writer.WriteLineAsync(line);
						}
					}
				}
			}

			#endregion

			#region マッチ結果をエクスポート(ExportMatches)
			private void ExportMatches()
			{
				var request = new SelectSaveFileQuestion
				{
					Callback = async (filename) =>
					{
						await ExportMatchesTo(filename);
					}
				};
				_selectSaveFileInteraction.Raise(request);
			}

			private async Task ExportMatchesTo(string filename)
			{

				if (!string.IsNullOrEmpty(filename))
				{
					using (var writer = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
					{
						var header = "ラウンド,プレイヤー1,プレイヤー2,プレイヤー1スコア,プレイヤー2スコア,プレイヤー1ID,プレイヤー2ID";
						await writer.WriteLineAsync(header);
						foreach (var match in Matches)
						{
							var line = $"{match.Round},{match.Player1Name},{match.Player2Name},{match.Player1Score},{match.Player2Score},{match.Player1},{match.Player2}";
							await writer.WriteLineAsync(line);
						}
					}
				}
			}
			#endregion

			public IMvxInteraction<SelectSaveFileQuestion> SelectSaveFileInteraction => _selectSaveFileInteraction;
			readonly MvxInteraction<SelectSaveFileQuestion> _selectSaveFileInteraction;

			void CopyParticipantsList()
			{
				var request = string.Join("\n", Participants.Select(p => p.Name));
				_copyToClipboardInteraction.Raise(request);
			}


			public IMvxInteraction<string> CopyToClipboardInteraction => _copyToClipboardInteraction;
			readonly MvxInteraction<string> _copyToClipboardInteraction;


		}
	}
}