using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOLZANO.DataMining.RestaurantsAndConsumers.Model;

namespace BOLZANO.DataMining.RestaurantsAndConsumers
{
    public class AprioryAlgorithmForUserRatingDataSet
    {
        private const string associationRuleId1 = "135085";
        private const string associationRuleId2 = "135052";

        //Structure for representing the restaurants that a particular user choosed to rate
        private Dictionary<string, List<string>> ratingFinalDataTransactions;
        //the output of the algorithm
        private List<Tuple<List<String>, int>> candidates;
        //Containing all the users
        private List<string> userIDList;
        //Containing all the restaurants
        private List<string> placeIDList;

        public AprioryAlgorithmForUserRatingDataSet(Dictionary<string, RestaurantsAndCustomersRating_finalEntity> input)
        {
            userIDList = new List<string>();
            placeIDList = new List<string>();
            candidates = new List<Tuple<List<string>, int>>();
            ratingFinalDataTransactions = new Dictionary<string, List<string>>();

            var uids = (from v in input
                          group v by v.Value.UserID into userIDs
                          select new { UserId = userIDs.Key }).ToList();
            userIDList.AddRange(uids.Select(item => item.UserId));

            var pids = (from v in input
                        group v by v.Value.PlaceID into placeIDs
                        let count = placeIDs.Count()
                        select new { PlaceId = placeIDs.Key, Count = count }).ToList();
            placeIDList.AddRange(pids.Select(pid => pid.PlaceId));
            //We initiate the candidates with only one unit per set and it's occurance into the data set - count
            candidates.AddRange(pids.Select(pid => new Tuple<List<string>, int>(new List<string>(new string[] { pid.PlaceId }), pid.Count)));

            foreach (var userID in userIDList)
            {
                var placeIDs = (from v in input
                               where v.Value.UserID == userID
                               select v.Value.PlaceID).ToList();

                ratingFinalDataTransactions.Add(userID, placeIDs);
            }
        }

        /// <summary>
        /// Runs the algorithms in a loop till there are no more supersets that are covering the argument sup
        /// in the context of the appriory algorithm
        /// </summary>
        /// <param name="sup"></param>
        /// <returns></returns>
        public List<Tuple<List<String>, int>> RunApriory(int sup)
        {
            List<Tuple<List<String>, int>> candidatesClone = null;

            while (1 == 1)
            {
                candidates.RemoveAll(candidate => candidate.Item2 < sup);

                if (candidates.Count != 0){
                    candidatesClone = new List<Tuple<List<String>, int>>(candidates);
                }

                if (GenerateCandidates() == false)
                    break;
            }

            candidates = candidatesClone;
            return candidates;
        }

        //Creating new potential candidates for associating rules withing appriory algorithm
        //works with the localy defined field - candidates
        private bool GenerateCandidates()
        {
            List<Tuple<List<String>, int>> newCandidates = new List<Tuple<List<string>,int>>();
            bool flag = false;
            //ToDo Check if new candidates could be generated
            //Generate if yes and return true
            
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = i + 1; j < candidates.Count; j++)
                {
                    var intersection = candidates[j].Item1.Intersect(candidates[i].Item1).ToList();
                                        
                    if (intersection.Count() == candidates[j].Item1.Count - 1)
                    {
                        intersection.AddRange(candidates[i].Item1.Where(candidate => intersection.All(intersectedItem => intersectedItem != candidate)));
                        intersection.AddRange(candidates[j].Item1.Where(candidate => intersection.All(intersectedItem => intersectedItem != candidate)));

                        int intersectionIntoDBCount = CheckIfCandidateExistIntoTheDB(intersection);
                        if (intersectionIntoDBCount > 0)
                        {
                            newCandidates.Add(new Tuple<List<string>, int>(intersection, intersectionIntoDBCount));
                            flag = true;
                        }
                    }                   
                }    
            }
            if (flag == true)
            {
                candidates = newCandidates;
            }

            return flag;
        }

        private int CheckIfCandidateExistIntoTheDB(List<string> candidate)
        {
            int count = 0;
            foreach (var transaction in ratingFinalDataTransactions)
            {
                List<string> intersect = transaction.Value.Intersect(candidate).ToList();
                if (intersect.Count == candidate.Count)
                {
                    count++;
                }
            }
            
            return count;
        }

        public void CalculateSupportConf()
        {
            int countAssocRule1 = 0;
            int countAssocRule1WithAssocRule2 = 0;

            foreach (var item in ratingFinalDataTransactions)
            {
                if(item.Value.Contains(associationRuleId1))
                {
                    countAssocRule1++;
                }

                if (item.Value.Contains(associationRuleId1) && item.Value.Contains(associationRuleId2))
                {
                    countAssocRule1WithAssocRule2++;
                }
            }
        }
    }
}
