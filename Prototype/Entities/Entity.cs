using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Misc;
using Prototype.Worlds;

namespace Prototype.Entities {
    public class Entity {

        public World World { get; protected set; }
        public Vector2 Position { get; protected set; }
        public int Id { get; internal set; }

        public Entity(World world) {
            this.World = world;
        }

        public virtual void Update(GameTime time) {
        }

        public virtual void Draw(GameTime time, SpriteBatch batch, float depth) {
        }

        public virtual bool IsColliding(RectangleF area, CollidingType type) {
            return area.Contains(this.Position);
        }

    }

    public enum CollidingType {

        Query,
        Draw

    }
}