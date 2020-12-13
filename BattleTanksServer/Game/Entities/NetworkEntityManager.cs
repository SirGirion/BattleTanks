using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksServer.Game.Entities
{
    /// <summary>
    /// Wraps a regular <see cref="EntityManager"/> to inject network functionality.
    /// </summary>
    public class NetworkEntityManager : IEntityManager
    {
        private Lobby.Lobby _lobby;
        private EntityManager _manager;

        public NetworkEntityManager(Lobby.Lobby lobby)
        {
            _lobby = lobby;
            _manager = new EntityManager();
        }

        public T AddEntity<T>(T entity) where T : Entity
        {
            var entityAdded = _manager.AddEntity(entity);
            var entityAddedPacket = EntitySpawnPacket.Create
            return entityAdded;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _manager.Draw(spriteBatch);
        }

        public IEnumerable<T> GetEntitiesOfType<T>()
        {
            return _manager.GetEntitiesOfType<T>();
        }

        public IEnumerable<T> GetEntitiesOfType<T>(Func<T, bool> predicate)
        {
            return _manager.GetEntitiesOfType<T>(predicate);
        }

        public void LoadMap(TiledMap map)
        {
            _manager.LoadMap(map);
        }

        public bool RemoveEntity(int entityId)
        {
            var removed = _manager.RemoveEntity(entityId);
            if (removed)
            {
                var entityRemovedPacket = EntityRemovedPacket.CreatePacket(entityId);
                _lobby.Server.NetworkServer.ScopedBroadcast(_lobby.PlayerConnections, entityRemovedPacket);
            }
            return removed;
        }

        public void Update(GameTime gameTime)
        {
            _manager.Update(gameTime);
        }
    }
}
