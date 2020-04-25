using Microsoft.Xna.Framework;
using Prototype.Tiles;

namespace Prototype.Worlds {
    public class TileLayer {

        private readonly Chunk chunk;
        public float Depth { get; private set; }
        public float Brightness { get; private set; }
        private Tile[,] tiles;

        public TileLayer(Chunk chunk, float depth, float brightness) {
            this.chunk = chunk;
            this.Depth = depth;
            this.Brightness = brightness;

            this.tiles = new Tile[Chunk.Size, Chunk.Size];
            for (var x = 0; x < Chunk.Size; x++) {
                for (var y = 0; y < Chunk.Size; y++) {
                    var tilePos = chunk.WorldPosition + new Point(x, y);
                    // TODO
                    var type = tilePos.Y > 1 || x == 1 ? TileType.Rock : TileType.Air;
                    this.SetTile(type, tilePos);
                }
            }
        }

        public Tile GetTile(Point worldPos) {
            var (x, y) = worldPos - this.chunk.WorldPosition;
            return this.tiles[x, y];
        }

        public void SetTile(TileType type, Point worldPos) {
            var tile = type.Create();
            tile.Place(this, worldPos);
            var (x, y) = worldPos - this.chunk.WorldPosition;
            this.tiles[x, y] = tile;
        }

    }
}