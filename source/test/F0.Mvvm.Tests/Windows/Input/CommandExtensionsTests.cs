using System.Threading.Tasks;
using F0.Tests.Shared;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class CommandExtensionsTests
	{
		[Fact]
		public void ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse_IInputCommand()
		{
			int executeCount = 0;
			TestCommand command = new(() => { }, () => executeCount++);

			Assert.True(command.CanExecute());
			Assert.True(command.TryExecute());
			Assert.Equal(1, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(command.TryExecute());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public async Task ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse_IAsyncCommand()
		{
			int executeCount = 0;
			AsyncTestCommand command = new(() => { }, async () => { await Task.Yield(); executeCount++; });

			Assert.True(command.CanExecute());
			Assert.True(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public async Task ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse_IAsyncCommandSlim()
		{
			int executeCount = 0;
			AsyncTestCommandSlim command = new(() => { }, async () => { await Task.Yield(); executeCount++; });

			Assert.True(command.CanExecute());
			Assert.True(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public async Task ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse_IBoundedCommand()
		{
			int executeCount = 0;
			bool isEnabled = true;
			IBoundedCommand command = new BoundedRelayCommand(async () => { await Task.Yield(); executeCount++; }, () => isEnabled);

			Assert.True(command.CanExecute());
			Assert.True(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);

			isEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public async Task ExecuteTheCommandOnlyIfTheCommandCanBeExecuted_ReturnTrueIfTheCommandWasExecuted_OtherwiseFalse_IBoundedCommandSlim()
		{
			int executeCount = 0;
			bool isEnabled = true;
			IBoundedCommandSlim command = new BoundedRelayCommandSlim(async () => { await Task.Yield(); executeCount++; }, () => isEnabled);

			Assert.True(command.CanExecute());
			Assert.True(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);

			isEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public void Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IInputCommand()
		{
			int executeCount = 0;
			TestCommand<int> command = new(_ => { }, integer => executeCount += integer);

			Assert.True(command.CanExecute(9));
			Assert.True(command.TryExecute(9));
			Assert.Equal(9, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(command.TryExecute(9));
			Assert.Equal(9, executeCount);
		}

		[Fact]
		public async Task Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IAsyncCommand()
		{
			int executeCount = 0;
			AsyncTestCommand<int> command = new(_ => { }, async integer => { await Task.Yield(); executeCount += integer; });

			Assert.True(command.CanExecute(9));
			Assert.True(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);
		}

		[Fact]
		public async Task Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IAsyncCommandSlim()
		{
			int executeCount = 0;
			AsyncTestCommandSlim<int> command = new(_ => { }, async integer => { await Task.Yield(); executeCount += integer; });

			Assert.True(command.CanExecute(9));
			Assert.True(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);
		}

		[Fact]
		public async Task Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IBoundedCommand()
		{
			int executeCount = 0;
			bool isEnabled = true;
			IBoundedCommand<int> command = new BoundedRelayCommand<int>(async integer => { await Task.Yield(); executeCount += integer; }, _ => isEnabled);

			Assert.True(command.CanExecute(9));
			Assert.True(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);

			isEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);
		}

		[Fact]
		public async Task Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IBoundedCommandSlim()
		{
			int executeCount = 0;
			bool isEnabled = true;
			IBoundedCommandSlim<int> command = new BoundedRelayCommandSlim<int>(async integer => { await Task.Yield(); executeCount += integer; }, _ => isEnabled);

			Assert.True(command.CanExecute(9));
			Assert.True(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);

			isEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);
		}
	}
}
