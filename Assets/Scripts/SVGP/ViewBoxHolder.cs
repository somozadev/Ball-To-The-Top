 namespace SVGP
{
    public partial class SVGParser
    {
        public struct ViewBoxHolder
        {
            public int MinX{ get; private set; }
            public int MinY{ get; private set; }
            public int X{ get; private set; }
            public int Y{ get; private set; }

            public ViewBoxHolder(int minX, int minY, int x, int y)
            {
                MinX = minX;
                MinY = minY;
                X = x;
                Y = y;
            }
        }
    }
}
