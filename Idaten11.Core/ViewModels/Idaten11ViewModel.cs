using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

using Aldentea.ChallongeSolkoff.Base.Services;
using Aldentea.ChallongeSolkoff.Base;

namespace Aldentea.Idaten11.Core.ViewModels
{

	public class Idaten11ViewModel : MvxViewModel
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

		public string ErrorMessage
		{
			get => _errorMessage;
			private set
			{
				SetProperty(ref _errorMessage, value);
			}
		}
		string _errorMessage;

		public string PreliminaryID
		{
			get => _preliminaryID;
			set
			{
				SetProperty(ref _preliminaryID, value);
			}

		}
		string _preliminaryID;

		public string MainID
		{
			get => _mainID;
			set
			{
				SetProperty(ref _mainID, value);
			}

		}
		string _mainID;


		public ObservableCollection<Match> PreliminaryMatches { get; } = new ObservableCollection<Match>();
		public ObservableCollection<Participant> Participants { get; } = new ObservableCollection<Participant>();

		#endregion



		// これらは、IMvxAsyncCommandではなく、IMvxCommandを使う。
		public IMvxCommand RetrievePreliminaryInfoCommand { get; private set; }

		public IMvxCommand<Match> UpdateMatchCommand { get; private set; }


		#region *RetrievePreliminaryMatchesTaskNotifierプロパティ
		public MvxNotifyTask RetrievePreliminaryInfoTaskNotifier
		{
			get => _retrievePreliminaryInfoTaskNotifier;
			private set => SetProperty(ref _retrievePreliminaryInfoTaskNotifier, value);
		}
		private MvxNotifyTask _retrievePreliminaryInfoTaskNotifier;
		#endregion

		#region *UpdateMatchTaskNotifierプロパティ
		public MvxNotifyTask UpdateMatchTaskNotifier
		{
			get => _updateMatchTaskNotifier;
			private set => SetProperty(ref _updateMatchTaskNotifier, value);
		}
		private MvxNotifyTask _updateMatchTaskNotifier;
		#endregion


		private void OnException(Exception ex)
		{
			ErrorMessage = ex.Message;
		}


		public Idaten11ViewModel(IChallongeWebService webService)
		{
			_challongeWebService = webService;

			RetrievePreliminaryInfoCommand
				= new MvxCommand(() => RetrievePreliminaryInfoTaskNotifier = MvxNotifyTask.Create(
					() => RetrievePreliminaryInfo(), onException: ex => OnException(ex)));

			// これで例外をキャッチできる？
			UpdateMatchCommand = new MvxCommand<Match>(match => UpdateMatch(match));

			_inputScoreInteraction = new MvxInteraction<InputScoreQuestion>();

			// とりあえず。
			_mainID = "210320_idaten_main";
			_preliminaryID = "210320_idaten_pre";

		}

		#region 予選の情報を取得

		public async Task RetrievePreliminaryInfo()
		{
			ErrorMessage = string.Empty;

			// 1.参加者情報を取得する
			Participants.Clear();
			if (!string.IsNullOrEmpty(PreliminaryID))
			{
				foreach (var participant in await _challongeWebService.GetParticipants(PreliminaryID, UserName, ApiKey))
				{
					Participants.Add(participant.Participant);
				}
			}

			// 2.マッチ情報を取得する。
			PreliminaryMatches.Clear();
			ErrorMessage = string.Empty;

			if (!string.IsNullOrEmpty(PreliminaryID))
			{
				var matches = await _challongeWebService.GetMatches(PreliminaryID, UserName, ApiKey);
				// Round1のマッチに対してのみ処理を行う。
				foreach (var match in matches.Where(mi => mi.Match.Round == 1))
				{
					// プレイヤー名を取得する。
						var participant1 = Participants.FirstOrDefault(p => p.ID == match.Player1);
						match.Match.Player1Name = participant1.Name;
						var participant2 = Participants.FirstOrDefault(p => p.ID == match.Player2);
						match.Match.Player2Name = participant2.Name;

						PreliminaryMatches.Add(match.Match);
					}
			}

		}

		#endregion


		void UpdateMatch(Match match)
		{
			ErrorMessage = string.Empty;
			var request = new InputScoreQuestion
			{
				Player1Name = match.Player1Name,
				Player2Name = match.Player2Name,

				InputScoreCallback = async (answer) =>
				{
					if (answer.Ok)
					{
						match.InputScores(answer.Player1Score, answer.Player2Score);
						// InputScoresメソッドで勝敗も判定する。
						// マッチ結果を送信。
						MatchItem match_item = null;
						try
						{
							match_item = await _challongeWebService.UpdateMatch(match, PreliminaryID, UserName, ApiKey);
						}
						catch (Exception ex)
						{
							OnException(ex);
							return;
						}
						// 勝者を本戦に登録する。
						Participant winner = null;
						if (match_item.Winner == match_item.Player1)
						{
							winner = Participants.FirstOrDefault(p => p.ID == match_item.Player1);
						}
						else if (match_item.Winner == match_item.Player2)
						{
							winner = Participants.FirstOrDefault(p => p.ID == match_item.Player2);
						}
						if (winner != null)
						{
								await _challongeWebService.AddParticipant(winner.Name, MainID, UserName, ApiKey);
						}
					}
				}
			};
			_inputScoreInteraction.Raise(request);

		}

		public IMvxInteraction<InputScoreQuestion> InputScoreInteraction => _inputScoreInteraction;
		readonly MvxInteraction<InputScoreQuestion> _inputScoreInteraction;

	}
}
