using System;
using System.Collections.Generic;
using System.ComponentModel;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;
using Microsoft.VisualBasic.FileIO;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class RestaurantsAndConsumersViewModel : ContextBase
    {
        //ToDo Change this hardcoded path.
        private const string geoplaces2Path = @"C:\Users\ele\Documents\GitHub\DataMining.Project.RestaurantsConsumers\BOLZANO.DataMining.RestaurantsAndConsumers\Database\geoplaces2.csv";
        private const string userProfilePath = @"C:\Users\ele\Documents\GitHub\DataMining.Project.RestaurantsConsumers\BOLZANO.DataMining.RestaurantsAndConsumers\Database\userprofile.csv";
        private const string rating_finalPath = @"C:\Users\ele\Documents\GitHub\DataMining.Project.RestaurantsConsumers\BOLZANO.DataMining.RestaurantsAndConsumers\Database\rating_final.csv";

        //The collections for loading the parsed data into the memmory
        private Dictionary<string, RestaurantsCustomersGeoplaceEntity> restaurants;
        private Dictionary<string, RestaurantCustomersUserProfileEntity> userProfiles;
        private Dictionary<string, RestaurantsAndCustomersRating_finalEntity> rating_finals;

        public RestaurantsAndConsumersViewModel()
        {
            this.ReadCSVFiles();
        }

        public Dictionary<string, RestaurantsCustomersGeoplaceEntity> Restaurants
        {
            get
            {
                return restaurants;
            }
            set
            {
                if (restaurants != value)
                {
                    restaurants = value;

                    RaisePropertyChange(() => Restaurants);
                }
            }
        }

        private void ReadCSVFiles()
        {
            try
            {
                Restaurants = new Dictionary<string, RestaurantsCustomersGeoplaceEntity>();
                userProfiles = new Dictionary<string, RestaurantCustomersUserProfileEntity>();
                rating_finals = new Dictionary<string, RestaurantsAndCustomersRating_finalEntity>();

                #region Reading geoplaces2.csv

                TextFieldParser restaurantsParser = new TextFieldParser(geoplaces2Path);
                restaurantsParser.TextFieldType = FieldType.Delimited;
                restaurantsParser.SetDelimiters(",");
                restaurantsParser.ReadFields();
                while (!restaurantsParser.EndOfData)
                {
                    //Processing row
                    RestaurantsCustomersGeoplaceEntity entity = new RestaurantsCustomersGeoplaceEntity();
                    string[] fields = restaurantsParser.ReadFields();
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

                    Restaurants.Add(fields[0], entity);
                }
                restaurantsParser.Close();

                #endregion

                #region Reading user profiles

                TextFieldParser userProfilesParser = new TextFieldParser(userProfilePath);
                userProfilesParser.TextFieldType = FieldType.Delimited;
                userProfilesParser.SetDelimiters(",");
                userProfilesParser.ReadFields();
                while (!userProfilesParser.EndOfData)
                {
                    //Processing row
                    RestaurantCustomersUserProfileEntity entity = new RestaurantCustomersUserProfileEntity();
                    string[] fields = userProfilesParser.ReadFields();
                }

                #endregion

                #region Reading ratings

                TextFieldParser ratingsParser = new TextFieldParser(rating_finalPath);
                ratingsParser.TextFieldType = FieldType.Delimited;
                ratingsParser.SetDelimiters(",");
                ratingsParser.ReadFields();
                while (!ratingsParser.EndOfData)
                {
                    //Processing row
                    RestaurantsAndCustomersRating_finalEntity entity = new RestaurantsAndCustomersRating_finalEntity();
                    string[] fields = ratingsParser.ReadFields();
                }

                #endregion

            }
            catch (Exception ex)
            {

            }
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

        private double CalculateMedianUserProfileWeight()
        {

            return 0.0;
        }

        //#region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //#endregion
    }
}
