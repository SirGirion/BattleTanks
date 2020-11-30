using BattleTanksClient.Controllers;
using BattleTanksClient.Network;
using BattleTanksCommon.Network.Entities;
using BattleTanksCommon.Network.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksClient.Entities
{
    /// <summary>
    /// Handles rendering the entities as well as receiving updates to any entity.
    /// </summary>
    public class EntityManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private MainGame _game;
        private MovementController _movementController;

        private ConcurrentDictionary<int, IRenderable> _renderableEntities;

        private RenderablePlayer _player;

        public EntityManager(MainGame game, NetworkClient client)
        {
            _game = game;
            _movementController = new MovementController(game.Camera, client);

            _renderableEntities = new ConcurrentDictionary<int, IRenderable>();
            
            // Add our entity callbacks
            client.OnNewPlayerPacket += OnNewPlayerPacket;
            client.OnEntitySpawnPacket += OnEntitySpawnPacket;
            client.OnPlayerUpdatePacket += OnPlayerUpdatePacket;
            client.OnEntityRemovedPacket += OnEntityRemovedPacket;
        }

        public void AddRenderableEntity(int entityId, IRenderable entity)
        {
            _renderableEntities[entityId] = entity;
        }

        public void RemoveRenderableEntity(int entityId)
        {
            _renderableEntities.TryRemove(entityId, out _);
        }

        public void LoadMap(TiledMap currentMap)
        {
            _movementController.LoadMap(currentMap);
        }

        public void Update(GameTime gameTime)
        {
            _movementController.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (var renderable in _renderableEntities.Values)
                renderable.Render(spriteBatch);
        }

        private void OnNewPlayerPacket(object sender, NewPlayerPacketArgs args)
        {
            // Server broadcasts a player packet to everyone, make sure we don't double count ourselves
            var packet = args.Packet;
            var playerData = new Player(packet.PlayerId, packet.X, packet.Y, packet.Color);
            if (packet.PlayerId == _game.PlayerId)
            {
                //_player = _entityManager.AddEntity(
                //        new Player(_atlas.GetRegion("tankBody_red"), _atlas.GetRegion("tankRed_barrel1"), _projectileFactory)
                //    );
                _player = new RenderablePlayer(playerData, _game.Atlas);
                _movementController.SetCurrentPlayer(_player);
                AddRenderableEntity(packet.PlayerId, _player);
            }
            else
            {
                var otherPlayer = new RenderablePlayer(playerData, _game.Atlas);
                AddRenderableEntity(packet.PlayerId, otherPlayer);
            }
        }

        private void OnEntitySpawnPacket(object sender, EntitySpawnPacketArgs args)
        {

        }

        private void OnEntityRemovedPacket(object sender, EntityRemovedPacketArgs args)
        {
            RemoveRenderableEntity(args.Packet.EntityId);
        }

        private void OnPlayerUpdatePacket(object sender, PlayerUpdatePacketArgs args)
        {
            var packet = args.Packet;
            _renderableEntities.TryGetValue(packet.PlayerId, out var entity);
            var player = (RenderablePlayer)entity;
            player.Data.Position = new Vector2(packet.X, packet.Y);
            player.Data.Rotation = packet.Rotation;
            player.Data.BarrelPosition = new Vector2(packet.BarrelX, packet.BarrelY);
            player.Data.BarrelRotation = packet.BarrelRotation;
            player.Data.Velocity = new Vector2(packet.VelocityX, packet.VelocityY);
        }
    }
}
