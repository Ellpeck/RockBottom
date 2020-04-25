using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Startup;
using MLEM.Textures;
using Prototype.Worlds;

namespace Prototype.Tiles {
    public class TileType {

        public static readonly UniformTextureAtlas Texture = new UniformTextureAtlas(MlemGame.LoadContent<Texture2D>("Tiles"), 8, 8);

        public static readonly TileType Air = new TileType(nameof(Air), null, () => new AirTile());
        public static readonly TileType Rock = new TileType(nameof(Rock), Texture[0, 0]);

        public string Name { get; }
        public TextureRegion TextureRegion { get; }
        public CreateDelegate Create { get; }

        public TileType(string name, TextureRegion textureRegion, CreateDelegate create = null) {
            this.Name = name;
            this.TextureRegion = textureRegion;
            this.Create = create ?? (() => new Tile(this));
        }

        public delegate Tile CreateDelegate();

    }
}