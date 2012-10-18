using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;

namespace BOLZANO.DataMining.RestaurantsAndConsumers.ViewModel
{
    public class RestaurantsAndConsumers16
    {
        private const string geoplaces2Path = @"E:\VG\BOLZANO\DataMining\DB\geoplaces2.csv";
        
        private Dictionary<string, RestaurantsCustomersGeoplaceEntity> restaurants;
        private Dictionary<string, RestaurantCustomersUserProfileEntity> userProfiles;

        public RestaurantsAndConsumers16()
        {
            this.ReadCSVFiles();
        }

        private void ReadCSVFiles()
        {
            restaurants = new Dictionary<string, RestaurantsCustomersGeoplaceEntity>();
            TextFieldParser parser = new TextFieldParser(geoplaces2Path);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadFields();
            while (!parser.EndOfData)
            {
                //Processing row
                RestaurantsCustomersGeoplaceEntity entity = new RestaurantsCustomersGeoplaceEntity();
                string[] fields = parser.ReadFields();
                int placeId;
                float latitude;
                long longtitude;
                Int32.TryParse(fields[0], out placeId);
                float.TryParse(fields[1], out latitude);
                long.TryParse(fields[2], out longtitude);

                entity.ID = placeId;
                entity.Latitutde = latitude;
                entity.Longtitude = longtitude;
                entity.The_geom_meter = fields[3];
                entity.Name = fields[4];
                entity.Address = fields[5];
                entity.City = fields[6];
                entity.State = fields[7];
                entity.Country = fields[8];
                //ToDo Change fax type to numeric one
                entity.Fax = fields[9];
                entity.Zip = fields[10];
                entity.ParseAlcoholString(fields[11]);
            }
            parser.Close();
        }

        private int CalculateMean(KeyValuePair<string, string> tableAttributeInput)
        {
            //ToDo Write it dynamically choose table and attribute by the input
            int countUserProfiles = 0;
            int sumUsersWeight = 0;
            foreach (var item in userProfiles)
            {
                countUserProfiles++;
                sumUsersWeight += item.Value.Weight;
            }

            return sumUsersWeight / countUserProfiles;
        }

        private double CalculateMedian()
        {

            return 0.0;
        }
    }
}
