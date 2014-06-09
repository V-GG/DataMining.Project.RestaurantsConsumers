using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;
using System.Windows;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class RestaurantsAndConsumersViewModel : ContextBase
    {
        private const int aprioryParam = 9;
        private const int kMeansNumberOfClustersParam = 10;
        private const double dbScanEpsiloneParam = 0.01;
        private const int dbScanMinPointsParam = 3;

        //The collections for loading the parsed data into the memmory
        private Dictionary<string, RestaurantsCustomersGeoplaceEntity> restaurants;
        private Dictionary<string, RestaurantCustomersUserProfileEntity> userProfiles;
        private Dictionary<string, RestaurantsAndCustomersRating_finalEntity> rating_finals;

        #region Weight summary fields
        private String userProfileWeightMedian;
        private String userProfileWeightMean;
        private String userProfileWeightMode;
        private String userProfileFirstQuarterWeight;
        private String userProfileThirdQuarterWeight;
        private String userProfileMinWeight;
        private String userProfileMaxWeight;
        #endregion

        #region Height summary fields
        private String userProfileHeightMedian;
        private String userProfileHeightMean;
        private String userProfileHeightMode;
        private String userProfileFirstQuarterHeight;
        private String userProfileThirdQuarterHeight;
        private String userProfileMinHeight;
        private String userProfileMaxHeight;
        #endregion

        #region BirthYear summary fields
        private String userProfileBirthYearMedian;
        private String userProfileBirthYearMean;
        private String userProfileBirthYearMode;
        private String userProfileFirstQuarterBirthYear;
        private String userProfileThirdQuarterBirthYear;
        private String userProfileMinBirthYear;
        private String userProfileMaxBirthYear;
        #endregion

        private Visibility aprioryAlgorithmVisibility;
        private Visibility kMeansAlgorithmVisibility;
        private Visibility dbScanAlgorithmVisibility;
        private List<Tuple<List<String>, int>> aprioryAlgorithmResult;
        private List<Tuple<double[], List<RestaurantCustomersEntity>>> kMeansResult;
        private List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>> dbScanResult;

        private DelegateCommand calculateUsersWeightSummaryCommand;
        private DelegateCommand runAprioryAlgorithmCommand;
        private DelegateCommand runKMeansAlgorithmCommand;
        private DelegateCommand runDBScanAlgorithmCommand;
        private DelegateCommand closeAprioryResultPopupCommand;
        private DelegateCommand closeKMeansResultPopupCommand;
        private DelegateCommand closeDBScanResultPopupCommand;

        public AprioryAlgorithmForUserRatingDataSet aprioryAlgorithm;
        public KMeansAlgorithm kMeansAlgorithm;
        public DBScanAlgorithm dbScanAlgorithm;

        public RestaurantsAndConsumersViewModel()
        {
            this.AprioryAlgorithmVisibility = Visibility.Collapsed;
            this.KMeansAlgorithmVisibility = Visibility.Collapsed;
            this.DBScanAlgorithmVisibility = Visibility.Collapsed;

            this.ReadCSVFiles();
            //this.GenerateRatings();
            this.GenerateDataForClassification();

            this.aprioryAlgorithm = new AprioryAlgorithmForUserRatingDataSet(this.Rating_finals);
            this.kMeansAlgorithm = new KMeansAlgorithm(kMeansNumberOfClustersParam);
            this.dbScanAlgorithm = new DBScanAlgorithm(dbScanEpsiloneParam, dbScanMinPointsParam);
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

        public Dictionary<string, RestaurantsAndCustomersRating_finalEntity> Rating_finals 
        {
            get
            {
                return rating_finals;
            }
            set
            {
                if (rating_finals != value)
                {
                    rating_finals = value;
                    
                    RaisePropertyChange(() => Rating_finals);
                }
            }
        }

        public List<Tuple<List<String>, int>> AprioryAlgorithmResult
        {
            get
            {
                return aprioryAlgorithmResult;
            }

            set
            {
                if (aprioryAlgorithmResult != value)
                {
                    aprioryAlgorithmResult = value;

                    RaisePropertyChange(() => AprioryAlgorithmResult);
                }
            }
        }

        public List<Tuple<double[], List<RestaurantCustomersEntity>>> KMeansResult
        {
            get
            {
                return kMeansResult;
            }
            set
            {
                if (kMeansResult != value)
                {
                    kMeansResult = value;

                    RaisePropertyChange(() => KMeansResult);
                }
            }
        }

        public List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>> DBeansResult
        {
            get
            {
                return dbScanResult;
            }
            set
            {
                if (dbScanResult != value)
                {
                    dbScanResult = value;

                    RaisePropertyChange(() => DBeansResult);
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

        public DelegateCommand RunAprioryAlgorithmCommand
        {
            get
            {
                if (runAprioryAlgorithmCommand == null)
                {
                    runAprioryAlgorithmCommand = new DelegateCommand(RunAprioryAlgorithm, CanRunAprioryAlgorithm);
                }

                return runAprioryAlgorithmCommand;
            }
        }

        public DelegateCommand RunKMeansAlgorithmCommand
        {
            get
            {
                if (runKMeansAlgorithmCommand == null)
                {
                    runKMeansAlgorithmCommand = new DelegateCommand(RunKMeansAlgorithm);
                }

                return runKMeansAlgorithmCommand;
            }
        }

        public DelegateCommand RunDBScanAlgorithmCommand
        {
            get
            {
                if (runDBScanAlgorithmCommand == null)
                {
                    runDBScanAlgorithmCommand = new DelegateCommand(RunDBScanAlgorithm);
                }

                return runDBScanAlgorithmCommand;
            }
        }

        public DelegateCommand CloseAprioryResultPopupCommand
        {
            get
            {
                if (closeAprioryResultPopupCommand == null)
                {
                    closeAprioryResultPopupCommand = new DelegateCommand(CloseAprioryResultPopup);
                }

                return closeAprioryResultPopupCommand;
            }
        }

        public DelegateCommand CloseKMeansResultPopupCommand 
        {
            get
            {
                if (closeKMeansResultPopupCommand == null)
                {
                    closeKMeansResultPopupCommand = new DelegateCommand(CloseKMeansResultPopup);
                }

                return closeKMeansResultPopupCommand;
            }
        }

        public DelegateCommand CloseDBScanResultPopupCommand
        {
            get
            {
                if (closeDBScanResultPopupCommand == null)
                {
                    closeDBScanResultPopupCommand = new DelegateCommand(CloseDBScanResultPopup);
                }

                return closeDBScanResultPopupCommand;
            }
        }

        public Visibility AprioryAlgorithmVisibility 
        {
            get
            {
                return aprioryAlgorithmVisibility;
            }
            set
            {
                if (aprioryAlgorithmVisibility != value)
                {
                    aprioryAlgorithmVisibility = value;

                    RaisePropertyChange(() => AprioryAlgorithmVisibility);
                }
            }
        }

        public Visibility KMeansAlgorithmVisibility 
        {
            get
            {
                return kMeansAlgorithmVisibility;
            }
            set
            {
                if (kMeansAlgorithmVisibility != value)
                {
                    kMeansAlgorithmVisibility = value;

                    RaisePropertyChange(() => KMeansAlgorithmVisibility);
                }
            }
        }

        public Visibility DBScanAlgorithmVisibility 
        {
            get
            {
                return dbScanAlgorithmVisibility;
            }
            set
            {
                if (dbScanAlgorithmVisibility != value)
                {
                    dbScanAlgorithmVisibility = value;

                    RaisePropertyChange(() => DBScanAlgorithmVisibility);
                }
            }
        }

        #region Weight Summary Properties
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
        public String UserProfileMinWeight 
        {
            get
            {
                return userProfileMinWeight;
            }
            set
            {
                if (userProfileMinWeight != value)
                {
                    userProfileMinWeight = value;

                    RaisePropertyChange(() => UserProfileMinWeight);
                }
            }
        }
        public String UserProfileMaxWeight
        {
            get
            {
                return userProfileMaxWeight;
            }
            set
            {
                if (userProfileMaxWeight != value)
                {
                    userProfileMaxWeight = value;

                    RaisePropertyChange(() => UserProfileMaxWeight);
                }
            }
        }
        #endregion

        #region Height Summary Properties
        public String UserProfileHeightMedian
        {
            get
            {
                return userProfileHeightMedian;
            }
            set
            {
                if (userProfileHeightMedian != value)
                {
                    userProfileHeightMedian = value;

                    RaisePropertyChange(() => UserProfileHeightMedian);
                }
            }
        }
        public String UserProfileHeightMean
        {
            get
            {
                return userProfileHeightMean;
            }
            set
            {
                if (userProfileHeightMean != value)
                {
                    userProfileHeightMean = value;

                    RaisePropertyChange(() => UserProfileHeightMean);
                }
            }
        }
        public String UserProfileHeightMode
        {
            get
            {
                return userProfileHeightMode;
            }
            set
            {
                if (userProfileHeightMode != value)
                {
                    userProfileHeightMode = value;

                    RaisePropertyChange(() => UserProfileHeightMode);
                }
            }
        }
        public String UserProfileFirstQuarterHeight
        {
            get
            {
                return userProfileFirstQuarterHeight;
            }
            set
            {
                if (userProfileFirstQuarterHeight != value)
                {
                    userProfileFirstQuarterHeight = value;

                    RaisePropertyChange(() => UserProfileFirstQuarterHeight);
                }
            }
        }
        public String UserProfileThirdQuarterHeight
        {
            get
            {
                return userProfileThirdQuarterHeight;
            }
            set
            {
                if (userProfileThirdQuarterHeight != value)
                {
                    userProfileThirdQuarterHeight = value;

                    RaisePropertyChange(() => UserProfileThirdQuarterHeight);
                }
            }
        }
        public String UserProfileMinHeight
        {
            get
            {
                return userProfileMinHeight;
            }
            set
            {
                if (userProfileMinHeight != value)
                {
                    userProfileMinHeight = value;

                    RaisePropertyChange(() => UserProfileMinHeight);
                }
            }
        }
        public String UserProfileMaxHeight
        {
            get
            {
                return userProfileMaxHeight;
            }
            set
            {
                if (userProfileMaxHeight != value)
                {
                    userProfileMaxHeight = value;

                    RaisePropertyChange(() => UserProfileMaxHeight);
                }
            }
        }
        #endregion

        #region BirthYear Summary Properties

        public String UserProfileBirthYearMedian
        {
            get
            {
                return userProfileBirthYearMedian;
            }
            set
            {
                if (userProfileBirthYearMedian != value)
                {
                    userProfileBirthYearMedian = value;

                    RaisePropertyChange(() => UserProfileBirthYearMedian);
                }
            }
        }
        public String UserProfileBirthYearMean
        {
            get
            {
                return userProfileBirthYearMean;
            }
            set
            {
                if (userProfileBirthYearMean != value)
                {
                    userProfileBirthYearMean = value;

                    RaisePropertyChange(() => UserProfileBirthYearMean);
                }
            }
        }
        public String UserProfileBirthYearMode
        {
            get
            {
                return userProfileBirthYearMode;
            }
            set
            {
                if (userProfileBirthYearMode != value)
                {
                    userProfileBirthYearMode = value;

                    RaisePropertyChange(() => UserProfileBirthYearMode);
                }
            }
        }
        public String UserProfileFirstQuarterBirthYear
        {
            get
            {
                return userProfileFirstQuarterBirthYear;
            }
            set
            {
                if (userProfileFirstQuarterBirthYear != value)
                {
                    userProfileFirstQuarterBirthYear = value;

                    RaisePropertyChange(() => UserProfileFirstQuarterBirthYear);
                }
            }
        }
        public String UserProfileThirdQuarterBirthYear
        {
            get
            {
                return userProfileThirdQuarterBirthYear;
            }
            set
            {
                if (userProfileThirdQuarterBirthYear != value)
                {
                    userProfileThirdQuarterBirthYear = value;

                    RaisePropertyChange(() => UserProfileThirdQuarterBirthYear);
                }
            }
        }
        public String UserProfileMinBirthYear
        {
            get
            {
                return userProfileMinBirthYear;
            }
            set
            {
                if (userProfileMinBirthYear != value)
                {
                    userProfileMinBirthYear = value;

                    RaisePropertyChange(() => UserProfileMinBirthYear);
                }
            }
        }
        public String UserProfileMaxBirthYear
        {
            get
            {
                return userProfileMaxBirthYear;
            }
            set
            {
                if (userProfileMaxBirthYear != value)
                {
                    userProfileMaxBirthYear = value;

                    RaisePropertyChange(() => UserProfileMaxBirthYear);
                }
            }
        }

        #endregion

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

                var groupedHeights = (from userProfile in UserProfiles
                                      group userProfile by userProfile.Value.Height into heights
                                      let count = heights.Count()
                                      orderby count
                                      select new { Value = heights.Key, Count = count }).ToList();

                var groupedBirthYears = (from userProfile in UserProfiles
                                         group userProfile by userProfile.Value.BirthYear into birthYears
                                         let count = birthYears.Count()
                                         orderby count
                                         select new { Value = birthYears.Key, Count = count }).ToList();

                var listOfWeights = (from userProfile in UserProfiles
                                     select userProfile.Value.Weight).ToList();

                var listOfHeights = (from userProfile in UserProfiles
                                     select userProfile.Value.Height).ToList();

                var listOfBirthYears = (from userProfile in UserProfiles
                                     select userProfile.Value.BirthYear).ToList();

                #region Median with Selection Algorithms

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                int medianSelectionAlgorithmIndex = MedianBySelectionAlgorithm(listOfWeights, 0, listOfWeights.Count);

                stopWatch.Stop();

                #endregion

                stopWatch.Reset();

                #region Median with sort algorithm

                stopWatch.Start();

                double weightMedian = this.GetMedian(listOfWeights.ToArray());

                stopWatch.Stop();

                double heightMedian = this.GetMedian(listOfHeights.ToArray());
                double birthYearMedian = this.GetMedian(listOfBirthYears.ToArray());

                #endregion

                #region Calculating the 5 numbers summary

                listOfWeights.Sort();

                int quarter1UserProfilesWeight = listOfWeights[listOfWeights.Count / 4];
                int quarter3UserProfilesWeight = listOfWeights[listOfWeights.Count * 3 / 4];

                listOfHeights.Sort();

                double quarter1UserProfilesHeight = listOfHeights[listOfHeights.Count / 4];
                double quarter3UserProfilesHeight = listOfHeights[listOfHeights.Count * 3 / 4];

                listOfBirthYears.Sort();

                int quarter1UserProfilesbirthYear = listOfBirthYears[listOfBirthYears.Count / 4];
                int quarter3UserProfilesbirthYear = listOfBirthYears[listOfBirthYears.Count * 3 / 4];
                #endregion

                UserProfileFirstQuarterWeight = string.Format("Q1 is: {0}", quarter1UserProfilesWeight);
                UserProfileThirdQuarterWeight = string.Format("Q3 is: {0}", quarter3UserProfilesWeight);
                UserProfileWeightMedian = string.Format("Median is: {0}", weightMedian);
                UserProfileWeightMean = string.Format("Mean is: {0}", UserProfiles.Sum(userProfile => userProfile.Value.Weight) / UserProfiles.Count);
                UserProfileWeightMode = string.Format("Mode is: {0}", groupedWeights.Last().Value);
                UserProfileMinWeight = string.Format("Min is: {0}", listOfWeights.Min());
                UserProfileMaxWeight = string.Format("Max is: {0}", listOfWeights.Max());

                UserProfileFirstQuarterHeight = string.Format("Q1 is: {0}", quarter1UserProfilesHeight);
                UserProfileThirdQuarterHeight = string.Format("Q3 is: {0}", quarter3UserProfilesHeight);
                UserProfileHeightMedian = string.Format("Median is: {0}", heightMedian);
                UserProfileHeightMean = string.Format("Mean is: {0}", UserProfiles.Sum(userProfile => userProfile.Value.Height) / UserProfiles.Count);
                UserProfileHeightMode = string.Format("Mode is: {0}", groupedHeights.Last().Value);
                UserProfileMinHeight = string.Format("Min is: {0}", listOfHeights.Min());
                UserProfileMaxHeight = string.Format("Max is: {0}", listOfHeights.Max());

                UserProfileFirstQuarterBirthYear = string.Format("Q1 is: {0}", quarter1UserProfilesbirthYear);
                UserProfileThirdQuarterBirthYear = string.Format("Q3 is: {0}", quarter3UserProfilesbirthYear);
                UserProfileBirthYearMedian = string.Format("Median is: {0}", birthYearMedian);
                UserProfileBirthYearMean = string.Format("Mean is: {0}", UserProfiles.Sum(userProfile => userProfile.Value.BirthYear) / UserProfiles.Count);
                UserProfileBirthYearMode = string.Format("Mode is: {0}", groupedBirthYears.Last().Value);
                UserProfileMinBirthYear = string.Format("Min is: {0}", listOfBirthYears.Min());
                UserProfileMaxBirthYear = string.Format("Max is: {0}", listOfBirthYears.Max());
            }
        }

        protected virtual void CloseAprioryResultPopup()
        {
            this.AprioryAlgorithmVisibility = Visibility.Collapsed;
        }

        protected virtual void CloseKMeansResultPopup()
        {
            this.KMeansAlgorithmVisibility = Visibility.Collapsed;
        }

        protected virtual void CloseDBScanResultPopup()
        {
            this.DBScanAlgorithmVisibility = Visibility.Collapsed;
        }

        protected virtual bool CanRunAprioryAlgorithm()
        {
            return true;
        }

        protected virtual void RunAprioryAlgorithm()
        {
            this.AprioryAlgorithmResult = this.aprioryAlgorithm.RunApriory(aprioryParam);
            this.AprioryAlgorithmVisibility = Visibility.Visible;

            this.aprioryAlgorithm.CalculateSupportConf();
        }

        protected virtual void RunKMeansAlgorithm()
        {
            Dictionary<string, double> avgRatings = new Dictionary<string, double>();
            var asd = (from rating in Rating_finals
                       select new Tuple<string, double>(rating.Value.PlaceID, (Convert.ToDouble(rating.Value.Rating + rating.Value.Service_Rating + rating.Value.Food_Rating)) / 3.0)).ToList();

            avgRatings = (from avgRate in asd
                          group avgRate by avgRate.Item1 into a
                          select new { Key = a.Key, Value = a.Average(v => v.Item2) }).ToDictionary(a => a.Key, a => a.Value);
            
            this.kMeansAlgorithm.RunAlgorithmForGeoPlace(Restaurants, avgRatings);
            this.KMeansAlgorithmVisibility = Visibility.Visible;
            this.KMeansResult = kMeansAlgorithm.Clusters;
        }

        protected virtual void RunDBScanAlgorithm()
        {
            Dictionary<string, double> avgRatings = new Dictionary<string, double>();
            var asd = (from rating in Rating_finals
                       select new Tuple<string, double>(rating.Value.PlaceID, (Convert.ToDouble(rating.Value.Rating + rating.Value.Service_Rating + rating.Value.Food_Rating)) / 3.0)).ToList();

            avgRatings = (from avgRate in asd
                          group avgRate by avgRate.Item1 into a
                          select new { Key = a.Key, Value = a.Average(v => v.Item2) }).ToDictionary(a => a.Key, a => a.Value);

            this.dbScanAlgorithm.RunDBScanAlgorithm(Restaurants, avgRatings);
            this.DBScanAlgorithmVisibility = Visibility.Visible;
            this.DBeansResult = this.dbScanAlgorithm.Clusters;
        }

        //The formula for calculating media based on intervals of frequencies
        private double MedianCalculation(int lowerBoundary, int countFreq, int lowerFrequencySum, int frequencySum, int width)
        {
            return ((lowerBoundary + countFreq - lowerFrequencySum) / frequencySum) * width;
        }

        #region The Selection Algorithm for finding the Median
        private int Partition(List<int> list, int left, int right, int pivotIndex)
        {
            int pivotValue = list[pivotIndex];
            int pom = list[pivotIndex];
            list[pivotIndex] = list[right];
            list[right] = pom;

            int storeIndex = left;
            for (int i = left; i <= right - 1; i++)
            {
                if (list[i] <= pivotValue)
                {
                    pom = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = pom;
                    storeIndex++;
                }
            }

            pom = list[storeIndex];
            list[storeIndex] = list[right];
            list[right] = pom;

            return storeIndex;
        }

        private int SelectIndx(List<int> list, int left, int right, int k)
        {
            if (left == right)
                return left;

            int pivotNewIndex = this.Partition(list, left, right, (right - left) / 2);
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
            int medianIdx;
            for (int i = 0; i < numMedians; i++)
            {
                int subLeft = left + i * 5;
                int subRight = subLeft + 5;

                medianIdx = this.SelectIndx(list, subLeft, subRight, 2);
                //??????????

                int pom2 = list[0];
                list[0] = list[medianIdx];
                list[medianIdx] = pom2;

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

        private double GetMedian(double[] sourceNumbers)
        {

            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0D;

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
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
                Rating_finals = new Dictionary<string, RestaurantsAndCustomersRating_finalEntity>();

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
                    double latitude;
                    double longtitude;
                    fields[1] = fields[1].Replace('.', ',');
                    fields[2] = fields[2].Replace('.', ',');
                    double.TryParse(fields[1], out latitude);
                    double.TryParse(fields[2], out longtitude);

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
                    try
                    {

                        RestaurantCustomersUserProfileEntity entity = new RestaurantCustomersUserProfileEntity();
                        string[] fields = userProfilesParser.ReadFields();
                        int weightInput;
                        int birthYearInput;
                        double heightInput;
                        Int32.TryParse(fields[16], out weightInput);
                        Int32.TryParse(fields[10], out birthYearInput);
                        double.TryParse(fields[18].Replace('.',','), out heightInput);
                        fields[5] = fields[5].Replace(' ', '_');
                        fields[5] = fields[5].Replace('-', '_');
                        fields[4] = fields[4].Replace(' ', '_');
                        fields[4] = fields[4].Replace('-', '_');
                        fields[6] = fields[6].Replace(' ', '_');
                        fields[6] = fields[6].Replace('-', '_');
                        fields[8] = fields[8].Replace(' ', '_');
                        fields[8] = fields[8].Replace('-', '_');
                        fields[7] = fields[7].Replace(' ', '_');
                        fields[7] = fields[7].Replace('-', '_');
                        fields[12] = fields[12].Replace(' ', '_');
                        fields[12] = fields[12].Replace('-', '_');
                        fields[11] = fields[11].Replace(' ', '_');
                        fields[11] = fields[11].Replace('-', '_');
                        fields[13] = fields[13].Replace(' ', '_');
                        fields[13] = fields[13].Replace('-', '_');
                        fields[14] = fields[14].Replace(' ', '_');
                        fields[14] = fields[14].Replace('-', '_');

                        entity.ID = fields[0];
                        entity.Name = string.Format("User {0}", fields[0]);
                        entity.Weight = weightInput;
                        entity.BirthYear = birthYearInput;
                        entity.Height = heightInput;
                        entity.Activity = fields[14] != "?" ? (activity)Enum.Parse(typeof(activity), fields[14]) : activity.missing;
                        entity.Ambience = fields[6] != "?" ?(ambience)Enum.Parse(typeof(ambience), fields[6]): ambience.missing;
                        entity.Budget = fields[17] != "?" ? (budget)Enum.Parse(typeof(budget), fields[17]): budget.missing;
                        entity.Color = fields[15] != "?" ?(color)Enum.Parse(typeof(color), fields[15]): color.missing;
                        entity.Dress_preference = fields[5] != "?" ? (dress_preference)Enum.Parse(typeof(dress_preference), fields[5]) : dress_preference.missing;
                        entity.Drink_level = fields[4] != "?" ? (drink_level)Enum.Parse(typeof(drink_level), fields[4]) : drink_level.missing;
                        entity.Hijos = fields[9] != "?" ? (hijos)Enum.Parse(typeof(hijos), fields[9]) : hijos.missing;
                        entity.Interest = fields[11] != "?" ? (interest)Enum.Parse(typeof(interest), fields[11]) : interest.missing;
                        entity.Marital_status = fields[8] != "?" ? (marital_status)Enum.Parse(typeof(marital_status), fields[8]) : marital_status.missing;
                        entity.Personality = fields[12] != "?" ? (personality)Enum.Parse(typeof(personality), fields[12]): personality.missing;
                        entity.Religion = fields[13] != "?" ? (religion)Enum.Parse(typeof(religion), fields[13]) : religion.missing;
                        entity.Smoker = fields[4] == "false" ? false : true;
                      // entity.Transport = (transport)Enum.Parse(typeof(transport), fields[7]);

                        UserProfiles.Add(fields[0], entity);
                    }
                    catch (Exception ex)
                    {
                        
                        throw ex;
                    }
                }

                #endregion

                #region Reading ratings

                TextFieldParser ratingsParser = new TextFieldParser(Properties.Settings.Default["rating_finalPath"].ToString());
                ratingsParser.TextFieldType = FieldType.Delimited;
                ratingsParser.SetDelimiters(",");
                ratingsParser.ReadFields();
                while (!ratingsParser.EndOfData)
                {
                    int rating;
                    int food_rating;
                    int service_rating;
                    
                    //Processing row
                    RestaurantsAndCustomersRating_finalEntity entity = new RestaurantsAndCustomersRating_finalEntity();
                    string[] fields = ratingsParser.ReadFields();
                    entity.UserID = fields[0];
                    entity.PlaceID = fields[1];

                    entity.ID = fields[0] + fields[1];
                    Int32.TryParse(fields[2], out rating);
                    entity.Rating = rating;
                    Int32.TryParse(fields[3], out food_rating);
                    entity.Food_Rating = food_rating;
                    Int32.TryParse(fields[4], out service_rating);
                    entity.Service_Rating = service_rating;

                    Rating_finals.Add(entity.ID,entity);
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GenerateRatings()
        {
            Rating_finals = new Dictionary<string, RestaurantsAndCustomersRating_finalEntity>();

            RestaurantsAndCustomersRating_finalEntity entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "10";
            entity.PlaceID = "l1";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "10";
            entity.PlaceID = "l3";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "10";
            entity.PlaceID = "l4";  
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "20";
            entity.PlaceID = "l2";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "20";
            entity.PlaceID = "l3";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "20";
            entity.PlaceID = "l5";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "30";
            entity.PlaceID = "l1";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "30";
            entity.PlaceID = "l2";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "30";
            entity.PlaceID = "l3";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "30";
            entity.PlaceID = "l5";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "40";
            entity.PlaceID = "l2";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);

            entity = new RestaurantsAndCustomersRating_finalEntity();
            entity.UserID = "40";
            entity.PlaceID = "l5";
            Rating_finals.Add(entity.UserID + entity.PlaceID, entity);
        }

        private void GenerateDataForClassification()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("e:\\classify.txt", true);
            String buffer = "";

            foreach (var user in UserProfiles)
            {
                var ratingsforprint = Rating_finals.Where(rating => rating.Value.UserID == user.Value.ID);

                foreach (var rating in ratingsforprint)
                {
                    buffer = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},'{17}'",
                        user.Value.Smoker, user.Value.Drink_level, user.Value.Dress_preference, 
                        user.Value.Ambience, user.Value.Transport, user.Value.Marital_status, user.Value.Hijos, user.Value.BirthYear, user.Value.Interest,
                        user.Value.Personality, user.Value.Religion, user.Value.Activity, user.Value.Color, user.Value.Weight,
                        user.Value.Budget, user.Value.Height.ToString().Replace(',','.'), rating.Value.Rating, restaurants[rating.Value.PlaceID].Name);

                    file.WriteLine(buffer);
                    buffer = String.Empty;
                }
            }

            file.Close();
        }
    }
}
