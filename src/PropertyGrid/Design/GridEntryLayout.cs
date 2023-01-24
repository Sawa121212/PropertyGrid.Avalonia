﻿using Avalonia.Controls;
using Avalonia.Controls.Generators;

namespace PropertyGrid.Design
{
    /// <summary>
    /// Defines a basement for GridEntry UI layouts (panels, lists, etc)
    /// </summary>
    /// <typeparam name="T">The type of elements in the control.</typeparam>
    
    public abstract class GridEntryLayout<T> : ItemsControl where T : GridEntryContainer, new()
    {
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            var result = new GridEntryLayoutContainer<T>(this);
            return result;
        }
    }
}