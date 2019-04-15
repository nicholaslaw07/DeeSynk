using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Algorithms
{
    public struct Rectangle
    {
        public readonly int X0, Y0;
        public readonly int X1, Y1;

        private Point c00;
        public Point C00 { get => c00; }
        private Point c01;
        public Point C01 { get => c01; }
        private Point c10;
        public Point C10 { get => c10; }
        private Point c11;
        public Point C11 { get => c11; }

        public Point C00_NoLoc { get => new Point(X0, Y0); }
        public Point C01_NoLoc { get => new Point(X0, Y1); }
        public Point C10_NoLoc { get => new Point(X1, Y0); }
        public Point C11_NoLoc { get => new Point(X1, Y1); }

        public int X0_Base { get => 0; }
        public int Y0_Base { get => 0; }
        public Point XY0_Base { get => new Point(0, 0); }

        private int x1_base;
        public int X1_Base { get => x1_base; }
        private int y1_base;
        public int Y1_Base { get => y1_base; }
        private Point xy1_base;
        public Point XY1_Base { get => xy1_base; }

        private int w;
        public int Width  { get => X1 - X0 + 1; }
        private int h;
        public int Height { get => Y1 - Y0 + 1; }

        public int Area { get => Width * Height; }

        public Rectangle(int x0, int y0, int x1, int y1)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;

            c00 = new Point(X0, Y0);
            c01 = new Point(X0, Y1);
            c10 = new Point(X1, Y0);
            c11 = new Point(X1, Y1);

            x1_base = X1 - X0;
            y1_base = Y1 - Y0;
            xy1_base = new Point(X1 - X0, Y1 - Y0);

            w = x1_base + 1;
            h = y1_base + 1;
        }

        public Point GetCorner(int corner)
        {
            switch (corner)
            {
                case (0): return C00;
                case (1): return C01;
                case (2): return C10;
                case (3): return C11;
            }

            return new Point(0, 0);
        }

        public bool ContainsPoint(Point p)
        {
            return X0 <= p.X && p.X <= X1 && Y0 <= p.Y && p.Y <= Y1;
        }

        public bool IsReset()
        {
            return X0 == 0 && Y0 == 0;
        }

        public bool OutOfBounds()
        {
            return C00.OutOfBounds() || 
                   C01.OutOfBounds() || 
                   C10.OutOfBounds() || 
                   C11.OutOfBounds();
        }

        public bool IsEdgePoint(Point p)
        {
            if (p.X == X0 || p.X == X1 || p.Y == Y0 || p.Y == Y1)
                return true;
            return false;
        }

        public static Rectangle Reset(Rectangle r)
        {
            return new Rectangle(0, 0, r.X1_Base, r.Y1_Base);
        }

        public static Rectangle MoveTo(Rectangle r, Point p)
        {
            return new Rectangle(p.X, p.Y, p.X + r.X1_Base, p.Y + r.Y1_Base);
        }

        public static bool EitherContain(Rectangle rA, Rectangle rB)
        {
            int totalWidths = rA.w + rB.w;
            int xDiff = Math.Max(rB.X1 - rA.X0, rA.X1 - rB.X0) + 1;
            int totalHeights = rA.h + rB.h;
            int yDiff = Math.Max(rB.Y1 - rA.Y0, rA.Y1 - rB.Y0) + 1;
            return xDiff < totalWidths && yDiff < totalHeights;
        }

        public static Rectangle StapleTo(Rectangle r, AttachmentPoint aP)
        {
            Point corner = r.GetCorner(aP.Corner);
            return new Rectangle(aP.X - corner.X, aP.Y - corner.Y, r.X1_Base + aP.X - corner.X, r.Y1_Base + aP.Y - corner.Y);
        }

        public static Rectangle operator +(Rectangle r, Point p)
        {
            return new Rectangle(r.X0 + p.X, r.Y0 + p.Y, r.X1 + p.X, r.Y1 + p.Y);
        }

        public static Rectangle operator +(Point p, Rectangle r)
        {
            return new Rectangle(r.X0 + p.X, r.Y0 + p.Y, r.X1 + p.X, r.Y1 + p.Y);
        }

        public static Rectangle operator -(Rectangle r, Point p)
        {
            return new Rectangle(r.X0 - p.X, r.Y0 - p.Y, r.X1 - p.X, r.Y1 - p.Y);
        }

        public override string ToString()
        {
            return "(" + X0 + ", " + Y0 + ") (" + X1 + ", " + Y1 + ")";
        }
    }
    public struct Point
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool OutOfBounds()
        {
            return X < 0 || Y < 0;
        }


        public static Point Shift(Point p, int shiftState)
        {
            switch (shiftState)
            {
                case  (0): return new Point(p.X    , p.Y - 1);
                case  (1): return new Point(p.X - 1, p.Y - 1);
                case  (2): return new Point(p.X - 1, p.Y    );
                case  (3): return new Point(p.X - 1, p.Y    );
                case  (4): return new Point(p.X - 1, p.Y + 1);
                case  (5): return new Point(p.X    , p.Y + 1);
                case  (6): return new Point(p.X    , p.Y + 1);
                case  (7): return new Point(p.X + 1, p.Y + 1);
                case  (8): return new Point(p.X + 1, p.Y    );
                case  (9): return new Point(p.X + 1, p.Y    );
                case (10): return new Point(p.X + 1, p.Y - 1);
                case (11): return new Point(p.X    , p.Y - 1);
            }
            return new Point(0, 0);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
    public struct AttachmentPoint
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Corner;

        public Point Point { get => new Point(X, Y); }

        public AttachmentPoint(int x, int y, int corner)
        {
            X = x;
            Y = y;
            Corner = corner;
        }

        public static int InvertCorner(int corner)
        {
            return 3 - corner;
        }

        public override string ToString()
        {
            string cornerString = "";
            switch (Corner)
            {
                case (0): cornerString = "C00"; break;
                case (1): cornerString = "C01"; break;
                case (2): cornerString = "C10"; break;
                case (3): cornerString = "C11"; break;
            }
            return "(" + X + ", " + Y + ") " + cornerString; 
        }
    }
    public struct ConfigurationPoint
    {
        public readonly int Index;
        public readonly int X, Y;
        public ConfigurationPoint(int x, int y, int idx)
        {
            Index = idx;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ") " + Index;
        }
    }

    public class AlgorithmRectangleCompaction
    {
        private Rectangle[] rectangles;
        private int rectangleCount;
        private bool[] pinnedRectangles;
        private int[] pinDiff;
        private int pinDiffCount;

        private AttachmentPoint[] attachmentPoints;
        private int[] aPIndices;
        private int aPCount;
        private bool[] activeAPs;

        private ConfigurationPoint[] bestResult;
        private float bestScore;

        public AlgorithmRectangleCompaction(List<System.Drawing.Rectangle> rects)
        {
            rectangleCount = rects.Count();
            rectangles = new Rectangle[rectangleCount];
            pinDiff = new int[rectangleCount];
            pinDiffCount = 0;
            pinnedRectangles = new bool[rectangleCount];
            for (int idx = 0; idx < rectangleCount; idx++)
            {
                rectangles[idx] = new Rectangle(0, 0, rects[idx].Width - 1, rects[idx].Height - 1);
                pinnedRectangles[idx] = false;
                pinDiff[idx] = -1;
            }

            aPCount = 12 * rectangleCount;
            attachmentPoints = new AttachmentPoint[12 * rectangleCount];
            aPIndices = new int[aPCount];
            activeAPs = new bool[12 * rectangleCount];

            bestResult = new ConfigurationPoint[rectangleCount];
            bestScore = 0.0f;
        }

        public Rectangle[] FindBestConfiguration()
        {
            BestConfiguration();
            for (int idx = 0; idx < rectangleCount; idx++)
            {
                rectangles[idx] = Rectangle.Reset(rectangles[idx]);
                rectangles[idx] = Rectangle.MoveTo(rectangles[idx], new Point(bestResult[idx].X, bestResult[idx].Y));
            }

            return rectangles;
        }

        private void BestConfiguration()
        {

            if (rectangleCount > 4 && rectangleCount <= 8 && bestScore >= 0.9)
                return;

            else if (bestScore >= 0.6 && rectangleCount > 8)
                return;

            if (NumberPinned() == rectangleCount)
            {
                ScoreAndStore();
                return;
            }
            else
                BuildAttachmentPoints();

            for(int idx = 0; idx < rectangleCount; idx++)
            {
                if(!pinnedRectangles[idx])
                {
                    if (NumberPinned() == 0)
                    {
                        //sw.Start();
                        //Console.WriteLine(idx);


                        Pin(idx);
                        BestConfiguration();
                        UnPin(idx);


                        //sw.Stop();
                        //Console.WriteLine(sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        for(int jdx = 0; jdx < aPCount; jdx++)
                        {
                            if (activeAPs[jdx])
                            {
                                if(IsValidPinLocation(idx, jdx))
                                {
                                    Pin(idx, jdx);
                                    BestConfiguration();
                                    UnPin(idx);
                                }
                            }
                        }
                        //Recurse
                        //Go until there are no more attachable rectangles left
                        //Call score function
                        //If score is the best, then store the configuation

                        //Innevitably return the configuration
                    }
                }
            }
        }

        private void ScoreAndStore()
        {
            int maxX = 0;
            int maxY = 0;

            float usedArea = 0;

            for(int idx = 0; idx < rectangleCount; idx++)
            {
                if (pinnedRectangles[idx])
                {
                    usedArea += (float)rectangles[idx].Area;
                    if (rectangles[idx].X1 > maxX)
                        maxX = rectangles[idx].X1;
                    if (rectangles[idx].Y1 > maxY)
                        maxY = rectangles[idx].Y1;
                }
            }

            float totalArea = (float)maxX * (float)maxY;

            if(bestScore < usedArea / totalArea)
            {
                bestScore = usedArea / totalArea;
                for(int idx = 0; idx < rectangleCount; idx++)
                {
                    bestResult[idx] = new ConfigurationPoint(rectangles[idx].X0, rectangles[idx].Y0, idx);
                }
            }

        }

        private bool IsValidPinLocation(int rectIndex, int apIndex)
        {
            Rectangle testPin = Rectangle.StapleTo(rectangles[rectIndex], attachmentPoints[apIndex]);
            bool isValid = !testPin.OutOfBounds();
            for (int idx = 0; idx < rectangleCount; idx++)
            {
                if (!isValid)
                    break;
                if (pinnedRectangles[idx])
                    isValid &= !Rectangle.EitherContain(testPin, rectangles[idx]);
            }
            return isValid;
        }

        //C00-0  C01-1  C10-2  C11-3
        private void BuildAttachmentPoints()
        {
            /*for(int jdx=0; jdx < pinDiffCount - 1; jdx ++)
            {
                int idx = pinDiff[jdx];
                Rectangle currentRect = rectangles[idx];
                //C00
                {
                    Point c = currentRect.C00;
                    attachmentPoints[12 * idx + 0] = new AttachmentPoint(Point.Shift(c, 0).X, Point.Shift(c, 0).Y, 1);
                    attachmentPoints[12 * idx + 1] = new AttachmentPoint(Point.Shift(c, 1).X, Point.Shift(c, 1).Y, 3);
                    attachmentPoints[12 * idx + 2] = new AttachmentPoint(Point.Shift(c, 2).X, Point.Shift(c, 2).Y, 2);
                }
                //C01
                {
                    Point c = currentRect.C01;
                    attachmentPoints[12 * idx + 3] = new AttachmentPoint(Point.Shift(c, 3).X, Point.Shift(c, 3).Y, 3);
                    attachmentPoints[12 * idx + 4] = new AttachmentPoint(Point.Shift(c, 4).X, Point.Shift(c, 4).Y, 2);
                    attachmentPoints[12 * idx + 5] = new AttachmentPoint(Point.Shift(c, 5).X, Point.Shift(c, 5).Y, 0);
                }
                //C10
                {
                    Point c = currentRect.C11;
                    attachmentPoints[12 * idx + 6] = new AttachmentPoint(Point.Shift(c, 6).X, Point.Shift(c, 6).Y, 2);
                    attachmentPoints[12 * idx + 7] = new AttachmentPoint(Point.Shift(c, 7).X, Point.Shift(c, 7).Y, 0);
                    attachmentPoints[12 * idx + 8] = new AttachmentPoint(Point.Shift(c, 8).X, Point.Shift(c, 8).Y, 1);
                }
                //C11
                {
                    Point c = currentRect.C10;
                    attachmentPoints[12 * idx + 9] = new AttachmentPoint(Point.Shift(c, 9).X, Point.Shift(c, 9).Y, 0);
                    attachmentPoints[12 * idx + 10] = new AttachmentPoint(Point.Shift(c, 10).X, Point.Shift(c, 10).Y, 1);
                    attachmentPoints[12 * idx + 11] = new AttachmentPoint(Point.Shift(c, 11).X, Point.Shift(c, 11).Y, 3);
                }
            }*/
            for(int idx=0; idx < rectangleCount; idx++)
            {
                if (pinnedRectangles[idx])
                {
                    Rectangle currentRect = rectangles[idx];
                    //C00
                    {
                        Point c = currentRect.C00;
                        attachmentPoints[12 * idx + 0] = new AttachmentPoint(Point.Shift(c, 0).X, Point.Shift(c, 0).Y, 1);
                        attachmentPoints[12 * idx + 1] = new AttachmentPoint(Point.Shift(c, 1).X, Point.Shift(c, 1).Y, 3);
                        attachmentPoints[12 * idx + 2] = new AttachmentPoint(Point.Shift(c, 2).X, Point.Shift(c, 2).Y, 2);
                    }
                    //C01
                    {
                        Point c = currentRect.C01;
                        attachmentPoints[12 * idx + 3] = new AttachmentPoint(Point.Shift(c, 3).X, Point.Shift(c, 3).Y, 3);
                        attachmentPoints[12 * idx + 4] = new AttachmentPoint(Point.Shift(c, 4).X, Point.Shift(c, 4).Y, 2);
                        attachmentPoints[12 * idx + 5] = new AttachmentPoint(Point.Shift(c, 5).X, Point.Shift(c, 5).Y, 0);
                    }
                    //C10
                    {
                        Point c = currentRect.C11;
                        attachmentPoints[12 * idx + 6] = new AttachmentPoint(Point.Shift(c, 6).X, Point.Shift(c, 6).Y, 2);
                        attachmentPoints[12 * idx + 7] = new AttachmentPoint(Point.Shift(c, 7).X, Point.Shift(c, 7).Y, 0);
                        attachmentPoints[12 * idx + 8] = new AttachmentPoint(Point.Shift(c, 8).X, Point.Shift(c, 8).Y, 1);
                    }
                    //C11
                    {
                        Point c = currentRect.C10;
                        attachmentPoints[12 * idx + 9] = new AttachmentPoint(Point.Shift(c, 9).X, Point.Shift(c, 9).Y, 0);
                        attachmentPoints[12 * idx + 10] = new AttachmentPoint(Point.Shift(c, 10).X, Point.Shift(c, 10).Y, 1);
                        attachmentPoints[12 * idx + 11] = new AttachmentPoint(Point.Shift(c, 11).X, Point.Shift(c, 11).Y, 3);
                    }
                }

                for(int jdx = 0; jdx < 12; jdx++)
                {
                    for(int kdx = 0; kdx < rectangleCount; kdx++)
                    {
                        if (pinnedRectangles[kdx])
                            activeAPs[12 * idx + jdx] = (!rectangles[kdx].ContainsPoint(attachmentPoints[12 * idx + jdx].Point)) || (!attachmentPoints[12 * idx + jdx].Point.OutOfBounds());
                    }
                }
            }

            for (int idx=0; idx < aPCount; idx++)
            {
                if (activeAPs[idx])
                {
                    if (attachmentPoints[idx].Point.X < 0 || attachmentPoints[idx].Point.Y < 0)
                    {
                        activeAPs[idx] = false;
                        continue;
                    }

                    bool isEntirelyBlocked = true;
                    for(int jdx=0; jdx < rectangleCount; jdx++)
                    {
                        if (pinnedRectangles[jdx])
                        {
                            if (rectangles[jdx].ContainsPoint(attachmentPoints[idx].Point))
                            {
                                activeAPs[idx] = false;
                                continue;
                            }
                        }
                        else
                        {
                            Rectangle testPin = Rectangle.StapleTo(rectangles[jdx], attachmentPoints[idx]);
                            isEntirelyBlocked &= testPin.OutOfBounds();
                            for (int kdx=0; kdx < rectangleCount; kdx++)
                            {
                                if (pinnedRectangles[kdx])
                                {
                                    isEntirelyBlocked &= Rectangle.EitherContain(rectangles[kdx], testPin);
                                }

                                if (!isEntirelyBlocked)
                                    break;
                            }
                        }
                    }

                    if (isEntirelyBlocked)
                        activeAPs[idx] = false;
                }
            }
        }

        private int NumberPinned()
        {
            int count = 0;
            for (int idx = 0; idx < rectangleCount; idx++)
                count += (pinnedRectangles[idx]) ? 1 : 0;
            return count;
        }

        private void Pin(int idx)
        {
            pinnedRectangles[idx] = true;
            pinDiff[pinDiffCount] = idx;
            pinDiffCount++;
        }

        private void Pin(int rectIndex, int apIndex)
        {
            rectangles[rectIndex] = Rectangle.StapleTo(rectangles[rectIndex], attachmentPoints[apIndex]);
            pinnedRectangles[rectIndex] = true;
            pinDiff[pinDiffCount] = rectIndex;
            pinDiffCount++;
        }

        private void UnPin(int idx)
        {
            rectangles[idx] = Rectangle.Reset(rectangles[idx]);
            pinnedRectangles[idx] = false;
            pinDiff[--pinDiffCount] = -1;
            for(int jdx = 0; jdx < 12; jdx++)
            {
                activeAPs[12 * idx + jdx] = false;
            }
        }


    }
}
