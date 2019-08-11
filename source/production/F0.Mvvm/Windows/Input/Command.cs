using System;
using System.Threading.Tasks;

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

		public static IAsyncCommand Create(Func<Task> onExecute)
		{
			return new AsyncRelayCommand(onExecute);
		}

		public static IAsyncCommand Create(Func<Task> onExecute, Func<bool> onCanExecute)
		{
			return new AsyncRelayCommand(onExecute, onCanExecute);
		}

		public static IAsyncCommand<T> Create<T>(Func<T, Task> onExecute)
		{
			return new AsyncRelayCommand<T>(onExecute);
		}

		public static IAsyncCommand<T> Create<T>(Func<T, Task> onExecute, Predicate<T> onCanExecute)
		{
			return new AsyncRelayCommand<T>(onExecute, onCanExecute);
		}
	}
}
