using Neo;
using Neo.SmartContract.Framework.Services;

namespace PEG
{
    public static class DomainStorage
    {
        public static readonly string mapName = "domain";

        public static void Put(string name, UInt160 owner) => Storage.CurrentContext.CreateMap(mapName).Put(name, owner);

        public static UInt160 Get(string name)
        {
            var value = Storage.CurrentContext.CreateMap(mapName).Get(name);
            return value.Length > 0 ? (UInt160)value : UInt160.Zero;
        }

        public static void Delete(UInt160 key) => Storage.CurrentContext.CreateMap(mapName).Delete(key);
    }
}
