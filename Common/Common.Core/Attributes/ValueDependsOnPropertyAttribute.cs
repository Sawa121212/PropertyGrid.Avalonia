using System;

namespace Common.Avalonia.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ValueDependsOnPropertyAttribute : Attribute
    {
        public ValueDependsOnPropertyAttribute(string sourcePropertyName)
        {
            SourceProperName = sourcePropertyName;
        }

        internal string SourceProperName { get; }
    }
}