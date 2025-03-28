using Microsoft.Xna.Framework.Graphics;

namespace FastPixels
{
	public struct PixelDeclaration
	{
		public uint val;
		public PixelDeclaration(uint value)
		{
			val = value;
		}

		// Recommended Vertex Declaration
		public static readonly VertexDeclaration VertexDeclaration = new(
			// Key Change: Specify the correct size and type
			new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
		);
	}

	public static class FastPixels
	{
		private static VertexBuffer _vertexBuffer;
		private static PixelDeclaration[] _pixels;

		private static GraphicsDevice _gd;
		private static int _resX, _resY;

		public static void Setup(GraphicsDevice gd, int resX, int resY)
		{
			_gd = gd;
			_resX = resX; _resY = resY;

			int pixelCount = resX * resY;
			_pixels = new PixelDeclaration[pixelCount];
			_vertexBuffer = new VertexBuffer(gd, PixelDeclaration.VertexDeclaration, pixelCount, BufferUsage.WriteOnly);
			gd.SetVertexBuffer(_vertexBuffer);
		}
		public static uint Pack(uint x, uint y, uint c)
		{
			x &= 0xFFF;
			y &= 0xFFF;
			c &= 0xFF;

			// Pack into a single 32-bit integer
			uint packed = (x << 20) | (y << 8) | c;
			return packed;
		}

		public static void SetPixel(uint x, uint y, byte colorIdx)
		{
			// Calculate the index from x and y
			int index = (int)((y * _resX) + x);

			_pixels[index].val = Pack(x, y, colorIdx);
		}
		public static void SetPixel(int x, int y, byte colorIdx)
		{
			// Calculate the index from x and y
			int index = (y * _resX) + x;

			_pixels[index].val = Pack((uint)x, (uint)y, colorIdx);
		}

		public static byte GetPixel(uint x, uint y)
		{
			int index = (int)((y * _resX) + x);
			return (byte)(_pixels[index].val & 0xFF);
		}

		public static byte GetPixel(int x, int y)
		{
			int index = (y * _resX) + x;
			return (byte)(_pixels[index].val & 0xFF);
		}

		public static void UpdatePixelBuffer()
		{
			_vertexBuffer.SetData(_pixels);
		}

		public static void Draw(Effect shader)
		{
			shader.CurrentTechnique.Passes[0].Apply();
			_gd.DrawPrimitives(PrimitiveType.PointList, 0, _pixels.Length);
		}
	}
}
