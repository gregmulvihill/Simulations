using SimulationLib.Enums;

using SkiaSharp.Views.Maui;

using System.Diagnostics;
using System.Windows.Input;

namespace SimulationLib.AttachedProperties
{
	public class EventHandlerAttachedProperty : BindableObject
	{
		//https://stackoverflow.com/questions/22538814/attached-dependencyproperty-with-enums-act-weird
		//https://www.generacodice.com/en/articolo/1048597/WPFMVVM---how-to-handle-double-click-on-TreeViewItems-in-the-ViewModel

		// TODO: EventTypes -> .tt code defined <T>?
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "BindableProperty")]
		public static BindableProperty EventsProperty = BindableProperty.CreateAttached("Events", typeof(EventTypes), typeof(EventHandlerAttachedProperty), default);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "BindableProperty")]
		public static BindableProperty CommandProperty = BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(EventHandlerAttachedProperty), default, propertyChanged: CommandChanged);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "BindableProperty")]
		public static BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(EventHandlerAttachedProperty), default);

		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "BindableProperty")]
		//public static BindableProperty CancelClosingProperty = BindableProperty.CreateAttached("CancelClosing", typeof(bool), typeof(EventHandlerAttachedProperty));

		public static void SetEvents(BindableObject target, EventTypes value)
		{
			target.SetValue(EventsProperty, value);
		}

		public static EventTypes GetEvents(BindableObject target)
		{
			return (EventTypes)target.GetValue(EventsProperty);
		}

		//public static void SetCancelClosing(BindableObject target, bool value)
		//{
		//	target.SetValue(CancelClosingProperty, value);
		//}

		//public static bool GetCancelClosing(BindableObject target)
		//{
		//	return (bool)target.GetValue(CancelClosingProperty);
		//}

		public static void SetCommand(BindableObject target, ICommand value)
		{
			target.SetValue(CommandProperty, value);
		}

		public static void SetCommandParameter(BindableObject target, object value)
		{
			target.SetValue(CommandParameterProperty, value);
		}

		public static object GetCommandParameter(BindableObject target)
		{
			return target.GetValue(CommandParameterProperty);
		}

		private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
		{
			EventTypes events = GetEvents(bindable);

			if (bindable is SkiaSharp.Views.Maui.Controls.SKCanvasView cv)
			{
				if (events.HasFlag(EventTypes.PaintSurface))
				{
					cv.PaintSurface += PaintSurface;
				}

				if (events.HasFlag(EventTypes.Touch))
				{
					cv.Touch += OnTouch;
					cv.EnableTouchEvents = true;
				}
			}

			if (bindable is ContentPage w)
			{
			}

			if (bindable is VisualElement ve)
			{
				if (events.HasFlag(EventTypes.SizeChanged))
				{
					ve.SizeChanged += SizeChanged;
				}

				if (events.HasFlag(EventTypes.Loaded))
				{
					ve.Loaded += Loaded;
				}

				if (events.HasFlag(EventTypes.Unloaded))
				{
					ve.Unloaded += Unloaded;
				}
			}

			if (bindable is NavigableElement ne)
			{
			}
		}

		private static void OnTouch(object sender, SKTouchEventArgs e)
		{
			On(sender, e, EventTypes.Touch);
		}

		private static void Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender is not Window w)
			{
				throw new ArgumentNullException(nameof(sender));
			}

			//e.Cancel = (bool)w.GetValue(CancelClosingProperty);
			//var cancelAction = new Action<bool>(cancel => e.Cancel = cancel);
			ICommand command = (ICommand)w.GetValue(CommandProperty);
			object commandParameter = w.GetValue(CommandParameterProperty);
			command.Execute(new EventHandlerEventArgs(EventTypes.Closing, commandParameter, e));
		}

		//private static void MouseUp(object sender, EventArgs e) { On(sender, e, EventTypes.MouseUp); }

		private static void Unloaded(object sender, EventArgs e)
		{
			On(sender, e, EventTypes.Unloaded);
		}

		private static void Loaded(object sender, EventArgs e)
		{
			On(sender, e, EventTypes.Loaded);
		}

		private static void SizeChanged(object sender, EventArgs e)
		{
			On(sender, e, EventTypes.SizeChanged);
		}

		private static void PaintSurface(object sender, EventArgs e)
		{
			On(sender, e, EventTypes.PaintSurface);
		}

		//

		//[DebuggerStepThrough]
		private static void On(object sender, EventArgs eventArgs, EventTypes eventType)
		{
			if (sender is not VisualElement ve)
			{
				throw new ArgumentNullException(nameof(sender));
			}

			ICommand command = (ICommand)ve.GetValue(CommandProperty);
			object commandParameter = ve.GetValue(CommandParameterProperty);
			command.Execute(new EventHandlerEventArgs(eventType, commandParameter, eventArgs));
		}

		public EventHandlerAttachedProperty()
		{
			//is this ever executed?
			Debugger.Break();
		}
	}
}
