using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Notes", menuName = "Create New Notes")]
public class Notes : SerializedScriptableObject
{
    #region Vars, Fields, Getters
    [SerializeField] private Dictionary<string, NoteContainer> notesList = new();

    [Title("Add New Notes")]
    // Key area
    [SerializeField]
    [OnValueChanged("LoadNotesToEdit")]
    private string key;

    // Notes area
    [TextArea(10, 20)]
    [SerializeField]
    [OnValueChanged("AddNotes")]
    private string notes = "Your notes here...";
    #endregion

    #region Utilities
    // save notes button
    [GUIColor(0.6f, 1f, 0.6f)]
    [ButtonGroup("Save Notes")]
    private void AddNotes()
    {
        SaveNotes(key, notes);
    }

    public NoteContainer GetNotes(string key)
    {
        notesList.TryGetValue(key, out NoteContainer notes);
        return notes;
    }

    public void SaveNotes(string key, string note)
    {
        notesList.TryGetValue(key, out NoteContainer notes);
        if (notes == null)
            notesList.Add(key, new NoteContainer(key, note));
        else
            notes.notes = note;
    }

    private void LoadNotesToEdit()
    {
        if (notesList.TryGetValue(key, out NoteContainer notes))
            this.notes = notes.notes;
    }

    [SerializeField]
    public class NoteContainer
    {
        public NoteContainer(string key, string notes)
        {
            this.key = key;
            this.notes = notes;
        }

        public string key;
        [TextArea]
        public string notes;
    }
    #endregion
}
