using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Aldentea.ChallongeSolkoff.Core
{
	public class Match
	{
		[JsonPropertyName("player1_id")]
		public int Player1 { get; set; }
		[JsonPropertyName("player2_id")]
		public int Player2 { get; set; }

		[JsonPropertyName("winner_id")]
		public int Winner { get; set; }

		[JsonPropertyName("loser_id")]
		public int Loser { get; set; }

		[JsonPropertyName("scores_csv")]
		public string Score {
			get
			{
				return $"{Player1Score}-${Player2Score}";
			}
			set {
				var scores = value.Split('-');
				if (scores.Length == 2)
				{
					Player1Score = Convert.ToInt32(scores[0]);
					Player2Score = Convert.ToInt32(scores[1]);
				}
				else
				{
					throw new Exception("Scoreプロパティに与える文字列の形式が不正です。");
				}
			}
		}


		public int Player1Score {
			get; set;
		}

		public int Player2Score
		{
			get; set;
		}

	}
}
