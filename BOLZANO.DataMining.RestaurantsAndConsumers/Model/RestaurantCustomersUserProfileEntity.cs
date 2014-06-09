using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOLZANO.DataMining.RestaurantsAndConsumers.Model
{
    public class RestaurantCustomersUserProfileEntity : RestaurantCustomersEntity
    {
        public int Weight { get; set; }

        public int BirthYear { get; set; }

        public double Height { get; set; }

        public bool Smoker { get; set; }
        
        public drink_level Drink_level { get; set; }

        public dress_preference Dress_preference { get; set; }

        public ambience Ambience { get; set; }

        public transport Transport { get; set; }

        public marital_status Marital_status { get; set; }

        public hijos Hijos { get; set; }

        public interest Interest { get; set; }

        public personality Personality { get; set; }

        public religion Religion { get; set; }

        public activity Activity { get; set; }

        public color Color { get; set; }

        public budget Budget { get; set; }
    }
}
