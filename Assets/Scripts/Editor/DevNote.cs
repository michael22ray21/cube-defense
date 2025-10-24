using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Notes", menuName = "Dev Notes/Create New Note")]
public class DevNote : ScriptableObject
{
    #region Vars, Fields, Getters
    [Title("Note Information")]
    [LabelText("Note Key")]
    public string key = "New Note";

    [Space(8)]
    [Title("Content")]
    [HideIf("_viewMode")]
    public SubNote[] entries;

    [Space(8)]
    [HideLabel]
    [Title("Content")]
    [DisplayAsString()]
    [ShowIf("_viewMode")]
    public List<ViewSubNote> content;

    public bool _viewMode = false;
    public bool ViewMode
    {
        get { return _viewMode; }
        set { _viewMode = value; }
    }
    #endregion

    #region Utilities
    public void ParseEntries()
    {
        content = new List<ViewSubNote>();
        var view = new ViewSubNote();
        foreach (SubNote subNote in entries)
        {
            Debug.Log($"SubNote.Type = '{subNote._type}' SubNote.content = '{subNote._content}'");
            switch (subNote._type)
            {
                case "Title":
                    view.Title = subNote._content;
                    break;
                case "Text":
                    view.text = subNote._content;
                    content.Add(view);
                    view = new ViewSubNote();
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
}

[Serializable]
public class ViewSubNote
{
    #region Vars, Fields, Getters
    private string _title;
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    [Title("$_title")]
    [HideLabel]
    public string text;
    #endregion

    #region Behavior
    public ViewSubNote() { }

    public ViewSubNote(string title, string value)
    {
        _title = title;
        text = value;
    }
    #endregion
}

[Serializable]
public class SubNote
{
    #region Vars, Fields, Getters
    [ValueDropdown("Types")]
    public string _type = "Text";
    public string _content = "Enter your note here...";

    private static readonly string[] Types = new string[] { "Space", "Text", "Title", "Image" };
    #endregion

    #region Behavior
    public SubNote()
    {
        _type = "Text";
        _content = "Enter your note here...";
    }

    public SubNote(string type, string content)
    {
        _type = type;
        _content = content;
    }
    #endregion
}