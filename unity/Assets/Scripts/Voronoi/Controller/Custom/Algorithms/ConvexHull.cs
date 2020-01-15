using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util.Geometry;

namespace Voronoi.Custom.Algorithms
{
    public static class CustomConvexHull
    {
        public static List<Vector2> QuickHull(List<Vector2> points)
        {
            List<Vector2> pts = new List<Vector2>(points);
            List<Vector2> hull = new List<Vector2>();

            if (pts.Count == 0)
            {
                return hull;
            }

            //Find leftmost and rightmost point
            Vector2 left = pts.First();
            Vector2 right = pts.First();
            foreach (Vector2 p in pts)
            {
                if (p.x < left.x)
                {
                    left = p;
                }

                if (p.x > right.x)
                {
                    right = p;
                }
            }

            //line from leftmost to rightmost point
            Line lr = new Line(left, right);
            Line rl = new Line(right, left);

            //Remove points from remaining point list
            pts.Remove(left);
            pts.Remove(right);

            //Get sets of vertices left and right of l
            List<Vector2> leftVertices = new List<Vector2>();
            List<Vector2> rightVertices = new List<Vector2>();
            foreach (Vector2 p in pts)
            {
                if (lr.PointRightOfLine(p))
                {
                    rightVertices.Add(p);
                }
                else
                {
                    leftVertices.Add(p);
                }
            }

            // construct hull

            //Left point
            hull.Add(left);

            //Points between left and right
            hull.AddRange(FindHull(rightVertices, lr));

            //Right point
            hull.Add(right);

            //Points between right and left
            hull.AddRange(FindHull(leftVertices, rl));
            
            // return hull
            return hull;
        }

        private static List<Vector2> FindHull(List<Vector2> points, Line line)
        {
            List<Vector2> pts = new List<Vector2>(points);
            List<Vector2> hullPts = new List<Vector2>();

            if (pts.Count == 0)
            {
                return hullPts;
            }
            
            //Find point with greatest distance
            Vector2 dist = pts.First();
            foreach (Vector2 p in pts)
            {
                if (line.DistanceToPoint(dist) < line.DistanceToPoint(p))
                {
                    dist = p;
                }
            }

            //Remove most distant point from remaining points
            pts.Remove(dist);

            //New lines of convex hull
            Line l1 = new Line(line.Point1, dist);
            Line l2 = new Line(dist, line.Point2);

            List<Vector2> p1 = new List<Vector2>();
            List<Vector2> p2 = new List<Vector2>();

            //Find points right of l1 and l2
            foreach (Vector2 p in pts)
            {
                if (l1.PointRightOfLine(p))
                {
                    p1.Add(p);
                }

                if (l2.PointRightOfLine(p))
                {
                    p2.Add(p);
                }
            }

            //Construct hull

            //Points between A and most distant point
            hullPts.AddRange(FindHull(p1, l1));
            //Add most distant point to hull
            hullPts.Add(dist);
            //Points between most distant point and B
            hullPts.AddRange(FindHull(p2, l2));

            //Return extra points on hull
            return hullPts;
        }
    }
}