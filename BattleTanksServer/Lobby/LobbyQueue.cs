using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleTanksServer.Lobby
{
    /// <summary>
    /// Class for managing the process of creating a lobby, waiting for players and spinning
    /// up the new lobbies. A Server will have one and only LobbyQueue
    /// </summary>
    public class LobbyQueue
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public const int LocalLobby = 0; // Local split-screen lobby
        public const int DeathMatchLobby = 1; // Online death match lobby
        public const int FreeForAllLobby = 2; // Online free for all lobby

        public static readonly int[] LobbyTypes = new[]
        {
            LocalLobby, DeathMatchLobby, FreeForAllLobby
        };

        public static readonly int[] LobbyPlayerSizes = new[]
        {
            2, 2, 2
        };

        private readonly Server _server;
        /// <summary>
        /// List of all currently activate lobbies.
        /// </summary>
        private List<ILobby> _lobbies;
        private Dictionary<int, ConcurrentBag<int>> _lobbyQueues;

        /// <summary>
        /// Load configs for the various types of lobbies
        /// </summary>
        public LobbyQueue(Server server)
        {
            _server = server;
            _lobbies = new List<ILobby>();
            _lobbyQueues = new Dictionary<int, ConcurrentBag<int>>();
            // Initialize the queues per lobby type
            foreach (var lobbyId in LobbyTypes)
                _lobbyQueues[lobbyId] = new ConcurrentBag<int>();
        }

        public void Update(GameTime gameTime)
        {
            // First try and create lobbies from the queue
            foreach (var idAndPlayers in _lobbyQueues)
            {
                var players = idAndPlayers.Value;
                // We have enough people for a lobby, take N players and put them in a lobby
                if (players.Count >= LobbyPlayerSizes[idAndPlayers.Key])
                {
                    var lobby = new Lobby(_server);
                    for (var i = 0; i < LobbyPlayerSizes[idAndPlayers.Key]; i++)
                    {
                        if (players.TryTake(out var playerId))
                        {
                            lobby.AddPlayer(playerId);
                        }
                    }
                    // Check that there is at least N/2 + 1 players in the lobby in case somehow people left.
                    if (lobby.PlayerCount >= (LobbyPlayerSizes[idAndPlayers.Key] / 2) + 1)
                    {
                        lobby.StartLobby();
                        _lobbies.Add(lobby);
                    }
                }
            }
            foreach (var lobby in _lobbies.Where(l => !l.IsFinished).ToArray())
                lobby.Update(gameTime);
            foreach (var lobby in _lobbies.Where(l => l.IsFinished).ToArray())
                _lobbies.Remove(lobby);
        }

        /// <summary>
        /// Adds a player to the queue for a given lobby
        /// </summary>
        /// <param name="lobbyType"></param>
        /// <param name="playerId"></param>
        public void AddPlayerToQueue(int lobbyType, int playerId)
        {
            if (!_lobbyQueues.ContainsKey(lobbyType))
            {
                Logger.Warn($"User {playerId} requested a lobby type ({lobbyType}) that does not exist.");
                return;
            }
            _lobbyQueues[lobbyType].Add(playerId);
        }
    }
}
