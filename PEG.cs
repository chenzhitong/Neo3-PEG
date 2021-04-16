using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace PEG
{
    public partial class PEG : SmartContract
    {
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> Transferred;

        [DisplayName("error")]
        public static event Action<string> Error;

        [DisplayName("stateChanged")]
        public static event Action<string> StateChanged;

        [DisplayName("blacklistChanged")]
        public static event Action<UInt160, bool> BlacklistChanged;

        [DisplayName("minterChanged")]
        public static event Action<UInt160, BigInteger> MinterChanged;

        [DisplayName("adminChanged")]
        public static event Action<UInt160, bool> AdminChanged;

        [InitialValue("NikhQp1aAD1YFCiwknhM5LQQebj4464bCJ", ContractParameterType.Hash160)]
        internal static readonly UInt160 InitialOwnerScriptHash = default;

        [DisplayName("balanceOf")]
        public static BigInteger BalanceOf(UInt160 account) => AssetStorage.Get(account);

        [DisplayName("decimals")]
        public static byte Decimals() => 8;

        [DisplayName("getAdmin")]
        public static UInt160 GetAdmin() => AdminStorage.Get();

        [DisplayName("getAllowance")]
        public static BigInteger GetAllowance(UInt160 minterAccount) => MinterStorage.Get(minterAccount);

        [DisplayName("getOwner")]
        public static UInt160 GetOwner() => OwnerStorage.Get();

        [DisplayName("initialize")]
        public static bool Initialize()
        {
            BlacklistStorage.Initialize();
            MinterStorage.Initialize();
            return true;
        }

        [DisplayName("isBlacklisted")]
        public static bool IsBlacklisted(UInt160 account) => BlacklistStorage.Exist(account);

        private static bool IsAdmin() => Runtime.CheckWitness(GetAdmin());

        private static bool IsMinter() => MinterStorage.IncludeWitness();

        [DisplayName("isMinter")]
        public static bool IsMinter(UInt160 minterAccount) => MinterStorage.Exist(minterAccount);

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        [DisplayName("isPaused")]
        public static bool IsPaused() => StateStorage.IsPaused();

        [DisplayName("name")]
        public static string Name() => "PEG";

        [DisplayName("supportedStandards")]
        public static string[] SupportedStandards() => new string[] { "NEP-5", "NEP-7", "NEP-10" };

        [DisplayName("symbol")]
        public static string Symbol() => "PEG";

        [DisplayName("totalAllowance")]
        public static BigInteger TotalAllowance() => TotalAllowanceStorage.Get();

        [DisplayName("totalBurn")]
        public static BigInteger TotalBurn() => TotalBurnStorage.Get();

        [DisplayName("totalSupply")]
        public static BigInteger TotalSupply() => TotalSupplyStorage.Get();

        [DisplayName("transfer")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null)
        {
            if (IsPaused())
            {
                Error("System is paused.");
                return false;
            }
            if (IsBlacklisted(from))
            {
                Error("Argument from is in blacklist.");
                return false;
            }
            if (IsBlacklisted(to))
            {
                Error("Argument to is in blacklist.");
                return false;
            }
            if (amount <= 0)
            {
                Error("The parameter amount MUST be greater than 0.");
                return false;
            }
            if (!Runtime.CheckWitness(from))
            {
                Error("No authorization.");
                return false;
            }
            if (AssetStorage.Get(from) < amount)
            {
                Error("Insufficient balance.");
                return false;
            }
            if (from == to) return true;

            AssetStorage.Reduce(from, amount);

            AssetStorage.Increase(to, amount);

            Transferred(from, to, amount);

            if (ContractManagement.GetContract(to) != null)
                Contract.Call(to, "onPayment", CallFlags.ReadOnly, new object[] { from, amount, data });

            return true;
        }

        public static bool OnNEP17Payment(UInt160 from, UInt160 to, BigInteger amount, object data) => true;
        public static bool OnNEP11Payment(UInt160 from, BigInteger amount, object data) => true;
    }
}