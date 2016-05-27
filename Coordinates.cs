using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Coordinates
    {
        private Byte xCoord;
        private Byte yCoord;
        private String sCoord;
        private String cCoord;
        private String[,] Array = new String[5,6];

        public Coordinates(String coord)
        {
            cCoord = coord;
        }
        /**@author Paul Harvey
         * 
         * Coordinates constructor called with integer value arguments
         * 	
         * @param x
         * @param y
         */
        public Coordinates(Byte x, Byte y)
        {
            xCoord = x;
            yCoord = y;
        }

        /**@author Paul Harvey
         * 
         * Used to convert a string value to a integer.
         * Typically used in a routine like target lock where the value
         * is a number on the board
         * 	
         * @param value
         */
        public void setXCoordinate(String value)
        {
            Char xCoordToParse = value[0];
            Byte.TryParse(xCoordToParse.ToString(), out xCoord);
            
            //xCoord = Character.digit(value.charAt(0), 10);
        }
        /**@author Paul Harvey
         * 
         * This return is the true X value in the case of A1 the result would 
         * be 0. 	
         * @return int xCoord
         */
        public Byte getXCoordinate()
        {
            return xCoord;
        }

        /**@author Paul Harvey
         * This return value is the false X value - so instead of 0 the 
         * value returned would be 1.  This would go with getYStringCoordinate.
         * @return int (xCoord+1)
         */
        public Byte displayXCoordinate()
        {
            xCoord++;
            return xCoord;
        }
        /**@author Paul Harvey
         * This return value is the true Y value.  The Y value return will be 
         * an integer and not a string.  A will not be returned, 0 will be 
         * the result. 
         * 
         * @param value 
         */
        public void setYCoordinate(String value)
        {
            Char yCoordToParse = value[1];
            Byte.TryParse(yCoordToParse.ToString(), out yCoord);
            //yCoord = Character.digit(value.charAt(1), 10);
        }
        /**@author Paul Harvey
         * This return is the true Y value in the case of A1 the result would 
         * be 0
         * @return int yCoord
         */
        public Byte getYCoordinate()
        {
            return yCoord;
        }
        /**@author Paul Harvey
         * This procedure sets the Y coordinate String value.  For instance if
         * the Class is set up with a coordinate of 3,4 : x=3, y=4.  Then 4 will
         * be converted to a Y value of E. 
         * 	
         */
        public void setXStringCoordinate()
        {

            switch (xCoord)
            {
                case 0:
                    sCoord = "A";
                    break;
                case 1:
                    sCoord = "B";
                    break;
                case 2:
                    sCoord = "C";
                    break;
                case 3:
                    sCoord = "D";
                    break;
                case 4:
                    sCoord = "E";
                    break;
                case 5:
                    sCoord = "F";
                    break;
                case 6:
                    sCoord = "G";
                    break;
                case 7:
                    sCoord = "H";
                    break;
                case 8:
                    sCoord = "I";
                    break;
                case 9:
                    sCoord = "J";
                    break;
                default:
                    break;
            }
        }
        /**@author Paul Harvey
         * 
         * The Y String value will be returned.  In my example the value E will be
         * returned.  This is most commonly paired with displayXCoordinate().
         * 	
         * @return String sCoord
         */
        public String getXStringCoordinate()
        {
            return sCoord;
        }
        /**@author Paul Harvey
         * 	
         * The setInputtedXCoordinate is used mainly during the input stage 
         * for validation and for conversion.  The user enters B3 - the computer 
         * typically doesn't work in strings it works in numbers so B3 needs to be 
         * converted to it's game board number.  Here we take on just the X number 
         * or the second number (3).  3 will become a 2.
         * 
         * Validation is checked too.  If someone entered a funny number then it will
         * fall through the case and the result will be a -1.  This will turn into an
         * error flag and tell the person to enter another better number.
         * 
         * @param value
         */
        public void setInputtedYCoordinate(String value)
        {
            String testx = "";

            if (value.Length == 3) {
                testx = value[1].ToString() + value[2].ToString();
            }
            else
            {
                testx = value[1].ToString();
            }

            switch (testx)
            {
                case "1":
                    yCoord = 0;
                    break;
                case "2":
                    yCoord = 1;
                    break;
                case "3":
                    yCoord = 2;
                    break;
                case "4":
                    yCoord = 3;
                    break;
                case "5":
                    yCoord = 4;
                    break;
                case "6":
                    yCoord = 5;
                    break;
                case "7":
                    yCoord = 6;
                    break;
                case "8":
                    yCoord = 7;
                    break;
                case "9":
                    yCoord = 8;
                    break;
                case "10":
                    yCoord = 9;
                    break;
                default:
                    yCoord = 10;
                    break;
            }
        }
        /**@author Paul Harvey
         * 
         * This return will pass the converted Integer value back
         * 
         * @return int xCoord
         */
        public Byte getInputtedYCoordinate()
        {
            return yCoord;
        }
        /**@author Paul Harvey
         * 	
         * The setInputtedYCoordinate works the same way as setInputtedXCoordinate.
         * Here we take on just the Y letter from our example B3 - B. 
         * B will become a 1.
         * 
         * Validation is checked too.  If someone entered a funny letter such as S 
         * then it will fall through the case and the result will be a -1.  This 
         * will turn into an error flag and tell the person to enter another 
         * better number.
         * 
         * @param value
         */
        public void setInputtedXCoordinate(String value)
        {
            
            char testy = value[0];
            switch (testy)
            {
                case 'A':
                    xCoord = 0;
                    break;
                case 'B':
                    xCoord = 1;
                    break;
                case 'C':
                    xCoord = 2;
                    break;
                case 'D':
                    xCoord = 3;
                    break;
                case 'E':
                    xCoord = 4;
                    break;
                case 'F':
                    xCoord = 5;
                    break;
                case 'G':
                    xCoord = 6;
                    break;
                case 'H':
                    xCoord = 7;
                    break;
                case 'I':
                    xCoord = 8;
                    break;
                case 'J':
                    xCoord = 9;
                    break;
                default:
                    xCoord = 10;
                    break;
            }
        }
        /**@author Paul Harvey
         * 
         * This return will pass the converted Integer value back
         * 
         * @return int yCoord
         *
         */
        public Byte getInputtedXCoordinate()
        {
            return xCoord;
        }
        /**@author Paul Harvey
         * 
         * This procedure fills the target lock full of possibilities.
         * The whole idea is:
         * make a TargetLockarray to hold values for the computer to narrow down the human ship
         * there are 4 branches - up,down,left and right.
         * make sure each branch doesnt run into a wall
         * if there is a null value put a double XX because they are easier to handle
         * The battleship module will control the actual Target Lock.
         *
         * @param xValue
         * @param yValue
         */
        public void setTargetLockArray(int xValue, int yValue)
        {
            Array[0,0] = "" + xValue + yValue;
            for (int branch = 1; branch < 5; ++branch)
            {
                for (int shipLength = 1; shipLength < 6; ++shipLength)
                {

                    if (branch == 1 && (yValue - shipLength) >= 0)
                    { //UP
                        Array[branch,shipLength] = "" + xValue + (yValue - shipLength);
                    }
                    if (branch == 2 && (yValue + shipLength) < 10)
                    { //DOWN
                        Array[branch,shipLength] = "" + xValue + (yValue + shipLength);
                    }
                    if (branch == 3 && (xValue - shipLength) >= 0)
                    { //LEFT
                        Array[branch,shipLength] = "" + (xValue - shipLength) + yValue;
                    }
                    if (branch == 4 && (xValue + shipLength) < 10)
                    { //RIGHT
                        Array[branch,shipLength] = "" + (xValue + shipLength) + yValue;
                    }
                    if (Array[branch,shipLength] == null)
                    {
                        Array[branch,shipLength] = "XX";
                    }

                }
            }
        }
        /**@author Paul Harvey
         * 
         * Passes the TargetLockArray to the BattleShip game.
         * 	
         * @return
         */
        public String[,] getTargetLockArray()
        {
            return Array;
        }
    }
}
