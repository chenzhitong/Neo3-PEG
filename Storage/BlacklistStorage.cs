using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class BlacklistStorage
    {
        public static readonly string mapName = "blacklist";

        public static bool Initialize()
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = storageMap.Get(mapName);
            var tx = Runtime.ScriptContainer as Transaction;
            var sender = tx.Sender;
            if (map != null) return false;
            storageMap.Put(mapName, StdLib.Serialize(new Map<UInt160, uint>()));
            return true;
        }

        public static void Add(UInt160 key, uint value)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            map[key] = value;

            storageMap.Put(mapName, StdLib.Serialize(map));
        }

        public static void Remove(UInt160 key)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            map.Remove(key);

            storageMap.Put(mapName, StdLib.Serialize(map));
        }

        public static bool Exist(UInt160 key)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            Map<UInt160, uint> map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, uint>;

            return map.HasKey(key);
        }
    }
}
