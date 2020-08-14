#region using references
using System;
using System.Collections;
using System.Collections.Specialized;
#endregion
/// <summary>
/// This class creates the state names used on some of the membership forms
/// </summary>
namespace UnitedStates
{
    #region get states class

    public static class StateNames
    {
        // this file is used in forms that use states dropdownlist
        private static string[] _states = new string[] 
        { 
         "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "Florida", 
         "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Luisiana", 
         "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", 
         "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", 
         "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", 
         "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
        };

        /// <summary>
        /// Returns an array with all states
        /// </summary>     
        public static StringCollection GetStates()
        {
            StringCollection states = new StringCollection();
            states.AddRange(_states);
            return states;
        }
        public static SortedList GetStates(bool insertEmpty)
        {
            SortedList states = new SortedList();
            if (insertEmpty)
                states.Add("", "Please select one...");
            foreach (String state in _states)
                states.Add(state, state);
            return states;
        }
    }

    #endregion
}