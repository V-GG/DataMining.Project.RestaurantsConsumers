using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class RestaurantComparer : IEqualityComparer<RestaurantsCustomersGeoplaceEntity>
    {
        #region IEqualityComparer<RestaurantsCustomersGeoplaceEntity> Members

        public bool Equals(RestaurantsCustomersGeoplaceEntity x, RestaurantsCustomersGeoplaceEntity y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(RestaurantsCustomersGeoplaceEntity obj)
        {
            return obj.ID.GetHashCode() ^
            obj.Name.GetHashCode();
        }

        #endregion
    }
}
