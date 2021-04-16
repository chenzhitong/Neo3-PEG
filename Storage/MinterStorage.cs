using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace PEG
{
    public static class MinterStorage
    {
        public static readonly string mapName = "minter";

        public static bool Initialize()
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = storageMap.Get(mapName);
            if (map != null) return false;
            storageMap.Put(mapName, StdLib.Serialize(new Map<UInt160, BigInteger>()));
            return true;
        }

        public static bool Add(UInt160 key, BigInteger value)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, BigInteger>;

            map[key] = value;

            storageMap.Put(mapName, StdLib.Serialize(map));

            return true;
        }

        public static BigInteger Get(UInt160 key)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, BigInteger>;
            if (!map.HasKey(key)) return 0;
            return map[key];
        }

        public static void Remove(UInt160 key)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, BigInteger>;

            map.Remove(key);

            storageMap.Put(mapName, StdLib.Serialize(map));
        }

        public static void ReduceAllowance(UInt160 key, BigInteger amount) => Add(key, Get(key) - amount);

        public static void IncreaseAllowance(UInt160 key, BigInteger amount) => Add(key, Get(key) + amount);

        public static bool IncludeWitness()
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, BigInteger>;

            foreach (var account in map.Keys)
            {
                if (Runtime.CheckWitness(account)) return true;
            }
            return false;
        }

        public static bool Exist(UInt160 key)
        {
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, mapName);
            var map = StdLib.Deserialize(storageMap.Get(mapName)) as Map<UInt160, BigInteger>;

            return map.HasKey(key);
        }
    }
}
