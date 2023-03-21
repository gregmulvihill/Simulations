using System.Globalization;

namespace SimulationLib.Converters
{
	public class NonNullToBooleanConverter : ConverterBase
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrWhiteSpace(value?.ToString());
		}
	}
}
