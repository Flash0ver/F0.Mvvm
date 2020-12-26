using System;
using System.Collections.Generic;
using System.ComponentModel;
using F0.Tests.Shared;
using Xunit;

namespace F0.Tests.ComponentModel
{
	public class ViewModelTests : IDisposable
	{
		private readonly TestViewModel viewModel;
		private readonly List<string> propertyNames;

		public ViewModelTests()
		{
			viewModel = new TestViewModel();
			viewModel.PropertyChanged += OnPropertyChanged;

			propertyNames = new List<string>();
		}

		void IDisposable.Dispose()
		{
			viewModel.PropertyChanged -= OnPropertyChanged;
			Assert.Empty(propertyNames);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Assert.Same(sender, viewModel);
			propertyNames.Add(e.PropertyName);
		}

		private void CheckPropertyNames(params string[] expected)
		{
			Assert.Equal(expected, propertyNames);
			propertyNames.Clear();
		}

		[Fact]
		public void WhenSettingPropertyToNewValue_ThenTheBackingFieldIsUpdatedAndPropertyChangedEventIsInvoked()
		{
			Assert.Equal(new Tuple<int>(-1), viewModel.Property);

			var value = new Tuple<int>(240);
			viewModel.Property = value;
			Assert.Equal(value, viewModel.Property);
			Assert.Same(value, viewModel.Property);

			CheckPropertyNames(nameof(viewModel.Property));
		}

		[Fact]
		public void WhenSettingPropertyToCurrentValue_ThenTheBackingFieldRemainsUnchangedAndPropertyChangedEventIsNotInvoked()
		{
			Assert.Equal(new Tuple<int>(-1), viewModel.Property);

			var first = new Tuple<int>(-1);
			viewModel.Property = first;
			Assert.Equal(first, viewModel.Property);
			Assert.NotSame(first, viewModel.Property);
			CheckPropertyNames();

			var second = new Tuple<int>(240);
			viewModel.Property = second;
			Assert.Equal(second, viewModel.Property);
			Assert.Same(second, viewModel.Property);
			CheckPropertyNames(nameof(viewModel.Property));

			viewModel.Property = new Tuple<int>(240);
			Assert.Equal(second, viewModel.Property);
			Assert.Same(second, viewModel.Property);
			CheckPropertyNames();
		}

		[Fact]
		public void OnlyUpdateBackingFieldAndInvokePropertyChangedEventWhenPropertyIsSetToNewValueButRemainSameWhenSetToOldValue()
		{
			Assert.Equal(new Tuple<int>(-2), viewModel.TryProperty);

			var first = new Tuple<int>(-2);
			viewModel.TryProperty = first;
			Assert.Equal(first, viewModel.TryProperty);
			Assert.NotSame(first, viewModel.TryProperty);
			CheckPropertyNames();
			Assert.Empty(viewModel.GetModifications());

			var second = new Tuple<int>(240);
			viewModel.TryProperty = second;
			Assert.Equal(second, viewModel.TryProperty);
			Assert.Same(second, viewModel.TryProperty);
			CheckPropertyNames(nameof(viewModel.TryProperty));
			Assert.Equal(new Tuple<int>[] { new Tuple<int>(240) }, viewModel.GetModifications());

			viewModel.TryProperty = new Tuple<int>(240);
			Assert.Equal(second, viewModel.TryProperty);
			Assert.Same(second, viewModel.TryProperty);
			CheckPropertyNames();
			Assert.Equal(new Tuple<int>[] { new Tuple<int>(240) }, viewModel.GetModifications());
		}

		[Fact]
		public void WhenPropertyValueChanges_ThenInvokePropertyChangedEvent()
		{
			var value = new Tuple<int>(240);
			viewModel.SetAutoImplementedProperty(value);
			Assert.Equal(value, viewModel.AutoImplementedProperty);
			Assert.Same(value, viewModel.AutoImplementedProperty);

			CheckPropertyNames(nameof(viewModel.AutoImplementedProperty));
		}

		[Fact]
		public void WhenThereAreNoSubscribersToThePropertyChangedEvent_ThenThereIsNoAttemptToRaiseThePropertyChangedEvent()
		{
			viewModel.PropertyChanged -= OnPropertyChanged;

			Assert.Equal(new Tuple<int>(-1), viewModel.Property);
			viewModel.Property = new Tuple<int>(240);
			Assert.Equal(new Tuple<int>(240), viewModel.Property);
			CheckPropertyNames();

			Assert.Equal(new Tuple<int>(-2), viewModel.TryProperty);
			viewModel.TryProperty = new Tuple<int>(240);
			Assert.Equal(new Tuple<int>(240), viewModel.TryProperty);
			Assert.Equal(new Tuple<int>[] { new Tuple<int>(240) }, viewModel.GetModifications());
			CheckPropertyNames();

			Assert.Equal(new Tuple<int>(-3), viewModel.AutoImplementedProperty);
			viewModel.SetAutoImplementedProperty(new Tuple<int>(240));
			Assert.Equal(new Tuple<int>(240), viewModel.AutoImplementedProperty);
			CheckPropertyNames();

			viewModel.PropertyChanged += OnPropertyChanged;
		}
	}
}
