using System;
using System.Threading.Tasks;
using F0.Windows.Input;

namespace F0.Tests.Shared
{
	internal sealed class AsyncTestCommand : AsyncCommandBase
	{
		private readonly Action onCanExecute;
		private readonly Func<Task> onExecute;

		private bool isEnabled;
		public bool IsEnabled
		{
			get => isEnabled;
			set
			{
				if (value != isEnabled)
				{
					isEnabled = value;
					RaiseCanExecuteChanged();
				}
			}
		}

		internal AsyncTestCommand()
		{
			isEnabled = true;
		}

		internal AsyncTestCommand(Action onCanExecute, Func<Task> onExecute)
			: this()
		{
			this.onCanExecute = onCanExecute;
			this.onExecute = onExecute;
		}

		protected override bool OnCanExecute()
		{
			onCanExecute?.Invoke();
			return IsEnabled;
		}

		protected override Task OnExecuteAsync()
		{
			return onExecute?.Invoke() ?? Task.CompletedTask;
		}
	}

	internal sealed class AsyncTestCommand<T> : AsyncCommandBase<T>
	{
		private readonly Action<T> onCanExecute;
		private readonly Func<T, Task> onExecute;

		private bool isEnabled;
		public bool IsEnabled
		{
			get => isEnabled;
			set
			{
				if (value != isEnabled)
				{
					isEnabled = value;
					RaiseCanExecuteChanged();
				}
			}
		}

		internal AsyncTestCommand()
		{
			isEnabled = true;
		}

		internal AsyncTestCommand(Action<T> onCanExecute, Func<T, Task> onExecute)
			: this()
		{
			this.onCanExecute = onCanExecute;
			this.onExecute = onExecute;
		}

		protected override bool OnCanExecute(T parameter)
		{
			onCanExecute?.Invoke(parameter);
			return IsEnabled;
		}

		protected override Task OnExecuteAsync(T parameter)
		{
			return onExecute?.Invoke(parameter) ?? Task.CompletedTask;
		}
	}
}
