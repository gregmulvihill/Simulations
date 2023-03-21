//using SimulationLib.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SimulationLib
{
	public abstract class SimulationBase
	{
		private readonly object _lock = new();

		private bool _sizeChanged;
		private SKRectI _deviceClipBounds;
		private SKRect _localClipBounds;

		protected bool _press;
		protected SKTouchEventArgs _touchEventArgs;

		private readonly SKPaint _fadeToBlack;

		//

		public bool Pause { get; set; }

		public double Width { get; private set; }
		public double Height { get; private set; }

		public double UpdateFps { get => 1000.0 / UpdateInterval; set => UpdateInterval = 1000.0 / value; }
		public double RenderFps { get => 1000.0 / RenderInterval; set => RenderInterval = 1000.0 / value; }

		public double UpdateInterval { get; private set; }
		public double RenderInterval { get; private set; }

		public bool InDraw { get; set; }

		//

		public SimulationBase()
		{
			_fadeToBlack = new SKPaint();
		}

		public void SetFadeToBlackAlpha(int alpha)
		{
			_fadeToBlack.Color = new SKColor(0x00, 0x00, 0x00, (byte)alpha);
			_fadeToBlack.BlendMode = SKBlendMode.Darken;
		}

		public void Update(object context)
		{
			if (Pause)
			{
				return;
			}

			if (Width == 0 || Height == 0)
			{
				return;
			}

			OnUpdate();
		}

		protected abstract void OnUpdate();

		public void SizeChanged(SKCanvasView sender)
		{
			_sizeChanged = true;
			Width = sender.Window.Width;
			Height = sender.Window.Height;
		}

		public void Draw(SKCanvasView sender, SKPaintSurfaceEventArgs psea)
		{
			InDraw = true;

			//Width = sender.Window.Width;
			//Height = sender.Window.Height;

			if (_sizeChanged)
			{
				_ = sender.Window.Width;
				_ = sender.Window.Height;

				_ = psea.Surface.Canvas.GetDeviceClipBounds(out _deviceClipBounds);
				_ = psea.Surface.Canvas.GetLocalClipBounds(out _localClipBounds);

				Width = _deviceClipBounds.Width;
				Height = _deviceClipBounds.Height;

				_sizeChanged = false;
			}

			if (_touchEventArgs != null)
			{
				_ = _touchEventArgs.Location.X;
				_ = _touchEventArgs.Location.Y;
				_ = _touchEventArgs.Pressure;
			}

			SKCanvas canvas = psea.Surface.Canvas;
			float w = (float)Width;
			float h = (float)Height;

			canvas.DrawRect(0, 0, w, h, _fadeToBlack);

			lock (_lock)
			{
				OnDraw(sender, psea);
			}

			InDraw = false;
		}

		protected abstract void OnDraw(SKCanvasView sender, SKPaintSurfaceEventArgs psea);

		public void Touch(SKCanvasView sender, SKTouchEventArgs te)
		{
			switch (te.ActionType)
			{
				case SKTouchAction.Entered:
					{
						break;
					}
				case SKTouchAction.Pressed:
					{
						//var json = System.Text.Json.JsonSerializer.Serialize(te);
						//_list.Add(json);
						_press = true;
						_touchEventArgs = te;
						te.Handled = true;
						break;
					}
				case SKTouchAction.Moved:
					{
						if (_press)
						{
							//var json = System.Text.Json.JsonSerializer.Serialize(te);
							//_list.Add(json);
							_touchEventArgs = te;
							te.Handled = true;
						}
						break;
					}
				case SKTouchAction.Released:
					{
						//var json = System.Text.Json.JsonSerializer.Serialize(te);
						//_list.Add(json);
						//_list.Clear();
						_touchEventArgs = null;
						_press = false;
						te.Handled = true;
						break;
					}
				case SKTouchAction.Exited:
					{
						//_press = false;
						break;
					}
				case SKTouchAction.Cancelled:
					{
						//_press = false;
						break;
					}
				case SKTouchAction.WheelChanged:
					{
						break;
					}
			}
		}


		//private void NewMethod1(double width, double height)
		//{
		//	var scaleX = width / _prevBounds;
		//	var scaleY = height / _prevHeight;

		//	_prevBounds = width;
		//	_prevHeight = height;

		//	foreach (var follower in _followers)
		//	{
		//		follower.X1 = scaleX * follower.X1;
		//		follower.Y1 = scaleY * follower.Y1;
		//		follower.X1 = scaleX * follower.X1;
		//		follower.Y2 = scaleY * follower.Y2;
		//	}
		//}

		//private void Update(Size size)
		//{
		//	double nRadius = Math.Minimum(size.Width, size.Height) / 2.0 * 0.9;
		//	//nRadius = 0;
		//	double nAngle = MathEx.RndNextD(0.0, 360.0);
		//	TrigonometryValues oUnitCircle = MathEx.UnitCircle(nAngle);

		//	var X1 = (oUnitCircle.Cos * nRadius) + (size.Width / 2.0);
		//	var Y1 = (oUnitCircle.Sin * nRadius) + (size.Height / 2.0);
		//}
	}
}