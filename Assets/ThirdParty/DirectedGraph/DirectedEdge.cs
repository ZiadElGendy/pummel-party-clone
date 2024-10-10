using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programming.Graphs
{
    public class DirectedEdge
    {
        private readonly int _v;
        private readonly int _w;
        private readonly double _weight;

        public DirectedEdge(int v, int w, double weight)
        {
            this._v = v;
            this._w = w;
            this._weight = weight;
        }

        public double Weight()
        {
            return _weight;
        }

        public int From()
        {
            return _v;
        }

        public int To()
        {
            return _w;
        }

        public override string ToString()
        {
            return String.Format("{0:d}->{1:d} {2:f}", _v, _w, _weight);
        }
    }
}