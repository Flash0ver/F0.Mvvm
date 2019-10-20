using System.Threading.Tasks;

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

		public static async Task<bool> TryExecuteAsync(this IAsyncCommand command)
		{
			if (command.CanExecute())
			{
				await command.ExecuteAsync();
				return true;
			}
			else
			{
				return false;
			}
		}

		public static async Task<bool> TryExecuteAsync<T>(this IAsyncCommand<T> command, T parameter)
		{
			if (command.CanExecute(parameter))
			{
				await command.ExecuteAsync(parameter);
				return true;
			}
			else
			{
				return false;
			}
		}

		public static async ValueTask<bool> TryExecuteAsync(this IAsyncCommandSlim command)
		{
			if (command.CanExecute())
			{
				await command.ExecuteAsync();
				return true;
			}
			else
			{
				return false;
			}
		}

		public static async ValueTask<bool> TryExecuteAsync<T>(this IAsyncCommandSlim<T> command, T parameter)
		{
			if (command.CanExecute(parameter))
			{
				await command.ExecuteAsync(parameter);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
