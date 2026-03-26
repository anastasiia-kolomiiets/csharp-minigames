using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percolation
{
    internal class UnionFind
    {
        private int[] parent;
        private int[] rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 1;
            }
        }

        public int Find(int p)
        {
            // якщо не є батьком сам собі, то продвигаємось далі
            if (parent[p] != p)
            {
                parent[p] = Find(parent[p]);
            }
            return parent[p];
        }

        public void Union(int p, int q)
        {
            int rootP = Find(p);
            int rootQ = Find(q);
            if (rootP != rootQ)
            {
                if (rank[rootP] > rank[rootQ])
                {
                    parent[rootQ] = rootP;
                }
                else if (rank[rootP] < rank[rootQ])
                {
                    parent[rootP] = rootQ;
                }
                else
                {
                    parent[rootQ] = rootP;
                    rank[rootP]++;
                }
            }
        }

        public bool Connected(int p, int q)
        {
            return Find(p) == Find(q);
        }
    }
}
