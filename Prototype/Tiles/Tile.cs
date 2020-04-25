using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Extensions;
using MLEM.Misc;
using MLEM.Startup;
using MLEM.Textures;
using Prototype.Entities;
using Prototype.Worlds;

namespace Prototype.Tiles {
    public class Tile {

        public TileType Type { get; }
        public TileLayer Layer { get; protected set; }
        public Point Position { get; protected set; }

        public Tile(TileType type) {
            this.Type = type;
        }

        public void Place(TileLayer layer, Point position) {
            this.Layer = layer;
            this.Position = position;
        }

        public virtual void Draw(GameTime time, SpriteBatch batch) {
            // TODO probably make this draw area calc nicer or move it to a better location
            var area = new Rectangle(this.Position.Multiply(World.DrawScale), new Point(World.DrawScale));
            var color = Color.Lerp(Color.Black, Color.White, this.Layer.Brightness);
            batch.Draw(this.Type.TextureRegion, area, color, 0, Vector2.Zero, SpriteEffects.None, this.Layer.Depth);
        }

        public virtual RectangleF GetCollisionBox(CollidingType type) {
            return new RectangleF(this.Position.X, this.Position.Y, 1, 1);
        }

    }
}