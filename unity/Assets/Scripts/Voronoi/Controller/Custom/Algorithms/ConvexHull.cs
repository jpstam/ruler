using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util.Geometry;

namespace Voronoi.Custom.Algorithms
{
    public static class ConvexHull
    {
        public static List<Vertex> QuickHull(List<Vertex> points)
        {
            List<Vertex> hull = new List<Vertex>();

            //Find leftmost and rightmost point
            Vertex left = points.First();
            Vertex right = points.First();
            foreach (Vertex p in points)
            {
                if (p.X < left.X)
                {
                    left = p;
                }

                if (p.X > right.X)
                {
                    right = p;
                }
            }
            //Add to hull
            hull.Add(left);
            hull.Add(right);

            //line from leftmost to rightmost point
            Line lr = new Line(left.point, right.point);
            Line rl = new Line(right.point, left.point);

            //Remove points from remaining point list
            points.Remove(left);
            points.Remove(right);

            //Get sets of vertices left and right of l
            List<Vertex> leftVertices = new List<Vertex>();
            List<Vertex> rightVertices = new List<Vertex>();
            foreach (Vertex p in points)
            {
                if (lr.PointRightOfLine(p.point))
                {
                    rightVertices.Add(p);
                }
                else
                {
                    leftVertices.Add(p);
                }
            }

            // find hull
            hull.AddRange(FindHull(leftVertices, rl));
            hull.AddRange(FindHull(rightVertices, lr));

            // return hull
            return hull;
        }

        private static List<Vertex> FindHull(List<Vertex> points, Line line)
        {
            List<Vertex> pts = new List<Vertex>();
            if (points.Count == 0)
            {
                return pts;
            }
            
            //Find point with greatest distance
            Vertex dist = points.First();
            foreach (Vertex p in points)
            {
                if (line.DistanceToPoint(dist.point) < line.DistanceToPoint(p.point))
                {
                    dist = p;
                }
            }

            //Add most distant point to hull
            pts.Add(dist);

            //Remove most distant point from remaining points
            points.Remove(dist);

            //New lines of convex hull
            Line l1 = new Line(line.Point1, dist.point);
            Line l2 = new Line(dist.point, line.Point2);

            List<Vertex> p1 = new List<Vertex>();
            List<Vertex> p2 = new List<Vertex>();

            //Find points right of l1 and l2
            foreach (Vertex p in points)
            {
                if (l1.PointRightOfLine(p.point))
                {
                    p1.Add(p);
                }

                if (l2.PointRightOfLine(p.point))
                {
                    p2.Add(p);
                }
            }

            //Recursion, get extra points outside hull, that are on the hull
            pts.AddRange(FindHull(p1, l1));
            pts.AddRange(FindHull(p2, l2));

            //Return extra points on hull
            return pts;
        }
    }
}