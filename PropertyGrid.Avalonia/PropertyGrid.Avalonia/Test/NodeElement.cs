using System.ComponentModel;
using Common.Core;

namespace PropertyGrid.Avalonia.Test;

public class NodeElement : ObservableObject
{
    private string _description;
    private bool _isRequired;
    private string _name;

    public NodeElement(string name)
    {
        Name = name;
    }


    /// <summary>
    /// Описание узла.
    /// </summary>
    [Category("Общая часть")]
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Имя")]
    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    /// <summary>
    /// Описание узла.
    /// </summary>
    [Category("Общая часть")]
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Описание")]
    public string Description
    {
        get => _description;
        set => Set(ref _description, value);
    }

    /// <summary>
    /// Обязательный узел. 
    /// </summary>
    [Browsable(false)]
    public bool IsRequired
    {
        get => _isRequired;
        set => Set(ref _isRequired, value);
    }
}