using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerHouse_API_Testing_Automation.AppManager
{
    public class CalculateBid
    {
        public int GetMaxBid(int initialBid )
        {
            decimal markUpPercentage = 0.35m;
            decimal markUpPrice = initialBid * markUpPercentage;
            decimal marketPlaceBidPrice = initialBid - markUpPrice;
            decimal maxbidPercentage = 0.10m;
            decimal newBidPrice = marketPlaceBidPrice -( maxbidPercentage * marketPlaceBidPrice);
            return Convert.ToInt32(newBidPrice);
        }

    }
}
