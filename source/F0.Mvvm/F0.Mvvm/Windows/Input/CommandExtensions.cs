namespace F0.Windows.Input
{
	public static class CommandExtensions
	{
		public static bool TryExecute(this IInputCommand command)
		{
			if (command.CanExecute())
			{
				command.Execute();
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool TryExecute<T>(this IInputCommand<T> command, T parameter)
		{
			if (command.CanExecute(parameter))
			{
				command.Execute(parameter);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
