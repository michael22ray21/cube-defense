using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    public class CreateNoteButton
    {
        private readonly DevNotesWindow window;

        public CreateNoteButton(DevNotesWindow window)
        {
            this.window = window;
        }

        [Button("Create New Note", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void CreateNote()
        {
            DevNote newNote = CreateInstance<DevNote>();
            newNote.key = "New Note";

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(
                $"{NOTES_PATH}/New Note.asset");

            AssetDatabase.CreateAsset(newNote, uniquePath);
            AssetDatabase.SaveAssets();

            window.ForceMenuTreeRebuild();

            // Select the newly created note
            window.TrySelectMenuItemWithObject(newNote);
        }
    }
}