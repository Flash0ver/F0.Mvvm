using System;
using System.Windows.Input;

namespace F0.Windows.Input
{
	public interface IInputCommand : ICommand
	{
		[Obsolete("Prefer parameterless " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object parameter);
		bool CanExecute();

		[Obsolete("Prefer parameterless " + nameof(Execute) + " method.", true)]
		new void Execute(object parameter);
		void Execute();

		void RaiseCanExecuteChanged();
	}

	public interface IInputCommand<T> : ICommand
	{
		[Obsolete("Prefer strongly typed " + nameof(CanExecute) + " method.", true)]
		new bool CanExecute(object parameter);
		bool CanExecute(T parameter);

		[Obsolete("Prefer strongly typed " + nameof(Execute) + " method.", true)]
		new void Execute(object parameter);
		void Execute(T parameter);

		void RaiseCanExecuteChanged();
	}
}
