using System;

namespace F0.Windows.Input
{
	public abstract class CommandBase : IInputCommand
	{
		public event EventHandler CanExecuteChanged;

		protected CommandBase()
		{
		}

		public bool CanExecute(object parameter)
		{
			if (parameter is { })
			{
				throw new ArgumentException("CommandParameter must be null", nameof(parameter));
			}

			return CanExecute();
		}

		public bool CanExecute()
		{
			return OnCanExecute();
		}

		public void Execute(object parameter)
		{
			if (parameter is { })
			{
				throw new ArgumentException("CommandParameter must be null", nameof(parameter));
			}

			Execute();
		}

		public void Execute()
		{
			OnExecute();
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		protected abstract bool OnCanExecute();
		protected abstract void OnExecute();
	}

	public abstract class CommandBase<T> : IInputCommand<T>
	{
		public event EventHandler CanExecuteChanged;

		protected CommandBase()
		{
		}

		public bool CanExecute(object parameter)
		{
			return CanExecute((T)parameter);
		}

		public bool CanExecute(T parameter)
		{
			if (parameter is null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			return OnCanExecute(parameter);
		}

		public void Execute(object parameter)
		{
			Execute((T)parameter);
		}

		public void Execute(T parameter)
		{
			if (parameter is null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			OnExecute(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		protected abstract bool OnCanExecute(T parameter);
		protected abstract void OnExecute(T parameter);
	}
}
