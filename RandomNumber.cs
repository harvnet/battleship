using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class RandomNumber
    {
        protected Random random;  // set up a random object
        protected int currentRandomNumber;  // initialize a random number

        public RandomNumber()   // Prepares a random number
        {
            currentRandomNumber = 0;
            random = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
        }

        public int GenerateRandomNumber()   // Generates a random number
        {
            currentRandomNumber = random.Next();   // Random number up to 2 billion
            return currentRandomNumber;
        }

        public int GetCurrentRandomNumber() { return currentRandomNumber; }
    }
}
