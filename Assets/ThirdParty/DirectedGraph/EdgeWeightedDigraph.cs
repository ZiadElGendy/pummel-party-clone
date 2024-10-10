using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programming.Graphs
{
    public class EdgeWeightedDigraph
    {
        private readonly int _v;
        private int _e;
        public LinkedList<DirectedEdge>[] adj;

        public EdgeWeightedDigraph(int V)
        {
            this._v = V;
            this._e = 0;

            adj = new LinkedList<DirectedEdge>[V];

            for (int v = 0; v < _v; v++)
            {
                adj[v] = new LinkedList<DirectedEdge>();
            }
        }

        public int V()
        {
            return _v;
        }

        public int E()
        {
            return _e;
        }

        public void AddEdge(DirectedEdge e)
        {
            int v = e.From();
            int w = e.To();

            bool ExistsInV = adj[v].Contains(e);

            if ((!ExistsInV))
            {
                adj[e.From()].AddFirst(e);
                _e++;
            }
        }

        public IEnumerable<DirectedEdge> Adj(int v)
        {
            return adj[v];
        }

        public IEnumerable<DirectedEdge> Edges()
        {
            LinkedList<DirectedEdge> linkedlist = new LinkedList<DirectedEdge>();

            for (int v = 0; v < _v; v++)
            {
                foreach (DirectedEdge e in adj[v])
                    linkedlist.AddFirst(e);
            }
            return linkedlist;
        }

        public EdgeWeightedDigraph getTranspose()
        {
            EdgeWeightedDigraph input = this;

            EdgeWeightedDigraph transpose = new EdgeWeightedDigraph(input.V());

            for (int v = 0; v < input.V(); v++)
            {
                foreach (DirectedEdge e in input.Adj(v))
                {
                    int current_start = e.From();
                    int current_end = e.To();
                    double current_weight = e.Weight();

                    DirectedEdge reverse = new DirectedEdge(current_end, current_start, current_weight);

                    transpose.adj[reverse.From()].AddFirst(reverse);
                }

            }

            return transpose;
        }



    }
}