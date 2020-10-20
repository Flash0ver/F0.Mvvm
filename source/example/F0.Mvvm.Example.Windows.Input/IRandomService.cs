using System.Threading.Tasks;

namespace F0.Mvvm.Example.Windows.Input
{
	internal interface IRandomService
	{
		Task<int> GetRandomNumberAsync(int minValue, int maxValue);
	}
}
