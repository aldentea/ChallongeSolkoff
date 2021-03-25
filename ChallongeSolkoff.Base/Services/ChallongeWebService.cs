using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;  // for MediaTypeWithQualityHeaderValue.
using System.Text.Json.Serialization;

using System.Xml.Linq;

namespace Aldentea.ChallongeSolkoff.Base.Services
{
	public class ChallongeWebService : IChallongeWebService
	{
		private static readonly HttpClient client = new HttpClient();


		#region *トーナメントからマッチを取得(GetMatches)
		/// <summary>
		/// 指定したトーナメントのすべてのマッチの情報を取得します（開始するまでは空集合です）。
		/// </summary>
		/// <param name="tournamentID">トーナメントID</param>
		/// <param name="userName">Challogeユーザ名</param>
		/// <param name="apiKey">ChallongeのAPIキー</param>
		/// <returns></returns>
		public async Task<IEnumerable<MatchItem>> GetMatches(string tournamentID, string userName, string apiKey)
		{
			//var uri = $@"https://{userName}:{apiKey}@api.challonge.com/v1/tournaments/{tournamentID}/matches.json";
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/matches.json";
			var request = new HttpRequestMessage(HttpMethod.Get, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<List<MatchItem>>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}
		}
		#endregion

		#region *トーナメントから参加者を取得(GetParticipants)
		/// <summary>
		/// 指定したトーナメントのすべての参加者の情報を取得します。
		/// </summary>
		/// <param name="tournamentID">トーナメントID</param>
		/// <param name="userName">Challogeユーザ名</param>
		/// <param name="apiKey">ChallongeのAPIキー</param>
		/// <returns></returns>
		public async Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey)
		{
			// コピペの巻、
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/participants.json";
			var request = new HttpRequestMessage(HttpMethod.Get, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<List<ParticipantItem>>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}
		#endregion


		#region *マッチ情報を更新(UpdateMatch)
		/// <summary>
		/// マッチ情報を更新します（とりあえず勝敗とスコアのみ。）
		/// </summary>
		/// <param name="match"></param>
		/// <param name="tournamentID"></param>
		/// <param name="userName"></param>
		/// <param name="apiKey"></param>
		/// <returns></returns>
		public async Task<MatchItem> UpdateMatch(Match match, string tournamentID, string userName, string apiKey)
		{
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/matches/{match.ID}.json";
			var request = new HttpRequestMessage(HttpMethod.Put, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);

			var content_string = $@"{{ ""match"": {{ ""scores_csv"": ""{match.Score}"", ""winner_id"": {match.Winner} }} }}";
			request.Content = new StringContent(content_string, Encoding.UTF8, "application/json");
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<MatchItem>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}
		#endregion


		#region *トーナメントに参加者を追加(AddParticipant)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="participantName"></param>
		/// <param name="tournamentID"></param>
		/// <param name="userName"></param>
		/// <param name="apiKey"></param>
		/// <param name="misc"></param>
		/// <returns></returns>
		public async Task<ParticipantItem> AddParticipant(string participantName, string tournamentID, string userName, string apiKey, string misc = null)
		{
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/participants.json";
			var request = new HttpRequestMessage(HttpMethod.Post, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);

			var misc_string = string.IsNullOrEmpty(misc) ? string.Empty : $@", ""misc"": ""{misc}""";
			var json_string = $@"{{ ""participant"": {{ ""name"": ""{participantName}""{misc_string} }} }}";
			request.Content = new StringContent(json_string, Encoding.UTF8, "application/json");
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<ParticipantItem>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}
		#endregion

	}


	public interface IChallongeWebService
	{
		Task<IEnumerable<MatchItem>> GetMatches(string tournamentID, string userName, string apiKey);
		Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey);
		Task<MatchItem> UpdateMatch(Match match, string tournamentID, string userName, string apiKey);
		Task<ParticipantItem> AddParticipant(string participantName, string tournamentID, string userName, string apiKey, string misc = null);
	}
}
