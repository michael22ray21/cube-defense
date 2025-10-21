using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Notes", menuName = "Dev Notes/Create New Notes")]
public class DevNotes : ScriptableObject
{
    [Title("Note Information")]
    [LabelText("Note Name")]
    public string key = "New Note";

    [Space(10)]
    [Title("Content")]
    [HideLabel]
    [MultiLineProperty(15)]
    public string notes = "Enter your note here...";
}