using F0.ComponentModel;
using F0.Windows.Input;

namespace F0.Mvvm.Example.Windows.Input
{
	internal class MainViewModel : ViewModel
	{
		private int number = 240;
		public int Number
		{
			get => number;
			set => SetField(ref number, value);
		}

		private int parameter = 0;
		public int Parameter
		{
			get => parameter;
			set
			{
				if (TrySetField(ref parameter, value))
				{
					AddCommand.RaiseCanExecuteChanged();
					SubtractCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public IInputCommand IncrementCommand { get; }
		public IInputCommand DecrementCommand { get; }
		public IInputCommand<int> AddCommand { get; }
		public IInputCommand<int> SubtractCommand { get; }

		public MainViewModel()
		{
			IncrementCommand = Command.Create(() => Number++);
			DecrementCommand = Command.Create(() => Number--);
			AddCommand = Command.Create<int>(parameter => Number += parameter, parameter => parameter != 0);
			SubtractCommand = Command.Create<int>(parameter => Number -= parameter, parameter => parameter != 0);
		}
	}
}
