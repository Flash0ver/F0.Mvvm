using System;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class RelayCommandTests
	{
		[Fact]
		public void DoNotCreateCommandWithNullDelegates()
		{
			Assert.Throws<ArgumentNullException>("onExecute", () => new RelayCommand(null));
			Assert.Throws<ArgumentNullException>("onExecute", () => new RelayCommand<int>(null));

			Assert.Throws<ArgumentNullException>("onCanExecute", () => new RelayCommand(() => { }, null));
			Assert.Throws<ArgumentNullException>("onCanExecute", () => new RelayCommand<int>(_ => { }, null));

			Assert.Throws<ArgumentNullException>("onExecute", () => new RelayCommand(null, () => true));
			Assert.Throws<ArgumentNullException>("onExecute", () => new RelayCommand<int>(null, _ => true));
		}

		[Fact]
		public void TheDefaultReturnValueForTheCanExecuteMethodIsTrue()
		{
			IInputCommand command = new RelayCommand(() => { });
			IInputCommand<string> commandT = new RelayCommand<string>(_ => { });

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public void TheExecutionStatusLogicDeterminesWhetherTheRelayCommandCanExecuteInItsCurrentState()
		{
			bool canExecute = false;
			IInputCommand command = new RelayCommand(() => { }, () => canExecute);
			IInputCommand<string> commandT = new RelayCommand<string>(_ => { }, _ => canExecute);

			Assert.False(command.CanExecute());
			Assert.False(commandT.CanExecute("F0"));

			canExecute = true;

			Assert.True(command.CanExecute());
			Assert.True(commandT.CanExecute("F0"));
		}

		[Fact]
		public void ExecuteTheRelayCommandOnTheCurrentCommandTarget()
		{
			int number = 0;
			IInputCommand command = new RelayCommand(() => number++);
			IInputCommand<string> commandT = new RelayCommand<string>(_ => number--);

			Assert.Equal(0, number);
			command.Execute();
			Assert.Equal(1, number);
			commandT.Execute("F0");
			Assert.Equal(0, number);
		}

		[Fact]
		public void InContrastToTheNonGenericCommandWhichDoesNotSupportDataToBePassed_DataIsUsedByTheStronglyTypedCommand()
		{
			int number = 0;
			IInputCommand<int> commandT = new RelayCommand<int>(addend => number += addend, integer => integer % 2 == 0);

			Assert.False(commandT.CanExecute(1));
			Assert.True(commandT.CanExecute(2));

			Assert.Equal(0, number);
			commandT.Execute(1);
			Assert.Equal(1, number);
			commandT.Execute(2);
			Assert.Equal(3, number);
			commandT.Execute(3);
			Assert.Equal(6, number);
			commandT.Execute(-9);
			Assert.Equal(-3, number);
		}

		[Fact]
		public void CommandCanAlwaysBeInvokedDirectly_RegardlessWhetherTheCommandCanBeExecutedOrNot()
		{
			int number = 0;
			bool canExecute = false;
			IInputCommand command = new RelayCommand(() => number++, () => canExecute);
			IInputCommand<string> commandT = new RelayCommand<string>(_ => number--, _ => canExecute);

			Assert.False(command.CanExecute());
			command.Execute();
			Assert.Equal(1, number);
			Assert.False(commandT.CanExecute("F0"));
			commandT.Execute("F0");
			Assert.Equal(0, number);

			canExecute = true;

			Assert.True(command.CanExecute());
			command.Execute();
			Assert.Equal(1, number);
			Assert.True(commandT.CanExecute("F0"));
			commandT.Execute("F0");
			Assert.Equal(0, number);
		}
	}
}
