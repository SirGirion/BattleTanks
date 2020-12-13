using BattleTanksCommon.Network.Entities;
using ENet;
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
        void AddPlayer(Tuple<Peer, int> player);
        void RemovePlayer(int playerId);
        void StartLobby();
        void EndLobby();
        /// <summary>
        /// Updates the state of the lobby
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
        bool IsFinished { get; }
        int PlayerCount { get; }
        int LobbyId { get; }
        IReadOnlyCollection<Peer> PlayerConnections { get; }
    }
}
