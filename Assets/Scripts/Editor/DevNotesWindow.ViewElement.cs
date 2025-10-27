using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    public class ViewElement
    {
        #region Vars, Fields, Getters
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [Title("$_title")]
        [DisplayAsString(false)]
        public string text;
        #endregion
    }
}