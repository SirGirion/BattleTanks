using BattleTanksCommon.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksServer.Lobby
{
    /// <summary>
    /// Interface for the various types of lobbies.
    /// </summary>
    public interface ILobby
    {
        void AddPlayer(int playerId);
        void AddPlayer(Player player);
        void RemovePlayer(int playerId);
        void RemovePlayer(Player player);
        void StartLobby();
        void EndLobby();
        /// <summary>
        /// Updates the state of the lobby
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
        bool IsFinished { get; }
        int PlayerCount { get; }
    }
}
