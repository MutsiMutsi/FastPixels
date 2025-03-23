using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastPixels
{
	public struct PixelDeclaration
	{
		public short X; // 16-bit integer for X
		public short Y; // 16-bit integer for Y
		public Color Color; // Color of the pixel

		public PixelDeclaration(short x, short y, Color color)
		{
			X = x;
			Y = y;
			Color = color;
		}

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Short2, VertexElementUsage.Position, 0),
			new VertexElement(4, VertexElementFormat.Color, VertexElementUsage.Color, 0)
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

		public static void SetPixel(int x, int y, Color color)
		{
			// Calculate the index from x and y
			int index = y * _resX + x;

			// Set the pixel data
			_pixels[index].X = (short)x;
			_pixels[index].Y = (short)y;
			_pixels[index].Color = color;
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
