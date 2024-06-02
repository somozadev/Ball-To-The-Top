using System;
using System.Xml;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using System.Collections.Generic;
using System.IO;
using SVGP;
using System.Text.RegularExpressions;

public class SVGToSpriteShapeWindow : EditorWindow
{
    string shapeName = "NewSpriteShape";
    string svgText = "";
    SpriteShape profile;
    private SVGParser parser;

    [MenuItem("Tools/ImageToSpriteShape")]
    static void Init()
    {
        SVGToSpriteShapeWindow window = GetWindow<SVGToSpriteShapeWindow>();
        window.titleContent = new GUIContent("SVG to SpriteShape");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name:", GUILayout.Width(80));
        shapeName = EditorGUILayout.TextField(shapeName);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Select an SVG file:", EditorStyles.boldLabel);
        if (GUILayout.Button("Load SVG File"))
            LoadSvgFile();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Select a profile:", EditorStyles.boldLabel);
        profile = (SpriteShape)EditorGUILayout.ObjectField(profile, typeof(SpriteShape), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate SpriteShape"))
        {
            GenerateSpriteShape();
        }
    }

    void LoadSvgFile()
    {
        var path = EditorUtility.OpenFilePanel("Load SVG File", "", "svg");
        if (string.IsNullOrEmpty(path)) return;
        parser = new SVGParser();
        parser.ParseSVG(path);
        parser.Print();
        // var doc = new XmlDocument();
        // doc.Load(path);
        // var pathElements = doc.GetElementsByTagName("path");
        //
        // foreach (XmlNode pathElement in pathElements)
        // {
        //     var dAttribute = pathElement.Attributes["d"];
        //     if (dAttribute == null) continue;
        //     var dContent = dAttribute.Value;
        //     svgText = dContent;
        // }
        //Debug.Log(svgText + "\n"); //full d= content
        // parser.Print();
    }

    private void GenerateSpriteShape()
    {
        // if (string.IsNullOrEmpty(svgText))
        // {
        //     Debug.LogError("No SVG content loaded.");
        //     return;
        // }

        List<Vector2> points = new List<Vector2>();
        string typesRegex = "[A-Za-z]";
        string[] commands = Regex.Split(svgText, typesRegex);
        foreach (var c in commands)
        {
            Debug.Log(c);
        }

        for (int i = 1; i < commands.Length - 1; i++)
        {
            string[] commandSplit = commands[i].Split(' ');
            if (commandSplit.Length % 2 != 0) continue;
            for (int j = 0; j < commandSplit.Length; j += 2)
            {
                var x = float.Parse(commandSplit[j]);
                var y = float.Parse(commandSplit[j + 1]);
                points.Add(new Vector2((x / 1000), (y / 1000)));
            }
            // if (i == 0) //Estamos dibujando una M
            // {
            // }
            //
            // else if (i == commands.Length - 1) //estamos dibujando una Z 
            // {
            //     
            // }
            // else //tenemos un comando de longitud X que hay que analizar, pero estamos dibujando una L o C o lo que sea, pero se tratara como L solo de momento 
            // {
            //     string[] commandSplit = commands[i].Split(' ');
            //     for (int j = 0; j < commandSplit.Length; j+=2)
            //     {
            //         var x = float.Parse(commandSplit[i]);
            //         var y = float.Parse(commandSplit[i + 1]);
            //         points.Add(new Vector2(x,y));
            //     }
            //
            // }
        }

        Debug.Log(svgText);
        CreateSpriteShape(points);
    }

    private void CreateSpriteShape(List<Vector2> points)
    {
        if (profile == null)
        {
            Debug.LogError("No SpriteShape profile selected.");
            return;
        }

        string savePath = $"Assets/SpriteShapes/{shapeName}.prefab";
        GameObject prefab = new GameObject(shapeName);
        SpriteShapeController spriteShapeController = prefab.AddComponent<SpriteShapeController>();
        spriteShapeController.spline.Clear();
        spriteShapeController.spriteShape = profile;

        // Agregar los puntos al SpriteShape
        spriteShapeController.spline.Clear();
        List<List<Vector2>> vecs = parser.GetSVGPoints();
        int j = 0;
        for (int i = 0; i < vecs.Count; i++)
        {
            foreach (var v in vecs[i])
            {
                spriteShapeController.spline.InsertPointAt(j, v);
                j++;
            }
            // spriteShapeController.spline.InsertPointAt(i, points[i]);
        }

        spriteShapeController.BakeCollider();
        spriteShapeController.BakeMesh();
        PrefabUtility.SaveAsPrefabAsset(prefab, savePath);
        DestroyImmediate(prefab);
    }
}