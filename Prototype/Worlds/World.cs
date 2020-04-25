using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Extensions;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Prototype.Entities;
using Prototype.Tiles;

namespace Prototype.Worlds {
    public class World : IWorldQuery {

        public const int DrawScale = 16;
        // TODO figure out a better way to do this than concurrent dicts, probably? maybe not if we multi-thread chunk gen
        private readonly IDictionary<Point, Chunk> chunks = new ConcurrentDictionary<Point, Chunk>();
        private int nextEntityId;

        public Chunk GetChunk(Point chunkPos) {
            if (!this.chunks.TryGetValue(chunkPos, out var chunk)) {
                chunk = new Chunk(chunkPos);
                this.chunks.Add(chunkPos, chunk);
            }
            return chunk;
        }

        public Tile GetTile(Point worldPos, string layer = null) {
            return this.GetChunk(ToChunkPos(worldPos)).GetTile(worldPos, layer);
        }

        public void SetTile(Tile tile, Point worldPos, string layer = null) {
            this.GetChunk(ToChunkPos(worldPos)).SetTile(tile, worldPos, layer);
        }

        public IEnumerable<T> EnumerateEntities<T>(RectangleF area) where T : Entity {
            foreach (var chunk in this.GetChunksForArea(area)) {
                foreach (var entity in chunk.EnumerateEntities<T>(area))
                    yield return entity;
            }
        }

        public T GetEntity<T>(int id) where T : Entity {
            foreach (var chunk in this.chunks.Values) {
                var ret = chunk.GetEntity<T>(id);
                if (ret != null)
                    return ret;
            }
            return null;
        }

        public void AddEntity(Entity entity) {
            entity.Id = this.nextEntityId;
            this.nextEntityId++;
            this.GetChunk(ToChunkPos(entity.Position)).AddEntity(entity);
        }

        private IEnumerable<Chunk> GetChunksForArea(RectangleF area) {
            var (left, top) = ToChunkPos(new Vector2(area.Left, area.Top));
            var (right, bottom) = ToChunkPos(new Vector2(area.Right, area.Bottom));
            for (var x = left; x <= right; x++) {
                for (var y = top; y <= bottom; y++)
                    yield return this.GetChunk(new Point(x, y));
            }
        }

        internal void Update(GameTime time) {
            foreach (var chunk in this.chunks.Values)
                chunk.Update(time);
        }

        internal void Draw(GameTime time, SpriteBatch batch, RectangleF visible) {
            var minX = (visible.Left / Chunk.Size).Floor();
            var minY = (visible.Top / Chunk.Size).Floor();
            var maxX = (visible.Right / Chunk.Size).Floor();
            var maxY = (visible.Bottom / Chunk.Size).Floor();
            var i = 0;
            for (var x = minX; x <= maxX; x++) {
                for (var y = minY; y <= maxY; y++) {
                    if (this.chunks.TryGetValue(new Point(x, y), out var chunk))
                        chunk.Draw(time, batch, visible);
                    i++;
                }
            }
            Console.WriteLine(i);
        }

        internal void AddDebugInfo(Group group) {
            group.AddChild(new Paragraph(Anchor.AutoLeft, 1, p => "Chunks: " + this.chunks.Count));
        }

        public static Point ToChunkPos(Vector2 worldPos) {
            return ToChunkPos(new Point(worldPos.X.Floor(), worldPos.Y.Floor()));
        }

        public static Point ToChunkPos(Point worldPos) {
            return worldPos.Divide(Chunk.Size);
        }

    }
}