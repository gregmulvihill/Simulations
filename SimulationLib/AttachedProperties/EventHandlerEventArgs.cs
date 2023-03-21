using SimulationLib.Enums;

using System.Diagnostics;

namespace SimulationLib.AttachedProperties
{
	public class EventHandlerEventArgs
	{
		public EventTypes EventType;
		public object CommandParameter;
		public EventArgs EventArg;

		//[DebuggerHidden()]
		[DebuggerStepThrough()]
		public EventHandlerEventArgs(EventTypes eventType, object commandParameter, EventArgs eventArg)
		{
			EventType = eventType;
			CommandParameter = commandParameter;
			EventArg = eventArg;
		}
	}
}