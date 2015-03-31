using Meteooor.Helpers;
using Meteooor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Meteooor.Repositories
{
    public static class CraterRepository
    {
        public static List<Crater> GetAll()
        {
            string[] urls = {
                                "http://www.passc.net/EarthImpactDatabase/Africa.html",
                                "http://www.passc.net/EarthImpactDatabase/AsiaRussia.html",
                                "http://www.passc.net/EarthImpactDatabase/Australia.html",
                                "http://www.passc.net/EarthImpactDatabase/Europe.html",
                                "http://www.passc.net/EarthImpactDatabase/NorthAmerica.html",
                                "http://www.passc.net/EarthImpactDatabase/SouthAmerica.html"
                            };

            List<Crater> result = urls.SelectMany(x => CraterRepository.GetCraters(x)).ToList();

            return result;
        }

        public static List<Crater> GetCraters(string url)
        {
            string html = new System.Net.WebClient().DownloadString(url);
            html = html.Replace("\n", "");
            Regex regex = new Regex(@"\<table.*\>(.*)\<\/table\>");

            List<Crater> craters = new List<Crater>();

            foreach (Match match in regex.Matches(html))
            {
                string table = match.Value;

                int skip = 5; // Skip the 5 most entries by default

                var parts = table.Split(new string[] { "<tr>" }, StringSplitOptions.None).Skip(skip);

                StringBuilder sb = new StringBuilder();

                foreach (string part in parts)
                {
                    var columns = part.Split(new string[] { "<td>" }, StringSplitOptions.None);

                    if (columns.Length == 11)
                    {
                        string name = StringHelper.SanitizeString(StringHelper.SanitizeString(columns[1]));
                        string location = StringHelper.SanitizeString(columns[2]);
                        string nCoor = StringHelper.SanitizeString(columns[3]).Replace("&deg;", "°");
                        string eCoor = StringHelper.SanitizeString(columns[4]).Replace("&deg;", "°");
                        double diameter = double.Parse(Regex.Match(StringHelper.SanitizeString(columns[5]), @"[\d\.]+").Value, new CultureInfo("en-US"));

                        craters.Add(new Crater
                            {
                                Name = name,
                                Url = url,
                                Location = location,
                                Latitude = nCoor,
                                Longitude = eCoor,
                                Diameter = diameter
                            });
                    }
                }
            }

            return craters;
        }
    }
}