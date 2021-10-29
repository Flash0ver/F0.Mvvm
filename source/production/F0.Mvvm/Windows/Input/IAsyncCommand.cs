using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace F0.Windows.Input
{
	public interface IAsyncCommand : ICommand
	{
		[Obsolete("Prefer parameterless " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object? parameter);
		bool CanExecute();

		[Obsolete("Prefer Task-returning parameterless " + nameof(ExecuteAsync) + " method.", true)]
		new void Execute(object? parameter);
		Task ExecuteAsync();

		void RaiseCanExecuteChanged();
	}

	public interface IAsyncCommand<T> : ICommand
	{
		[Obsolete("Prefer strongly typed " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object? parameter);
		bool CanExecute(T parameter);

		[Obsolete("Prefer Task-returning strongly typed " + nameof(ExecuteAsync) + " method.", true)]
		new void Execute(object? parameter);
		Task ExecuteAsync(T parameter);

		void RaiseCanExecuteChanged();
	}
}
