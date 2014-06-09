using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
   public class KMeansAlgorithm 
    {
       public int k;
       private Dictionary<string, RestaurantsCustomersGeoplaceEntity> inputDataSet;
       private List<Tuple<double[], List<RestaurantCustomersEntity>>> clusters;
       private Dictionary<string, double> restaurantsAvgRating;

       public KMeansAlgorithm(int numberOfClusters)
       {
           this.k = numberOfClusters;
           this.clusters = new List<Tuple<double[], List<RestaurantCustomersEntity>>>();
       }

       public List<Tuple<double[], List<RestaurantCustomersEntity>>> Clusters 
       {
           get
           {
               return clusters;
           }
       }

       public bool RunAlgorithmForGeoPlace(Dictionary<string, RestaurantsCustomersGeoplaceEntity> geoplaces, Dictionary<string, double> restaurantsAvgRating)
       {
           inputDataSet = new Dictionary<string, RestaurantsCustomersGeoplaceEntity>(geoplaces);
           this.restaurantsAvgRating = restaurantsAvgRating;
           NormalizeRestaurantsData();

           if (k != 0 && k<= inputDataSet.Count)
           {
               int kVar = k;
               while (kVar-- != 0)
               {
                   var restaurant = (inputDataSet.ElementAt(kVar).Value as RestaurantsCustomersGeoplaceEntity);
                   clusters.Add(new Tuple<double[], List<RestaurantCustomersEntity>>(new double[] { restaurant.Latitutde, 
                       restaurant.Longtitude, 
                       restaurantsAvgRating[restaurant.ID] },
                      new List<RestaurantCustomersEntity>(new RestaurantCustomersEntity[]{restaurant})));

                   inputDataSet.Remove(inputDataSet.ElementAt(kVar).Key);
               }

               foreach (var restaurant in inputDataSet)
               {
                   int indexToPut = 0;
                   var value = restaurant.Value as RestaurantsCustomersGeoplaceEntity;
                   double minDistance = DistanceForGeoplaceClustering(new double[] { value.Latitutde, value.Latitutde, restaurantsAvgRating[value.ID] }, clusters[0].Item1);
                   for (int i = 1; i < clusters.Count; i++)
                   {
                       double currentDistance = DistanceForGeoplaceClustering(new double[] { value.Latitutde, value.Latitutde, restaurantsAvgRating[value.ID] }, clusters[i].Item1);
                       if (currentDistance < minDistance)
                       {
                           indexToPut = i;
                       }
                   }

                   clusters[indexToPut].Item2.Add(restaurant.Value);
               }

               bool[] flags = new bool[k];

               try
               {
                   do
                   {
                       flags = new bool[k];
                       CalculateGeoplaceMeanForClusters();

                       for (int j = 0; j < clusters.Count; j++)
                       {
                           for (int y = 0; y < clusters[j].Item2.Count; y++)
                           {
                               var value = clusters[j].Item2[y] as RestaurantsCustomersGeoplaceEntity;
                               int indexToPut = j;
                               double minDistance = DistanceForGeoplaceClustering(new double[] { value.Latitutde, value.Latitutde, restaurantsAvgRating[value.ID] }, clusters[j].Item1);
                               for (int i = 0; i < clusters.Count; i++)
                               {
                                   double currentDistance = DistanceForGeoplaceClustering(new double[] { value.Latitutde, value.Latitutde, restaurantsAvgRating[value.ID] }, clusters[i].Item1);
                                   if (currentDistance < minDistance)
                                   {
                                       indexToPut = i;
                                   }
                               }

                               if (indexToPut != j)
                               {
                                   flags[j] = true;
                                   clusters[indexToPut].Item2.Add(value);
                                   clusters[j].Item2.RemoveAt(y);
                               }
                           }
                       }

                   } while (flags.Any(flag => flag == true));
               }
               catch (Exception)
               {
                   return false;
               }
               
           }

           //System.IO.StreamWriter file = new System.IO.StreamWriter("e:\\test.txt", true);
           

           //int ind = 1;
           //foreach (var cluster in clusters)
           //{
           //    string lines = String.Format("Cluster{0} ========================", ind);
           //    file.WriteLine(lines);
           //    ind++;
           //    foreach (var restaurant in cluster.Item2)
           //    {
           //        lines = String.Format("{0}  {1}  {2}", restaurant.Name, (restaurant as RestaurantsCustomersGeoplaceEntity).SmokingAreas, (restaurant as RestaurantsCustomersGeoplaceEntity).Alcohol);
           //        file.WriteLine(lines);
           //    }
           //}
           //file.Close();
           return true;
       }

       private void CalculateGeoplaceMeanForClusters()
       {
           foreach (var cluster in clusters)
           {
               var lattitudes = (from restaurant in cluster.Item2
                                        select (restaurant as RestaurantsCustomersGeoplaceEntity).Latitutde).ToList();

               var longtitudes = (from restaurant in cluster.Item2
                                 select (restaurant as RestaurantsCustomersGeoplaceEntity).Longtitude).ToList();

               var avgRatings = (from restaurantRate in restaurantsAvgRating
                                 join clusterRes in cluster.Item2 on restaurantRate.Key equals clusterRes.ID
                                 select restaurantRate.Value).ToList();

               double lattitude = lattitudes.Sum() / lattitudes.Count;
               double longtitude = longtitudes.Sum() / longtitudes.Count;
               double avgRating = avgRatings.Sum(rate => rate) / avgRatings.Count();

               cluster.Item1[0] = lattitude;
               cluster.Item1[1] = longtitude;
               cluster.Item1[2] = avgRating;
           }
       }

       //Both array should contain 3 elements representing the coordinates of the points in a 3 dimensional Euclidean space
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
