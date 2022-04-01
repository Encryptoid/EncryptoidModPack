using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Eleon;
using Eleon.Modding;

namespace EmpyrionModdingFramework
{
    public enum PlayerPermission
    {
        None = 0,
        GameMaster = 3,
        Moderator = 6, // Admin?
        Admin = 9  // Owner?
    }
    public class CommandManager
    {
        private readonly IModApi modAPI;
        private readonly RequestManager _requestManager;

        public CommandManager(in IModApi refModApi, RequestManager requestManager)
        {
            modAPI = refModApi;
            _requestManager = requestManager;
        }

        public List<ChatCommand> CommandList { get; set; } = new List<ChatCommand>();

        public string CommandPrexix { get; set; }

        public void ProcessChatMessageAsync(MessageData data)
        {
            modAPI.Log($"Chat message to channel = {data.Channel} with text = {data.Text}");

            switch (data.Channel)
            {
                case Eleon.MsgChannel.Server:
                    ProcessChatCommand(data);
                    break;
                case Eleon.MsgChannel.Global:
                    break;
                case Eleon.MsgChannel.Alliance:
                    break;
                case Eleon.MsgChannel.Faction:
                    break;
                case Eleon.MsgChannel.SinglePlayer:
                    break;
                default:
                    modAPI.Log($"Unknown Eleon.MsgChannel = {data.Channel}");
                    break;
            }
        }

        public async void ProcessChatCommand(MessageData data)
        {
            ChatCommand chatCommand = CommandList.FirstOrDefault(C => data.Text.Split()[0].Trim().Equals(CommandPrexix + C.cmdText));
            if (chatCommand?.cmdHandler == null && chatCommand?.cmdParamHandler == null)
            {
                return;
            }

            PlayerInfo player = (PlayerInfo)await _requestManager.SendGameRequest(
                CmdId.Request_Player_Info, new Id() { id = data.SenderEntityId }
            );

            var parameters = data.Text.Split();

            if (chatCommand.ParamCount + 1 > parameters.Count()) //+1 for command text
            {
                modAPI.Log($"Player {player.playerName} attempted command {data.Text} but passed incorrect parameters. Required Count: {chatCommand.ParamCount}, Their Count: {parameters.Count()-1}");
                return;
            }

            if (player.permission < (int) chatCommand.Permission)
            {
                modAPI.Log($"Player {player.playerName} attempted command {data.Text} but did not have permission. Required: {chatCommand.Permission}, Theirs: {player.permission}");
                return;
            }

            if(chatCommand.ParamCount > 0)
            {
                await chatCommand.cmdParamHandler(data, parameters.Skip(1).Select(p => p.Trim()).ToArray());
                return;
            }

            await chatCommand.cmdHandler(data);
        }
    }

    public class ChatCommand
    {
        public delegate Task ChatCommandHandler(MessageData messageData);
        public ChatCommandHandler cmdHandler;

        public delegate Task ChatCommandHandlerParams(MessageData messageData, object[] parameters);
        public ChatCommandHandlerParams cmdParamHandler;

        public readonly string cmdText;
        public PlayerPermission Permission;
        public int ParamCount;

        public ChatCommand(string command, ChatCommandHandlerParams handler, PlayerPermission permission, int paramCount) 
        { // Param cnstr
            ParamCount = paramCount;
            cmdText = command;
            cmdParamHandler = handler;
            Permission = permission;
        }

        public ChatCommand(string command, ChatCommandHandler handler, PlayerPermission permission = PlayerPermission.None) 
        { // No param cnstr
            ParamCount = 0;
            cmdText = command;
            cmdHandler = handler;
            Permission = permission;
        }

        public static bool ParseIntParam(object[] parameters, int paramIndex, out int paramValue)
        {
            return int.TryParse(parameters[paramIndex] as string, out paramValue);
        }

        public static bool ParseStringParam(object[] parameters, int paramIndex, out string paramValue)
        {
            paramValue = parameters[paramIndex].ToString();
            return paramValue != null;
        }
    }
}