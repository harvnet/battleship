using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    sealed class RangedRandomNumber : RandomNumber
    {
        private int minimum;   //lowest number the random object should create
        private int maximum;   //lowest number the random object should create

        public RangedRandomNumber()  //Default Constructor
        {
            minimum = 1;
            maximum = 10;
        }

        public RangedRandomNumber(int minimum, int maximum)  //Overloaded Constructor
        {
            this.maximum = maximum;
            this.minimum = minimum;
        }

        public new int GenerateRandomNumber()   // Overriden method
        {
            if (maximum < minimum)
            {
                int swap = maximum;  // swap is used as a placeholder
                maximum = minimum;
                minimum = swap;
            }
            else if (maximum == minimum)
            {
                return minimum;
            }

            int currentRandomNumber = random.Next(minimum, maximum + 1);
            // give us a random number between minimum and maximum + 1
            return currentRandomNumber;
        }

        //Getters

        public int GetMinimum() { return minimum; }
        public int GetMaximum() { return maximum; }

        //Setters

        public void SetMinimum(int minimum)
        {
            if (minimum < 0)
            {
                minimum = 0;
            }
            this.minimum = minimum;
        }

        public void SetMaximum(int maximum)
        {
            if (maximum < 1)
            {
                maximum = 1;
            }
            this.maximum = maximum;
        }

        public void SetMinMax(int minimum, int maximum)
        {
            if (minimum < 0)
            {
                minimum = 0;
            }
            if (maximum < 1)
            {
                maximum = 1;
            }
            if (maximum < minimum)
            {
                int swap = maximum;
                maximum = minimum;
                minimum = swap;
            }
            this.maximum = maximum;
            this.minimum = minimum;
        }
    }
}
