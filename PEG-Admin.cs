using Neo;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace PEG
{
    public partial class PEG
    {
        [DisplayName("blacklist")]
        public static bool Blacklist(UInt160 account)
        {
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }
            if (GetAdmin().Equals(account) || GetOwner().Equals(account))
            {
                Error("No authorization.");
                return false;
            }

            //BlacklistStorage.Add(account, Ledger.CurrentIndex);
            BlacklistChanged(account, true);
            return true;
        }

        [DisplayName("destroy")]
        public static bool Destroy()
        {
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }
            ContractManagement.Destroy();
            return true;
        }

        [DisplayName("migrate")]
        public static bool Migrate(UInt160 script, string manifest)
        {
            if (!IsPaused())
            {
                Error("System is not paused.");
                return false;
            }
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }
            if (script != null && script.Equals(ContractManagement.GetContract(Runtime.ExecutingScriptHash))) return true;
            ContractManagement.Update(script, manifest);
            return true;
        }

        [DisplayName("pause")]
        public static bool Pause()
        {
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }

            StateStorage.Pause();
            StateChanged("pause");
            return true;
        }

        [DisplayName("resume")]
        public static bool Resume()
        {
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }

            StateStorage.Resume();
            StateChanged("resume");
            return true;
        }

        [DisplayName("unBlacklist")]
        public static bool UnBlacklist(UInt160 account)
        {
            if (!IsAdmin())
            {
                Error("No authorization.");
                return false;
            }

            //BlacklistStorage.Remove(account);
            BlacklistChanged(account, false);
            return true;
        }
    }
}
