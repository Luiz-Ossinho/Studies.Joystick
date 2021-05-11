using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using Studies.Joystick.Abstract;
using Studies.Joystick.GameObjects;
using Studies.Joystick.Input;

namespace Studies.Joystick
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private Ball Ball;
        private DualStick DualStick;
        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;
        private OrthographicCamera _camera;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 600);

            _camera = new OrthographicCamera(viewportadapter);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("samplemap");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

            Ball = new Ball(Content);
            font = Content.Load<SpriteFont>("font");
            DualStick = new DualStick(font);

            DualStick.LeftStick.SetAsFree();
            DualStick.RightStick.SetAsFree();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _tiledMapRenderer.Update(gameTime);

            DualStick.Update(gameTime);
            var relativePostion = new
            {
                Left = DualStick.LeftStick.GetRelativeVector(DualStick.aliveZoneSize),
                Right = DualStick.RightStick.GetRelativeVector(DualStick.aliveZoneSize)
            };
            Ball.Move(relativePostion.Left, gameTime);
            _camera.LookAt(Ball.Position + relativePostion.Right);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _tiledMapRenderer.Draw(_camera.GetViewMatrix());

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            Ball.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin();
            DualStick.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
