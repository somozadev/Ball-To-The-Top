using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

namespace SVGP
{
    public class Command
    {
        public char CommandType { get; private set; }
        protected List<float> Parameters { get; private set; }
        private List<Vector2> Vectors;

        public Command(char commandType, string parameters)
        {
            CommandType = commandType;
            Parameters = ParseParameters(parameters);
            Vectors = ParseToVectors(Parameters);
            FixYSignForUnity();
        }
        public List<Vector2> ParseToSpriteShapeVectors(UnityEngine.Vector2 currentPos)
        {
            List<UnityEngine.Vector2> points = new List<UnityEngine.Vector2>();
            switch (CommandType)
            {
                case 'M': 
                case 'm': 
                    currentPos = Vectors[0];
                    points.Add(currentPos);
                    break;
                case 'L':
                case 'l':
                    currentPos = Vectors[0];
                    points.Add(currentPos);
                    break;
                case 'C':
                    UnityEngine.Vector2 leftTangentC =  (Vectors[0] - currentPos) * 0.5f;
                    UnityEngine.Vector2 rightTangentC =  (Vectors[1] - Vectors[2]) * 0.5f;
                    currentPos = Vectors[2];
                    points.Add(currentPos);
                    points.Add(leftTangentC);
                    points.Add(rightTangentC);
                    break;
                case 'c':
                    UnityEngine.Vector2 leftTangentc = currentPos + (Vectors[0] - currentPos) * 0.5f;
                    UnityEngine.Vector2 rightTangentc = Vectors[2] + (Vectors[1] - Vectors[2]) * 0.5f;
                    currentPos = Vectors[2];
                    points.Add(currentPos);
                    points.Add(leftTangentc);
                    points.Add(rightTangentc);
                break;  
            }
            return points;  

        }
        private List<UnityEngine.Vector2> ParseToVectors(List<float> parameters)
        {
            List<Vector2> vectors = new List<Vector2>();
            for (int i = 1; i < parameters.Count; i += 2)
            {
                vectors.Add(new Vector2(parameters[i-1], parameters[i]));
            }
            return vectors;
        }
        private List<float> ParseParameters(string parameters)
        {
            List<float> result = new List<float>();
            string[] parts = Regex.Split(parameters, @"(?<=[a-zA-Z])|(?<=[^\s])(?=[-\s])|(?<=[-\s])(?=[^\s-])");

            bool nextMinuts = false;
            foreach (string part in parts)
            {

                if (!string.IsNullOrWhiteSpace(part) && float.TryParse(part, out float value))
                {
                    if (nextMinuts) { value *= -1; }
                    result.Add(value);
                }
                if (string.Equals(part, "-"))
                    nextMinuts = true;
                else
                    nextMinuts = false;
            }
            return result;
        }
        private void FixYSignForUnity() //svg Y axis goes negative on up, unity Y axis goes negative on down
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = new UnityEngine.Vector2(Vectors[i].x, Vectors[i].y * -1);
            } 
        }
        public override string ToString()
        {
            string txt = "";
            foreach(Vector2 vec in Vectors)
               txt+= string.Join(", ","X->" + vec.x, "Y->" + vec.y) + " ";
            return $"{CommandType}: { txt }";
        }
        
    }
}
