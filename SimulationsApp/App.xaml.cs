﻿namespace PatternsTinkeringApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();
		}

		protected override Window CreateWindow(IActivationState activationState)
		{
			var window = base.CreateWindow(activationState);
			window.Width = 1600;
			window.Height = 1000;

			return window;
		}
	}
}