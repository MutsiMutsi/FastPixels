using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FastPixels
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public const int RESOLUTION_X = 1920;
		public const int RESOLUTION_Y = 1080;

		private Color[] rngCol = new Color[256];
		private Effect pixelShader;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			IsFixedTimeStep = false;
			InactiveSleepTime = TimeSpan.FromMilliseconds(0);

			_graphics.PreferredBackBufferWidth = RESOLUTION_X;
			_graphics.PreferredBackBufferHeight = RESOLUTION_Y;
			_graphics.SynchronizeWithVerticalRetrace = false;
			_graphics.ApplyChanges();

			//Just to test the implementation, we get some random colours cached.
			for (int i = 0; i < 256; i++)
			{
				Color GetRandomColor()
				{
					// Generate random values for red, green, and blue
					int r = Random.Shared.Next(256); // Random value between 0 and 255
					int g = Random.Shared.Next(256); // Random value between 0 and 255
					int b = Random.Shared.Next(256); // Random value between 0 and 255

					// Create a color from the random values
					return new Color(r, g, b);
				}
				rngCol[i] = GetRandomColor();
			}

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			FastPixels.Setup(GraphicsDevice, RESOLUTION_X, RESOLUTION_Y);
			pixelShader = Content.Load<Effect>("PixelShader");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			//Spam random colours for every single pixel every single frame to stresstest.
			for (int y = 0; y < RESOLUTION_Y; y++)
			{
				for (int x = 0; x < RESOLUTION_X; x++)
				{
					FastPixels.SetPixel(x, y, rngCol[Random.Shared.Next(0, 256)]);
				}
			}
			FastPixels.UpdatePixelBuffer();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			FastPixels.Draw(pixelShader);

			base.Draw(gameTime);
		}
	}
}
