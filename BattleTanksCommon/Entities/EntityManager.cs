using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleTanksCommon.Entities
{
    public interface IEntityManager
    {
        T AddEntity<T>(T entity) where T : Entity;

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
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

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities.Where(e => !e.IsDestroyed))
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
            foreach (var entity in _entities.Where(e => !e.IsDestroyed))
            {
                entity.Draw(spriteBatch);
            }
        }

        public void LoadMap(TiledMap map)
        {
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
