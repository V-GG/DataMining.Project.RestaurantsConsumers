using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOLZANO.DataMining.RestaurantsAndConsumers.Model
{
    public class RestaurantsCustomersGeoplaceEntity : RestaurantCustomersEntity
    {
        public float Latitutde { get; set; }

        public long Longtitude { get; set; }

        public string The_geom_meter { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Fax { get; set; }

        public string Zip { get; set; }

        public Alcohol Alcohol { get; set; }
        public void ParseAlcoholString(string input)
        {
            switch (input)
            {
                case "No_Alcohol_Served": 
                    this.Alcohol = Model.Alcohol.No_Alcohol_Served;
                    break;
                case "Wine_Beer":
                    this.Alcohol = Model.Alcohol.Wine_Beer;
                    break;
                case "Full_Bar":
                    this.Alcohol = Model.Alcohol.Full_Bar;
                    break;
            }
        }

        public Smoking_area SmokingAreas { get; set; }
        public void ParseSmokingAreas(string input)
        {
            switch (input)
            {
                case "none": this.SmokingAreas = Smoking_area.none;
                    break;
                //case "":
            }
        }
    }
}
