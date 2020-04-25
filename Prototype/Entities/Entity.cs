using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Extensions;
using MLEM.Misc;
using Prototype.Worlds;

namespace Prototype.Entities {
    public class Entity {

        public World World { get; protected set; }
        public Vector2 Position { get; set; }
        public int Id { get; internal set; }
        public Vector2 Motion { get; protected set; }
        public Vector2 Gravity { get; protected set; }
        public Vector2 Friction { get; protected set; }
        public bool CollidingHorizontally { get; protected set; }
        public bool CollidingVertically { get; protected set; }
        public bool IsOnGround { get; protected set; }

        public Entity(World world) {
            this.World = world;
            this.Gravity = Vector2.One;
            this.Friction = new Vector2(0.8F, 1);
        }

        public virtual void Update(GameTime time) {
            this.Move(time);
        }

        public virtual void Draw(GameTime time, SpriteBatch batch) {
        }

        public virtual RectangleF GetCollisionBox(CollidingType type, Vector2? position = null) {
            var pos = position ?? this.Position;
            return new RectangleF(pos.X, pos.Y, 1, 1);
        }

        protected virtual void Move(GameTime time) {
            var grav = this.Gravity * this.World.Gravity;
            this.Motion += grav;
            this.Motion *= this.Friction;

            var motion = this.Motion;
            if (motion != Vector2.Zero) {
                var ownBox = this.GetCollisionBox(CollidingType.Collide, this.Position);
                var ownBoxMotion = this.GetCollisionBox(CollidingType.Collide, this.Position + motion);
                for (var x = ownBoxMotion.Left.Floor(); x < ownBoxMotion.Right.Ceil(); x++) {
                    for (var y = ownBoxMotion.Top.Floor(); y < ownBoxMotion.Bottom.Ceil(); y++) {
                        // TODO iterate over all layers here
                        var tile = this.World.GetTile(new Point(x, y));
                        var box = tile.GetCollisionBox(CollidingType.Collide);
                        if (box.IsEmpty)
                            continue;
                        if (ownBox.Right > box.Left && ownBox.Left < box.Right) {
                            if (motion.Y > 0 && ownBox.Bottom <= box.Top) {
                                motion.Y = Math.Min(motion.Y, box.Top - ownBox.Bottom);
                            } else if (motion.Y < 0 && ownBox.Top >= box.Bottom) {
                                motion.Y = Math.Max(motion.Y, box.Bottom - ownBox.Top);
                            }
                        }
                        if (ownBox.Bottom > box.Top && ownBox.Top < box.Bottom) {
                            if (motion.X > 0 && ownBox.Right <= box.Left) {
                                motion.X = Math.Min(motion.X, box.Left - ownBox.Right);
                            } else if (motion.X < 0 && ownBox.Left >= box.Right) {
                                motion.X = Math.Max(motion.X, box.Right - ownBox.Left);
                            }
                        }
                    }
                }
                this.Position += motion;

                this.CollidingHorizontally = motion.X != this.Motion.X;
                this.CollidingVertically = motion.Y != this.Motion.Y;
                this.IsOnGround = this.CollidingVertically && this.Motion.Y > 0;
                if (this.CollidingVertically)
                    this.Motion = new Vector2(this.Motion.X, 0);
            }
        }

    }

    public enum CollidingType {

        Query,
        Draw,
        Collide

    }
}