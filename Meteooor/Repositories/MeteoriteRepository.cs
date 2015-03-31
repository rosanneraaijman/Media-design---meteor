using Meteooor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Meteooor.Repositories
{
    public static class MeteoriteRepository
    {
        private static List<Meteorite> _cache;

        public static List<Meteorite> GetMeteorites(string marketUrl)
        {
            if(_cache != null)
            {
                return _cache;
            }

            List<Meteorite> meteorites = new List<Meteorite>();

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string html = wc.DownloadString(marketUrl);
            html = html.Replace("\n", "");

            string marker = "Clickon the meteorite name to see a photo catalog";
            string endMarker = "Miscellaneous meteorites";

            int markerIndex = html.IndexOf(marker);
            int endMarkerIndex = html.IndexOf(endMarker);
            List<int> startTableIndexes = Regex.Matches(html, "<table").Cast<Match>().Select(m => m.Index).ToList();
            List<int> endTableIndexes = Regex.Matches(html, "</table").Cast<Match>().Select(m => m.Index).ToList();
            startTableIndexes.Add(markerIndex);
            endTableIndexes.Add(endMarkerIndex);
            startTableIndexes.Sort();
            endTableIndexes.Sort();

            int startTableIndex = startTableIndexes.IndexOf(markerIndex) - 1;
            int endTableIndex = endTableIndexes.IndexOf(endMarkerIndex) + 2;

            html = html.Substring(startTableIndexes[startTableIndex], endTableIndexes[endTableIndex] - startTableIndexes[startTableIndex]);

            int skipIndex = 1; // Skip the header row

            string baseUrl = "http://www.meteoritemarket.com/";
            string[] parts = html.Split(new string[] { "<li>" }, StringSplitOptions.None);

            foreach (string part in parts.Skip(skipIndex))
            {
                string url = baseUrl + Regex.Matches(part, "href=\"([a-zA-Z0-9\\.]+)\"").Cast<Match>().Select(x => x.Groups[1].Value).First();

                System.Net.WebClient wc2 = new System.Net.WebClient();
                wc2.Encoding = Encoding.UTF8;

                string pageHtml = wc2.DownloadString(url)
                                    .Replace("\n", "")
                                    .Replace("\r", "")
                                    .Replace("&quot;", "'")
                                    .Replace("&#8242;", "'")
                                    .Replace("º", "'")
                                    .Replace("°", "'")
                                    .Replace("’", "'")
                                    .Replace("′", "'")
                                    .Replace("E</font>", "'E</font>")
                                    .Replace("' E", "'E")
                                    .Replace("' W", "'W")
                                    .Replace("&nbsp;", " ");

                int locationMarker = pageHtml.IndexOf("Location");
                int nameEndMarker = pageHtml.IndexOf("'W");

                if (nameEndMarker < 0)
                {
                    nameEndMarker = pageHtml.IndexOf("'E");
                }

                if (nameEndMarker < 0)
                {
                    continue;
                }

                string name = pageHtml.Substring(locationMarker + "Location".Length, (nameEndMarker - (locationMarker + "Location".Length)) + 2)
                                .Replace(":", "")
                                .Replace(".", " ")
                                .Replace("''", "'")
                                .Replace("</b>", "")
                                .Replace("</font>", "")
                                .Replace("<font size=\"4\">", "")
                                .Replace("font size=\"4\"", "")
                                .Replace("<strong>", "")
                                .Replace("</strong>", "")
                                .Replace("<br style=\"font-weight normal;\" />", "")
                                .Replace("<span style=\"font-weight normal;\">", "")
                                .Replace("</span>", "")
                                .Replace(">", "")
                                .Replace("<", "")
                                .Trim();

                name = RemoveExtraSpaces(name);
                Tuple<string, string> coords = GetCoords(name);
                name = Regex.Replace(name, @"\d", "")
                        .Replace("(", "")
                        .Replace("'", "")
                        .Replace(" N", "")
                        .Replace(" S", "")
                        .Replace(" W", "")
                        .Replace(" E", "")
                        .Replace(" ,", "")
                        .Replace("~", "")
                        .Trim();

                var prices = GetPrices(pageHtml);

                meteorites.Add(new Meteorite
                    {
                        Name = name,
                        Url = url,
                        Prices = prices,
                        Latitude = coords.Item1,
                        Longitude = coords.Item2
                    });
            }

            _cache = meteorites;

            return meteorites;
        }

        static List<PriceModel> GetPrices(string input)
        {
            List<PriceModel> prs = new List<PriceModel>();

            List<int> startTableIndexes = AllIndeciesOf(input, "<table");
            List<int> endTableIndexes = AllIndeciesOf(input, "</table>");

            for (int i = 0; i < startTableIndexes.Count; i++)
            {
                string table = input.Substring(startTableIndexes[i], endTableIndexes[i] - startTableIndexes[i] + 8);

                var startTr = AllIndeciesOf(table, "<tr");
                if (startTr.Count <= 2)
                {
                    continue;
                }

                List<int> endTr = AllIndeciesOf(table, "</tr>");

                string pricesRow = table.Substring(startTr.Last(), endTr.Last() - startTr.Last() + 5);

                int[] startTds = AllIndeciesOf(pricesRow, "<td").ToArray();
                int[] endTds = AllIndeciesOf(pricesRow, "</td>").ToArray();

                List<string> prices = new List<string>();

                for (int j = 0; j < startTds.Length; j++)
                {
                    string td = pricesRow.Substring(startTds[j] + 4, (endTds[j] - startTds[j]) - 4)
                                    .Replace("<font size=\"4\">", "")
                                    .Replace("</font>", "")
                                    .Replace("<p>", "")
                                    .Replace("</p>", "")
                                    .Replace("<font size=\"3\">", "")
                                    .Replace("/gm 10gm minimum", "")
                                    .Replace("/g, $25 minimum", "")
                                    .Replace(" (hold)", "")
                                    .Replace("height=\"22\">", "")
                                    .Replace("<strong>", "")
                                    .Replace("colspan=\"2\">", "")
                                    .Replace("/g--$20 min.", "")
                                    .Replace("</strong>", "")
                                    .Replace("$1.50 /gm $25 minimum order.", "")
                                    .Replace("Price:<strong>", "")
                                    .Replace("/g 10g minimum", "")
                                    .Replace("(hold)", "")
                                    .Replace("/g, 10g minimum", "")
                                    .Replace("each", "")
                                    .Trim();

                    if (td.Contains("Price") || td.Contains("$") || td.Contains("sold"))
                    {
                        prices.Add(td.Replace("Price: ", ""));
                    }
                }

                string weightRow = table.Substring(startTr.ElementAt(2), endTr.ElementAt(2) - startTr.ElementAt(2));

                int[] startWeightTds = AllIndeciesOf(weightRow, "<td").ToArray();
                int[] endWeightTds = AllIndeciesOf(weightRow, "</td>").ToArray();

                List<string> weights = new List<string>();

                for (int j = 0; j < startWeightTds.Length; j++)
                {
                    string td = weightRow.Substring(startWeightTds[j] + 4, (endWeightTds[j] - startWeightTds[j]) - 4)
                                .Replace("Weight: <strong>", "")
                                .Replace("Weight:", "")
                                .Replace("Weight:<strong>", "")
                                .Replace("Weight<strong>:", "")
                                .Replace("</sup>", "")
                                .Replace("Weight; ", "")
                                .Replace("<p>", "")
                                .Replace("</p>", "")
                                .Replace("Weight:<span style=\"font-weight: bold;\">", "")
                                .Replace("<span style=\"font-weight: bold;\">", "")
                                .Replace("</span><strong>", "")
                                .Replace("to ??g", "gm")
                                .Replace("CD", "")
                                .Replace("<strong>", "")
                                .Replace("Weight: ", "")
                                .Replace("<font size=\"4\">", "")
                                .Replace("</font>", "")
                                .Replace("Appx ", "")
                                .Replace(" to 5", "")
                                .Replace("Weight: <span style=\"font-weight: bold;\">", "")
                                .Replace("colspan=\"2\"># IC-micro", "")
                                .Replace("# IC1-02", "")
                                .Replace("#IC1-23", "")
                                .Replace("This is a suggestion for a gift or presentation box.  The box is shown with a 60g Sikhote-Alin individual.", "")
                                .Replace("# IC1-213", "")
                                .Replace("Weight ", "")
                                .Replace("</strong>", "")
                                .Trim();

                    if (td.Contains(" small one centimeter ") || td.Contains("valign=top"))
                    {
                        td = string.Empty;
                    }

                    weights.Add(td);
                }

                for (int price = 0; price < prices.Count; price++)
                {
                    prs.Add(new PriceModel
                        {
                            Price = prices.ElementAt(price),
                            Weight = weights.ElementAt(price)
                        });
                }
            }

            return prs;
        }

        static List<int> AllIndeciesOf(string input, string keyword)
        {
            return Regex.Matches(input, keyword).Cast<Match>().Select(m => m.Index).ToList();
        }

        static Tuple<string, string> GetCoords(string input)
        {
            Regex coor1Regex = new Regex(@"(\d{1,3}[']?[ ]?){1,3}[NS]");
            Regex coor2Regex = new Regex(@"(\d{1,3}[']?[ ]?){1,3}[']?[WE]");

            string coor1 = coor1Regex.Match(input).Value;
            string coor2 = coor2Regex.Match(input).Value;

            if (string.IsNullOrEmpty(coor1) || string.IsNullOrEmpty(coor2))
            {
                // Hmm..
                return null;
            }

            string leftPart = ParseCoordinate(coor1, "N", "S");
            string rightPart = ParseCoordinate(coor2, "E", "W");

            return new Tuple<string, string>(leftPart, rightPart);
        }

        static string ParseCoordinate(string input, string hemisphere1, string hemisphere2)
        {
            string hemisphere = input.Substring(input.Length - 1, 1);

            if (hemisphere != hemisphere1 && hemisphere != hemisphere2)
            {
                return string.Empty;
            }

            Regex degRegex = new Regex(@"^(\d+) (\d+)");

            Match match = degRegex.Match(input);

            int degrees = 0;
            int minutes = 0;

            if (match.Success)
            {
                degrees = int.Parse(match.Groups[1].Value);
                minutes = int.Parse(match.Groups[2].Value);
            }
            else
            {
                string[] coorParts = input.Split('\'');
                degrees = int.Parse(coorParts[0]);

                string[] minuteParts = coorParts[1].Trim().Replace(hemisphere, "").Split(' ');

                if (minuteParts.Length > 0 && !string.IsNullOrEmpty(minuteParts[0]))
                {
                    minutes = int.Parse(minuteParts[0]);
                }
            }

            return string.Format("{0} {1}° {2}'", hemisphere, degrees, minutes);
        }

        static string RemoveExtraSpaces(string input)
        {
            return string.Join(" ", input.Split(' ').Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}