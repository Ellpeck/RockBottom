using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Extensions;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Prototype.Worlds;

namespace Prototype.Entities {
    public class PlayerEntity : Entity {

        public PlayerEntity(World world) : base(world) {
        }

        public override void Update(GameTime time) {
            base.Update(time);

            // TODO proper chunk loading
            const int range = 5;
            for (var x = -range; x <= range; x++) {
                for (var y = -range; y <= range; y++)
                    this.World.GetChunk(new Point(this.Position.X.Floor() + x, this.Position.Y.Floor() + y));
            }
        }

        public override void Draw(GameTime time, SpriteBatch batch, float depth) {
            batch.Draw(batch.GetBlankTexture(), new RectangleF(this.Position * World.DrawScale, new Vector2(World.DrawScale)), Color.Red);
        }

        public void AddDebugInfo(Group group) {
            group.AddChild(new Paragraph(Anchor.AutoLeft, 1, p => "Position: " + this.Position));
        }

    }
}