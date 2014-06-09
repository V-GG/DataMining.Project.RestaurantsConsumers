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
        Full_Bar
    }

    public enum Smoking_area
    {
        none, only_at_bar, permitted, section, not_permitted,missing
    }

    public enum drink_level
    {
        abstemious,social_drinker,casual_drinker,missing
    }

    public enum dress_preference
    {
        informal,formal,no_preference,elegant,missing
    }

    public enum ambience
    {
        family,friends,solitary,missing
    }

    public enum transport
    {
        on_foot,public_transport,car_owner,missing
    }

    public enum marital_status
    {
        single, married, widow,missing
    }

    public enum hijos
    {
        independent, kids, dependent,missing
    }

    public enum interest
    {
        variety,technology,none,retro,eco_friendly,missing
    }

    public enum personality
    {
        thrifty_protector,hunter_ostentatious,hard_worker,conformist,missing
    }

    public enum religion
    {
        none, Catholic, Christian, Mormon, Jewish,missing
    }

    public enum activity
    {
        student,professional,unemployed,working_class,missing
    }

    public enum color
    {
        black,red,blue,green,purple,orange,yellow,white,missing
    }

    public enum budget
    {
        medium, low, high,missing
    }
}
