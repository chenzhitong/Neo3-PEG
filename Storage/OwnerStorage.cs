using Neo;
using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class OwnerStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "owner";

        public static void Put(UInt160 account) => new StorageMap(Storage.CurrentContext, mapName).Put(key, account);

        public static UInt160 Get()
        {
            var value = new StorageMap(Storage.CurrentContext, mapName).Get(key);
            return value.Length > 0 ? (UInt160)value : PEG.InitialOwnerScriptHash;
        }
    }
}
