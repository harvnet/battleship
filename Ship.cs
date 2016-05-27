using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Battleship
{
    class Ship
    {
        private RangedRandomNumber shipSizeGenerator;
        private RangedRandomNumber xCoordGenerator;
        private RangedRandomNumber yCoordGenerator;
        private RangedRandomNumber directionGenerator;
       
        private String shipClassification;
        private String firstCoordinates;
        private String secondCoordinates;
        private int numberOfHits;
        private Boolean isItSunk;
        
        private Byte yCoord = 0;
        private Byte xCoord = 0;
        private Byte direction = 0;
        private String secondCoord;
        String[] availOptions = new String[4];

        Boolean placeShipFlag = true;
        String[,] placeholderBoard;
        String[] shipCoord = new String[6];

        private int fX;
        private int fY;
        private int sX;
        private int sY;
        /**@author Paul Harvey
         * 
         * A minimum of 2 ships per player and a maximum of 5 ships.
         * 
         * @param shipSize
         * @param fCoord
         * @param sCoord
         * @param hits
         * @param sunk
         */
        public Ship()
        {

            shipSizeGenerator = new RangedRandomNumber();
            xCoordGenerator = new RangedRandomNumber();
            yCoordGenerator = new RangedRandomNumber();
            directionGenerator = new RangedRandomNumber();
        }

        /**@author Paul Harvey
   * 
   *  Set the ship size.  Between 2 and 6.	
   */
        public Byte setShipSize(Byte min, Byte max)
        {
           
            shipSizeGenerator.SetMinimum(min);
            shipSizeGenerator.SetMaximum(max);
            Byte size = Convert.ToByte(shipSizeGenerator.GenerateRandomNumber());

            return size;
            
        }

        /**@author Paul Harvey
        * 
        *  Set the first position of the boat.  The human will require coordinate 
        *  validation.  The computer will not.
        * 	
        * @param playerNum
        */

        public String setFirstPosition()
       {
            xCoordGenerator.SetMinimum(0);
            xCoordGenerator.SetMaximum(9);
            xCoord = Convert.ToByte(xCoordGenerator.GenerateRandomNumber());
            yCoordGenerator.SetMinimum(0);
            yCoordGenerator.SetMaximum(9);
            yCoord = Convert.ToByte(yCoordGenerator.GenerateRandomNumber());

            firstCoordinates = "" + xCoord.ToString() + yCoord.ToString();

            return firstCoordinates;
        }

        public String setSecondPosition(Byte size, String coord, String[,] board)
        {

            String[,] placeholderBoard = board;
            Boolean errorFlag = false;
            /*
            *  giveOptions: procedure that chooses up to 4 available options.  
            *  The computer user will randomly choose the second position.
            */
            giveOptions(size, coord, placeholderBoard);

        
            Boolean LoopBreak = true;
            do
            {
                    directionGenerator.SetMinimum(0);
                    directionGenerator.SetMaximum(3);
                    direction = Convert.ToByte(directionGenerator.GenerateRandomNumber());
                Debug.WriteLine("availOptions[" + direction + "] =" + availOptions[direction]);
                if (availOptions[direction].Equals("XX"))
                {
                    // dont break loop
                    LoopBreak = true;
                }
                else
                {
                    secondCoord = availOptions[direction];
                    LoopBreak = false;
                }
                if (availOptions[0].Equals("XX") && availOptions[1].Equals("XX") && availOptions[2].Equals("XX") && availOptions[2].Equals("XX"))
                {
                    // God forbid we end up in an endless loop.
                    secondCoord = "XX";
                    LoopBreak = false;
                    errorFlag = true;
                }
                
                Debug.WriteLine("Ship Class - Setting Direction");
            } while (LoopBreak == true);

            // peel out the individual coordinates

            if (!errorFlag)
            {
                Char xcharCoord = secondCoord[0];
                Char ycharCoord = secondCoord[1];

                secondCoordinates = "" + xcharCoord.ToString() + ycharCoord.ToString();

            }
            return secondCoordinates; 
        }

        public void giveOptions(int size, String coord, String[,] board)
        {
            placeholderBoard = board;
            Byte x = 0;
            Byte y = 0;
            //Convert string coord to integers

            Char xchar = coord[0];
            Char ychar = coord[1];

            //int x = Character.digit(coord.charAt(0), 10);

            Byte.TryParse(xchar.ToString(), out x);

            //int y = Character.digit(coord.charAt(1), 10);

            Byte.TryParse(ychar.ToString(), out y);

            for (int options = 0; options < 4; ++options)
            {
                switch (options)
                {
                    case 0: //UP
                        if ((y - size + 1) < 0)
                        {
                            //option bad run into a wall
                            availOptions[0] = "XX";
                        }
                        else
                        {
                            //possible option but check for overlap
                            Boolean noOverlap1 = true;
                            noOverlap1 = optionOverlap(x, x, (y - size + 1), y, placeholderBoard);
                            if (noOverlap1 == true)
                            {
                                availOptions[0] = "" + x + (y - size + 1);
                            }
                            else
                            {
                                availOptions[0] = "XX";
                            }
                        }
                        break;
                    case 1: //DOWN
                        if ((y + size - 1) > 9)
                        {
                            //option bad run into a wall
                            availOptions[1] = "XX";
                        }
                        else
                        {
                            // possible option but check for overlap
                            Boolean noOverlap2 = true;
                            noOverlap2 = optionOverlap(x, x, y, (y + size - 1), placeholderBoard);
                            if (noOverlap2 == true)
                            {
                                availOptions[1] = "" + x + (y + size - 1);
                            }
                            else
                            {
                                availOptions[1] = "XX";
                            }
                        }
                        break;
                    case 2: // LEFT
                        if ((x - size + 1) < 0)
                        {
                            //option bad run into a wall
                            availOptions[2] = "XX";
                        }
                        else
                        {
                            //possible option but check for overlap
                            Boolean noOverlap3 = true;
                            noOverlap3 = optionOverlap((x - size + 1), x, y, y, placeholderBoard);
                            if (noOverlap3 == true)
                            {
                                availOptions[2] = "" + (x - size + 1) + y;
                            }
                            else
                            {
                                availOptions[2] = "XX";
                            }
                        }
                        break;
                    case 3: //RIGHT 
                        if ((x + size - 1) > 9)
                        {
                            // option bad run into a wall
                            availOptions[3] = "XX";
                        }
                        else
                        {
                            // possible option but check for overlap
                            Boolean noOverlap4 = true;
                            noOverlap4 = optionOverlap(x, (x + size - 1), y, y, placeholderBoard);
                            if (noOverlap4 == true)
                            {
                                availOptions[3] = "" + (x + size - 1) + y;
                            }
                            else
                            {
                                availOptions[3] = "XX";
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public Boolean optionOverlap(int xMin, int xMax, int yMin, int YMax, String[,] board)
        {
            Boolean goodOption = true;
            String[,] placeholderBoard = board;

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= YMax; y++)
                {
                    if (placeholderBoard[x, y] == "#" || placeholderBoard[x, y] == "@" || placeholderBoard[x, y] == "X")
                    {
                        goodOption = false;
                    }
                }
            }

            return goodOption;
        }

        public Boolean checkOverlap(String firstCoord, String secondCoord, String[,] board)
        {

            placeholderBoard = board;


            rearrangeXY(firstCoord, secondCoord);

            placeShipFlag = true;
            for (int x = fX; x <= sX; ++x)
            {
                for (int y = fY; y <= sY; ++y)
                {

                    if (placeholderBoard[x, y] == "#" || placeholderBoard[x, y] == "@" || placeholderBoard[x, y] == "X")
                    {
                        // ship found at this location - do not place
                        placeShipFlag = false; // as soon as this is tripped then the ship is scrapped.
                    }
                    else
                    {
                        //nothing
                    }
                }
            }

            return placeShipFlag;
        }

        public void rearrangeXY(String first, String second)
        {


            Coordinates firsttest = new Coordinates(first);
            firsttest.setXCoordinate(first);
            int firstX = firsttest.getXCoordinate();
            firsttest.setYCoordinate(first);
            int firstY = firsttest.getYCoordinate();

            Coordinates secondtest = new Coordinates(second);
            secondtest.setXCoordinate(second);
            int secondX = secondtest.getXCoordinate();
            secondtest.setYCoordinate(second);
            int secondY = secondtest.getYCoordinate();

            if (firstX > secondX)
            {    // ship pointed left
                fX = secondX;
                sX = firstX;
            }
            else
            {               // ship pointed right
                fX = firstX;
                sX = secondX;
            }

            if (firstY > secondY)
            {  // ship pointed up
                fY = secondY;
                sY = firstY;
            }
            else
            {               // ship pointed down
                fY = firstY;
                sY = secondY;
            }
        }

        public String[,] placeShip(String firstCoord, String secondCoord)
        {
            
           
            


            rearrangeXY(firstCoord, secondCoord);
            for (int x = fX; x <= sX; ++x)
            {
                for (int y = fY; y <= sY; ++y)
                {
                    placeholderBoard[x, y] = "#";
                }
            }
            return placeholderBoard;
        }

        public void setShipNumber(int shipNo)
        {
            

            rearrangeXY(firstCoordinates, secondCoordinates);

            shipCoord[shipNo] = "" + fX + fY;  // try not to pad first value with null

            if (fX == sX)
            {
                fY++;  // needs to be increased by one so there isnt a double
            }
            else
            {
                fX++;  // needs to be increased by one so there isnt a double
            }
            for (int x = fX; x <= sX; ++x)
            {
                for (int y = fY; y <= sY; ++y)
                {
                    shipCoord[shipNo] = shipCoord[shipNo] + x + y;
                }
            }
        }

       
        /**@author Paul Harvey
         * 
         * This increments the hit counter for the ship that has been hit.
         * 	
         */
        public void setHits()
        {
            numberOfHits++;
        }
        
        /**@author Paul Harvey
         * 
         * This will return the current ship hit counter
         * 	
         * @return int numberOfHits
         */
        public int getHits()
        {
            return numberOfHits;
        }
        
        /**@author Paul Harvey
         * 
         * This calculates from the numberOfHits if the current ship has been sunk.
         * And if so will mark it as such.
         */
        public void setSunk(Byte shipSize)
        {
            if (numberOfHits == shipSize)
            {
                isItSunk = true;
            }
        }
        
        /**@author Paul Harvey
         * 
         *  Determines if the ship is sunk and will give a "typical" taunting cheer.
         *  
         * @return boolean isItSunk
         */
        public Boolean getSunk()
        {
            
            if (isItSunk == true)
            {
                // Display something in the output screen

                //System.out.println("YOU! sank our " + this.getShipClassification());
            }
            return isItSunk;
        }
        
        
        /**@author Paul Harvey
         * 
         * Returns all of the Coordinates for the ShipNumber
         * 
         * @return String[] shipCoord
         */
           public String[] getShipNumber()
           {

               return shipCoord;
           }
           
        /**@author Paul Harvey
         * 
         *  Returns the ship Classification or type (ex. Battleship) 
         * 	
         * @return String shipClassification
         */
        
       public String getShipClassification()
       {
           return shipClassification;
       }
       
        /**@author Paul Harvey
         * 
         *  Classification will set the size to the proper class type. 
         * 
         */
        
       public void setShipClassification(Byte shipSize)
       {
           switch (shipSize)
           {
               case 2:
                   shipClassification = "Destroyer";
                   break;
               case 3:
                   shipClassification = "Cruiser";
                   break;
               case 4:
                   shipClassification = "Submarine";
                   break;
               case 5:
                   shipClassification = "Battleship";
                   break;
               case 6:
                   shipClassification = "Carrier";
                   break;
               default:
                   shipClassification = "Dingy";
                   break;
           }
       }
       
 }


 }

