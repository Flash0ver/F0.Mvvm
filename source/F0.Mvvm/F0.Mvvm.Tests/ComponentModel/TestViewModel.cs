using System;
using System.Collections.Generic;
using F0.ComponentModel;

namespace F0.Tests.ComponentModel
{
	internal sealed class TestViewModel : ViewModel
	{
		private Tuple<int> backingField;
		public Tuple<int> Property
		{
			get => backingField;
			set => SetField(ref backingField, value);
		}

		private Tuple<int> tryField;
		public Tuple<int> TryProperty
		{
			get => tryField;
			set
			{
				if (TrySetField(ref tryField, value))
				{
					modifications.Add(value);
				}
			}
		}

		public Tuple<int> AutoImplementedProperty { get; private set; }

		private readonly List<Tuple<int>> modifications = new List<Tuple<int>>();

		internal TestViewModel()
		{
			backingField = Tuple.Create(-1);
			tryField = Tuple.Create(-2);
			AutoImplementedProperty = Tuple.Create(-3);
		}

		internal void SetAutoImplementedProperty(Tuple<int> value)
		{
			AutoImplementedProperty = value;
			RaisePropertyChangedEvent(() => AutoImplementedProperty);
		}

		internal IReadOnlyList<Tuple<int>> GetModifications()
		{
			return modifications;
		}
	}
}
