using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    public class ViewDevNote
    {
        #region Vars, Fields, Getters
        [Title("Note Information")]
        [LabelText("Note Name")]
        [DisplayAsString]
        public string key;

        [Space(8)]
        [Title("Content")]
        [DisplayAsString(false)]
        public SubNote[] entries;

        private readonly DevNote _devNote;
        // private bool viewMode = false;
        public DevNote DevNote => _devNote;
        #endregion

        #region Behavior
        public ViewDevNote(DevNote note)
        {
            key = note.key;
            entries = note.entries;
            _devNote = note;
        }
        #endregion
    }
}