using System;
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
        private readonly Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        public TileLayer this[string s] => this.layers[s];
        public Point Position { get; private set; }
        public Point WorldPosition => this.Position.Multiply(Size);

        public Chunk(Point position) {
            this.Position = position;

            // TODO layer addition + layer depth?
            this.layers.Add("Main", new TileLayer(this, 0.55F));
            this.layers.Add("Background", new TileLayer(this, 0.45F));
        }

        public Tile GetTile(Point worldPos, string layer = null) {
            return this.layers[layer ?? "Main"][worldPos];
        }

        public void SetTile(Tile tile, Point worldPos, string layer = null) {
            this.layers[layer ?? "Main"][worldPos] = tile;
        }

        public IEnumerable<T> EnumerateEntities<T>(RectangleF area) where T : Entity {
            foreach (var entity in this.entities.Values) {
                if (entity is T t && entity.IsColliding(area, CollidingType.Query))
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

        internal void Update(GameTime time) {
            foreach (var entity in this.entities.Values)
                entity.Update(time);
        }

        internal void Draw(GameTime time, SpriteBatch batch, RectangleF visible) {
            foreach (var entity in this.entities.Values) {
                if (entity.IsColliding(visible, CollidingType.Draw))
                    entity.Draw(time, batch, 0.5F);
            }

            var visibleHere = RectangleF.Intersect(visible, new RectangleF(this.WorldPosition.ToVector2(), new Vector2(Size)));
            visibleHere.Offset(-this.WorldPosition.X, -this.WorldPosition.Y);
            var maxX = visibleHere.Right.Ceil();
            var maxY = visibleHere.Bottom.Ceil();
            foreach (var layer in this.layers.Values) {
                for (var x = visibleHere.Left.Floor(); x < maxX; x++) {
                    for (var y = visibleHere.Top.Floor(); y < maxY; y++)
                        layer[new Point(x, y)].Draw(time, batch, layer.Depth);
                }
            }
        }

    }
}