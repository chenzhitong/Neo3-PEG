using Neo;
using System.ComponentModel;
using System.Numerics;

namespace PEG
{
    public partial class PEG
    {
        [DisplayName("addMinter")]
        public static bool AddMinter(UInt160 minterAccount, BigInteger amount)
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
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }
            if (MinterStorage.Exist(minterAccount))
            {
                Error("Minter already exists.");
                return false;
            }
            if (MinterStorage.Add(minterAccount, amount))
            {
                TotalAllowanceStorage.Increase(amount);
                MinterChanged(minterAccount, amount);
                return true;
            }
            return false;
        }

        [DisplayName("removeMinter")]
        public static bool RemoveMinter(UInt160 minterAccount)
        {
            if (IsPaused())
            {
                Error("System is paused.");
                return false;
            }
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }
            if (!MinterStorage.Exist(minterAccount))
            {
                Error("Minter not exists.");
                return false;
            }
            if (BalanceOf(minterAccount) > 0)
            {
                Error("The balance is not 0.");
                return false;
            }
            var allowance = GetAllowance(minterAccount);
            MinterStorage.Remove(minterAccount);
            TotalAllowanceStorage.Reduce(allowance);
            MinterChanged(minterAccount, 0);
            return true;
        }

        [DisplayName("setAdmin")]
        public static bool SetAdmin(UInt160 newAdmin)
        {
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }

            AdminChanged(GetAdmin(), false);
            AdminChanged(newAdmin, true);
            AdminStorage.Put(newAdmin);

            return true;
        }

        [DisplayName("setAllowance")]
        public static bool SetAllowance(UInt160 minterAccount, BigInteger amount)
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
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }
            if (!IsMinter(minterAccount))
            {
                Error("MinterAccount is not minter.");
                return false;
            }
            var oldAllowance = GetAllowance(minterAccount);
            if (amount == oldAllowance) return false;
            if (amount > oldAllowance)
                TotalAllowanceStorage.Increase(amount - oldAllowance);
            if (amount < oldAllowance)
                TotalAllowanceStorage.Reduce(oldAllowance - amount);
            MinterStorage.Add(minterAccount, amount);
            MinterChanged(minterAccount, amount);
            return true;
        }

        [DisplayName("setOwner")]
        public static bool SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }
            OwnerStorage.Put(newOwner);
            return true;
        }
    }
}
