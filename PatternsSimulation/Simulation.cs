using SimulationLib;
using SimulationLib.Tools;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using PatternsSimulation.Models;

namespace PatternsSimulation
{
	public class Simulation : SimulationBase
	{
		//private readonly object _lock = new();
		public readonly List<Individual> Leaders = new();
		private bool _populationChanged;
		private int _leaderCount;
		private int _followerCount;
		//private bool _sizeChanged;
		//private SKRectI _deviceClipBounds;
		//private SKRect _localClipBounds;
		//private bool _press;
		//private SKTouchEventArgs _touchEventArgs;
		//private List<object> _list = new List<object>();
		//private readonly SKPaint _fadeToBlack;
		private readonly bool _useParallel;

		//

		//public double LeaderRadius { get; set; }
		//public double FollowerRadius { get; set; }

		//public bool Pause { get; set; }

		//public double Width { get; private set; }
		//public double Height { get; private set; }

		//public double UpdateFps { get => 1000.0 / UpdateInterval; set => UpdateInterval = 1000.0 / value; }
		//public double RenderFps { get => 1000.0 / RenderInterval; set => RenderInterval = 1000.0 / value; }

		//public double UpdateInterval { get; private set; }
		//public double RenderInterval { get; private set; }

		//public bool InDraw { get; set; }

		//

		public Simulation(bool useParallel = true /*int leaderCount, int followerCount*/)
			: base()
		{
			//_leaderCount = leaderCount;
			//_followerCount = followerCount;

			_populationChanged = true;
			CreatePopulation();

			//LeaderRadius = 5;
			//FollowerRadius = 2;

			//UpdateFps = 15.0;
			//RenderFps = 24.0;

			//			_fadeToBlack = new SKPaint();
			//SetFadeToBlackAlpha(20);

			_useParallel = useParallel;
		}

		//public void SetFadeToBlackAlpha(int alpha)
		//{
		//	_fadeToBlack.Color = new SKColor(0x00, 0x00, 0x00, (byte)alpha);
		//	_fadeToBlack.BlendMode = SKBlendMode.Darken;
		//}

		public void CreatePopulation()
		{
			//lock (_lock)
			{
				if (!_populationChanged)
				{
					return;
				}

				_populationChanged = false;

				while (Leaders.Count > _leaderCount)
				{
					Leaders.RemoveAt(0);
				}

				while (Leaders.Count < _leaderCount)
				{
					Leaders.Add(new Individual(this, null));
				}

				for (int i = 0; i < _leaderCount; i++)
				{
					Individual leader = Leaders[i];
					List<Individual> followers = leader.Followers;

					while (followers.Count > _followerCount)
					{
						followers.RemoveAt(0);
					}

					while (followers.Count < _followerCount)
					{
						followers.Add(new Individual(this, leader));
					}
				}

				double colorRange = 360.0 / _leaderCount;

				for (int i = 0; i < _leaderCount; i++)
				{
					Individual leader = Leaders[i];
					List<Individual> followers = leader.Followers;

					double groupHue = 360.0 * i / _leaderCount;
					leader.SetHue(groupHue);

					for (int j = 0; j < _followerCount; j++)
					{
						Individual follower = followers[j];

						double individualHue = groupHue + (double)(MathEx.RND.NextDouble() * colorRange - colorRange / 2.0);
						follower.SetHue(individualHue);
					}
				}
			}
		}

		protected override void OnUpdate()
		{
			//if (Pause)
			//{
			//	return;
			//}

			//if (Width == 0 || Height == 0)
			//{
			//	return;
			//}

			//lock (_lock)
			{
				Individual[] leaders = Leaders.ToArray();
				int countLeaders = leaders.Length;

				if (_useParallel)
				{
					_ = Parallel.For(0, countLeaders, i => { Update(leaders[i]); });
				}
				else
				{
					for (int i = 0; i < countLeaders; i++) { Update(leaders[i]); }
				}
			}
		}

		private static void Update(Individual leader)
		{
			leader.Update();

			List<Individual> individuals = leader.Followers;
			int countFollowers = individuals.Count;

			for (int j = 0; j < countFollowers; j++)
			{
				individuals[j].Update();
			}
		}

		public void SetLeaderCount(int leaderCount)
		{
			_leaderCount = leaderCount;
			_populationChanged = true;
			CreatePopulation();
		}

		public void SetFollowerCount(int followerCount)
		{
			_followerCount = followerCount;
			_populationChanged = true;
			CreatePopulation();
		}

		//public void SizeChanged(SKCanvasView sender)
		//{
		//	//_sizeChanged = true;

		//	//_canvasView = sender;
		//	//var w = sender.Window.Width;
		//	//var r = sender.Window.Height;
		//}


		protected override void OnDraw(SKCanvasView sender, SKPaintSurfaceEventArgs psea)
		{
			//InDraw = true;

			//Width = sender.Window.Width;
			//Height = sender.Window.Height;

			//if (_sizeChanged)
			//{
			//	_ = sender.Window.Width;
			//	_ = sender.Window.Height;

			//	_ = psea.Surface.Canvas.GetDeviceClipBounds(out _deviceClipBounds);
			//	_ = psea.Surface.Canvas.GetLocalClipBounds(out _localClipBounds);

			//	Width = _deviceClipBounds.Width;
			//	Height = _deviceClipBounds.Height;

			//	_sizeChanged = false;
			//}

			//

			bool press = false;
			double X = 0.0;
			double Y = 0.0;
			double pressure = 1.0;

			if (_touchEventArgs != null)
			{
				X = _touchEventArgs.Location.X;
				Y = _touchEventArgs.Location.Y;
				pressure = _touchEventArgs.Pressure;
				press = true;
			}

			SKCanvas canvas = psea.Surface.Canvas;
			_ = (float)Width;
			_ = (float)Height;

			//canvas.DrawRect(0, 0, w, h, _fadeToBlack);

			//lock (_lock)
			{
				Individual[] copy = Leaders.ToArray();
				int count = copy.Length;

				for (int i = 0; i < count; i++)
				{
					Individual leader = Leaders[i];
					double size = leader.Size;

					if (press)
					{
						bool insideCircle = InsideCircle(leader.X1, leader.Y1, X, Y, 500.0 * pressure);
						size = insideCircle ? leader.Size * 3 : leader.Size;
					}

					canvas.DrawCircle((float)leader.X1, (float)leader.Y1, (float)size, leader.Color);
					//canvas.DrawCircle((float)leader.X2, (float)leader.Y2, (float)LeaderRadius, leader.Color);

					List<Individual> followers = leader.Followers;

					for (int j = 0; j < _followerCount; j++)
					{
						Individual follower = followers[j];

						if (press)
						{
							bool insideCircle = InsideCircle(follower.X1, follower.Y1, X, Y, 500.0 * pressure);
							size = insideCircle ? follower.Size * 3 : follower.Size;
						}

						canvas.DrawCircle((float)follower.X1, (float)follower.Y1, (float)size, follower.Color);
						//canvas.DrawCircle((float)follower.X2, (float)follower.Y2, (float)FollowerRadius, follower.Color);
					}
				}
			}

			//InDraw = false;
		}

		//public void Touch(SKCanvasView sender, SKTouchEventArgs te)
		//{
		//	switch (te.ActionType)
		//	{
		//		case SKTouchAction.Entered:
		//			{
		//				break;
		//			}
		//		case SKTouchAction.Pressed:
		//			{
		//				//var json = System.Text.Json.JsonSerializer.Serialize(te);
		//				//_list.Add(json);
		//				_press = true;
		//				_touchEventArgs = te;
		//				te.Handled = true;
		//				break;
		//			}
		//		case SKTouchAction.Moved:
		//			{
		//				if (_press)
		//				{
		//					//var json = System.Text.Json.JsonSerializer.Serialize(te);
		//					//_list.Add(json);
		//					_touchEventArgs = te;
		//					te.Handled = true;
		//				}
		//				break;
		//			}
		//		case SKTouchAction.Released:
		//			{
		//				//var json = System.Text.Json.JsonSerializer.Serialize(te);
		//				//_list.Add(json);
		//				//_list.Clear();
		//				_touchEventArgs = null;
		//				_press = false;
		//				te.Handled = true;
		//				break;
		//			}
		//		case SKTouchAction.Exited:
		//			{
		//				//_press = false;
		//				break;
		//			}
		//		case SKTouchAction.Cancelled:
		//			{
		//				//_press = false;
		//				break;
		//			}
		//		case SKTouchAction.WheelChanged:
		//			{
		//				break;
		//			}
		//	}
		//}

		private bool InsideCircle(double cx, double cy, double px, double py, double r)
		{
			double a = cx - px;
			double b = cy - py;
			bool outside = a * a + b * b > r * r;

			return !outside;
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