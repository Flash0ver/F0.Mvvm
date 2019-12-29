using System;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	public abstract class AsyncCommandBase : IAsyncCommand
	{
		public event EventHandler CanExecuteChanged;

		protected AsyncCommandBase()
		{
		}

		public bool CanExecute(object parameter)
		{
			if (!(parameter is null))
			{
				throw new ArgumentException("CommandParameter must be null", nameof(parameter));
			}

			return CanExecute();
		}

		public bool CanExecute()
		{
			return OnCanExecute();
		}

		public async void Execute(object parameter)
		{
			await InternalExecuteAsync(parameter);
		}

		public Task ExecuteAsync()
		{
			return OnExecuteAsync();
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		internal async Task InternalExecuteAsync(object parameter)
		{
			if (!(parameter is null))
			{
				throw new ArgumentException("CommandParameter must be null", nameof(parameter));
			}

			await ExecuteAsync();
		}

		protected abstract bool OnCanExecute();
		protected abstract Task OnExecuteAsync();
	}

	public abstract class AsyncCommandBase<T> : IAsyncCommand<T>
	{
		public event EventHandler CanExecuteChanged;

		protected AsyncCommandBase()
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

		public async void Execute(object parameter)
		{
			await InternalExecuteAsync(parameter);
		}

		public Task ExecuteAsync(T parameter)
		{
			if (parameter is null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			return OnExecuteAsync(parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		internal async Task InternalExecuteAsync(object parameter)
		{
			await ExecuteAsync((T)parameter);
		}

		protected abstract bool OnCanExecute(T parameter);
		protected abstract Task OnExecuteAsync(T parameter);
	}
}
