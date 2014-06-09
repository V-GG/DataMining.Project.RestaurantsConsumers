using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOLZANO.DataMining.RestaurantsAndConsumers.Model
{
    public enum Alcohol
    {
        No_Alcohol_Served,
        Wine_Beer,
        Full_Bar,
        Dinner
    }

    public enum Smoking_area
    {
        none, only_at_bar, permitted, section, not_permitted
    }
}
