using UnityEditor;
using UnityEngine;

namespace GistImporter
{
    public class GistImporterWindow : EditorWindow
    {
        string gistUrl = string.Empty;
        string destinationFolder = string.Empty;
        public bool ChooseFolder = true;

        [MenuItem("Tools/Gist Importer")]
        public static void OpenGistImporter()
        {
            GetWindow<GistImporterWindow>("Gist Importer");

        }


        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            gistUrl = EditorGUILayout.TextField("Gist URL", gistUrl);

            EditorGUILayout.Space(10);
            ChooseFolder = EditorGUILayout.Toggle("Choose Folder", ChooseFolder);

            if (ChooseFolder)
            {

                //
                GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
                gUIStyle.padding = new RectOffset(20, 0, 0, 0);
                gUIStyle.fontStyle = FontStyle.Italic;
                EditorGUILayout.LabelField("Try drag and drop a folder.", gUIStyle);

                destinationFolder = EditorGUILayout.TextField("Destination folder:", destinationFolder);

                if (mouseOverWindow == this)
                {
                    if (DragAndDrop.paths.Length > 0)
                    {
                        destinationFolder = DragAndDrop.paths[0];
                        destinationFolder = destinationFolder.Substring("Assets/".Length);
                    }
                }

            }
            else
            {
                destinationFolder = "";

            }




            EditorGUILayout.Space(10);

            if (GUILayout.Button("Import"))
            {
                if (GistImporter.ValidateGistUrl(gistUrl))
                {
                    GistImporter.ImportGist(gistUrl, destinationFolder);

                }
            }



        }


    }
}