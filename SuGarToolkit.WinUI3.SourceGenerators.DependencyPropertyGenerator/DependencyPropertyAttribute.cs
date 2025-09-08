using System;

namespace SuGarToolkit.WinUI3.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DependencyPropertyAttribute : Attribute
    {
        /// <summary>
        /// Constant default value.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Path.To.Target.Value
        /// </summary>
        public string DefaultValuePath { get; set; }

        /// <summary>
        /// nameof(DependencyPropertyChangedCallback)
        /// where DependencyPropertyChangedCallback is:
        /// <br/>
        /// delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        /// </summary>
        public string PropertyChanged { get; set; }
    }
}