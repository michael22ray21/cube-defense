using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Notes", menuName = "Dev Notes/Create New Note")]
public class DevNote : ScriptableObject
{
    #region Vars, Fields, Getters
    [Title("Note Information")]
    [LabelText("Note Name")]
    public string key = "New Note";

    [Space(8)]
    [Title("Content")]
    [HideLabel]
    [MultiLineProperty(12)]
    public string notes = "Enter your note here...";
    #endregion
}