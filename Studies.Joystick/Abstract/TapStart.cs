using Microsoft.Xna.Framework;

namespace Studies.Joystick.Abstract
{
    public struct TapStart
    {
        public int Id;
        public double Time;
        public Vector2 Pos;
        public TapStart(int id, double time, Vector2 pos)
        {
            Id = id;
            Time = time;
            Pos = pos;
        }
    }
}