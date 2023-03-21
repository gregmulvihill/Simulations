//using SkiaSharp.Views.Maui;

namespace SimulationLib.Enums
{
	// TODO: use .tt to generate at build time
	[Flags]
	public enum EventTypes
	{
		None = 0,
		Closing = 1,
		SizeChanged = 2,
		Loaded = 4,
		Unloaded = 8,
		MouseUp = 16,
		PaintSurface = 32,
		Touch = 64,
	}
}
