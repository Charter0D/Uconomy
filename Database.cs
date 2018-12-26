using LiteDB;
using Rocket.Core.Logging;
using System;
using System.IO;
using Logger = Rocket.Core.Logging.Logger;

namespace fr34kyn01535.Uconomy
{
    public class DatabaseManager
    {
        public string Name = "Uconomy";
        public string Path;
        public LiteCollection<Account> Base;

        public DatabaseManager()
        {
            Path = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Database" + System.IO.Path.DirectorySeparatorChar + Name + ".db";
            Logger.Log(Path);
            if (!Directory.Exists(Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Database"))
            {
                Directory.CreateDirectory((Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Database"));
            }
            var db = new LiteDatabase(Path);
            Base = db.GetCollection<Account>(Name);
            Logger.Log("LiteDB loaded.");
        }
        
        /// <summary>
        /// returns the current balance of an account
        /// </summary>
        /// <param name="steamId">SteamID of a player</param>
        /// <returns></returns>
        public decimal GetBalance(string id)
        {
            return Base.FindOne(x => x.ID == ulong.Parse(id)).Balance;
        }

        /// <summary>
        /// Increasing balance to increaseBy (can be negative)
        /// </summary>
        /// <param name="steamId">steamid of the accountowner</param>
        /// <param name="increaseBy">amount to change</param>
        /// <returns>the new balance</returns>
        public decimal IncreaseBalance(string id, decimal increaseBy)
        {
            var account = Base.FindOne(x => x.ID == ulong.Parse(id));
            account.Balance += increaseBy;
            Base.Update(account);
            return account.Balance;
        }

        
        public void CheckSetupAccount(Steamworks.CSteamID id)
        {
            if (Base.FindOne(x => x.ID == id.m_SteamID) == null)
            {
                Base.Insert(new Account() { ID = id.m_SteamID, Balance = Uconomy.Instance.Configuration.Instance.InitialBalance });
            }
        }
        

        public class Account
        {
            public ulong ID { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
