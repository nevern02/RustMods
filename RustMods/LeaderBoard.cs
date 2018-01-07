
using Oxide.Core;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("LeaderBoard", "open_mailbox", "0.1")]
    [Description("A leaderboard for encouraging competition in various categories.")]
    class LeaderBoard : RustPlugin
    {
        #region definitions
        private class StoredData
        {
            public const string FILE_NAME = "leaderboard_data";

            public List<PlayerInfo> Players = new List<PlayerInfo>();
        }

        private class PlayerInfo
        {
            public ulong  UserId { get; }
            public string Name   { get; }

            public int Kills  { get; set; }
            public int Deaths { get; set; }
            public int Streak { get; set; }

            public PlayerInfo(BasePlayer player)
            {
                UserId = player.userID;
                Name   = player.displayName;
                Kills  = 0;
                Deaths = 0;
                Streak = 0;
            }
        }
        #endregion

        private StoredData _storedData;

        #region oxide hooks
        void Loaded()
        {
            _storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>(StoredData.FILE_NAME);
        }

        void OnPlayerDie(BasePlayer player, HitInfo info)
        {
            if (player.lastAttacker == null) return; // Can't determine killer
            if (!(player.lastAttacker is BasePlayer)) return;

            var data         = GetPlayerInfo(player);
            var attackerData = GetPlayerInfo(player.lastAttacker as BasePlayer);

            data.Deaths++;
            attackerData.Kills++;

            data.Streak = 0;
            attackerData.Streak++;

            Interface.Oxide.DataFileSystem.WriteObject(StoredData.FILE_NAME, _storedData);
        }
        #endregion

        #region util functions
        private PlayerInfo GetPlayerInfo(BasePlayer player)
        {
            var data = _storedData.Players.Find(x => x.UserId == player.userID);

            if (data == null)
            {
                data = new PlayerInfo(player);
                _storedData.Players.Add(data);
            }

            return data;
        }
        #endregion
    }
}
