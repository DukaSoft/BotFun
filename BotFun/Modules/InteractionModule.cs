using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BotFun.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
	private readonly ulong roleId = 1246052153949294672;

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

	[SlashCommand("newjoke", "Add a new joke!")]
	public async Task HandleNewJokeCommand()
	{
		var button = new ButtonBuilder()
		{
			Label = "Click me to add a new joke!",
			CustomId = "button",
			Style = ButtonStyle.Primary
		};

		var component = new ComponentBuilder();
		component.WithButton(button);

		await RespondAsync("", components: component.Build());
	}

	[SlashCommand("wasfirst", "Were you first?")]
	public async Task HandleWasFirstCommand()
	{
		var menu = new SelectMenuBuilder()
		{
			CustomId = "menu",
			Placeholder = "Was First!"
		};
		menu.AddOption("You were FIRST!", "first");
		menu.AddOption("Ohh no, too late!", "second");

		var component = new ComponentBuilder();

		component.WithSelectMenu(menu);

		await RespondAsync("", components: component.Build());

	}

	[ComponentInteraction("menu")]
	public async Task HandleMenuSelection(string[] inputs)
	{
		switch (inputs[0])
		{
			case "first":
				await RespondAsync("Wohoo, You were FIRST!");
				break;

			case "second":
				await RespondAsync("Womp womp, too late!");
				break;
		}
	}


	[ModalInteraction("demo_modal")]
	public async Task HandleModalInput(DemoModal modal)
	{
		string input = modal.Greeting;
		await RespondAsync("You've added: " + input);
	}
	[ComponentInteraction("button")]
	public async Task HandleButtonInput()
	{
		await RespondWithModalAsync<DemoModal>("demo_modal");
	}

	[UserCommand("give-role")]
	public async Task HandleUserCommand(IUser user)
	{
		await (user as SocketGuildUser).AddRoleAsync(roleId);

		var roles = (user as SocketGuildUser).Roles;
		string rolesList = string.Empty;
		foreach (var role in roles)
		{
			rolesList += role.Name + "\n";
		}

		await RespondAsync($"User {user.Mention} has the following roles\n" + rolesList);

	}

	[MessageCommand("msg-command")]
	public async Task HandleMessageCommand(IMessage message)
	{
		await RespondAsync($"Message author is: {message.Author.Username}");
	}
}


	public class DemoModal : IModal
	{
		public string Title => "Add a new joke";
		[InputLabel("Joke!")]
		[ModalTextInput("greeting input", TextInputStyle.Short, placeholder: "Be nice...", maxLength: 100)]
		public string Greeting { get; set; }
	}
