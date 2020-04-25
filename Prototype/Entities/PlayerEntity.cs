using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Extensions;
using MLEM.Input;
using MLEM.Misc;
using MLEM.Startup;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Prototype.Tiles;
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

            var mouse = MlemGame.Input.MousePosition.ToVector2();
            var worldMouse = GameImpl.Instance.Camera.ToWorldPos(mouse) / World.DrawScale;
            var tileMouse = new Point(worldMouse.X.Floor(), worldMouse.Y.Floor());
            if (MlemGame.Input.IsMouseButtonPressed(MouseButton.Left)) {
                this.World.SetTile(TileType.Air, tileMouse);
            } else if (MlemGame.Input.IsMouseButtonPressed(MouseButton.Right)) {
                this.World.SetTile(TileType.Rock, tileMouse);
            }
        }

        public override void Draw(GameTime time, SpriteBatch batch) {
            batch.Draw(batch.GetBlankTexture(), new RectangleF(this.Position * World.DrawScale, new Vector2(World.DrawScale)), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.5F);
        }

        public void AddDebugInfo(Group group) {
            group.AddChild(new Paragraph(Anchor.AutoLeft, 1, p => "Position: " + this.Position));
        }

    }
}