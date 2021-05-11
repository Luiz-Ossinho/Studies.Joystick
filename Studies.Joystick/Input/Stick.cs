using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Studies.Joystick.Abstract;
using System.Collections.Generic;

namespace Studies.Joystick.Input
{
    public class Stick
    {
        public Stick(float deadZoneSize, Rectangle StartRegion, float aliveZoneSize, float aliveZoneFollowFactor, float aliveZoneFollowSpeed, float edgeSpacing)
        {
            this.deadZoneSize = deadZoneSize;
            this.StartRegion = StartRegion;
            this.aliveZoneFollowFactor = aliveZoneFollowFactor;
            this.aliveZoneFollowSpeed = aliveZoneFollowSpeed;
            this.aliveZoneSize = aliveZoneSize;
            this.edgeSpacing = edgeSpacing;
            Style = TouchStickStyle.Free;
        }
        public readonly float deadZoneSize;
        public readonly float aliveZoneSize;
        public readonly float aliveZoneFollowFactor;
        public readonly float aliveZoneFollowSpeed;
        public readonly float edgeSpacing;
        public TouchLocation? touchLocation = null;
        public readonly Rectangle StartRegion;
        public Vector2 StartLocation;
        public Vector2 FixedLocation;
        public Vector2 Direction;
        public Vector2 Pos;
        public float Magnitude;
        public int lastExcludedRightTouchId = -1;
        public List<Rectangle> startExcludeRegions = new List<Rectangle>(5);
        public TouchStickStyle Style { get; private set; }

        public void SetAsFixed(Vector2 fixedLocation = default)
        {
            if (fixedLocation == default)
                fixedLocation = FixedLocation;
            FixedLocation = fixedLocation;
            StartLocation = fixedLocation;
            Style = TouchStickStyle.Fixed;
        }

        public void SetAsFreefollow()
            => Style = TouchStickStyle.FreeFollow;

        public void SetAsFree()
            => Style = TouchStickStyle.Free;

        public Vector2 GetPositionVector(float aliveZoneSize)
            => StartLocation + (Direction * new Vector2(1, -1) * Magnitude * aliveZoneSize);

        public Vector2 GetRelativeVector(float aliveZoneSize)
            => Direction * new Vector2(1, -1) * Magnitude * aliveZoneSize;

        public void Update(TouchCollection state, TouchLocation? continueLocation, float dt)
        {
            if (continueLocation.HasValue)
            {
                touchLocation = continueLocation;
                Pos = continueLocation.Value.Position;
                EvaluatePoint(dt);
            }
            else
            {
                bool foundNew = false;
                if (touchLocation.HasValue)
                {
                    foreach (TouchLocation loc in state)
                    {
                        Vector2 pos = loc.Position;
                        Vector2.DistanceSquared(ref pos, ref Pos, out float distSqr);
                        if (distSqr < 100f)
                        {
                            foundNew = true;
                            touchLocation = loc;
                            Pos = loc.Position;
                            EvaluatePoint(dt);
                        }
                    }
                }
                if (!foundNew)
                {
                    touchLocation = null;
                    Direction = Vector2.Zero;
                    Magnitude = 0.0f;
                }
            }
        }

        /// <summary>
        /// Calculate the stick's direction and magnitude
        /// </summary>
        /// param name="dt" The gameTime in float
        public void EvaluatePoint(float dt)
        {
            Direction = Pos - StartLocation;
            float stickLength = Direction.Length();
            if (stickLength <= deadZoneSize)
            {
                Direction = Vector2.Zero;
                Magnitude = 0.0f;
            }
            else
            {
                Direction.Normalize();
                Direction.Y *= -1.0f;
                if (stickLength < aliveZoneSize)
                {
                    Magnitude = stickLength / aliveZoneSize;
                    Direction = new Vector2(Direction.X * Magnitude, Direction.Y * Magnitude);
                }
                else
                {
                    Magnitude = 1.0f;
                    if (Style == TouchStickStyle.FreeFollow && stickLength > aliveZoneSize * aliveZoneFollowFactor)
                    {
                        Vector2 targetLoc = new Vector2(
                                                Pos.X - Direction.X * aliveZoneSize * aliveZoneFollowFactor,
                                                Pos.Y + Direction.Y * aliveZoneSize * aliveZoneFollowFactor);

                        Vector2.Lerp(ref StartLocation, ref targetLoc,
                                     (stickLength - aliveZoneSize * aliveZoneFollowFactor) * aliveZoneFollowSpeed * dt,
                                     out StartLocation);

                        if (StartLocation.X < StartRegion.Left)
                            StartLocation.X = StartRegion.Left;
                        if (StartLocation.Y < StartRegion.Top)
                            StartLocation.Y = StartRegion.Top;
                        if (StartLocation.X > StartRegion.Right - edgeSpacing)
                            StartLocation.X = StartRegion.Right - edgeSpacing;
                        if (StartLocation.Y > StartRegion.Bottom - edgeSpacing)
                            StartLocation.Y = StartRegion.Bottom - edgeSpacing;
                    }
                }
            }
        }
    }
}