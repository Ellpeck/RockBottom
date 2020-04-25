using Microsoft.Xna.Framework;
using Prototype.Tiles;

namespace Prototype.Worlds {
    public class TileLayer {

        private readonly Chunk chunk;
        public float Depth { get; private set; }
        private Tile[,] tiles;
        public Tile this[Point pos] {
            get => this.tiles[pos.X, pos.Y];
            set => this.tiles[pos.X, pos.Y] = value;
        }

        public TileLayer(Chunk chunk, float depth) {
            this.chunk = chunk;
            this.Depth = depth;

            this.tiles = new Tile[Chunk.Size, Chunk.Size];
            for (var x = 0; x < Chunk.Size; x++) {
                for (var y = 0; y < Chunk.Size; y++) {
                    var innerPos = new Point(x, y);
                    var tilePos = chunk.WorldPosition + innerPos;
                    // TODO
                    this[innerPos] = tilePos.Y > 1 || x == 1 ? new Tile(this, tilePos) : new AirTile(this, tilePos);
                }
            }
        }

    }
}