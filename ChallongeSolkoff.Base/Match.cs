using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Aldentea.ChallongeSolkoff.Base
{
	public class MatchItem
	{
		[JsonPropertyName("match")]
		public Match Match { get; set; }

		public int? Player1 { get => Match.Player1; }
		public int? Player2 { get => Match.Player2; }

		public int Winner { get => Match.Winner ?? 0; }

		public int Loser { get => Match.Loser ?? 0; }
	}

	public class Match
	{
		[JsonPropertyName("id")]
		public int ID { get; set; }

		[JsonPropertyName("round")]
		public int Round { get; set; }

		[JsonPropertyName("player1_id")]
		public int? Player1 { get; set; }
		[JsonPropertyName("player2_id")]
		public int? Player2 { get; set; }

		public string Player1Name { get; set; }

		public string Player2Name { get; set; }


		[JsonPropertyName("winner_id")]
		public int? Winner { get; set; }

		[JsonPropertyName("loser_id")]
		public int? Loser { get; set; }

		[JsonPropertyName("scores_csv")]
		public string Score {
			get
			{
				return $"{Player1Score}-{Player2Score}";
			}
			set {
				if (string.IsNullOrEmpty(value))
				{
					Player1Score = 0;
					Player2Score = 0;
				}
				else
				{
					// 複数セットか否かを確認する。
					if (value.Contains(","))
					{
						// 複数セット
						// とりあえず、獲得セット数を得点とする。
						int score1 = 0;
						int score2 = 0;
						foreach (var set_score in value.Split(','))
						{
							var scores = set_score.Split('-');
							if (scores.Length == 2)
							{
								var s1 = Convert.ToInt32(scores[0]);
								var s2 = Convert.ToInt32(scores[1]);
								// これをswitchで書けるのは、C#9.0から。
								if (s1 > s2)
								{
									score1 += 1;
								}
								else if (s1 < s2)
								{
									score2 += 1;
								}
								// 同点の場合は特に何もしない。
							}
							else
							{
								throw new Exception("Scoreプロパティに与える文字列の形式が不正です。");
							}
						}
						Player1Score = score1;
						Player2Score = score2;

					}
					else
					{
						// 単一セット
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
			}
		}


		public int Player1Score {
			get; set;
		}

		public int Player2Score
		{
			get; set;
		}

		public void InputScores(int player1Score, int player2Score)
		{
			this.Player1Score = player1Score;
			this.Player2Score = player2Score;
			if (player1Score > player2Score)
			{
				Winner = Player1;
				Loser = Player2;
			}
			else if (player1Score < player2Score)
			{
				Winner = Player2;
				Loser = Player1;
			}
		}

	}
}
