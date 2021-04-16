using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class StateStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "state";

        public static void Pause() => new StorageMap(Storage.CurrentContext, mapName).Put(key, "pause");

        public static void Resume() => new StorageMap(Storage.CurrentContext, mapName).Put(key, "");

        public static string GetState() => new StorageMap(Storage.CurrentContext, mapName).Get(key) == "pause" ? "pause" : "run";

        public static bool IsPaused() => GetState() == "pause";
    }
}
