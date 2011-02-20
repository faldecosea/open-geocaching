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
    public class GPX_Parser
    {
        // Create variables
        private double latitude;            // Cache latitude coordinates
        private double longitude;           // Cache longitude coordinates
        private string datePlaced;          // Date cache was placed
        private string cacheID;             // Cache ID number
        private string description;         // Cache description
        private string cacheURL;            // Cache url
        private string cacheArchived;       // Is cache archived
        private string cacheName;           // Cache name
        private string placedBy;            // Who placed the cache
        private string cacheType;           // What type of cache is it
        private string cacheContainer;      // Type of container
        private string difficulty;          // How difficult is it to find the cache
        private string terrain;             // How bad is the terrain to get to the cache
        private string country;             // What country is the cache located
        private string state;               // What state is the cache located
        private string shortDescription;    // Short description of the cache
        private string longDescription;     // Long description of the cache
        private string hints;               // Hints to find the cache
        private string[] cacheLogs;         // Cache logs
        private string[] travelBugs;        // Travel bugs in cache container
        
        private XmlDocument doc;            // Holds the xml document
        private XmlElement element;         // Gets the specified element in the xml file
        private XmlNodeList wptNodes;       // Holds all occurrences of waypoints in the gpx file
        private XmlNodeList logNodes;       // Holds all occurrences of logs in the gpx file
        private XmlNodeList travelBugNodes; // Holds all occurrences of travel bugs in the gpx file

        private int numLogs;                // Number of logs in the cache
        private int numBugs;                // Number of travel bugs in the cache
        private int currentIndex = 0;       // Keeps the current index for cacheLogs and travelBugs
        private int wptIndex = 0;           // Keeps the current index for waypoints

        /// <summary>
        /// Creates a new GPX class and parses the specified gpx file
        /// </summary>
        /// <param name="gpxFile">The path to the gpx file</param>
        public GPX_Parser(string gpxFile, bool pocketQuery)
        {
            // Create a new xml document
            doc = new XmlDocument();
            // Load xml file into doc
            doc.Load(gpxFile);
            // Create a new xml element
            element = doc.DocumentElement;

            // Get all the occurrences of wpt in the xml file
            wptNodes = element.GetElementsByTagName("wpt");

            if (pocketQuery == false)
            {
                parseFile(0);
            }
        }

        private void parseFile(int currentWPT)
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
            latitude = Convert.ToDouble(wptNodes[currentWPT].Attributes["lat"].InnerText, CultureInfo.InvariantCulture);

            // Get the longitude of the cache, apply the current local number format and convert it to a double
            // CultureInfo.InvariantCulture applies the users current number format based on their locale
            longitude = Convert.ToDouble(wptNodes[currentWPT].Attributes["lon"].InnerText, CultureInfo.InvariantCulture);

            // Get the date the cache was placed
            datePlaced = wptNodes[currentWPT]["time"].InnerText.Remove(wptNodes[currentWPT]["time"].InnerText.IndexOf('T'));

            // Get the ID that geocaching.com uses to uniquely identify the cache
            cacheID = wptNodes[currentWPT]["name"].InnerText;

            // Get the cache description
            description = wptNodes[currentWPT]["desc"].InnerText;

            // Get the direct url to the cache on geocaching.com
            cacheURL = wptNodes[currentWPT]["url"].InnerText;

            // Get whether or not the cache is currently archived
            cacheArchived = wptNodes[currentWPT]["groundspeak:cache"].Attributes["archived"].InnerText;

            // Get the cache name
            cacheName = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:name"].InnerText;

            // Get the user who placed the cache
            placedBy = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:placed_by"].InnerText;

            // Get the type of cache it is
            cacheType = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:type"].InnerText;

            // Get the type of container the cache is
            cacheContainer = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:container"].InnerText;

            // Get the difficulty of how hard it is to find the cache
            difficulty = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:difficulty"].InnerText;

            // Get the terrain of how hard it is to get to the cache
            terrain = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:terrain"].InnerText;

            // Get the country where the geocache is located
            country = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:country"].InnerText;

            // Get the state where the geocache is located
            state = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:state"].InnerText;

            // Get the short description of the cache
            shortDescription = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:short_description"].InnerText;

            // Get the long description of the cache
            longDescription = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:long_description"].InnerText;

            // Get the hints of the cache
            hints = wptNodes[currentWPT]["groundspeak:cache"]["groundspeak:encoded_hints"].InnerText;

            // Checks whether the number of logs is greater than 0
            if (numLogs > 0)
            {
                // Initialize the cacheLogs string array based on number of logs
                cacheLogs = new string[numLogs];

                // Set currentIndex to 0
                currentIndex = 0;

                // Loop through each occurrence of a log
                for (int i = 0; i < logNodes.Count; i++)
                {
                    // Get the date the log was placed
                    cacheLogs[currentIndex] = logNodes[i]["groundspeak:date"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;

                    // Get the type of log (i.e. found, not found)
                    cacheLogs[currentIndex] = logNodes[i]["groundspeak:type"].InnerText;

                    // Increase the currentIndex by 1
                    currentIndex++;

                    // Get the user who wrote the log
                    cacheLogs[currentIndex] = logNodes[i]["groundspeak:finder"].InnerText;

                    // Increase the currentIndex by 1
                    currentIndex++;

                    // Get the text the user wrote for the log
                    cacheLogs[currentIndex] = logNodes[i]["groundspeak:text"].InnerText;

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
                cacheLogs = new string[1];

                // Set the text of the only string in the array to No logs available
                cacheLogs[0] = "No logs available";
            }

            // Check whether the number of travel bugs is greater than 0
            if (numBugs > 0)
            {
                // Initialize the travelBugs array based on number of bugs
                travelBugs = new string[numBugs];

                // Set currentIndex to 0
                currentIndex = 0;

                // Loop through each occurence of a travelbug in the cache
                for (int i = 0; i < travelBugNodes.Count; i++)
                {
                    // Get the reference number of the travelbug
                    travelBugs[currentIndex] = travelBugNodes[i].Attributes["ref"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;

                    // Get the name of the travelbug
                    travelBugs[currentIndex] = travelBugNodes[i]["groundspeak:name"].InnerText;

                    // Increase currentIndex by 1
                    currentIndex++;
                }
            }
            // If the number of bugs are not greater than 0 then
            // there are no travelbugs in the cache
            else
            {
                // Intialize travelBugs array to 1 string
                travelBugs = new string[1];

                // Set the text to the only string in the array to No travel bugs available
                travelBugs[0] = "No travel bugs available";
            }
        }

        /// <summary>
        /// Checks to see if another cache exists and parses it.
        /// </summary>
        /// <returns>Returns number of caches left to parse</returns>
        public int getNextCache()
        {
            if (wptIndex < wptNodes.Count)
            {
                parseFile(wptIndex);
                wptIndex++;
                if (wptNodes.Count - wptIndex <= 0)
                {
                    return 1;
                }
                else
                {
                    return wptNodes.Count - wptIndex;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the latitude of the cache
        /// </summary>
        /// <returns>Returns a double</returns>
        public double getLatitude()
        {
            return latitude;
        }

        /// <summary>
        /// Gets the longitude of the cache
        /// </summary>
        /// <returns>Returns a double</returns>
        public double getLongitude()
        {
            return longitude;
        }

        /// <summary>
        /// Get the date the cache was placed
        /// </summary>
        /// <returns>Returns a string in (yyyy:mm:dd) format</returns>
        public string getDatePlaced()
        {
            return datePlaced;
        }

        /// <summary>
        /// Get the cache ID number
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getCacheID()
        {
            return cacheID;
        }

        /// <summary>
        /// Gets the description of the cache
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getDescription()
        {
            return description;
        }

        /// <summary>
        /// Get the name of the cache
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getCacheName()
        {
            return cacheName;
        }

        /// <summary>
        /// Get the person who placed the cache
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getPlacedBy()
        {
            return placedBy;
        }

        /// <summary>
        /// Get the type of cache
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getCacheType()
        {
            return cacheType;
        }

        /// <summary>
        /// Get the type of container the cache is
        /// </summary>
        /// <returns>Returns a string</returns>
        public string getCacheContainer()
        {
            return cacheContainer;
        }

        /// <summary>
        /// Get the difficulty of the cache
        /// </summary>
        /// <returns>Return a string</returns>
        public string getDifficulty()
        {
            return difficulty;
        }

        /// <summary>
        /// Get the terrain of the cache
        /// </summary>
        /// <returns>Return a string</returns>
        public string getTerrain()
        {
            return terrain;
        }

        /// <summary>
        /// Get the country the cache is located in
        /// </summary>
        /// <returns>Return a string</returns>
        public string getCountry()
        {
            return country;
        }

        /// <summary>
        /// Get the state the cache is located in
        /// </summary>
        /// <returns>Return a string</returns>
        public string getState()
        {
            return state;
        }

        /// <summary>
        /// Get the short descripton of the cache
        /// </summary>
        /// <returns>Return a sring</returns>
        public string getShortDescription()
        {
            return shortDescription;
        }

        /// <summary>
        /// Get the long description of the cache
        /// </summary>
        /// <returns>Return a string</returns>
        public string getLongDescription()
        {
            return longDescription;
        }

        /// <summary>
        /// Get the hints of the cache
        /// </summary>
        /// <returns>Return a string</returns>
        public string getHints()
        {
            return hints;
        }

        /// <summary>
        /// Get the logs in the cache
        /// </summary>
        /// <returns>Return a string array</returns>
        public string[] getCacheLogs()
        {
            return cacheLogs;
        }

        /// <summary>
        /// Get the travel bugs in the cache
        /// </summary>
        /// <returns>Return a string array</returns>
        public string[] getTravelBugs()
        {
            return travelBugs;
        }
    }
}
