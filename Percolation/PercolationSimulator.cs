using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percolation
{
    internal class PercolationSimulator
    {
        public static bool DoesPercolate(int m, int n, int k, Random rnd)
        {
            int total = m * n;
            var sites = Enumerable.Range(0, total).ToList();
            // перемішуємо вузли, щоб випадково вибрати блоковані
            sites = sites.OrderBy(x => rnd.Next()).ToList();
            HashSet<int> blocked = new HashSet<int>(sites.Take(k));

            int virtualTop = total;
            int virtualBottom = total + 1;
            UnionFind uf = new UnionFind(total + 2);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int idx = i * n + j; // номер поточного елемента
                    // якщо вузол заблокований, то не приєднуємо його
                    if (blocked.Contains(idx)) continue;

                    // приєднуємо вгору
                    if (i > 0)
                    {
                        int up = (i - 1) * n + j; // номер верхнього елемента
                        if (!blocked.Contains(up))
                        {
                            uf.Union(idx, up);
                        }
                    }

                    // приєднуємо зліва
                    if (j > 0)
                    {
                        int left = i * n + j - 1;
                        if (!blocked.Contains(left))
                        {
                            uf.Union(idx, left);
                        }
                    }

                    // приєднуємо до уявного верхнього елемента, якщо з першого (0) ряду
                    if (i == 0)
                    {
                        uf.Union(idx, virtualTop);
                    }

                    // приєднуємо до уявного нижнього елемента, якщо з останнього ряду
                    if (i == m - 1)
                    {
                        uf.Union(idx, virtualBottom);
                    }
                }
            }

            return uf.Connected(virtualTop, virtualBottom);
        }
    }
}
