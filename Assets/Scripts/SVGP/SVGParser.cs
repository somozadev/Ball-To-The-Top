using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;

namespace SVGP
{
    public partial class SVGParser
    {
        private CanvasConfig canvasConfig;
        private Regex regex = new Regex(@"([MLHVCSQTAZmlhvcsqtaz])([-+]?(?:\d+\.\d+|\.\d+|\d+\.?\d*)(?:[eE][-+]?\d+)?(?:\s*[-+]?(?:\d+\.\d+|\.\d+|\d+\.?\d*)(?:[eE][-+]?\d+)?)*)");
        private List<Command> Commands = new List<Command>();
        public void ParseSVG(string svg)
        {
            canvasConfig = ReadXML(svg);
            Commands = ParseSVGPath(canvasConfig.Path);
        }

        public List<List<Vector2>> GetSVGPoints()
        {
            Vector2 currentVector = Vector2.zero;
            List<List<Vector2>> vs = new List<List<Vector2>>();
            for (int i = 0; i < Commands.Count; i++)
            {
                List<Vector2> v = Commands[i].ParseToSpriteShapeVectors(currentVector);
                currentVector = v[0];
                vs.Add(v);
            }

            return vs;
        }
        public  void Print()
        {
            foreach (Command command in Commands)
            {
                Debug.Log(command);
            }
        }
        private  CanvasConfig ReadXML(string path)
        {
           // if (!string.IsNullOrEmpty(path)) return null;
            var doc = new XmlDocument();
            doc.Load(path);
           // if (doc.DocumentElement == null || doc.DocumentElement.Name != "svg") return null;
            int width = int.Parse(doc.DocumentElement.Attributes["width"].Value.Split(new[] { "in", "cm", "mm", "pt", "pc", "px", "%" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            int height = int.Parse(doc.DocumentElement.Attributes["height"].Value.Split(new[] { "in", "cm", "mm", "pt", "pc", "px", "%" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            string format = Regex.Replace(doc.DocumentElement.Attributes["width"].Value, @"[\d\s.]", "");
            string[] viewBox = doc.DocumentElement.Attributes["viewBox"].Value.Split(' ');
            string svgText="";

            var pathElements = doc.GetElementsByTagName("path");
            foreach (XmlNode pathElement in pathElements)
            {
                var dAttribute = pathElement.Attributes["d"];
                if (dAttribute == null) continue;
                var dContent = dAttribute.Value;
                svgText = dContent;
            }

            return new CanvasConfig(width, height, format, new ViewBoxHolder(int.Parse(viewBox[0]), int.Parse(viewBox[1]), int.Parse(viewBox[2]), int.Parse(viewBox[3])), svgText);

        }
        private  List<Command> ParseSVGPath(string svgpath)
        {
            List<Command> commands = new List<Command>();
            MatchCollection matches = regex.Matches(svgpath);
            foreach (Match match in matches)
            {
                char commandType = match.Value[0];
                string parameters = match.Value.Substring(1);
                Command command = new Command(commandType, parameters);
                commands.Add(command);
            }
            return commands;
        }

    }
}
