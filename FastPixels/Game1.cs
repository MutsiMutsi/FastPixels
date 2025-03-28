using FastPixels.Util;
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

		public const int RESOLUTION_X = 3440;
		public const int RESOLUTION_Y = 1440;

		private Color[] rngCol = new Color[256];
		private Effect pixelShader;

		PerformanceCounter pc = new PerformanceCounter(1024);

		System.Random _rand = new System.Random(); // Reuse this for performance
		private int _threadCount;
		private int _rowsPerThread;


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

			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			_graphics.ApplyChanges();

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

			//Spam random colours for every single pixel every single frame to stresstest.
			for (uint y = 0; y < RESOLUTION_Y; y++)
			{
				for (uint x = 0; x < RESOLUTION_X; x++)
				{
					FastPixels.SetPixel(x, y, (byte)Random.Shared.Next(0, 8));
				}
			}

			// Determine optimal thread count
			_threadCount = Environment.ProcessorCount;
			_rowsPerThread = Math.Max(1, RESOLUTION_Y / _threadCount);

		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();


			pc.Update(gameTime, this.Window, 0, 0);

			var ms = Mouse.GetState();


			try
			{

				if (ms.RightButton == ButtonState.Pressed)
				{
					for (int y = -20; y < 20; y++)
					{
						for (int x = -20; x < 20; x++)
						{
							FastPixels.SetPixel((uint)(ms.X + x), (uint)(ms.Y + y), 56 - 17);
						}
					}
				}

				if (ms.LeftButton == ButtonState.Pressed)
				{
					for (int y = -20; y < 20; y++)
					{
						for (int x = -20; x < 20; x++)
						{
							FastPixels.SetPixel((uint)(ms.X + x), (uint)(ms.Y + y), 18);
						}
					}
				}
			}
			catch
			{
			}


			byte black = 56 - 17;
			// Process bottom-up to prevent multiple moves in a single frame
			for (int y = RESOLUTION_Y - 2; y >= 1; y--)
			{
				// Alternate row processing direction to reduce bias
				bool processLeftToRight = (y % 2 == 0);
				int startX = processLeftToRight ? 0 : RESOLUTION_X - 1;
				int endX = processLeftToRight ? RESOLUTION_X : -1;
				int increment = processLeftToRight ? 1 : -1;

				for (int x = startX; x != endX; x += increment)
				{
					byte current = FastPixels.GetPixel(x, y);
					if (current == black) continue;

					// Directly below
					if (FastPixels.GetPixel(x, y + 1) == black)
					{
						FastPixels.SetPixel(x, y + 1, current);
						FastPixels.SetPixel(x, y, black);
						continue;
					}

					int newX = x + -1;
					if (newX >= 0 && newX < RESOLUTION_X &&
						FastPixels.GetPixel(newX, y + 1) == black)
					{
						FastPixels.SetPixel(newX, y + 1, current);
						FastPixels.SetPixel(x, y, black);
					}
					newX = x + 1;
					if (newX >= 0 && newX < RESOLUTION_X &&
						FastPixels.GetPixel(newX, y + 1) == black)
					{
						FastPixels.SetPixel(newX, y + 1, current);
						FastPixels.SetPixel(x, y, black);
					}
				}
			}

			FastPixels.UpdatePixelBuffer();

			base.Update(gameTime);
		}

		// Fisher-Yates shuffle to randomize directions more fairly
		private void Shuffle(int[] array)
		{
			int n = array.Length;
			while (n > 1)
			{
				n--;
				int k = _rand.Next(n + 1);
				(array[k], array[n]) = (array[n], array[k]);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			FastPixels.Draw(pixelShader);

			base.Draw(gameTime);
		}
	}
}
