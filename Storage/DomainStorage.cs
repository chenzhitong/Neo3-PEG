using Neo;
using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class DomainStorage
    {
        public static readonly string mapName = "domain";

        public static void Put(string name, UInt160 owner) => new StorageMap(Storage.CurrentContext, mapName).Put(name, owner);

        public static UInt160 Get(string name)
        {
            var value = new StorageMap(Storage.CurrentContext, mapName).Get(name);
            return value.Length > 0 ? (UInt160)value : UInt160.Zero;
        }

        public static void Delete(UInt160 key) => new StorageMap(Storage.CurrentContext, mapName).Delete(key);
    }
}
