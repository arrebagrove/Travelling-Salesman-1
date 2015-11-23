using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class ListComparer : IEqualityComparer<Tuple<int,List<int>>>
    {
        public bool Equals(Tuple<int, List<int>> x, Tuple<int, List<int>> y)
        {
            return x.Item2.SequenceEqual(y.Item2) && x.Item1.Equals(y.Item1);
        }

        public int GetHashCode(Tuple<int, List<int>> obj)
        {
            int hashcode = 0;
            foreach (int t in obj.Item2)
            {
                hashcode ^= t.GetHashCode();
            }
            return hashcode;
        }
    }
}
