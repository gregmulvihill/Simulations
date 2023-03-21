using CommunityToolkit.Mvvm.Input;

using SimulationLib;
using SimulationLib.AttachedProperties;
using SimulationLib.Enums;
using SimulationLib.ViewModels;

using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PatternsSimulation.ViewModels
{
	public partial class SimulationViewModel : ViewModelBase
	{
		private Simulation _simulation;
		public override SimulationBase Simulation => _simulation;

		public double LeaderCount
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double FollowerCount
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double UpdateFps
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double RenderFps
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double FadeAlpha
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double LeaderRadius
		{
			get => GetValue<double>();
			set => SetValue(value);
		}
		public double FollowerRadius
		{
			get => GetValue<double>();
			set => SetValue(value);
		}

		public bool IsConfigurationVisible
		{
			get => GetValue<bool>();
			set => SetValue(value);
		}

		public ICommand ToggleSettingsCommand
		{
			get;
			set;
		}

		public SimulationViewModel()
			//: base()
		{
			ToggleSettingsCommand = new RelayCommand<object>(OnToggleSettings, CanToggleSettingsExecute);

			LeaderCount = 12;
			FollowerCount = 300;

			UpdateFps = 60;
			RenderFps = 24;

			FadeAlpha = 48;

			LeaderRadius = 1;
			FollowerRadius = 2;

			//

			_simulation = new Simulation();

			//TODO: convert methods to properties -> vm properties access directly
			_simulation.SetLeaderCount((int)LeaderCount);
			_simulation.SetFollowerCount((int)FollowerCount);
			_simulation.UpdateFps = UpdateFps;
			_simulation.RenderFps = RenderFps;
			_simulation.SetFadeToBlackAlpha((int)FadeAlpha);

			foreach (var leader in _simulation.Leaders)
			{
				leader.Size = LeaderRadius;

				foreach (var follower in leader.Followers)
				{
					follower.Size = FollowerRadius;
				}
			}
		}

		private bool CanToggleSettingsExecute(object obj)
		{
			return true;
		}

		private void OnToggleSettings(object obj)
		{
			IsConfigurationVisible = !IsConfigurationVisible;
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (_simulation == null)
			{
				return;
			}

			switch (e.PropertyName)
			{
				case nameof(LeaderCount):
					_simulation.SetLeaderCount((int)LeaderCount);
					break;
				case nameof(FollowerCount):
					_simulation.SetFollowerCount((int)FollowerCount);
					break;
				case nameof(UpdateFps):
					_simulation.UpdateFps = UpdateFps;
					break;
				case nameof(RenderFps):
					_simulation.RenderFps = RenderFps;
					break;
				case nameof(FadeAlpha):
					_simulation.SetFadeToBlackAlpha((int)FadeAlpha);
					break;
				case nameof(LeaderRadius):
					foreach (var leader in _simulation.Leaders)
					{
						leader.Size = LeaderRadius;
					}
					break;
				case nameof(FollowerRadius):
					foreach (var leader in _simulation.Leaders)
					{
						foreach (var follower in leader.Followers)
						{
							follower.Size = FollowerRadius;
						}
					}
					break;
				case nameof(IsConfigurationVisible):
					break;
				default:
					Debugger.Break();
					break;
			}

			string msg =
				$"{nameof(LeaderCount)} = {LeaderCount}, " +
				$"{nameof(FollowerCount)} = {FollowerCount}, " +
				$"{nameof(UpdateFps)} = {UpdateFps}, " +
				$"{nameof(RenderFps)} = {RenderFps}, " +
				$"{nameof(FadeAlpha)} = {FadeAlpha}, " +
				$"{nameof(LeaderRadius)} = {LeaderRadius}, " +
				$"{nameof(FollowerRadius)} = {FollowerRadius}, ";
			Debug.WriteLine(msg);

			base.OnPropertyChanged(e);
		}
	}
}
