using BotFun.Modules;
using BotFun.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotFun;

public class Program
{
	public static Task Main() => new Program().MainAsync();

	private string? _BotToken;
	private string? _BotGuildId;

	public async Task MainAsync()
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets<Program>()
			.Build();

		_BotToken = config["discordToken"]?.ToString();
		_BotGuildId = config["discordGuild"]?.ToString();
		
		// TODO: Finish config

		using IHost host = Host.CreateDefaultBuilder()
			.ConfigureServices((_, services) =>
				services
				.AddSingleton(config)
				.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
				{
					GatewayIntents = GatewayIntents.All,
					AlwaysDownloadUsers = true
				}
			))
				.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
				.AddSingleton<InteractionHandler>()
				.AddSingleton(x => new CommandService())
				.AddSingleton<PrefixHandler>()
			)
			.Build();

		await RunAsync(host);
	}

	public async Task RunAsync(IHost host)
	{
		using IServiceScope serviceScope = host.Services.CreateScope();
		IServiceProvider provider = serviceScope.ServiceProvider;

		var _client = provider.GetRequiredService<DiscordSocketClient>();
		var sCommands = provider.GetRequiredService<InteractionService>();
		await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

		var pCommands = provider.GetRequiredService<PrefixHandler>();
		pCommands.AddModule<PrefixModule>();
		await pCommands.InitializeAsync();

		_client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
		sCommands.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
		

		_client.Ready += async () =>
		{
			Console.WriteLine("Bot Ready!");
			//await sCommands.RegisterCommandsToGuildAsync(UInt64.Parse(config["test"]))
			//await sCommands.RegisterCommandsGloballyAsync();
			await sCommands.RegisterCommandsToGuildAsync(UInt64.Parse(_BotGuildId ?? ""));
		};

		await _client.LoginAsync(TokenType.Bot, _BotToken);
		await _client.StartAsync();

		await Task.Delay(-1);
	}
}