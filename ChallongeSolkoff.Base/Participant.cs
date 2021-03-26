using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Aldentea.ChallongeSolkoff.Base
{

	public class ParticipantItem
	{
		[JsonPropertyName("participant")]
		public Participant Participant { get; set; }

	}

	public class Participant : INotifyPropertyChanged
	{
		[JsonPropertyName("id")]
		public int ID { get; set; }

		[JsonPropertyName("display_name")]
		public string Name { get; set; }

		public int Wins
		{
			get => _wins;
			set
			{
				if (_wins != value)
				{
					_wins = value;
					NotifyPropertyChanged("Wins");
				}
			}
		}
		int _wins = 0;

		public int Loses
		{
			get => _loses;
			set
			{
				if (_loses != value)
				{
					_loses = value;
					NotifyPropertyChanged("Loses");
				}
			}
		}
		int _loses = 0;

		public int Plus
		{
			get => _plus;
			set
			{
				if (_plus != value)
				{
					_plus = value;
					NotifyPropertyChanged("Plus");
				}
			}
		}
		int _plus = 0;

		public int Minus
		{
			get => _minus;
			set
			{
				if (_minus != value)
				{
					_minus = value;
					NotifyPropertyChanged("Minus");
				}
			}
		}
		int _minus = 0;

		public int Solkoff
		{
			get => _solkoff;
			set
			{
				if (_solkoff != value)
				{
					_solkoff = value;
					NotifyPropertyChanged("Solkoff");
					NotifyPropertyChanged("Delta");
				}
			}
		}
		int _solkoff = 0;

		public int SbScore
		{
			get => _sb_score;
			set
			{
				if (_sb_score != value)
				{
					_sb_score = value;
					NotifyPropertyChanged("SbScore");
					NotifyPropertyChanged("Delta");
				}
			}
		}
		int _sb_score = 0;

		public int Delta
		{
			get => Plus - Minus;
		}

		#region INotifyPropertyChanged実装
		protected void NotifyPropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		#endregion

	}
}
