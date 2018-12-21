using System;

namespace F0.Windows.Input
{
	public static class Command
	{
		public static IInputCommand Create(Action onExecute)
		{
			return new RelayCommand(onExecute);
		}

		public static IInputCommand Create(Action onExecute, Func<bool> onCanExecute)
		{
			return new RelayCommand(onExecute, onCanExecute);
		}

		public static IInputCommand<T> Create<T>(Action<T> onExecute)
		{
			return new RelayCommand<T>(onExecute);
		}

		public static IInputCommand<T> Create<T>(Action<T> onExecute, Predicate<T> onCanExecute)
		{
			return new RelayCommand<T>(onExecute, onCanExecute);
		}
	}
}
