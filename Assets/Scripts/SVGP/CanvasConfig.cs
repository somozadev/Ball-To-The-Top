using UnityEngine;

namespace SVGP
{
    public partial class SVGParser
    {
        private struct CanvasConfig
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Format { get; set; }
            public ViewBoxHolder ViewBox { get; set; }
            public string Path { get; set; }

            public CanvasConfig(int width, int height, string format, ViewBoxHolder viewBox, string path)
            {
                Width = width;
                Height = height;
                Format = format;
                ViewBox = viewBox;
                Path = path;
            }
        }
    }
}
