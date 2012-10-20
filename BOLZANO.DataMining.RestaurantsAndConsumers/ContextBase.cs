using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public abstract class ContextBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = new Dictionary<string, PropertyChangedEventArgs>();

        protected virtual void RaisePropertyChange<T>(Expression<Func<T>> propertySelector)
        {
            var myName = GetPropertyName(propertySelector);
            if (!string.IsNullOrEmpty(myName))
                NotifyChange(myName);
        }

        protected virtual void NotifyChange(string propertyName)
        {
            if (_argsCache != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChange(_argsCache[propertyName]);
            }
        }

        private void NotifyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        protected string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
