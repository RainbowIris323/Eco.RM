using Eco.Core.Controller;
using System.ComponentModel;

namespace Eco.RM.Framework.UI;

[AutogenClass]
public class TextElement : IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, UITypeName("StringTitle")] public string Title { get; }
    [SyncToView, Autogen, Notify, UITypeName("StringDisplay")] public string Text { get; private set; }
    public void SetText(string text)
    {
        Text = text;
        this.Changed(nameof(Text));
    }

    public TextElement(string title, string text)
    {
        Title = title;
        Text = text;
    }

    #region IController
    int controllerID;
    public ref int ControllerID => ref controllerID;
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion
}