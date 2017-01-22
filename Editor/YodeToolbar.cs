﻿using System.Collections;
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
        var window = GetWindow<YodeToolbar>();
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

        //pressed = (GUILayout.Toggle(pressed, "ToogleMe", "button",GUILayout.Height(30)));

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    //[MenuItem("GameObject/Create Empty Parent", false, 0)]
    public static void MakeParent()
    {
        var gameobjects = Selection.gameObjects;
        var newParent = new GameObject("GameObject1");
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