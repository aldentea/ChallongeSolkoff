using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;  // for MediaTypeWithQualityHeaderValue.
using System.Text.Json.Serialization;

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

	}



	public interface IChallongeWebService
	{
		Task<IEnumerable<MatchItem>> GetMatches(string tournamentID, string userName, string apiKey);
		Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey);
	}
}
