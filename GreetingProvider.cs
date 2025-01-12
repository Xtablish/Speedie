using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedie
{
    public class GreetingProvider
    {
        public static string GetGreeting()
        {
            // Get the current hour
            int hour = DateTime.Now.Hour;

            // Determine the appropriate greeting
            if (hour >= 5 && hour < 12)
            {
                return "Good Morning";
            }
            else if (hour >= 12 && hour < 17)
            {
                return "Good Afternoon";
            }
            else if (hour >= 17 && hour < 21)
            {
                return "Good Evening";
            }
            else
            {
                return "Good Night";
            }
        }
    }
}
