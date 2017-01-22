using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;


public class YodeToolbar : EditorWindow
{
    [MenuItem("Window/YodeToolBar")]
    public static void ShowMyAwesomeExtention()
    {
        GetWindow<YodeToolbar>();
    }

    private void OnEnable()
    {
        titleContent.image = EditorGUIUtility.Load("Assets/YodeToolBar/Icons/Title.png") as Texture2D;
        titleContent.text = "YodeToolBar";
        minSize = new Vector2(500, 32);
    }

    //private bool pressed = true;
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        //  Make new button
        if (
            GUILayout.Button(new GUIContent(
                EditorGUIUtility.Load("Assets/YodeToolBar/Icons/Hierarchy.png") as Texture2D,
                "Create Empty Parent"), GUILayout.Height(28), GUILayout.Width(32)))
        {
            MakeParent();
        }
        //GUILayout.Space(5);
        var deselectGuiContent = new GUIContent(
            EditorGUIUtility.Load("Assets/YodeToolBar/Icons/Deselect.png") as Texture2D,
            "Deselect All");
        if (GUILayout.Button(deselectGuiContent,
            GUILayout.Height(28),
            GUILayout.Width(32)))
        {
            DeselectAll();
        }

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        var triGuiContent = new GUIContent(string.Format("Tris: {0}", GetTrisCount()),
            EditorGUIUtility.Load("Assets/YodeToolBar/Icons/Tris.png") as Texture2D,
            "Number of triangles in selected object");
        GUILayout.Label(triGuiContent, GUILayout.Height(16));
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        var vertGuiContent = new GUIContent(string.Format("Verts: {0}", GetVertsCount()),
            EditorGUIUtility.Load("Assets/YodeToolBar/Icons/Vertices.png") as Texture2D,
            "Number of vertices in selected object");
        GUILayout.Label(vertGuiContent,GUILayout.Height(16));
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();


        GUILayout.Space(10);
        GUILayout.EndHorizontal();
    }

    private string GetTrisCount()
    {

        if (Selection.activeGameObject != null)
        {
            var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                return string.Format("{0}", meshFilter.sharedMesh.triangles.Length);
            }
        }
        return "N/A";
    }

    private string GetVertsCount()
    {

        if (Selection.activeGameObject != null)
        {
            var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                return string.Format("{0}", meshFilter.sharedMesh.vertexCount);
            }
        }
        return "N/A";
    }

    [MenuItem("GameObject/Deselect All &f", true, 1000)]
    private static bool ValidateDeselectAll()
    {
        return Selection.activeGameObject != null;
    }

    [MenuItem("GameObject/Deselect All &f", false, 1000)]
    private static void DeselectAll()
    {
        Selection.activeGameObject = null;
    }

    //[MenuItem("GameObject/Create Empty Parent", false, 0)]
    public static void MakeParent()
    {
        var gameobjects = Selection.gameObjects;
        var newParent = new GameObject("GameObject");
        Undo.RegisterCreatedObjectUndo(newParent, "Created parent");

        //  Select the topmost transform and copy parent value
        newParent.transform.parent = gameobjects.Select((go) => new
        {
            depth = CalculateTransformDepth(go.transform),
            go
        }).Aggregate((arg1, arg2) => arg1.depth < arg2.depth ? arg1 : arg2).go.transform.parent;

        //  Place in the center of the gameobjects
        newParent.transform.position =
            gameobjects.Select((gameobject) => gameobject.transform.position)
                .Aggregate((vector1, vector2) => vector1 + vector2)/gameobjects.Length;

        //  Move GameObjects to the new transform
        foreach (var gameobject in gameobjects)
        {
            Undo.SetTransformParent(gameobject.transform, newParent.transform, "Set new parent");
        }

        Selection.activeGameObject = newParent;
        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }

    private static int CalculateTransformDepth(Transform transform)
    {
        if (transform == transform.root)
            return 0;
        return CalculateTransformDepth(transform.parent) + 1;
    }
}