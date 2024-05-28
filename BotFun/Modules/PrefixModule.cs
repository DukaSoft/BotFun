using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFun.Modules;

public class PrefixModule : ModuleBase<SocketCommandContext>
{
	[Command("ping")]
	public async Task HandlePingCommand()
	{
		await Context.Message.ReplyAsync("PING!");
	}
}
