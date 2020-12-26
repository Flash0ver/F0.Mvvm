using System;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	public abstract class AsyncCommandSlimBase : IAsyncCommandSlim
	{
		public event EventHandler CanExecuteChanged;

		protected AsyncCommandSlimBase()
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

		public async void Execute(object parameter)
		{
			await InternalExecuteAsync(parameter);
		}

		public ValueTask ExecuteAsync()
		{
			return OnExecuteAsync();
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		internal async ValueTask InternalExecuteAsync(object parameter)
		{
			if (parameter is { })
			{
				throw new ArgumentException("CommandParameter must be null", nameof(parameter));
			}

			await ExecuteAsync();
		}

		protected abstract bool OnCanExecute();
		protected abstract ValueTask OnExecuteAsync();
	}

	public abstract class AsyncCommandSlimBase<T> : IAsyncCommandSlim<T>
	{
		public event EventHandler CanExecuteChanged;

		protected AsyncCommandSlimBase()
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

		public ValueTask ExecuteAsync(T parameter)
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

		internal async ValueTask InternalExecuteAsync(object parameter)
		{
			await ExecuteAsync((T)parameter);
		}

		protected abstract bool OnCanExecute(T parameter);
		protected abstract ValueTask OnExecuteAsync(T parameter);
	}
}
