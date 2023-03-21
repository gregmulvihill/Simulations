using CommunityToolkit.Mvvm.Input;

using SimulationLib.AttachedProperties;
using SimulationLib.Enums;
using SimulationLib.ViewModels;

using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using SkiaSharp.Views.Maui;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SimulationLib.ViewModels
{
	public abstract class ViewModelBase : ObservableObject
	{
		private readonly Dictionary<string, object> _map = new();
		private Task _renderThread;
		private Task _updateThread;
		private bool _done;

		public abstract SimulationBase Simulation { get; }

		public ICommand EventHandlerCommand
		{
			get;
			set;
		}

		public ViewModelBase()
		{
			EventHandlerCommand = new RelayCommand<EventHandlerEventArgs>(OnEventHandler, CanEventHandedlerExecute);
		}

		private bool CanEventHandedlerExecute(object context)
		{
			return true;
		}

		private void OnEventHandler(EventHandlerEventArgs e)
		{
			object sender = e.CommandParameter;

			switch (sender)
			{
				case SKCanvasView cv:
					{
						OnEventHandler(cv, e);
						break;
					}
				default:
					{
						Debugger.Break();
						break;
					}
			}
		}

		private void OnEventHandler(SKCanvasView sender, EventHandlerEventArgs e)
		{
			switch (e.EventType)
			{
				case EventTypes.SizeChanged:
					{
						Simulation.SizeChanged(sender);
						break;
					}
				case EventTypes.Loaded:
					{
						_renderThread = Task.Run(() => RenderThread(sender));
						_updateThread = Task.Run(UpdateThread);
						break;
					}
				case EventTypes.Unloaded:
					{
						_done = true;
						_updateThread.Wait();
						_renderThread.Wait();
						break;
					}
				case EventTypes.PaintSurface:
					{
						if (e.EventArg is SKPaintSurfaceEventArgs psea)
						{
							Simulation.Draw(sender, psea);
						}
						break;
					}
				case EventTypes.Touch:
					{
						if (e.EventArg is SKTouchEventArgs te)
						{
							Simulation.Touch(sender, te);
						}
						break;
					}
				default:
					Debugger.Break();
					break;
			}
		}

		//

		private void UpdateThread()
		{
			//Thread.CurrentThread.Priority = ThreadPriority.Lowest;

			while (!_done)
			{
				Simulation?.Update(default);
				Thread.Sleep((int)Simulation.UpdateInterval);
			}
		}

		private void RenderThread(SKCanvasView viewer)
		{
			//Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

			while (!_done)
			{
				_ = viewer.Dispatcher.Dispatch(() => Invalidate(viewer));
				Thread.Sleep((int)Simulation.RenderInterval);
			}
		}

		private void Invalidate(SKCanvasView viewer)
		{
			if (!Simulation.InDraw && !_done && viewer.IsLoaded)
			{
				viewer.InvalidateSurface();
			}
		}

		//

		public bool SetValue<T>(T value, [CallerMemberName] string name = null)
		{
			_map[name] = value;
			OnPropertyChanged(name);

			return true;
		}

		public T GetValue<T>(T defaultValue = default, [CallerMemberName] string name = null)
		{
			if (!_map.TryGetValue(name, out object value))
			{
				_map[name] = value = defaultValue;
			}

			return (T)value;
		}
	}
}