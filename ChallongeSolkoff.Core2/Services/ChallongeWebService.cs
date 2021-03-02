﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;  // for MediaTypeWithQualityHeaderValue.
using System.Text.Json.Serialization;

namespace Aldentea.ChallongeSolkoff.Core2.Services
{
	public class ChallongeWebService : IChallongeWebService
	{
		private static readonly HttpClient client = new HttpClient();

		private static async Task ProcessRepositories()
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json")
				);
			client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");


			var stringTask = client.GetStringAsync("https//api.challonge.com/v1/tounaments.json?");
			var message = await stringTask;

			// do something
		}

		public async Task<IEnumerable<Match>> GetMatches(string tournamentID, string userName, string apiKey)
		{
			var stream = await client.GetStreamAsync($"https//{userName}:{apiKey}@api.challonge.com/v1/tounaments/{tournamentID}/matches.json");
			return await System.Text.Json.JsonSerializer.DeserializeAsync<List<Match>>(stream);
		}

	}



	public interface IChallongeWebService
	{
		Task<IEnumerable<Match>> GetMatches(string tournamentID, string userName, string apiKey);
	}
}