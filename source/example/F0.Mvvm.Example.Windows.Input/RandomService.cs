using System;
using System.Threading;
using System.Threading.Tasks;

namespace F0.Mvvm.Example.Windows.Input
{
	internal class RandomService : IRandomService, IDisposable
	{
		private readonly Random random;
		private readonly SemaphoreSlim semaphore;

		public RandomService()
		{
			random = new Random();
			semaphore = new SemaphoreSlim(1);
		}

		public async Task<int> GetRandomNumberAsync(int minValue, int maxValue)
		{
			await semaphore.WaitAsync();

			int number = random.Next(minValue, maxValue);

			_ = semaphore.Release();

			return number;
		}

		public void Dispose()
		{
			semaphore.Dispose();
		}
	}
}
