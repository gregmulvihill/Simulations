namespace SimulationLib
{
	public static class AppHostBuilderExtensions
	{
		public static MauiAppBuilder UsePixelRendering(this MauiAppBuilder builder)
		{
			//builder.UseSkiaSharp();

			return builder.ConfigureMauiHandlers(delegate (IMauiHandlersCollection handlers)
			{
				//handlers.AddHandler<PixelRenderingView, PixelRenderingViewHandler>();
			}).ConfigureImageSources(delegate (IImageSourceServiceCollection sources)
			{
				//sources.AddService<IXxxxxxImageSource, XxxxxxSourceService>();
			});
		}
	}
}