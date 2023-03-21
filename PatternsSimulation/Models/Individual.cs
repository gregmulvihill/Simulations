using SimulationLib.Tools;

using SkiaSharp;

namespace PatternsSimulation.Models
{
	public class Individual
	{
		private static readonly double _minAngleVelocity = 1.0;//7.0;
		private static readonly double _maxAngleVelocity = 2 * 15.0;//11.0;

		private static readonly double _minVelocity = 1.5 * 0.50;
		private static readonly double _maxVelocity = 4.5 * 0.50;
		private static readonly int _catchRadius = 50;
		private static readonly int _catchDriftCount = 10;//125;

		//

		private double _angle;
		private double _velocity;
		private double _angleVelocity;
		private double _targetAngle;

		// TODO: add drift type... squiggly, crazy, strait line, ?
		private int _driftCount = 0;

		public double X1 { get; set; }
		public double Y1 { get; set; }
		public double X2 { get; set; }
		public double Y2 { get; set; }
		public double Size { get; set; }
		public SKPaint Color => _driftCount > 0 ? _paintTimeout : _paintActive;

		public List<Individual> Followers { get; internal set; }
		public Individual Leader { get; set; }

		private readonly Simulation _simulation;

		private SKPaint _paintActive = new() { Color = SKColors.Orange };
		private readonly SKPaint _paintTimeout = new() { Color = SKColors.Gray };  // TODO:make gray'ed version of _paintActive

		public Individual(Simulation simulation, Individual leader)
		{
			_simulation = simulation;
			Leader = leader;
			Followers = new List<Individual>();

			_angle = MathEx.RndNextD(0, MathEx.CircleDegrees);
			_velocity = MathEx.RndNextD(_minVelocity, _maxVelocity);
			_targetAngle = MathEx.RndNextD(0, MathEx.CircleDegrees);
			_angleVelocity = MathEx.RndNextD(_minAngleVelocity, _maxAngleVelocity) * MathEx.RandomDirection();
			_driftCount = 0;
		}

		public void SetHue(double hue)
		{
			_paintActive = new SKPaint { Color = SKColor.FromHsl((float)hue, (float)100.0, (float)(100.0 / 2.0)), };
		}

		public void Update()
		{
			TrigonometryValues oUnitCircle = MathEx.UnitCircle(_angle);
			double nXD = _velocity * oUnitCircle.Sin;
			double nYD = _velocity * oUnitCircle.Cos;

			X2 = (int)(X1 + 0.5 * nXD);
			Y2 = (int)(Y1 + 0.5 * nYD);

			X1 += nXD;
			Y1 += nYD;

			_angle += _angleVelocity;

			MathEx.WrapRef(ref _angle, 0, MathEx.CircleDegrees);

			if (MathEx.RND.NextDouble() < 0.25)
			{
				Bounce();
			}
			else
			{
				WrapPosition();
			}

			CalculateVector();

			if (Leader != null)
			{
				if (_driftCount > 0)
				{
					_driftCount--;
				}
				else
				{
					CheckCatch();
				}
			}
		}

		protected void CalculateVector()
		{
			if (Leader == null)
			{
				_targetAngle = MathEx.RndNextD(0, MathEx.CircleDegrees);
				//_targetAngle += MathEx.RndNext(-30, 30);

				_velocity = MathEx.RndNextD(_minVelocity, _maxVelocity);
			}
			else
			{
				_targetAngle = Math.Atan2(Y1 - Leader.Y1, Leader.X1 - X1) * MathEx.DegreesPerRadian;

				//if we got to close to the leader, go away from it
				if (_driftCount > 0)
				{
					_targetAngle += MathEx.CircleDegrees / 2.0;
				}
				else  //option?
				{
					//wander...
					_targetAngle += MathEx.RandomDirection() * 45.0;  //TODO: move to public static
				}
			}

			MathEx.WrapRef(ref _targetAngle, 0.0, MathEx.CircleDegrees);

			double diff = _angle - _targetAngle;
			MathEx.WrapRef(ref diff, -180.0, 180.0);

			_angleVelocity = Math.Sign(diff) * Math.Abs(_angleVelocity);
		}

		private void RotateTowardPreferredAngle()
		{
			double cw;
			double ccw;

			//which way to turn...
			if (_targetAngle <= _angle)
			{
				ccw = MathEx.CircleDegrees - _angle + _targetAngle;
				cw = _angle - _targetAngle;
			}
			else
			{
				ccw = _targetAngle - _angle;
				cw = MathEx.CircleDegrees - _targetAngle + _angle;
			}

			_angleVelocity = cw <= ccw ? -Math.Abs(_angleVelocity) : Math.Abs(_angleVelocity);
		}

		protected void WrapPosition()
		{
			double x1 = X1;
			double y1 = Y1;
			MathEx.WrapRef(ref x1, 0, _simulation.Width);
			MathEx.WrapRef(ref y1, 0, _simulation.Height);
			X1 = x1;
			Y1 = y1;
		}

		protected void Bounce()
		{
			if (X1 < 0)
			{
				X1 = 0;
				//Bounce();
			}
			else if (X1 >= _simulation.Width)
			{
				X1 = _simulation.Width - 1;
				//Bounce();
			}

			if (Y1 < 0)
			{
				Y1 = 0;
				//Bounce();
			}
			else if (Y1 >= _simulation.Height)
			{
				Y1 = _simulation.Height - 1;
				//Bounce();
			}
		}

		protected void Bounce0()
		{
			//http://stackoverflow.com/a/13112994/640326
			_angle = MathEx.CircleDegrees - 1 - _angle;

			//if (IsLeader) _nDriftCount = 25;
		}

		protected void CheckCatch()
		{
			if (Leader == null)
			{
				return;
			}

			double a = X1 - Leader.X1;
			double b = Y1 - Leader.Y1;

			if (0 == _driftCount && a * a + b * b < _catchRadius * _catchRadius)
			{
				_driftCount = _catchDriftCount;
				//_thetaSpeed = -_thetaSpeed;
				//find new leader????
			}
		}
	}
}
