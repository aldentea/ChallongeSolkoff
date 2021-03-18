using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Aldentea.ChallongeSolkoff.Core.ViewModels
{
	using Services;

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



		public ObservableCollection<Match> PreliminaryMatches { get; } = new ObservableCollection<Match>();
		public ObservableCollection<Participant> Participants { get; } = new ObservableCollection<Participant>();

		#endregion



		// これらは、IMvxAsyncCommandではなく、IMvxCommandを使う。
		public IMvxCommand RetrievePreliminaryInfoCommand { get; private set; }
		//public IMvxCommand RetrievePreliminaryParticipantsCommand { get; private set; }

		#region *RetrievePreliminaryMatchesTaskNotifierプロパティ
		public MvxNotifyTask RetrievePreliminaryInfoTaskNotifier
		{
			get => _retrievePreliminaryInfoTaskNotifier;
			private set => SetProperty(ref _retrievePreliminaryInfoTaskNotifier, value);
		}
		private MvxNotifyTask _retrievePreliminaryInfoTaskNotifier;
		#endregion

		#region *RetrievePreliminaryParticipantsTaskNotifierプロパティ
		//public MvxNotifyTask RetrievePreliminaryParticipantsTaskNotifier
		//{
		//	get => _retrievePreliminaryParticipantsTaskNotifier;
		//	private set => SetProperty(ref _retrievePreliminaryParticipantsTaskNotifier, value);
		//}
		//private MvxNotifyTask _retrievePreliminaryParticipantsTaskNotifier;
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
			//RetrievePreliminaryParticipantsCommand
			//	= new MvxCommand(() => RetrievePreliminaryParticipantsTaskNotifier = MvxNotifyTask.Create(
			//		() => RetrievePreliminaryParticipants(), onException: ex => OnException(ex)));

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


	}
}
