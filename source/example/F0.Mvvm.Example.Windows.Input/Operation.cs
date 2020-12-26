using System.Threading.Tasks;
using F0.ComponentModel;

namespace F0.Mvvm.Example.Windows.Input
{
	internal class Operation : ViewModel
	{
		private readonly IRandomService randomService;

		private int progress;
		public int Progress
		{
			get => progress;
			set => SetProperty(ref progress, value);
		}

		public Operation(IRandomService randomService)
		{
			this.randomService = randomService;
		}

		public async Task ExecuteAsync()
		{
			for (int i = 0; i < 100; i++)
			{
				int millisecondsDelay = await randomService.GetRandomNumberAsync(0, 240 / 2);

				await Task.Delay(millisecondsDelay);

				Progress = i + 1;
			}
		}
	}
}
