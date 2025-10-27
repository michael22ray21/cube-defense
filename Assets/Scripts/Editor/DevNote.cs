using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Notes", menuName = "Dev Notes/Create New Note")]
public class DevNote : ScriptableObject
{
    #region Vars, Fields, Getters
    [Title("Note Information")]
    [LabelText("Note Key")]
    [ShowIf("_show")]
    public string key = "New Note";

    [Space(8)]
    [Title("Content")]
    [HideLabel]
    [ShowIf("_show")]
    public List<SubNote> subNote = new();

    private bool _show = false;
    public bool Show
    {
        get { return _show; }
        set { _show = value; }
    }
    #endregion
}