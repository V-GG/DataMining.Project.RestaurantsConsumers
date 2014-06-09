using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOLZANO.DataMining.RestaurantsAndConsumers.Model
{
    public class RestaurantsAndCustomersRating_finalEntity : RestaurantCustomersEntity
    {
        public string UserID { get; set; }

        public string PlaceID { get; set; }

        public int Rating { get; set; }

        public int Food_Rating { get; set; }

        public int Service_Rating { get; set; }
    }
}
