using System;
using System.Collections.Generic;
using System.Text;

namespace Aldentea.ChallongeSolkoff.Core
{
	public class InputScoreQuestion
	{
		public Action<InputScoreAnswer> InputScoreCallback { get; set; }

		public string Player1Name { get; set; }
		public string Player2Name { get; set; }

	}

	public class InputScoreAnswer
	{
		public bool Ok { get; set; }
		public int Player1Score { get; set; }
		public int Player2Score { get; set; }
	}

}
