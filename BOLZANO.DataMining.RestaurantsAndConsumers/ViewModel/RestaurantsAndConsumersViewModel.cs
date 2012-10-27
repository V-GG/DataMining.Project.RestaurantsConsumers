using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class RestaurantsAndConsumersViewModel : ContextBase
    {
        //The collections for loading the parsed data into the memmory
        private Dictionary<string, RestaurantsCustomersGeoplaceEntity> restaurants;
        private Dictionary<string, RestaurantCustomersUserProfileEntity> userProfiles;
        private Dictionary<string, RestaurantsAndCustomersRating_finalEntity> rating_finals;

        private String userProfileWeightMedian;
        private String userProfileWeightMean;
        private String userProfileWeightMode;
        private String userProfileFirstQuarterWeight;
        private String userProfileThirdQuarterWeight;

        private DelegateCommand calculateUsersWeightSummaryCommand;

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

        public Dictionary<string, RestaurantCustomersUserProfileEntity> UserProfiles
        {
            get
            {
                return userProfiles;
            }

            set
            {
                if (userProfiles != value)
                {
                    userProfiles = value;

                    RaisePropertyChange(() => UserProfiles);
                }
            }
        }

        public DelegateCommand CalculateUsersWeightSummaryCommand 
        {
            get
            {
                if (calculateUsersWeightSummaryCommand == null)
                {
                    calculateUsersWeightSummaryCommand = new DelegateCommand(CalculateUsersWeightSummary, CanCalculateUsersWeightSummary);
                }

                return calculateUsersWeightSummaryCommand;
            }
        }

        public String UserProfileWeightMedian 
        {
            get
            {
                return userProfileWeightMedian;
            }
            set
            {
                if (userProfileWeightMedian != value)
                {
                    userProfileWeightMedian = value;

                    RaisePropertyChange(() => UserProfileWeightMedian);
                }
            }
        }

        public String UserProfileWeightMean 
        {
            get
            {
                return userProfileWeightMean;
            }
            set
            {
                if (userProfileWeightMean != value)
                {
                    userProfileWeightMean = value;

                    RaisePropertyChange(() => UserProfileWeightMean);
                }
            }
        }

        public String UserProfileWeightMode
        {
            get
            {
                return userProfileWeightMode;
            }
            set
            {
                if (userProfileWeightMode != value)
                {
                    userProfileWeightMode = value;

                    RaisePropertyChange(() => UserProfileWeightMode);
                }
            }
        }

        public String UserProfileFirstQuarterWeight 
        {
            get
            {
                return userProfileFirstQuarterWeight;
            }
            set
            {
                if (userProfileFirstQuarterWeight != value)
                {
                    userProfileFirstQuarterWeight = value;

                    RaisePropertyChange(() => UserProfileFirstQuarterWeight);
                }
            }
        }

         public String UserProfileThirdQuarterWeight 
        {
            get
            {
                return userProfileThirdQuarterWeight;
            }
            set
            {
                if (userProfileThirdQuarterWeight != value)
                {
                    userProfileThirdQuarterWeight = value;

                    RaisePropertyChange(() => UserProfileThirdQuarterWeight);
                }
            }
        }

        protected virtual bool CanCalculateUsersWeightSummary()
        {
            return true;
        }

        protected virtual void CalculateUsersWeightSummary()
        {
            if (UserProfiles != null)
            {
                ////Grouping the users by weights which deploys a list of frequencies which are defining how many ppl has a proper weight
                var groupedWeights = (from userProfile in UserProfiles
                                     group userProfile by userProfile.Value.Weight into weights
                                     let count = weights.Count()
                                     orderby count
                                     select new { Value = weights.Key, Count = count }).ToList();

                var listOfWeights = (from userProfile in UserProfiles
                                     select userProfile.Value.Weight).ToList();

                #region Median with Selection Algorithms

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                int medianSelectionAlgorithmIndex = MedianBySelectionAlgorithm(listOfWeights, 0, listOfWeights.Count);

                stopWatch.Stop();
                
                #endregion

                stopWatch.Reset();

                #region Median with sort algorithm

                stopWatch.Start();

                double median = this.GetMedian(listOfWeights.ToArray());

                stopWatch.Stop();

                #endregion

                #region Calculating the 5 numbers summary

                listOfWeights.Sort();

                int quarter1UserProfilesWeight = listOfWeights[listOfWeights.Count / 4];
                int quarter3UserProfilesWeight = listOfWeights[listOfWeights.Count * 3 / 4];
                #endregion

                UserProfileFirstQuarterWeight = string.Format("Q1 is: {0}", quarter1UserProfilesWeight);
                UserProfileThirdQuarterWeight = string.Format("Q3 is: {0}", quarter3UserProfilesWeight);
                UserProfileWeightMedian = string.Format("Median is: {0}", median);
                UserProfileWeightMean = string.Format("Mean is: {0}", UserProfiles.Sum(userProfile => userProfile.Value.Weight) / UserProfiles.Count);
                UserProfileWeightMode = string.Format("Mode is: {0}", groupedWeights.Last().Value);
            }
        }

        //The formula for calculating media based on intervals of frequencies
        private double MedianCalculation(int lowerBoundary, int countFreq, int lowerFrequencySum, int frequencySum, int width)
        {
            return ((lowerBoundary + countFreq - lowerFrequencySum) / frequencySum) * width;
        }

        #region The Selection Algorithm for finding the Median
        private int Partition(List<int> list, int left, int right, int k)
        {
            int pivotValue = list[k];
            int pom = list[pivotValue];
            list[pivotValue] = list[right];
            list[right] = pom;

            int storeIndex = left;
            for (int i = left; i < right - 1; i++)
            {
                if (list[i] <= pivotValue)
                {
                    pom = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = pom;
                    storeIndex++;
                }

                pom = list[storeIndex];
                list[storeIndex] = list[right];
                list[right] = pom;

                return storeIndex;
            }

            return 0;
        }

        private int SelectIndx(List<int> list, int left, int right, int k)
        {
            if (left == right) 
                return left;

            int pivotNewIndex = this.Partition(list, left, right, k);
            int pivotDestination = pivotNewIndex - left + 1;

            if (pivotDestination == k)
                return pivotNewIndex;
            else if (k < pivotDestination)
            {
                return SelectIndx(list, left, pivotNewIndex - 1, k);
            }
            else
            {
                return SelectIndx(list, pivotNewIndex + 1, right, k - pivotDestination);
            }
        }
        
        //return the median index into list
        private int MedianBySelectionAlgorithm(List<int> list, int left, int right)
        {
            int numMedians = (right - left) / 5;
            for (int i = 0; i < numMedians; i++)
            {
                int subLeft = left + i * 5;
                int subRight = subLeft + 5;

                int medianIdx = this.SelectIndx(list, subLeft, subRight, 2);
                //??????????
                int pom = i;
                i = medianIdx;
                medianIdx = pom;
            }

            return SelectIndx(list, left, left + numMedians, numMedians / 2);
        }
        #endregion

        #region Algorithms finding  median by sort

        private double GetMedian(int[] sourceNumbers)
        {
              
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0D;

            //make sure the list is sorted, but use a new array
            int[] sortedPNumbers = (int[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        #endregion

        private void ReadCSVFiles()
        {
            try
            {
                Restaurants = new Dictionary<string, RestaurantsCustomersGeoplaceEntity>();
                UserProfiles = new Dictionary<string, RestaurantCustomersUserProfileEntity>();
                rating_finals = new Dictionary<string, RestaurantsAndCustomersRating_finalEntity>();

                #region Reading geoplaces2.csv

                TextFieldParser restaurantsParser = new TextFieldParser(Properties.Settings.Default["geoplaces2Path"].ToString());
                restaurantsParser.TextFieldType = FieldType.Delimited;
                restaurantsParser.SetDelimiters(",");
                restaurantsParser.ReadFields();
                while (!restaurantsParser.EndOfData)
                {
                    //Processing row
                    RestaurantsCustomersGeoplaceEntity entity = new RestaurantsCustomersGeoplaceEntity();
                    string[] fields = restaurantsParser.ReadFields();
                    float latitude;
                    long longtitude;
                    float.TryParse(fields[1], out latitude);
                    long.TryParse(fields[2], out longtitude);

                    entity.ID = fields[0];
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

                TextFieldParser userProfilesParser = new TextFieldParser(Properties.Settings.Default["userProfilePath"].ToString());
                userProfilesParser.TextFieldType = FieldType.Delimited;
                userProfilesParser.SetDelimiters(",");
                userProfilesParser.ReadFields();
                while (!userProfilesParser.EndOfData)
                {
                    //Processing row
                    RestaurantCustomersUserProfileEntity entity = new RestaurantCustomersUserProfileEntity();
                    string[] fields = userProfilesParser.ReadFields();
                    int weightInput;
                    Int32.TryParse(fields[16], out weightInput);

                    entity.ID = fields[0];
                    entity.Name = string.Format("User {0}", fields[0]);
                    entity.Weight = weightInput;

                    UserProfiles.Add(fields[0], entity);
                }

                #endregion

                #region Reading ratings

                TextFieldParser ratingsParser = new TextFieldParser(Properties.Settings.Default["rating_finalPath"].ToString());
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
    }
}
