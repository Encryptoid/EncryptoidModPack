using Eleon.Modding;
using System;

namespace EmpyrionModdingFramework
{
    public abstract partial class EmpyrionModdingFrameworkBase : IMod, ModInterface
    {
        protected static ModGameAPI LegacyAPI { get; private set; }
        protected static IModApi ModAPI { get; private set; }

        public string ModName { get; set; }

        protected ConfigManager ConfigManager { get; private set; }
        protected CommandManager CommandManager { get; private set; }
        protected RequestManager RequestManager { get; private set; }
        protected FrameworkConfig FrameworkConfig { get; private set; }
        protected Helpers Helpers { get; private set; }

        protected delegate void Game_EventHandler(CmdId eventId, ushort seqNr, object data);
        protected event Game_EventHandler Game_EventRaised;

        protected delegate void Game_ExitHandler();
        protected event Game_ExitHandler Game_ExitRaised;

        protected delegate void Game_UpdateHandler(ulong tick);
        protected event Game_UpdateHandler Game_UpdateRaised;

        public bool IsDebug = false;

        protected abstract void Initialize();

        public void Init(IModApi modApi)
        {
            ModAPI = modApi;

            ConfigManager = new ConfigManager();
            CommandManager = new CommandManager(ModAPI, RequestManager);
            Helpers = new Helpers(ModAPI, RequestManager);
            FrameworkConfig = new FrameworkConfig();

            ModAPI.Application.ChatMessageSent += CommandManager.ProcessChatMessageAsync;
            ModAPI.Application.OnPlayfieldLoaded += ApplicationOnOnPlayfieldLoaded;

            try
            {
                Initialize();
            }
            catch (Exception error)
            {
                Log("Initialization exception.");
                Log(error.ToString());
            }
            if (LegacyAPI == null)
            {
                LogDebug("LegacyAPI not found. Only Client mods supported.");
            }
        }

        private void ApplicationOnOnPlayfieldLoaded(IPlayfield playfield)
        {
            Log("Playfield loaded: " + playfield.Name);
        }

        public void Shutdown()
        {
            ModAPI.Application.ChatMessageSent -= CommandManager.ProcessChatMessageAsync;
            Log($"shutting down");
        }

        public void Game_Start(ModGameAPI dediAPI)
        {
            if (dediAPI == null)
            {
                return;
            }
            LegacyAPI = dediAPI;
            RequestManager = new RequestManager(LegacyAPI);
        }

        public void Game_Event(CmdId eventId, ushort seqNr, object data)
        {
            try
            {
                if (RequestManager.HandleRequestResponse(eventId, seqNr, data))
                {
                    LogDebug($"RequestManager is handling Event {eventId} for the Request {seqNr}.");
                }
            }
            catch (Exception error)
            {
                LogDebug($"Game_Event Exception: EventId: {eventId} SeqNr: {seqNr} Data: {data?.ToString()} Error: {error}");
            }

            switch (eventId)
            {
                case CmdId.Event_Ok:
                    LogDebug($"Game_Event OK for SeqNr: {seqNr} Data: {data?.ToString()}");
                    break;
                case CmdId.Event_Error:
                    ErrorInfo err = (ErrorInfo)data;
                    LogDebug($"Game_Event ERROR for SeqNr: {seqNr}, ErrorType: {err.errorType}");
                  
                    break;
                default:
                    LogDebug($"Game_Event {eventId} SeqNr: {seqNr} Data: {data?.ToString()}");
                    break;
            }

            Game_EventRaised?.Invoke(eventId, seqNr, data);
        }

        public void Game_Exit()
        {
            Game_ExitRaised?.Invoke();
        }

        public void Game_Update()
        {
            Game_UpdateRaised?.Invoke(LegacyAPI.Game_GetTickTime());
        }

        public void Log(string msg)
        {         
             ModAPI.Log($"[{ModName}] {msg}");
        }

        public void LogDebug(string msg)
        {
            if (IsDebug)
            {
                Log(msg);
            }
        }
    }
}