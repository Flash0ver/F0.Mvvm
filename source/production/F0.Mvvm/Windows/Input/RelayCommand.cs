using System;

namespace F0.Windows.Input
{
	internal sealed class RelayCommand : CommandBase
	{
		private readonly Action onExecute;
		private readonly Func<bool>? onCanExecute;

		internal RelayCommand(Action onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal RelayCommand(Action onExecute, Func<bool> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute()
		{
			return onCanExecute is null || onCanExecute();
		}

		protected override void OnExecute()
		{
			onExecute();
		}
	}

	internal sealed class RelayCommand<T> : CommandBase<T> where T : notnull
	{
		private readonly Action<T> onExecute;
		private readonly Predicate<T>? onCanExecute;

		internal RelayCommand(Action<T> onExecute)
		{
			this.onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		}

		internal RelayCommand(Action<T> onExecute, Predicate<T> onCanExecute)
			: this(onExecute)
		{
			this.onCanExecute = onCanExecute ?? throw new ArgumentNullException(nameof(onCanExecute));
		}

		protected override bool OnCanExecute(T parameter)
		{
			return onCanExecute is null || onCanExecute(parameter);
		}

		protected override void OnExecute(T parameter)
		{
			onExecute(parameter);
		}
	}
}
