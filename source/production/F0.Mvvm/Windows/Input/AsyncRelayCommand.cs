using System;
using System.Threading.Tasks;

namespace F0.Windows.Input
{
	internal sealed class AsyncRelayCommand : AsyncCommandBase
	{
		private readonly Func<Task> onExecute;
		private readonly Func<bool> onCanExecute;

		internal AsyncRelayCommand(Func<Task> onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal AsyncRelayCommand(Func<Task> onExecute, Func<bool> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute()
		{
			return onCanExecute is null || onCanExecute();
		}

		protected override Task OnExecuteAsync()
		{
			return onExecute();
		}
	}

	internal sealed class AsyncRelayCommand<T> : AsyncCommandBase<T>
	{
		private readonly Func<T, Task> onExecute;
		private readonly Predicate<T> onCanExecute;

		internal AsyncRelayCommand(Func<T, Task> onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal AsyncRelayCommand(Func<T, Task> onExecute, Predicate<T> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute(T parameter)
		{
			return onCanExecute is null || onCanExecute(parameter);
		}

		protected override Task OnExecuteAsync(T parameter)
		{
			return onExecute(parameter);
		}
	}
}
