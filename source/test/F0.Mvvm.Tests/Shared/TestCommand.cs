using System;
using F0.Windows.Input;

namespace F0.Tests.Shared
{
	internal sealed class TestCommand : CommandBase
	{
		private readonly Action onCanExecute;
		private readonly Action onExecute;

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

		internal TestCommand()
		{
			isEnabled = true;
		}

		internal TestCommand(Action onCanExecute, Action onExecute)
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

		protected override void OnExecute()
		{
			onExecute?.Invoke();
		}
	}

	internal sealed class TestCommand<T> : CommandBase<T>
	{
		private readonly Action<T> onCanExecute;
		private readonly Action<T> onExecute;

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

		internal TestCommand()
		{
			isEnabled = true;
		}

		internal TestCommand(Action<T> onCanExecute, Action<T> onExecute)
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

		protected override void OnExecute(T parameter)
		{
			onExecute?.Invoke(parameter);
		}
	}
}
