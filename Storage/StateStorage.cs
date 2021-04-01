using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class StateStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "state";

        public static void Pause() => Storage.CurrentContext.CreateMap(mapName).Put(key, "pause");

        public static void Resume() => Storage.CurrentContext.CreateMap(mapName).Put(key, "");

        public static string GetState() => Storage.CurrentContext.CreateMap(mapName).Get(key) == "pause" ? "pause" : "run";

        public static bool IsPaused() => GetState() == "pause";
    }
}
