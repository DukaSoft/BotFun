using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BotFun.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("ping", "Recieve a ping message!")]
	public async Task HandlePingCommand()
	{
		await RespondAsync("PING!");
	}

	[SlashCommand("dadjoke", "Tells a dadjoke!")]
	public async Task HandleDadJoke()
	{
		HttpClient httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

		var joke = await httpClient.GetFromJsonAsync<Joke>("https://icanhazdadjoke.com");
		if (joke is not null)
		{
			await RespondAsync(joke.joke);
		}
		else
		{
			await RespondAsync("There was an error getting a dadjoke!");
		}
	}

	[SlashCommand("momjoke", "Tells a momjoke!")]
	public async Task HandleMomJoke()
	{

		await RespondAsync("You do not joke about moms!");

	}
}
