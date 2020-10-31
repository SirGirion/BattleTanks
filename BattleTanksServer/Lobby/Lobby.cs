using BattleTanksCommon.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksServer.Lobby
{
    public class Lobby : ILobby
    {
        public bool IsFinished => throw new NotImplementedException();

        public int PlayerCount => throw new NotImplementedException();

        /// <summary>
        /// Server instance this lobby is running on.
        /// </summary>
        private Server _server;

        public Lobby(Server server)
        {
            _server = server;
        }

        public void AddPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public void AddPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public void EndLobby()
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public void StartLobby()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
