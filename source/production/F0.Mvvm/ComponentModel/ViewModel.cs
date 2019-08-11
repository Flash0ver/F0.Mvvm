using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace F0.ComponentModel
{
	public abstract class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected ViewModel()
		{
		}

		protected void SetField<TField>(ref TField field, TField value, [CallerMemberName] string propertyName = "")
		{
			if (!EqualityComparer<TField>.Default.Equals(field, value))
			{
				field = value;
				RaisePropertyChangedEvent(propertyName);
			}
		}

		protected bool TrySetField<TField>(ref TField field, TField value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<TField>.Default.Equals(field, value))
			{
				return false;
			}
			else
			{
				field = value;
				RaisePropertyChangedEvent(propertyName);
				return true;
			}
		}

		protected virtual void RaisePropertyChangedEvent<T>(Expression<Func<T>> propertyExpression)
		{
			string propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
			RaisePropertyChangedEvent(propertyName);
		}

		private void RaisePropertyChangedEvent(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
