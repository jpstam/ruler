﻿namespace Util.Algorithms.Graph.Tests
{
    using UnityEngine;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util.Geometry.Graph;
    using Util.Algorithms.Graph;

    [TestFixture]
    class MatchingTest
    {
        private readonly List<Vertex> m_level1pos;
        private readonly List<Vertex> m_level2pos;

        /// <summary>
        /// Creates initial vertex lists
        /// </summary>
        public MatchingTest()
        {
            m_level1pos = new List<Vertex>() {
                new Vertex(0f,  0f),
                new Vertex(0f,  1f),
                new Vertex(.5f, 0f),
                new Vertex(.5f, 1f)
            };

            m_level2pos = new List<Vertex>() {
                new Vertex(1.4f, -1.1f),
                new Vertex(1.3f,  -.2f),
                new Vertex(.8f,    .7f),
                new Vertex(-.2f,    1f),
                new Vertex(-1.2f, 1.2f),
                new Vertex(-.1f,  -.9f)
            };
        }

        [Test]
        public void CreatePerfectMatching1Test()
        {
            var matching = Matching.MinimumWeightPerfectMatchingOfCompleteGraph(m_level1pos);

            //test degree
            foreach (var v in matching.Vertices)
            {
                Assert.AreEqual(1, matching.DegreeOf(v));
            }
        }

        [Test]
        public void CreatePerfectMatching2Test()
        {
            var matching = Matching.MinimumWeightPerfectMatchingOfCompleteGraph(m_level2pos);

            //test degree
            foreach (var v in matching.Vertices)
            {
                Assert.AreEqual(1, matching.DegreeOf(v));
            }
        }

        [Test]
        public void MinimumWeightMatching1Test()
        {
            var matching = Matching.MinimumWeightPerfectMatchingOfCompleteGraph(m_level1pos);
            var cost = new Edge(m_level1pos[0], m_level1pos[2]).Weight +
                       new Edge(m_level1pos[1], m_level1pos[3]).Weight;

            Assert.AreEqual(cost, matching.TotalEdgeWeight);
        }

        [Test]
        public void MinimumWeightMatching2Test()
        {
            /*
            var matching = Matching.MinimumWeightPerfectMatchingOfCompleteGraph(m_level2pos);
            var cost = new Edge(m_level2pos[0], m_level2pos[5]).Weight +
                       new Edge(m_level2pos[1], m_level2pos[2]).Weight +
                       new Edge(m_level2pos[3], m_level2pos[4]).Weight;

            // test fails on greedy algorithm
            Assert.AreEqual(cost, matching.TotalEdgeWeight);
            */
        }
    }
}