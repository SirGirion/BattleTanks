using BattleTanksClient.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleTanksCommon.Network.Entities
{
    public interface IEntityManager
    {
        T AddEntity<T>(T entity) where T : Entity;

        bool RemoveEntity(int entityId);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);

        IEnumerable<T> GetEntitiesOfType<T>();

        IEnumerable<T> GetEntitiesOfType<T>(Func<T, bool> predicate);

        void LoadMap(TiledMap map);
    }

    public class EntityManager : IEntityManager
    {
        private readonly List<Entity> _entities;
        public IEnumerable<Entity> Entities => _entities;

        private CollisionComponent _collisionSpace;

        public EntityManager()
        {
            _entities = new List<Entity>();
        }

        public T AddEntity<T>(T entity) where T : Entity
        {
            _entities.Add(entity);
            if (entity is ICollisionActor actor)
                _collisionSpace.Insert(actor);
            return entity;
        }

        public bool RemoveEntity(int entityId)
        {
            var removed = _entities.RemoveAll(e => e.Id == entityId);
            if (removed > 1)
                throw new InvalidOperationException($"More than one entity had ID {entityId}");
            return removed == 1;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities.Where(e => !e.IsDestroyed).ToList())
            {
                entity.Update(gameTime);
            }
            _collisionSpace.Update(gameTime);

            foreach (var e in _entities.Where(e => e.IsDestroyed).ToList())
            {
                _entities.Remove(e);
                if (e is ICollisionActor actor)
                    _collisionSpace.Remove(actor);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in _entities.Where(e => !e.IsDestroyed).ToList())
            {
                entity.Draw(spriteBatch);
            }
        }
        
        public IEnumerable<T> GetEntitiesOfType<T>()
        {
            return _entities.Where(e => e.GetType() == typeof(T)).Cast<T>();
        }

        public IEnumerable<T> GetEntitiesOfType<T>(Func<T, bool> predicate)
        {
            return GetEntitiesOfType<T>().Where(predicate);
        }

        public void LoadMap(TiledMap map)
        {
            if (map == null)
            {
                map = new TiledMap("default", 100, 100, 32, 32, TiledMapTileDrawOrder.LeftDown, TiledMapOrientation.Orthogonal);
            }
            _collisionSpace = new CollisionComponent(new RectangleF(-1, -1, map.WidthInPixels + 2, map.HeightInPixels + 2));
            // Setup the fake walls
            var leftWall = new DummyCollisionEntity();
            leftWall.Position = new Vector2(-1, -1);
            leftWall.Bounds = new RectangleF(leftWall.Position.ToPoint(), new Size2(1, map.HeightInPixels + 2));

            var rightWall = new DummyCollisionEntity();
            rightWall.Position = new Vector2(map.WidthInPixels + 1, -1);
            rightWall.Bounds = new RectangleF(rightWall.Position.ToPoint(), new Size2(1, map.HeightInPixels + 2));

            var topWall = new DummyCollisionEntity();
            topWall.Position = new Vector2(-1, -1);
            topWall.Bounds = new RectangleF(topWall.Position.ToPoint(), new Size2(map.WidthInPixels + 2, 1));

            var bottomWall = new DummyCollisionEntity();
            bottomWall.Position = new Vector2(-1, map.HeightInPixels + 1);
            bottomWall.Bounds = new RectangleF(bottomWall.Position.ToPoint(), new Size2(map.WidthInPixels + 2, 1));

            _collisionSpace.Insert(leftWall);
            _collisionSpace.Insert(rightWall);
            _collisionSpace.Insert(topWall);
            _collisionSpace.Insert(bottomWall);
        }
    }
}
