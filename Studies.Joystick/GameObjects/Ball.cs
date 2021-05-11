using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Studies.Joystick.GameObjects
{
    public class Ball
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public Ball(ContentManager CM)
        {
            Texture = CM.Load<Texture2D>("ball");
            Position = new Vector2(TouchPanel.DisplayWidth / 2, TouchPanel.DisplayHeight / 2);
            Speed = 0.006f;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, color: Color.White, rotation: 0f, origin: new Vector2(Texture.Width / 2, Texture.Height / 2), scale: Vector2.One, effects: SpriteEffects.None, layerDepth: 0f, sourceRectangle: null);
        }

        public void Move(Vector2 direction, GameTime gameTime)
        {
            Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds * Speed;
        }
    }
}