using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;  // for MediaTypeWithQualityHeaderValue.
using System.Text.Json.Serialization;

using System.Xml.Linq;

namespace Aldentea.ChallongeSolkoff.Core.Services
{
	public class ChallongeWebService : IChallongeWebService
	{
		private static readonly HttpClient client = new HttpClient();


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
				return new List<MatchItem>();
			}
			//var stream = await client.GetStreamAsync(uri);
			//return await System.Text.Json.JsonSerializer.DeserializeAsync<List<Match>>(stream);
		}

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
				//return new List<ParticipantItem>();
				throw new Exception(response.StatusCode.ToString());
			}

		}

		public async Task<MatchItem> UpdateMatch(Match match, string tournamentID, string userName, string apiKey)
		{
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/matches/{match.ID}.json";
			var request = new HttpRequestMessage(HttpMethod.Put, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);
			//request.Headers.Add("Content-Type", "application/json");
			
			//var root = new XElement("match", new XAttribute("scores_csv", match.Score), new XAttribute("winner_id", match.Winner));
			//var xml = new XDocument(root);
			//var xml_string = xml.ToString();
			//request.Content = new StringContent(xml_string);

			//System.Text.Json.

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
				//return new List<ParticipantItem>();
				throw new Exception(response.StatusCode.ToString());
			}

		}


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
				//return new List<ParticipantItem>();
				throw new Exception(response.StatusCode.ToString());
			}

		}

	}



	public interface IChallongeWebService
	{
		Task<IEnumerable<MatchItem>> GetMatches(string tournamentID, string userName, string apiKey);
		Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey);
		Task<MatchItem> UpdateMatch(Match match, string tournamentID, string userName, string apiKey);
		Task<ParticipantItem> AddParticipant(string participantName, string tournamentID, string userName, string apiKey, string misc = null);
	}
}
