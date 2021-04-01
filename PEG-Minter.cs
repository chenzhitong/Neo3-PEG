using Neo;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace PEG
{
    public partial class PEG
    {
        [DisplayName("burn")]
        public static bool Burn(UInt160 minterAccount, BigInteger amount)
        {
            if (IsPaused())
            {
                Error("System is paused.");
                return false;
            }
            if (amount <= 0)
            {
                Error("The parameter amount MUST be greater than 0.");
                return false;
            }
            if (!IsMinter() || !Runtime.CheckWitness(minterAccount))
            {
                Error("No authorization.");
                return false;
            }
            if (IsBlacklisted(minterAccount))
            {
                Error("Argument account is in blacklist.");
                return false;
            }
            if (AssetStorage.Get(minterAccount) < amount)
            {
                Error("Insufficient balance.");
                return false;
            }

            TotalSupplyStorage.Reduce(amount);

            TotalBurnStorage.Increase(amount);

            AssetStorage.Reduce(minterAccount, amount);

            Transferred(minterAccount, null, amount);

            return true;
        }

        [DisplayName("addSubMinter")]
        public static bool AddSubMinter(UInt160 minterAccount, UInt160 subMinterAccount, BigInteger amount)
        {
            if (IsPaused())
            {
                Error("System is paused.");
                return false;
            }
            if (amount <= 0)
            {
                Error("The parameter amount MUST be greater than 0.");
                return false;
            }
            if (!IsMinter() || !Runtime.CheckWitness(minterAccount))
            {
                Error("No authorization.");
                return false;
            }
            if (IsBlacklisted(minterAccount))
            {
                Error("Argument account is in blacklist.");
                return false;
            }
            if (MinterStorage.Exist(subMinterAccount))
            {
                Error("Minter already exists.");
                return false;
            }
            var allowance = GetAllowance(minterAccount);
            if (allowance < amount)
            {
                Error("Insufficient allowance.");
                return false;
            }

            MinterStorage.ReduceAllowance(minterAccount, amount);
            MinterStorage.IncreaseAllowance(subMinterAccount, amount);
            MinterStorage.Add(subMinterAccount, amount);

            MinterChanged(minterAccount, allowance - amount);
            MinterChanged(subMinterAccount, amount);
            return true;
        }

        [DisplayName("mint")]
        public static bool Mint(UInt160 minterAccount, BigInteger amount)
        {
            if (IsPaused())
            {
                Error("System is paused.");
                return false;
            }
            if (amount <= 0)
            {
                Error("The parameter amount MUST be greater than 0.");
                return false;
            }
            if (!IsMinter() || !Runtime.CheckWitness(minterAccount))
            {
                Error("No authorization.");
                return false;
            }
            if (IsBlacklisted(minterAccount))
            {
                Error("Argument account is in blacklist.");
                return false;
            }
            var allowance = GetAllowance(minterAccount);
            if (allowance < amount)
            {
                Error("Insufficient allowance.");
                return false;
            }

            TotalSupplyStorage.Increase(amount);

            TotalAllowanceStorage.Reduce(amount);

            AssetStorage.Increase(minterAccount, amount);

            MinterStorage.ReduceAllowance(minterAccount, amount);

            Transferred(null, minterAccount, amount);

            return true;
        }
    }
}
