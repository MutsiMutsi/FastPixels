using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPixels.Util
{
	internal class PerformanceCounter
	{
		private int _fpsCounter;
		private TimeSpan _counterElapsed;
		private readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);
		private readonly long[] _updateTicks;
		private readonly long[] _renderTicks;
		private int _updateTickIndex;
		private int _renderTickIndex;
		private long _lastHeapSize;
		private long _currentHeapSize;
		private readonly StringBuilder _titleBuilder = new(200);

		public PerformanceCounter(int sampleSize)
		{
			_updateTicks = new long[sampleSize];
			_renderTicks = new long[sampleSize];
		}

		public void Update(GameTime gameTime, GameWindow window, long updateTick, long renderTick)
		{
			_fpsCounter++;
			_counterElapsed += gameTime.ElapsedGameTime;

			_updateTicks[_updateTickIndex] = updateTick;
			_renderTicks[_renderTickIndex] = renderTick;

			_updateTickIndex = (_updateTickIndex + 1) % _updateTicks.Length;
			_renderTickIndex = (_renderTickIndex + 1) % _renderTicks.Length;

			if (_counterElapsed >= _oneSecond)
			{
				UpdateTitle(window);
				_fpsCounter = 0;
				_counterElapsed -= _oneSecond;
			}
		}

		private void UpdateTitle(GameWindow window)
		{
			long totalUpdate = 0, totalRender = 0;
			for (int i = 0; i < _updateTicks.Length; i++)
			{
				totalUpdate += _updateTicks[i];
				totalRender += _renderTicks[i];
			}

			double avgUpdate = totalUpdate / (double)(_updateTicks.Length * TimeSpan.TicksPerMillisecond);
			double avgRender = totalRender / (double)(_renderTicks.Length * TimeSpan.TicksPerMillisecond);

			_lastHeapSize = _currentHeapSize;
			_currentHeapSize = GC.GetTotalMemory(false);

			_ = _titleBuilder.Clear();
			_ = _titleBuilder.Append(_fpsCounter).Append(" SYNC FPS - UPDATE ")
						 .Append(avgUpdate.ToString("0.00")).Append("ms - RENDER ")
						 .Append(avgRender.ToString("0.00")).Append("ms - ")
						 .Append((_currentHeapSize / 1048576f).ToString("F")).Append(" MB (+")
						 .Append(((_currentHeapSize - _lastHeapSize) / 1048576f).ToString("0.0000")).Append(" MB)");

			window.Title = _titleBuilder.ToString();
		}
	}
}
