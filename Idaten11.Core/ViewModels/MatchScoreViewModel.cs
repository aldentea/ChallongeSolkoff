using System;
using System.Collections.Generic;
using System.Text;

using MvvmCross.ViewModels;
using MvvmCross.Commands;

namespace Aldentea.Idaten11.Core.ViewModels
{
	public class MatchScoreViewModel : MvxViewModel
	{
		#region プロパティ

		public int MatchID { get => _matchID;
			set
			{
				SetProperty(ref _matchID, value);
			}

		}
		int _matchID;

		public string Player1Name
		{
			get => _player1Name;
			set
			{
				SetProperty(ref _player1Name, value);
			}
		}
		string _player1Name;


		public string Player2Name
		{
			get => _player2Name;
			set
			{
				SetProperty(ref _player2Name, value);
			}
		}
		string _player2Name;

		public int Player1Score
		{
			get => _player1Score;
			set
			{
				SetProperty(ref _player1Score, value);
			}
		}
		int _player1Score;

		public int Player2Score {
			get => _player2Score;
			set
			{
				SetProperty(ref _player2Score, value);
			}
		}
		int _player2Score;

		#endregion

		public IMvxCommand SubmitCommand { get; private set; }
		public IMvxCommand CancelCommand { get; private set; }


	}
}
