using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Cameras;
using MLEM.Extensions;
using MLEM.Misc;
using Prototype.Entities;
using Prototype.Tiles;

namespace Prototype.Worlds {
    public class Chunk : IWorldQuery {

        public const int Size = 16;

        private readonly Dictionary<string, TileLayer> layers = new Dictionary<string, TileLayer>();
        // TODO consider making this non-concurrent?
        private readonly IDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        public World World { get; }
        public TileLayer this[string s] => this.layers[s];
        public Point Position { get; }
        public Point WorldPosition => this.Position.Multiply(Size);
        private TimeSpan loadTime;

        public Chunk(World world, Point position) {
            this.Position = position;
            this.World = world;

            // TODO layer addition + layer depth?
            this.layers.Add("Main", new TileLayer(this, 0.55F, 1));
            this.layers.Add("Background", new TileLayer(this, 0.45F, 0.65F));
        }

        public Tile GetTile(Point worldPos, string layer = null) {
            return this.layers[layer ?? "Main"].GetTile(worldPos);
        }

        public void SetTile(TileType tile, Point worldPos, string layer = null) {
            this.layers[layer ?? "Main"].SetTile(tile, worldPos);
        }

        public IEnumerable<T> EnumerateEntities<T>(RectangleF area) where T : Entity {
            foreach (var entity in this.entities.Values) {
                if (!(entity is T t))
                    continue;
                var box = t.GetCollisionBox(CollidingType.Query);
                if (!box.IsEmpty && box.Intersects(area))
                    yield return t;
            }
        }

        public T GetEntity<T>(int id) where T : Entity {
            if (this.entities.TryGetValue(id, out var entity) && entity is T t)
                return t;
            return null;
        }

        internal void AddEntity(Entity entity) {
            this.entities.Add(entity.Id, entity);
        }

        internal void Refresh() {
            this.loadTime = TimeSpan.FromSeconds(20);
        }

        internal bool Update(GameTime time) {
            foreach (var entity in this.entities.Values) {
                entity.Update(time);

                var newPos = new Point((entity.Position.X / Size).Floor(), (entity.Position.Y / Size).Floor());
                if (newPos != this.Position) {
                    var chunk = this.World.GetChunk(newPos, false);
                    if (chunk != null && this.entities.Remove(entity.Id))
                        chunk.AddEntity(entity);
                }
            }

            this.loadTime -= time.ElapsedGameTime;
            return this.loadTime <= TimeSpan.Zero;
        }

        internal void Draw(GameTime time, SpriteBatch batch, RectangleF visible) {
            foreach (var entity in this.entities.Values) {
                var box = entity.GetCollisionBox(CollidingType.Draw);
                if (!box.IsEmpty && box.Intersects(visible))
                    entity.Draw(time, batch);
            }

            var visibleHere = RectangleF.Intersect(visible, new RectangleF(this.WorldPosition.ToVector2(), new Vector2(Size)));
            visibleHere.Offset(-this.WorldPosition.X, -this.WorldPosition.Y);
            var maxX = visibleHere.Right.Ceil();
            var maxY = visibleHere.Bottom.Ceil();
            foreach (var layer in this.layers.Values) {
                for (var x = visibleHere.Left.Floor(); x < maxX; x++) {
                    for (var y = visibleHere.Top.Floor(); y < maxY; y++)
                        layer.GetTile(this.WorldPosition + new Point(x, y)).Draw(time, batch);
                }
            }
        }

    }
}