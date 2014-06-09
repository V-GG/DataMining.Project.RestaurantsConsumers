using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class ListComparer<T> : IEqualityComparer<T> where T : Tuple<List<String>, int>
    {
        #region IEqualityComparer Members

        public bool Equals(T x, T y)
        {            
            Tuple<List<String>, int> tup1 = x as Tuple<List<String>, int>;
            Tuple<List<String>, int> tup2 = y as Tuple<List<String>, int>;

            if (tup1.Item1.Intersect(tup2.Item1).Count() == tup1.Item1.Count)
                return true;
            else
                return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
