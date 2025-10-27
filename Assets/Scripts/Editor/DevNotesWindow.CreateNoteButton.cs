using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

// Helper class to create a new note

public partial class DevNotesWindow : OdinMenuEditorWindow
{
    public class CreateNoteButton
    {
        private readonly DevNotesWindow window;
        public string path;
        public string key = "New Note";

        public CreateNoteButton(DevNotesWindow window)
        {
            this.window = window;
        }

        [Button("Create New Note", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void CreateNote()
        {
            DevNote newNote = CreateInstance<DevNote>();
            newNote.key = key;

            path = path.Trim('/');
            if (path.Length != 0) path = "/" + path;
            else path = "";
            path = $"{NOTES_PATH}{path}/{key}.asset";
            CheckPathExists(path);
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(newNote, uniquePath);
            AssetDatabase.SaveAssets();

            window.ForceMenuTreeRebuild();

            // Select the newly created note
            window.TrySelectMenuItemWithObject(newNote);
        }

        private void CheckPathExists(string path)
        {
            // Check the folder path
            if (!AssetDatabase.IsValidFolder(path))
            {
                // if not found, then create the directory procedurally
                string[] folders = path.Split('/');
                string currentPath = folders[0];

                for (int i = 1; i < folders.Length; i++)
                {
                    string newPath = currentPath + "/" + folders[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, folders[i]);
                    }
                    currentPath = newPath;
                }
                AssetDatabase.Refresh();
            }
        }
    }
}