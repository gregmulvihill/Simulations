namespace SimulationLib.Tools
{
	public static class MathEx
	{
		public static double UnitCircleRadians = 2.0 * Math.PI;
		public static double CircleDegrees = 360.0F;
		public static double DegreesPerRadian = CircleDegrees / UnitCircleRadians;
		public static double RadiansPerDegree = UnitCircleRadians / CircleDegrees;

		public static int PrecalculationScale = 10;
		public static int PrecalculationValueCount = (int)(CircleDegrees * PrecalculationScale);

		private static readonly TrigonometryValues[] _oUnitCircleTrigonometryValues = new TrigonometryValues[PrecalculationValueCount];

		public static Random RND = new();

		static MathEx()
		{
			double nOffset = Math.PI / 2.0;

			// Pre-calculate trigonometry values
			for (int nScaledAngle = 0; nScaledAngle < PrecalculationValueCount; nScaledAngle++)
			{
				double nAngleDegrees = nScaledAngle / (double)PrecalculationScale;
				double nAngle = (RadiansPerDegree * nAngleDegrees) + nOffset;

				_oUnitCircleTrigonometryValues[nScaledAngle].Sin = Math.Sin(nAngle);
				_oUnitCircleTrigonometryValues[nScaledAngle].Cos = Math.Cos(nAngle);
			}
		}

		public static TrigonometryValues UnitCircle(double nAngleDegrees)
		{
			nAngleDegrees *= PrecalculationScale;
			WrapRef(ref nAngleDegrees, 0, PrecalculationValueCount - 1);

			return _oUnitCircleTrigonometryValues[(int)nAngleDegrees];
		}

		public static float RndNextF(float nMin, float nMax)
		{
			return (float)((RND.NextDouble() * (nMax - nMin)) + nMin);
		}

		public static double RndNextD(double nMin, double nMax)
		{
			return (RND.NextDouble() * (nMax - nMin)) + nMin;
		}

		public static int RandomDirection()
		{
			return RND.Next(2) == 0 ? -1 : 1;
		}

		public static void Clamp(ref int nValue, int nMin, int nMax)
		{
			nValue = Math.Min(Math.Max(nMin, nValue), nMax);
		}

		public static void Clamp(ref double nValue, double nMin, double nMax)
		{
			nValue = Math.Min(Math.Max(nMin, nValue), nMax);
		}

		public static void WrapRef(ref int nValue, int nMin, int nMax)
		{
			//http://stackoverflow.com/a/14415822/640326
			nValue = ((nValue + nMax - nMin) % (nMax - nMin)) + nMin;

			//http://stackoverflow.com/a/29871193/640326
			//var nRange = nMax - nMin;
			//nValue = nMin + (nRange + (nValue - nMin) % nMax) % nRange;
		}

		public static void WrapRef(ref double nValue, double nMin, double nMax)
		{
			nValue = ((nValue + nMax - nMin) % (nMax - nMin)) + nMin;
		}
	}

	public struct TrigonometryValues
	{
		public double Sin;
		public double Cos;

		public override string ToString()
		{
			return string.Format("{0,15:0.00000000000}, {1,15:0.00000000000}", Sin, Cos);
		}
	}
}
