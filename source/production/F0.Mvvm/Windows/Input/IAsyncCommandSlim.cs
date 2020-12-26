using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace F0.Windows.Input
{
	public interface IAsyncCommandSlim : ICommand
	{
		[Obsolete("Prefer parameterless " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object parameter);
		bool CanExecute();

		[Obsolete("Prefer ValueTask-returning parameterless " + nameof(ExecuteAsync) + " method.", true)]
		new void Execute(object parameter);
		ValueTask ExecuteAsync();

		void RaiseCanExecuteChanged();
	}

	public interface IAsyncCommandSlim<T> : ICommand
	{
		[Obsolete("Prefer strongly typed " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object parameter);
		bool CanExecute(T parameter);

		[Obsolete("Prefer ValueTask-returning strongly typed " + nameof(ExecuteAsync) + " method.", true)]
		new void Execute(object parameter);
		ValueTask ExecuteAsync(T parameter);

		void RaiseCanExecuteChanged();
	}
}
