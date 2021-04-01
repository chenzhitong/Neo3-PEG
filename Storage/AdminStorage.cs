using Neo;
using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class AdminStorage
    {
        public static readonly string mapName = "contract";

        public static readonly string key = "admin";

        public static void Put(UInt160 account) => Storage.CurrentContext.CreateMap(mapName).Put(key, account);

        public static UInt160 Get()
        {
            var value = Storage.CurrentContext.CreateMap(mapName).Get(key);
            return value.Length > 0 ? (UInt160)value : PEG.InitialOwnerScriptHash;
        }
    }
}
