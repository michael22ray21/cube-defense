using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    [Serializable]
    public class ViewDevNote
    {
        #region Vars, Fields, Getters
        [Title("Note Information")]
        [LabelText("Note Key")]
        [DisplayAsString]
        public string key;

        [Space(8)]
        [Title("Content")]
        [HideLabel]
        [MultiLineProperty(12)]
        [DisplayAsString(false)]
        // public string notes;
        // public List<ViewElement> content;

        private readonly DevNote _devNote;
        public DevNote DevNote => _devNote;
        #endregion

        #region Behavior
        public ViewDevNote(DevNote note)
        {
            key = note.key;
            // notes = note.notes;
            _devNote = note;
        }
        #endregion

        #region Utilities
        // public void ParseSubNote()
        // {
        //     content = new List<ViewElement>();

        //     ViewElement view = new();
        //     foreach (SubNote subNote in _devNote.subNote)
        //     {
        //         switch (subNote.type)
        //         {
        //             case "Title":
        //                 view.Title = subNote.value;
        //                 break;
        //             case "Text":
        //                 view.text = subNote.value;
        //                 content.Add(view);
        //                 view = new();
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        // }
        #endregion
    }
}