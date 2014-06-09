using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class DBScanAlgorithm
    {
        private double eplsilone;
        private int minPoints;
        private Dictionary<string, RestaurantsCustomersGeoplaceEntity> inputDataSet;
        private Dictionary<string, double> restaurantsAvgRating;
        private List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>> clusters;

        public DBScanAlgorithm(double eplsilone, int MinPts)
        {
            this.eplsilone = eplsilone;
            this.minPoints = MinPts;
            this.clusters = new List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>>();
        }

        public List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>> Clusters
        {
            get
            {
                return clusters;
            }
        }

        public void RunDBScanAlgorithm(Dictionary<string, RestaurantsCustomersGeoplaceEntity> geoplaces, Dictionary<string, double> restaurantsAvgRating)
        {
            inputDataSet = new Dictionary<string, RestaurantsCustomersGeoplaceEntity>(geoplaces);
            this.restaurantsAvgRating = restaurantsAvgRating;
            NormalizeRestaurantsData();

            //ToDo populate clusters with core points based on epsilone and minpoints
            foreach (var restaurantI in inputDataSet)
	        {
                List<RestaurantsCustomersGeoplaceEntity> pointsWithinEpsi = new List<RestaurantsCustomersGeoplaceEntity>();
                pointsWithinEpsi.Add(restaurantI.Value);

		        foreach (var restaurantJ in inputDataSet)
	            {
                    if (restaurantI.Key != restaurantJ.Key)
                    {
                        if (DistanceForGeoplaceClustering(new double[] { restaurantI.Value.Latitutde, restaurantI.Value.Longtitude, restaurantsAvgRating[restaurantI.Value.ID] },
                                                                                new double[] { restaurantJ.Value.Latitutde, restaurantJ.Value.Longtitude, restaurantsAvgRating[restaurantJ.Value.ID] }) <= eplsilone)
                        {
                            pointsWithinEpsi.Add(restaurantJ.Value);
                        }
                    }
	            }

                if (pointsWithinEpsi.Count >= minPoints)
                {
                    clusters.Add(new Tuple<double[],List<RestaurantsCustomersGeoplaceEntity>>(new double[] { restaurantI.Value.Latitutde, restaurantI.Value.Longtitude, restaurantsAvgRating[restaurantI.Value.ID] },
                                         pointsWithinEpsi));
                                        
                }
	        }

            bool[] clustersForDeletion = new bool[clusters.Count];

            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = i+1; j < clusters.Count; j++)
                {
                    if (clusters[j].Item2.Any(restaurant => restaurant.Latitutde == clusters[i].Item1[0] &&
                                                                            restaurant.Longtitude == clusters[i].Item1[1] &&
                                                                            restaurantsAvgRating[restaurant.ID] == clusters[i].Item1[2]))
                    {
                        clusters[j].Item2.AddRange(clusters[i].Item2);
                        clustersForDeletion[i] = true;
                    }
                }
            }

            var pom = new List<Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>>();
            //System.IO.StreamWriter file = new System.IO.StreamWriter("e:\\test.txt", true);
            for (int i = 0; i < clustersForDeletion.Count(); i++)
            {
                if (clustersForDeletion[i] == true)
                {
                    continue;
                }

                pom.Add(new Tuple<double[], List<RestaurantsCustomersGeoplaceEntity>>(clusters[i].Item1, clusters[i].Item2.Distinct(new RestaurantComparer()).ToList()));
                // Compose a string that consists of three lines.
                //string lines = String.Format("Cluster{0} ========================/n",pom.Count);
                //file.WriteLine(lines);
                //foreach (var item in pom.Last().Item2)
                //{
                //    lines = String.Format("{0}  {1}  {2}", item.Name, item.SmokingAreas, item.Alcohol, item);
                //    file.WriteLine(lines);
                //}

                //file.WriteLine(" ");

                // Write the string to a file.
                
                               
            }
            //file.Close();
            clusters = pom;
        }

        private double DistanceForGeoplaceClustering(double[] point1, double[] point2)
        {
            return Math.Sqrt(Math.Pow(point1[0] - point2[0], 2) + Math.Pow(point1[1] - point2[1], 2) + Math.Pow(point1[2] - point2[2], 2));
        }

        private void NormalizeRestaurantsData()
        {
            if (inputDataSet != null)
            {
                var maxLatitude = inputDataSet.Max(restaurant => restaurant.Value.Latitutde);
                var maxLongtitude = inputDataSet.Max(restaurant => restaurant.Value.Longtitude);
                var maxAvgRating = restaurantsAvgRating.Max(avgRate => avgRate.Value);

                foreach (var restaurant in inputDataSet)
                {
                    restaurant.Value.Latitutde /= maxLatitude;
                    restaurant.Value.Longtitude /= maxLongtitude;
                }

                var keys = new List<string>(restaurantsAvgRating.Keys);
                foreach (string key in keys)
                {
                    restaurantsAvgRating[key] /= maxAvgRating;
                }
            }
        }           
    }
}
