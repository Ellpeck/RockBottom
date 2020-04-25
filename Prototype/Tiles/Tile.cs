using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Extensions;
using MLEM.Misc;
using Prototype.Entities;
using Prototype.Worlds;

namespace Prototype.Tiles {
    public class Tile {

        public Point Position { get; protected set; }
        public TileLayer Layer { get; protected set; }

        public Tile(TileLayer layer, Point position) {
            this.Position = position;
            this.Layer = layer;
        }

        public virtual void Draw(GameTime time, SpriteBatch batch, float depth) {
            // TODO probably make this draw area calc nicer or move it to a better location
            var area = new Rectangle(this.Position.Multiply(World.DrawScale), new Point(World.DrawScale));
            batch.Draw(batch.GetBlankTexture(), area, Color.Gray);
        }

        public virtual bool IsColliding(RectangleF area, CollidingType type) {
            return area.Intersects(new RectangleF(this.Position.ToVector2(), Vector2.One));
        }

    }
}