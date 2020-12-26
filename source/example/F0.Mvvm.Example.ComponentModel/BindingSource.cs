using System;
using F0.ComponentModel;

namespace F0.Mvvm.Example.ComponentModel
{
	internal class BindingSource<T> : ViewModel
	{
		public string Caption { get; }

		private T _value;
		public T Value
		{
			get => _value;
			set => HasChanged = TrySetProperty(ref _value, value);
		}

		private bool hasChanged;
		public bool HasChanged
		{
			get => hasChanged;
			private set => SetProperty(ref hasChanged, value);
		}

		internal BindingSource()
		{
		}

		internal BindingSource(string caption)
		{
			Caption = caption;
		}

		internal BindingSource(T value)
		{
			_value = value;
		}

		internal BindingSource(string caption, T value)
		{
			Caption = caption;
			_value = value;
		}

		internal void Mutate(T value)
		{
			Value = value;
		}
	}

	internal class BindingSource<TProperty, TMutator> : BindingSource<TProperty>
	{
		private readonly Func<TMutator, TProperty> mutator;

		internal BindingSource(Func<TMutator, TProperty> mutator)
			: base()
		{
			this.mutator = mutator;
		}

		internal BindingSource(string caption, Func<TMutator, TProperty> mutator)
			: base(caption)
		{
			this.mutator = mutator;
		}

		internal BindingSource(TProperty value, Func<TMutator, TProperty> mutator)
			: base(value)
		{
			this.mutator = mutator;
		}

		internal BindingSource(string caption, TProperty value, Func<TMutator, TProperty> mutator)
			: base(caption, value)
		{
			this.mutator = mutator;
		}

		internal void Mutate(TMutator value)
		{
			Value = mutator(value);
		}
	}
}
