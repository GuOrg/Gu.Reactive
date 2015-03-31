﻿namespace Gu.Reactive
{
    using System.ComponentModel;

    public class PropertyChangedAndValueEventArgs<TProperty> : PropertyChangedEventArgs
    {
        public PropertyChangedAndValueEventArgs(string propertyName, TProperty value, bool isDefaultValue)
            : base(propertyName)
        {
            Value = value;
            IsDefaultValue = isDefaultValue;
        }

        public TProperty Value { get; private set; }

        /// <summary>
        /// Use this to check if the returned value is a default value or read from source
        /// </summary>
        public bool IsDefaultValue { get; private set; }
    }
}