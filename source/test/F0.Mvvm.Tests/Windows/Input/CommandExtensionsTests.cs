using System.Threading.Tasks;
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
			var command = new TestCommand(() => { }, () => executeCount++);

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
			var command = new AsyncTestCommand(() => { }, async () => { await Task.Yield(); executeCount++; });

			Assert.True(command.CanExecute());
			Assert.True(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
			Assert.False(await command.TryExecuteAsync());
			Assert.Equal(1, executeCount);
		}

		[Fact]
		public void Return_TrueIfTheCommandHasBeenExecuted_FalseIfTheCommandHasNotBeenExecuted_IInputCommand()
		{
			int executeCount = 0;
			var command = new TestCommand<int>(_ => { }, integer => executeCount += integer);

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
			var command = new AsyncTestCommand<int>(_ => { }, async integer => { await Task.Yield(); executeCount += integer; });

			Assert.True(command.CanExecute(9));
			Assert.True(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);

			command.IsEnabled = false;
			Assert.False(command.CanExecute(9));
			Assert.False(await command.TryExecuteAsync(9));
			Assert.Equal(9, executeCount);
		}
	}
}
