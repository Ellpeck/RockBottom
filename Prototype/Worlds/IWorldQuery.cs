using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MLEM.Misc;
using Prototype.Entities;
using Prototype.Tiles;

namespace Prototype.Worlds {
    public interface IWorldQuery {

        Tile GetTile(Point worldPos, string layer = null);

        void SetTile(Tile tile, Point worldPos, string layer = null);

        IEnumerable<T> EnumerateEntities<T>(RectangleF area) where T : Entity;

        T GetEntity<T>(int id) where T : Entity;

    }
}