using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Misc;
using Prototype.Entities;
using Prototype.Worlds;

namespace Prototype.Tiles {
    public class AirTile : Tile {

        public AirTile(TileLayer layer, Point position) : base(layer, position) {
        }

        public override RectangleF GetCollisionBox(CollidingType type) {
            return RectangleF.Empty;
        }

        public override void Draw(GameTime time, SpriteBatch batch, float depth) {
            // noop
        }

    }
}