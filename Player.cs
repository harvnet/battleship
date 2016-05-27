using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Player
    {
        protected string name;
        protected Byte numOfShips;


        public Player( String name, Byte numOfShips)
        {
            this.name = name;
            this.numOfShips = numOfShips;
        }

        public bool setName(string name)
        {
            this.name = name;
            return true;
        }

        public string getName()
        {
            return name;
        }

        public bool setNumOfShips(Byte numOfShips)
        {
            this.numOfShips = numOfShips;
            return true;
        }

        public Byte getNumOfShips()
        {
            return numOfShips;
        }

    }
}
