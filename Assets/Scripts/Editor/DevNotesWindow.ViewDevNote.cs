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
        [HideLabel]
        [MultiLineProperty(12)]
        [DisplayAsString(false)]
        public string notes;

        private readonly DevNote _devNote;
        public DevNote DevNote => _devNote;
        #endregion

        #region Behavior
        public ViewDevNote(DevNote note)
        {
            key = note.key;
            notes = note.notes;
            _devNote = note;
        }
        #endregion
    }
}