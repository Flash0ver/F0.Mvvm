using System;
using System.Reflection;
using System.Windows.Input;
using F0.Windows.Input;
using Xunit;

namespace F0.Tests.Windows.Input
{
	public class CommandBaseTests
	{
		[Fact]
		public void CommandBase_FrameworkMethodsMustBeCalledWithoutData()
		{
			ICommand command = new TestCommand();

			bool canExecute = command.CanExecute(null);
			Assert.True(canExecute);
			Assert.Throws<ArgumentException>("parameter", () => command.CanExecute(240));

			command.Execute(null);
			Assert.Throws<ArgumentException>("parameter", () => command.Execute(240));
		}

		[Fact]
		public void CommandBase_T_FrameworkMethodsMustBeCalledWithData_ValueType()
		{
			ICommand command = new TestCommand<int>();

			bool canExecute = command.CanExecute(240);
			Assert.True(canExecute);
			Assert.Throws<NullReferenceException>(() => command.CanExecute(null));

			command.Execute(240);
			Assert.Throws<NullReferenceException>(() => command.Execute(null));
		}

		[Fact]
		public void CommandBase_T_FrameworkMethodsMustBeCalledWithData_ReferenceType()
		{
			ICommand command = new TestCommand<string>();

			bool canExecute = command.CanExecute("240");
			Assert.True(canExecute);
			Assert.Throws<ArgumentNullException>("parameter", () => command.CanExecute(null));

			command.Execute("240");
			Assert.Throws<ArgumentNullException>("parameter", () => command.Execute(null));
		}

		[Fact]
		public void CommandBase_T_NonGenericFrameworkMethodsMustBeCalledWithDataOfTypeCompatibleWithGenericTypeParameter()
		{
			ICommand command = new TestCommand<float>();

			Assert.Throws<InvalidCastException>(() => command.CanExecute(240.0));
			Assert.Throws<InvalidCastException>(() => command.Execute(240.0));
		}

		[Fact]
		public void CommandBase_FrameworkMethodsCallIntoParameterlessImplementations()
		{
			string text = null;
			ICommand command = new TestCommand(() => text = nameof(ICommand.CanExecute), () => text = nameof(ICommand.Execute));

			bool canExecute = command.CanExecute(null);
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(null);
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void CommandBase_T_FrameworkMethodsCallIntoStronglyTypedImplementations()
		{
			string text = null;
			ICommand command = new TestCommand<string>(t => text = t, t => text = t);

			bool canExecute = command.CanExecute(nameof(ICommand.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(nameof(ICommand.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void DiscourageTheUseOfWeaklyTypedMethods()
		{
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IInputCommand));
			CheckThatTheWeaklyTypedCommandMethodsAreObsolete(typeof(IInputCommand<>));

			void CheckThatTheWeaklyTypedCommandMethodsAreObsolete(Type type)
			{
				MethodInfo canExecute = type.GetMethod(nameof(ICommand.CanExecute), new Type[] { typeof(object) });
				MethodInfo execute = type.GetMethod(nameof(ICommand.Execute), new Type[] { typeof(object) });

				Assert.NotNull(canExecute);
				Assert.NotNull(execute);

				CheckThatMethodIsObsoleteWithError(canExecute);
				CheckThatMethodIsObsoleteWithError(execute);
			}

			void CheckThatMethodIsObsoleteWithError(MethodInfo methodInfo)
			{
				ObsoleteAttribute attribute = methodInfo.GetCustomAttribute<ObsoleteAttribute>();

				Assert.NotNull(attribute);
				Assert.True(attribute.IsError);
			}
		}

		[Fact]
		public void CommandBase_EncourageTheUseOfOverloadedMethodsWhichTakeNoParameter()
		{
			string text = null;
			IInputCommand command = new TestCommand(() => text = nameof(IInputCommand.CanExecute), () => text = nameof(IInputCommand.Execute));

			bool canExecute = command.CanExecute();
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute();
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void CommandBase_T_EncourageTheUseOfOverloadedMethodsWhichTakeGenericParameter()
		{
			string text = null;
			IInputCommand<string> command = new TestCommand<string>(t => text = t, t => text = t);

			bool canExecute = command.CanExecute(nameof(IInputCommand<string>.CanExecute));
			Assert.Equal("CanExecute", text);
			Assert.True(canExecute);

			command.Execute(nameof(IInputCommand<string>.Execute));
			Assert.Equal("Execute", text);
		}

		[Fact]
		public void CommandBase_StateOfTheCommandCanChange()
		{
			var command = new TestCommand();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute());

			command.IsEnabled = false;
			Assert.False(command.CanExecute());
		}

		[Fact]
		public void CommandBase_T_StateOfTheCommandCanChange()
		{
			var command = new TestCommand<string>();

			Assert.True(command.IsEnabled);
			Assert.True(command.CanExecute("F0"));

			command.IsEnabled = false;
			Assert.False(command.CanExecute("F0"));
		}

		[Fact]
		public void CommandBase_RaiseTheCanExecuteChangedEventToIndicateThatTheReturnValueOfTheCanExecuteMethodHasChanged()
		{
			string text = null;
			IInputCommand command = new TestCommand();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(TestCommand)} | {typeof(EventArgs)}", text);
		}

		[Fact]
		public void CommandBase_T_RaiseCanExecuteChangedNeedsToBeCalledWheneverCanExecuteIsExpectedToReturnADifferentValue()
		{
			string text = null;
			IInputCommand<string> command = new TestCommand<string>();
			command.CanExecuteChanged += (sender, e) => text = $"{sender.GetType()} | {e.GetType()}";

			Assert.Null(text);
			command.RaiseCanExecuteChanged();
			Assert.Equal($"{typeof(TestCommand<string>)} | {typeof(EventArgs)}", text);
		}
	}
}
