using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteooor.Models
{
    public class PriceModel
    {
        public string Price { get; set; }
        public string Weight { get; set; }

        public decimal DecimalPrice
        {
            get
            {
                if (Price.Equals("sold", StringComparison.InvariantCultureIgnoreCase) || !Price.Contains("$"))
                {
                    return -1;
                }
                else
                {
                    return decimal.Parse(Price.Replace("$", "").Trim());
                }
            }
        }

        public double DecimalWeight
        {
            get
            {
                bool isMgr = Weight.Contains("mg");

                string weight = Weight.Replace("gm", "")
                                .Replace("~", "")
                                .Replace("mg", "")
                                .Replace("g", "");

                if(string.IsNullOrEmpty(weight))
                {
                    return -1;
                }

                if (isMgr)
                {
                    return double.Parse(weight) / 1000;
                }

                return double.Parse(weight);
            }
        }

        public decimal PricePerGram
        {
            get
            {
                if(DecimalPrice > -1 && DecimalWeight > -1)
                {
                    return DecimalPrice / (decimal)DecimalWeight;
                }

                return -1;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Price, Weight);
        }
    }
}
