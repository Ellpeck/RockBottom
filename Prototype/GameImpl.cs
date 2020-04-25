using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Cameras;
using MLEM.Extensions;
using MLEM.Font;
using MLEM.Misc;
using MLEM.Startup;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Prototype.Entities;
using Prototype.Worlds;

namespace Prototype {
    public class GameImpl : MlemGame {

        public static GameImpl Instance { get; private set; }
        private World world;
        private PlayerEntity player;
        public Camera Camera { get; private set; }

        public GameImpl() {
            Instance = this;
            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            base.LoadContent();

            this.world = new World();
            this.player = new PlayerEntity(this.world) {Position = new Vector2(0, -10)};
            this.world.AddEntity(this.player);
            this.Camera = new Camera(this.GraphicsDevice) {
                Scale = 3,
                LookingPosition = new Vector2(0, -10)
            };

            this.UiSystem.Style.Font = new GenericSpriteFont(LoadContent<SpriteFont>("Font"));
            this.UiSystem.Style.TextScale = 0.15F;
            this.InputHandler.HandleKeyboardRepeats = false;

            var debug = new Group(Anchor.TopLeft, Vector2.One);
            this.world.AddDebugInfo(debug);
            this.player.AddDebugInfo(debug);
            this.UiSystem.Add("Debug", debug);
        }

        protected override void DoUpdate(GameTime gameTime) {
            base.DoUpdate(gameTime);
            this.world.Update(gameTime);

            var goalPos = (this.player.Position + new Vector2(0.5F, -1)) * World.DrawScale;
            this.Camera.LookingPosition = Vector2.Lerp(this.Camera.LookingPosition, goalPos, 0.5F);
        }

        protected override void DoDraw(GameTime gameTime) {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.DoDraw(gameTime);

            this.SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, this.Camera.ViewMatrix);
            var rect = this.Camera.GetVisibleRectangle();
            var visible = new RectangleF(rect.Left / World.DrawScale, rect.Top / World.DrawScale, rect.Width / World.DrawScale, rect.Height / World.DrawScale);
            this.world.Draw(gameTime, this.SpriteBatch, visible);
            this.SpriteBatch.End();
        }

    }
}