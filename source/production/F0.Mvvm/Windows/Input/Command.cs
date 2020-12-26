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

		public static IAsyncCommandSlim Create(Func<ValueTask> onExecute)
		{
			return new AsyncRelayCommandSlim(onExecute);
		}

		public static IAsyncCommandSlim Create(Func<ValueTask> onExecute, Func<bool> onCanExecute)
		{
			return new AsyncRelayCommandSlim(onExecute, onCanExecute);
		}

		public static IAsyncCommandSlim<T> Create<T>(Func<T, ValueTask> onExecute)
		{
			return new AsyncRelayCommandSlim<T>(onExecute);
		}

		public static IAsyncCommandSlim<T> Create<T>(Func<T, ValueTask> onExecute, Predicate<T> onCanExecute)
		{
			return new AsyncRelayCommandSlim<T>(onExecute, onCanExecute);
		}

		public static IBoundedCommand Create(Func<Task> onExecute, int maxCount)
		{
			return new BoundedRelayCommand(onExecute, maxCount);
		}

		public static IBoundedCommand Create(Func<Task> onExecute, Func<bool> onCanExecute, int maxCount)
		{
			return new BoundedRelayCommand(onExecute, onCanExecute, maxCount);
		}

		public static IBoundedCommand<T> Create<T>(Func<T, Task> onExecute, int maxCount)
		{
			return new BoundedRelayCommand<T>(onExecute, maxCount);
		}

		public static IBoundedCommand<T> Create<T>(Func<T, Task> onExecute, Predicate<T> onCanExecute, int maxCount)
		{
			return new BoundedRelayCommand<T>(onExecute, onCanExecute, maxCount);
		}

		public static IBoundedCommandSlim Create(Func<ValueTask> onExecute, int maxCount)
		{
			return new BoundedRelayCommandSlim(onExecute, maxCount);
		}

		public static IBoundedCommandSlim Create(Func<ValueTask> onExecute, Func<bool> onCanExecute, int maxCount)
		{
			return new BoundedRelayCommandSlim(onExecute, onCanExecute, maxCount);
		}

		public static IBoundedCommandSlim<T> Create<T>(Func<T, ValueTask> onExecute, int maxCount)
		{
			return new BoundedRelayCommandSlim<T>(onExecute, maxCount);
		}

		public static IBoundedCommandSlim<T> Create<T>(Func<T, ValueTask> onExecute, Predicate<T> onCanExecute, int maxCount)
		{
			return new BoundedRelayCommandSlim<T>(onExecute, onCanExecute, maxCount);
		}
	}
}
