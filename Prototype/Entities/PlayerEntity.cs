using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Extensions;
using MLEM.Misc;
using MLEM.Startup;
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
            const int range = 3;
            for (var x = -range; x <= range; x++) {
                for (var y = -range; y <= range; y++) {
                    var pos = (this.Position / Chunk.Size).ToPoint() + new Point(x, y);
                    this.World.GetChunk(pos);
                }
            }

            if (this.IsOnGround && MlemGame.Input.IsKeyPressed(Keys.Space))
                this.Motion += new Vector2(0, -0.25F);
            if (MlemGame.Input.IsKeyDown(Keys.A))
                this.Motion += new Vector2(-0.05F, 0);
            if (MlemGame.Input.IsKeyDown(Keys.D))
                this.Motion += new Vector2(0.05F, 0);
        }

        public override void Draw(GameTime time, SpriteBatch batch, float depth) {
            batch.Draw(batch.GetBlankTexture(), new RectangleF(this.Position * World.DrawScale, new Vector2(World.DrawScale)), Color.Red);
        }

        public void AddDebugInfo(Group group) {
            group.AddChild(new Paragraph(Anchor.AutoLeft, 1, p => "Position: " + this.Position));
        }

    }
}