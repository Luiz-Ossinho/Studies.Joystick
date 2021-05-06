using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Studies.Joystick.Abstract;

namespace Studies.Joystick.Input
{
    public class DualStick
    {
        // How quickly the touch stick follows in FreeFollow mode
        public readonly float aliveZoneFollowSpeed;
        // How far from the alive zone we can get before the touch stick starts to follow in FreeFollow mode
        public readonly float aliveZoneFollowFactor;
        // If we let the touch origin get too close to the screen edge,
        // the direction is less accurate, so push it away from the edge.
        public readonly float edgeSpacing;
        // Where touches register, if they first land beyond this point,
        // the touch wont be registered as occuring inside the stick
        public readonly float aliveZoneSize;
        // Current state of the TouchPanel
        private TouchCollection state;
        // Keeps information of last 4 taps
        private readonly TapStart[] tapStarts = new TapStart[4];
        private int tapStartCount = 0;
        // this keeps counting, no ideia why i cant reset it
        private double totalTime;
        public DualStick(SpriteFont font, float aliveZoneFollowFactor = 1.3f, float aliveZoneFollowSpeed = 0.05f, float edgeSpacing = 25f, float aliveZoneSize = 65f, float deadZoneSize = 5f)
        {
            this.aliveZoneFollowFactor = aliveZoneFollowFactor;
            this.aliveZoneFollowSpeed = aliveZoneFollowSpeed;
            this.edgeSpacing = edgeSpacing;
            this.aliveZoneSize = aliveZoneSize;
            this.font = font;

            LeftStick = new Stick(deadZoneSize,
                 new Rectangle(0, 100, (int)(TouchPanel.DisplayWidth * 0.3f), TouchPanel.DisplayHeight - 100))
            {
                FixedLocation = new Vector2(aliveZoneSize * aliveZoneFollowFactor, TouchPanel.DisplayHeight - aliveZoneSize * aliveZoneFollowFactor)
            };
            RightStick = new Stick(deadZoneSize,
                new Rectangle((int)(TouchPanel.DisplayWidth * 0.5f), 100, (int)(TouchPanel.DisplayWidth * 0.5f), TouchPanel.DisplayHeight - 100))
            {
                FixedLocation = new Vector2(TouchPanel.DisplayWidth - aliveZoneSize * aliveZoneFollowFactor, TouchPanel.DisplayHeight - aliveZoneSize * aliveZoneFollowFactor)
            };

            //TouchPanel.EnabledGestures = GestureType.None;
            //TouchPanel.DisplayOrientation = DisplayOrientation.LandscapeLeft;
        }
        private readonly SpriteFont font;
        public Stick RightStick { get; set; }
        public Stick LeftStick { get; set; }
        public void Update(float dt)
        {
            totalTime += dt;

            state = TouchPanel.GetState();
            TouchLocation? leftTouch = null, rightTouch = null;

            if (tapStartCount > state.Count)
                tapStartCount = state.Count;

            foreach (TouchLocation loc in state)
            {
                if (loc.State == TouchLocationState.Released)
                {
                    int tapStartId = -1;
                    for (int i = 0; i < tapStartCount; ++i)
                    {
                        if (tapStarts[i].Id == loc.Id)
                        {
                            tapStartId = i;
                            break;
                        }
                    }
                    if (tapStartId >= 0)
                    {
                        for (int i = tapStartId; i < tapStartCount - 1; ++i)
                            tapStarts[i] = tapStarts[i + 1];
                        tapStartCount--;
                    }
                    continue;
                }
                else if (loc.State == TouchLocationState.Pressed && tapStartCount < tapStarts.Length)
                {
                    tapStarts[tapStartCount] = new TapStart(loc.Id, totalTime, loc.Position);
                    tapStartCount++;
                }
                if (LeftStick.touchLocation.HasValue && loc.Id == LeftStick.touchLocation.Value.Id)
                {
                    leftTouch = loc;
                    continue;
                }
                if (RightStick.touchLocation.HasValue && loc.Id == RightStick.touchLocation.Value.Id)
                {
                    rightTouch = loc;
                    continue;
                }

                if (!loc.TryGetPreviousLocation(out TouchLocation locPrev))
                    locPrev = loc;

                if (!LeftStick.touchLocation.HasValue)
                {
                    if (LeftStick.StartRegion.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                    {
                        if (LeftStick.Style == TouchStickStyle.Fixed)
                        {
                            if (Vector2.Distance(locPrev.Position, LeftStick.StartLocation) < aliveZoneSize)
                            {
                                leftTouch = locPrev;
                            }
                        }
                        else
                        {
                            leftTouch = locPrev;
                            LeftStick.StartLocation = leftTouch.Value.Position;
                            if (LeftStick.StartLocation.X < LeftStick.StartRegion.Left + edgeSpacing)
                                LeftStick.StartLocation.X = LeftStick.StartRegion.Left + edgeSpacing;
                            if (LeftStick.StartLocation.Y > LeftStick.StartRegion.Bottom - edgeSpacing)
                                LeftStick.StartLocation.Y = LeftStick.StartRegion.Bottom - edgeSpacing;
                        }
                        continue;
                    }
                }

                if (!RightStick.touchLocation.HasValue && locPrev.Id != RightStick.lastExcludedRightTouchId)
                {
                    if (RightStick.StartRegion.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                    {
                        bool excluded = false;
                        foreach (Rectangle r in RightStick.startExcludeRegions)
                        {
                            if (r.Contains((int)locPrev.Position.X, (int)locPrev.Position.Y))
                            {
                                excluded = true;
                                RightStick.lastExcludedRightTouchId = locPrev.Id;
                                continue;
                            }
                        }
                        if (excluded)
                            continue;
                        RightStick.lastExcludedRightTouchId = -1;
                        if (RightStick.Style == TouchStickStyle.Fixed)
                        {
                            if (Vector2.Distance(locPrev.Position, RightStick.StartLocation) < aliveZoneSize)
                            {
                                rightTouch = locPrev;
                            }
                        }
                        else
                        {
                            rightTouch = locPrev;
                            RightStick.StartLocation = rightTouch.Value.Position;
                            if (RightStick.StartLocation.X > RightStick.StartRegion.Right - edgeSpacing)
                                RightStick.StartLocation.X = RightStick.StartRegion.Right - edgeSpacing;
                            if (RightStick.StartLocation.Y > RightStick.StartRegion.Bottom - edgeSpacing)
                                RightStick.StartLocation.Y = RightStick.StartRegion.Bottom - edgeSpacing;
                        }
                        continue;
                    }
                }
            }
            if (leftTouch.HasValue)
            {
                LeftStick.touchLocation = leftTouch;
                LeftStick.Pos = leftTouch.Value.Position;
                LeftStick.EvaluatePoint(dt, aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing);
            }
            else
            {
                bool foundNew = false;
                if (LeftStick.touchLocation.HasValue)
                {
                    foreach (TouchLocation loc in state)
                    {
                        Vector2 pos = loc.Position;
                        Vector2.DistanceSquared(ref pos, ref LeftStick.Pos, out float distSqr);
                        if (distSqr < 100f)
                        {
                            foundNew = true;
                            LeftStick.touchLocation = loc;
                            LeftStick.Pos = loc.Position;
                            LeftStick.EvaluatePoint(dt, aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing);
                        }
                    }
                }
                if (!foundNew)
                {
                    LeftStick.touchLocation = null;
                    LeftStick.Direction = Vector2.Zero;
                    LeftStick.Magnitude = 0.0f;
                }
            }

            if (rightTouch.HasValue)
            {
                RightStick.touchLocation = rightTouch;
                RightStick.Pos = rightTouch.Value.Position;
                RightStick.EvaluatePoint(dt, aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing);
                //EvaluateRightPoint(RightStick.Pos, dt);
            }
            else
            {
                bool foundNew = false;
                if (RightStick.touchLocation.HasValue)
                {
                    foreach (TouchLocation loc in state)
                    {
                        Vector2 pos = loc.Position;
                        Vector2.DistanceSquared(ref pos, ref RightStick.Pos, out float distSqr);
                        if (distSqr < 100f)
                        {
                            foundNew = true;
                            RightStick.touchLocation = loc;
                            RightStick.Pos = loc.Position;
                            RightStick.EvaluatePoint(dt, aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing);
                            //EvaluateRightPoint(RightStick.Pos, dt);
                        }
                    }
                }
                if (!foundNew)
                {
                    RightStick.touchLocation = null;
                    RightStick.Direction = Vector2.Zero;
                    RightStick.Magnitude = 0.0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawStringCentered($"L", LeftStick.StartLocation, Color.Black, spriteBatch);
            DrawStringCentered($"L@L", LeftStick.GetPositionVector(aliveZoneSize), Color.Black, spriteBatch);


            DrawStringCentered($"R", RightStick.StartLocation, Color.Black, spriteBatch);
            DrawStringCentered($"R@R", RightStick.GetPositionVector(aliveZoneSize), Color.Black, spriteBatch);
        }
        private void DrawStringCentered(string text, Vector2 position, Color color, SpriteBatch spriteBatch)
        {
            var size = font.MeasureString(text);
            var origin = size * 0.5f;

            spriteBatch.DrawString(font, text, position, color, 0, origin, 1, SpriteEffects.None, 0);
        }
    }
}