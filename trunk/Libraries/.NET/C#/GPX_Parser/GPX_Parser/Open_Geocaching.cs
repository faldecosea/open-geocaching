/*
    GPX_Parser parses gpx files provided from <http://www.geocaching.com>
    Copyright (C) <2010>  <Michael Hand>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Open_Geocaching
{
    /// <summary>
    /// Creates a new GPX_Parser class.
    /// </summary>
    public class GPX_Parser
    {
        // Create variables
        private double latitude;             // Cache latitude coordinates
        private double longitude;            // Cache longitude coordinates
        private string date_Placed;          // Date cache was placed
        private string cacheID;              // Cache ID number
        private string description;          // Cache description
        private string cacheURL;             // Cache url
        private string cacheArchived;        // Is cache archived
        private string cacheName;            // Cache name
        private string placed_By;            // Who placed the cache
        private string cacheType;            // What type of cache is it
        private string cacheContainer;       // Type of container
        private string difficulty;           // How difficult is it to find the cache
        private string terrain;              // How bad is the terrain to get to the cache
        private string country;              // What country is the cache located
        private string state;                // What state is the cache located
        private string short_Description;    // Short description of the cache
        private string long_Description;     // Long description of the cache
        private string hints;                // Hints to find the cache
        private string[] cache_Logs;         // Cache logs
        private string[] travel_Bugs;        // Travel bugs in cache container
        
        private XmlDocument doc;             // Holds the xml document
        private XmlElement element;          // Gets the specified element in the xml file
        private XmlNodeList wptNodes;        // Holds all occurrences of waypoints in the gpx file
        private XmlNodeList logNodes;        // Holds all occurrences of logs in the gpx file
        private XmlNodeList travelBugNodes;  // Holds all occurrences of travel bugs in the gpx file

        private int numLogs;                 // Number of logs in the cache
        private int numBugs;                 // Number of travel bugs in the cache
        private int currentIndex = 0;        // Keeps the current index for cacheLogs and travelBugs
        private int wptIndex = 0;            // Keeps the current index for waypoints

        /// <summary>
        /// Creates a new GPX_Parser object and intializes class variables.
        /// </summary>
        /// <param name="gpxFile">The location of the gpx file</param>
        public GPX_Parser(string gpxFile)
        {
            // Create a new xml document
            doc = new XmlDocument();
            // Load xml file into doc
            doc.Load(gpxFile);
            // Create a new xml element
            element = doc.DocumentElement;

            // Get all the occurrences of wpt in the xml file
            wptNodes = element.GetElementsByTagName("wpt");
        }

        /// <summary>
        /// Parses the current cache.
        /// </summary>
        /// <param name="currentCache"></param>
        public void getCache(int currentCache)
        {
            // Get all the occurrences of groundspeak:log in the xml file
            logNodes = element["wpt"].GetElementsByTagName("groundspeak:log");

            // Get all the occurrences of groundspeak:travelbugs
            travelBugNodes = element["wpt"].GetElementsByTagName("groundspeak:travelbug");

            // Sets the number of logs in the cache
            numLogs = (logNodes.Count * 4);

            // Sets the number of travel bugs in the cache
            numBugs = (travelBugNodes.Count * 2);

            // Get the latitude of the cache, apply the current local number format and convert it to a double
            // CultureInfo.InvariantCulture applies the user current number format based on their locale
            latitude = Convert.ToDouble(wptNodes[currentCache].Attributes["lat"].InnerText, CultureInfo.InvariantCulture);

            // Get the longitude of the cache, apply the current local number format and convert it to a double
            // CultureInfo.InvariantCulture applies the users current number format based on their locale
            longitude = Convert.ToDouble(wptNodes[currentCache].Attributes["lon"].InnerText, CultureInfo.InvariantCulture);

            // Get the date the cache was placed
            date_Placed = wptNodes[currentCache]["time"].InnerText.Remove(wptNodes[currentCache]["time"].InnerText.IndexOf('T'));

            // Get the ID that geocaching.com uses to uniquely identify the cache
            cacheID = wptNodes[currentCache]["name"].InnerText;

            // Get the cache description
            description = wptNodes[currentCache]["desc"].InnerText;

            // Get the direct url to the cache on geocaching.com
            cacheURL = wptNodes[currentCache]["url"].InnerText;

            // Get whether or not the cache is currently archived
            cacheArchived = wptNodes[currentCache]["groundspeak:cache"].Attributes["archived"].InnerText;

            // Get the cache name
            cacheName = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:name"].InnerText;

            // Get the user who placed the cache
            placed_By = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:placed_by"].InnerText;

            // Get the type of cache it is
            cacheType = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:type"].InnerText;

            // Get the type of container the cache is
            cacheContainer = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:container"].InnerText;

            // Get the difficulty of how hard it is to find the cache
            difficulty = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:difficulty"].InnerText;

            // Get the terrain of how hard it is to get to the cache
            terrain = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:terrain"].InnerText;

            // Get the country where the geocache is located
            country = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:country"].InnerText;

            // Get the state where the geocache is located
            state = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:state"].InnerText;

            // Get the short description of the cache
            short_Description = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:short_description"].InnerText;

            // Get the long description of the cache
            long_Description = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:long_description"].InnerText;

            // Get the hints of the cache
            hints = wptNodes[currentCache]["groundspeak:cache"]["groundspeak:encoded_hints"].InnerText;

            // Checks whether the number of logs is greater than 0
            if (numLogs > 0)
            {
                // Initialize the cacheLogs string array based on number of logs
                cache_Logs = new string[numLogs];

                // Set currentIndex to 0
                currentIndex = 0;

                // Loop through each occurrence of a log
                for (int i = 0; i < logNodes.Count; i++)
                {
                    // Get the date the log was placed
                    cache_Logs[currentIndex] = logNodes[i]["groundspeak:date"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;

                    // Get the type of log (i.e. found, not found)
                    cache_Logs[currentIndex] = logNodes[i]["groundspeak:type"].InnerText;

                    // Increase the currentIndex by 1
                    currentIndex++;

                    // Get the user who wrote the log
                    cache_Logs[currentIndex] = logNodes[i]["groundspeak:finder"].InnerText;

                    // Increase the currentIndex by 1
                    currentIndex++;

                    // Get the text the user wrote for the log
                    cache_Logs[currentIndex] = logNodes[i]["groundspeak:text"].InnerText;

                    // Increase the currentIndex by 1
                    currentIndex++;
                }
            }
            // If number of logs is not greater than 0 then we need to create
            // the string array with just 1 string because there are not logs
            // in this cache
            else
            {
                // Initialize the cacheLogs string array to 1 string
                cache_Logs = new string[1];

                // Set the text of the only string in the array to No logs available
                cache_Logs[0] = "No logs available";
            }

            // Check whether the number of travel bugs is greater than 0
            if (numBugs > 0)
            {
                // Initialize the travelBugs array based on number of bugs
                travel_Bugs = new string[numBugs];

                // Set currentIndex to 0
                currentIndex = 0;

                // Loop through each occurence of a travelbug in the cache
                for (int i = 0; i < travelBugNodes.Count; i++)
                {
                    // Get the reference number of the travelbug
                    travel_Bugs[currentIndex] = travelBugNodes[i].Attributes["ref"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;

                    // Get the name of the travelbug
                    travel_Bugs[currentIndex] = travelBugNodes[i]["groundspeak:name"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;
                }
            }
            // If the number of bugs are not greater than 0 then
            // there are no travelbugs in the cache
            else
            {
                // Intialize travelBugs array to 1 string
                travel_Bugs = new string[1];

                // Set the text to the only string in the array to No travel bugs available
                travel_Bugs[0] = "No travel bugs available";
            }
        }

        /// <summary>
        /// Returns the number of caches in the file.
        /// <para>(*Use in a loop to iterate through all caches and cache properties.*)</para>
        /// </summary>
        /// <example>Shows you how to call <see cref="numCaches"/> getter.
        /// <code>
        /// GPX_Parser parser = new GPX_Parser("Path To GPX File")
        ///       
        /// for(int i = 0; i (less than symbol) parser.numCaches; i++)
        /// {
        ///     parser.getCache(i);
        ///             
        ///     double Latitude = parser.Latitude;
        ///     etc.....
        /// }
        /// </code>
        /// </example>
        public int numCaches
        {
            get
            {
                return wptNodes.Count;
            }
        }

        /// <summary>
        /// Returns the latitude coordinates of the cache.
        /// </summary>
        /// <returns>Returns a double</returns>
        public double Cache_Latitude
        {
            get
            {
                return latitude;
            }
        }

        /// <summary>
        /// Returns the longitude coordinates of the cache.
        /// </summary>
        /// <returns>Returns a double</returns>
        public double Cache_Longitude
        {
            get
            {
                return longitude;
            }
        }

        /// <summary>
        /// Returns the date the cache was placed.
        /// </summary>
        /// <returns>Returns a string in (yyyy:mm:dd) format</returns>
        public string Cache_Date_Placed
        {
            get
            {
                return date_Placed;
            }
        }

        /// <summary>
        /// Returns the cache ID name.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_ID
        {
            get
            {
                return cacheID;
            }
        }

        /// <summary>
        /// Returns the description of the cache.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Returns the web url of the cache.
        /// </summary>
        public string Cache_URL
        {
            get
            {
                return cacheURL;
            }
        }

        /// <summary>
        /// Returns whether the cache is archived.
        /// </summary>
        public string Cache_Archived
        {
            get
            {
                return cacheArchived;
            }
        }

        /// <summary>
        /// Returns the name of the cache.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_Name
        {
            get
            {
                return cacheName;
            }
        }

        /// <summary>
        /// Returns the person who placed the cache.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_Placed_By
        {
            get
            {
                return placed_By;
            }
        }

        /// <summary>
        /// Returns the type of cache.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_Type
        {
            get
            {
                return cacheType;
            }
        }

        /// <summary>
        /// Returns the type of container the cache is.
        /// </summary>
        /// <returns>Returns a string</returns>
        public string Cache_Container
        {
            get
            {
                return cacheContainer;
            }
        }

        /// <summary>
        /// Returns the difficulty of the cache.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_Difficulty
        {
            get
            {
                return difficulty;
            }
        }

        /// <summary>
        /// Returns the terrain of the cache.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_Terrain
        {
            get
            {
                return terrain;
            }
        }

        /// <summary>
        /// Returns the country the cache is located in.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_Country
        {
            get
            {
                return country;
            }
        }

        /// <summary>
        /// Returns the state the cache is located in.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Returns the short descripton of the cache.
        /// </summary>
        /// <returns>Return a sring</returns>
        public string Cache_Short_Description
        {
            get
            {
                return short_Description;
            }
        }

        /// <summary>
        /// Returns the long description of the cache.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_Long_Description
        {
            get
            {
                return long_Description;
            }
        }

        /// <summary>
        /// Returns the hint of the cache.
        /// </summary>
        /// <returns>Return a string</returns>
        public string Cache_Hints
        {
            get
            {
                return hints;
            }
        }

        /// <summary>
        /// Returns all the logs in the cache.
        /// </summary>
        /// <returns>Return a string array</returns>
        public string[] Cache_Logs
        {
            get
            {
                return cache_Logs;
            }
        }

        /// <summary>
        /// Returns all the travel bugs in the cache.
        /// </summary>
        /// <returns>Return a string array</returns>
        public string[] Cache_Travel_Bugs
        {
            get
            {
                return travel_Bugs;
            }
        }
    }

    /// <summary>
    /// Creates a new LOC_Parser class.
    /// </summary>
    public class LOC_Parser
    {
        private string cacheID;         // String to hold cache ID name
        private string cacheName;       // String to hold cache name
        private double latitude;        // Double to hold latitude value
        private double longitude;       // Double to hold longitude value
        private string cacheURL;       // String to hold cache web link

        private XmlDocument xmlDoc;     // Document to hold xml file
        private XmlElement xmlElement;  // Get the specified element in the xml file
        private XmlNodeList wptNodes;   // Holds all occurrences of waypoints in the loc file

        /// <summary>
        /// Creates a new LOC_Parser object and intializes class variables.
        /// </summary>
        /// <param name="locFile">Location of LOC file.</param>
        public LOC_Parser(string locFile)
        {
            xmlDoc = new XmlDocument();

            xmlDoc.Load(locFile);

            xmlElement = xmlDoc.DocumentElement;

            wptNodes = xmlElement.GetElementsByTagName("waypoint");
        }

        /// <summary>
        /// Parses the next cache in the LOC file
        /// </summary>
        /// <param name="currentCache">Number of selected cache</param>
        public void getCache(int currentCache)
        {
            cacheID = wptNodes[currentCache]["name"].Attributes["id"].InnerText;

            cacheName = wptNodes[currentCache]["name"].InnerText;

            latitude = Convert.ToDouble(wptNodes[currentCache]["coord"].Attributes["lat"].InnerText, CultureInfo.InvariantCulture);

            longitude = Convert.ToDouble(wptNodes[currentCache]["coord"].Attributes["lon"].InnerText, CultureInfo.InvariantCulture);

            cacheURL = wptNodes[currentCache]["link"].InnerText;
        }

        /// <summary>
        /// Returns the number of caches in the file.
        /// <para>(*Use in a loop to iterate through all caches and cache properties.*)</para>
        /// </summary>
        /// <example>Shows you how to call <see cref="numCaches"/> getter.
        /// <code>
        /// GPX_Parser parser = new GPX_Parser("Path To GPX File")
        ///       
        /// for(int i = 0; i (less than symbol) parser.numCaches; i++)
        /// {
        ///     parser.getCache(i);
        ///             
        ///     double Latitude = parser.Latitude;
        ///     etc.....
        /// }
        /// </code>
        /// </example>
        public int numCaches
        {
            get
            {
                return wptNodes.Count;
            }
        }

        /// <summary>
        /// Get the cache ID name.
        /// </summary>
        /// <returns>Geocaching cache ID name</returns>
        public string Cache_ID
        {
            get
            {
                return cacheID;
            }
        }

        /// <summary>
        /// Get the cache name.
        /// </summary>
        /// <returns>Geocaching cache name</returns>
        public string Cache_Name
        {
            get
            {
                return cacheName;
            }
        }

        /// <summary>
        /// Get the latitude coordinates of the cache.
        /// </summary>
        /// <returns>Latitude coordinates of the cache</returns>
        public double Cache_Latitude
        {
            get
            {
                return latitude;
            }
        }

        /// <summary>
        /// Get the longitude coordinates of the cache.
        /// </summary>
        /// <returns>Longitude coordinates of the cache</returns>
        public double Cache_Longitude
        {
            get
            {
                return longitude;
            }
        }

        /// <summary>
        /// Get the cache web link.
        /// </summary>
        /// <returns>Geocaching cache web link</returns>
        public string Cache_URL
        {
            get
            {
                return cacheURL;
            }
        }
    }
}
