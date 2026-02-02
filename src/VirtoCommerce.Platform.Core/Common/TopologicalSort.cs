using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Platform.Core.Common
{
    public static class TopologicalSort
    {
        /// <summary>
        /// Topological Sorting (Kahn's algorithm) 
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Topological_sorting</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes">All nodes of directed acyclic graph.</param>
        /// <param name="edges">All edges of directed acyclic graph.</param>
        /// <returns>Sorted node in topological order.</returns>
        public static List<T> Sort<T>(HashSet<T> nodes, HashSet<Tuple<T, T>> edges) where T : IEquatable<T>
        {
            // Empty list that will contain the sorted elements
            var L = new List<T>();

            // Set of all nodes with no incoming edges
            var S = new HashSet<T>(nodes.Where(n => edges.All(e => !e.Item2.Equals(n))));

            // Build a lookup dictionary for edges by source node to avoid O(n²) searches
            // Use HashSet for O(1) removal operations
            var edgesBySource = edges.GroupBy(e => e.Item1)
                                     .ToDictionary(g => g.Key, g => new HashSet<Tuple<T, T>>(g));

            // while S is non-empty do
            while (S.Any())
            {

                //  remove a node n from S
                var n = S.First();
                S.Remove(n);

                // add n to tail of L
                L.Add(n);

                // for each node m with an edge e from n to m do
                if (edgesBySource.TryGetValue(n, out var nodeEdges))
                {
                    foreach (var e in nodeEdges.ToList())
                    {
                        var m = e.Item2;

                        // remove edge e from the graph
                        edges.Remove(e);
                        nodeEdges.Remove(e);

                        // if m has no other incoming edges then
                        if (edges.All(me => !me.Item2.Equals(m)))
                        {
                            // insert m into S
                            S.Add(m);
                        }
                    }
                }
            }

            // if graph has edges then
            if (edges.Any())
            {
                // return error (graph has at least one cycle)
                return null;
            }
            else
            {
                // return L (a topologically sorted order)
                return L;
            }
        }
    }
}
