using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Battleship
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePlayAI : Page
    {

        private RangedRandomNumber xCoordGenerator;
        private RangedRandomNumber yCoordGenerator;

        public object MessageBox { get; private set; }
        public object MessageBoxButtons { get; private set; }
        public object DialogResult { get; private set; }

        public GamePlayAI()
        {
            this.InitializeComponent();
            humanGameBoardDisplay(App.humanBoard);
            xCoordGenerator = new RangedRandomNumber();
            yCoordGenerator = new RangedRandomNumber();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // save data on screen somehow
        }

        private void NextTurn_Click(object sender, RoutedEventArgs e)
        {              
            Boolean error = true;
            Byte xCoord=0;
            Byte yCoord=0;

            do
            {
                if (App.targetLock == false)
                {
                    xCoordGenerator.SetMinimum(0);
                    xCoordGenerator.SetMaximum(9);
                    xCoord = Convert.ToByte(xCoordGenerator.GenerateRandomNumber());
                    yCoordGenerator.SetMinimum(0);
                    yCoordGenerator.SetMaximum(9);
                    yCoord = Convert.ToByte(yCoordGenerator.GenerateRandomNumber());
}
                else
                {
                    if (App.targetLockArray[App.branch,App.indexPosition] == "XX" || App.targetLockArray[App.branch,App.indexPosition] == null)
                    {
                        /*
                            * Likely hit XX in the targetLockArray which means the 
                            * target lock needs to change direction.  null would be an error.
                            */
                        error = true;
                        if (App.targetLock == true && App.branch != 3)
                        {  //target lock will end too soon
                            App.branch++;
                            App.indexPosition = 1;
                        }
                    }
                    else
                    {
                        /*
                            *  The targetLockArray is a string value. So the coordinates 
                            *  need to be converted to integer
                            */
                        Coordinates array = new Coordinates(App.targetLockArray[App.branch,App.indexPosition]);
                        array.setXCoordinate(App.targetLockArray[App.branch,App.indexPosition]);
                        xCoord = array.getXCoordinate();
                        array.setYCoordinate(App.targetLockArray[App.branch,App.indexPosition]);
                        yCoord = array.getYCoordinate();
                    }
                }

                if (App.humanBoard[xCoord,yCoord] == "@" || App.humanBoard[xCoord,yCoord] == "X")
                {
                    /*
                        *  The target lock hit a coordinate that was already hit.  
                        *  Change direction of the target lock.  This is the one 
                        *  unfortunate way to cheat the target lock - bunch your 
                        *  ships together.  You might get lucky.
                        */
                    if (App.targetLock == true)
                    {
                        App.branch++;
                        App.indexPosition = 1;
                    }
                }
                else
                {
                    /*
                        * Target Lock found another perfect hit.
                        */
                    if (App.targetLock == true)
                    {
                        App.indexPosition++;
                    }
                    error = false;
                }
            } while (error == true);

            /*
                * Conversion to display the coordinates the computer found
                */
            Coordinates compSelected = new Coordinates(xCoord, yCoord);
            compSelected.setXStringCoordinate();
            String xString = compSelected.getXStringCoordinate();
            App.messageOut = App.aiPlayerName + ", has selected Coordinates " + xString + (yCoord + 1) + " and has ";
//                System.out.println("The computer has selected Coordinates = " + "" + yString + (xCoord + 1));



            checkForHitOrMiss(xCoord, yCoord);  // if good then prepare the player

            // do not do next lines if game over.

            App.messageOut += "It is now " + App.humanPlayerName + "'s turn.";
            Frame.Navigate(typeof(GamePlayHuman), App.messageOut);
            
        }

        private void checkForHitOrMiss(Byte xCoord, Byte yCoord)
        {

            String shipClassification = "";
            Byte shipSize = 0;
            String hitShip = "";

            if (App.humanBoard[xCoord, yCoord] == "#" || App.humanBoard[xCoord, yCoord] == " ")
            {
                switch (App.humanBoard[xCoord, yCoord])
                {
                    case "#":
                        /*
                            *  Hit - What happens?	1. Change marker to @
                            *  					2. Figure out which ship was hit
                            *  					3. Set Hit counter on Ship
                            *  					4. See if Ship is Sunk
                            *  					5. See if All Ships were sunk
                            *  					6. If it is the first time a ship is hit - turn on the Target Lock. 							
                            */
                        //statusFlag = true;
                        //statusMessage = "The computer has hit a human ship";
                        Debug.WriteLine("Hit Coordinates are = " + xCoord + " " + yCoord);
                        App.humanBoard[xCoord, yCoord] = "@";

                        int shipNumber = 0;
                        String[] shipHumanNumberCoord = new String[App.humanNumOfShips];

                        Boolean gotShipNumber = false;
                        do
                        {
                            for (Byte loop = 0; loop < App.humanNumOfShips; ++loop)
                            {

                                hitShip = "" + xCoord.ToString() + yCoord.ToString();
 
                                shipHumanNumberCoord = App.humanShips[loop].getShipNumber();

                                Debug.WriteLine("shipHumanNumberCoord=" + shipHumanNumberCoord[loop]);

                                Debug.WriteLine("Length" + shipHumanNumberCoord[loop].Length);
                                shipSize = (Byte) (shipHumanNumberCoord[loop].Length);



                                for (int i = 0; i < shipSize; i = i + 2)
                                {

                                    // need to get the ship number of the ship that was hit  
                                    // looks like  141516
                                    Debug.WriteLine("Shipsize= " + shipSize);
                                    Debug.WriteLine("shipHumanNumberCoord[loop].Substring(i, 2)=" + shipHumanNumberCoord[loop].Substring(i, 2));
                                    Debug.WriteLine("hitShip=" + hitShip);

                                    if (shipHumanNumberCoord[loop].Substring(i, 2) == hitShip)
                                    {
                                        shipNumber = loop;
                                        gotShipNumber = true;
                                        App.humanShips[loop].setShipClassification((Byte)(shipSize / 2));
                                        shipClassification = App.humanShips[loop].getShipClassification();


                                    }
                                }
                            }
                        } while (gotShipNumber == false);

                        App.messageOut += "hit a human " + shipClassification + " \n";
                        App.humanShips[shipNumber].setHits();  // set hits counter on ship
                        App.humanShips[shipNumber].setSunk((Byte)(shipSize / 2));  // determine if  ship is sunk
                        Boolean humanShipSunk = App.humanShips[shipNumber].getSunk();  // is the ship truly sunk

                        if (humanShipSunk == true)
                        {  // now we check and see if all ships are sunk
                            App.humanShipsSunk++;
                            if (App.humanShipsSunk == App.humanNumOfShips)
                            {
                                App.messageOut += "All of the puny human ships have sunk. " + App.aiPlayerName + "RULES!";
                                
                                // change the states of the buttons at the bottom
                                nextTurn.Visibility = Visibility.Collapsed;
                                quit.Visibility = Visibility.Visible;
                                newGame.Visibility = Visibility.Visible;
                                highScores.Visibility = Visibility.Visible;
                            }
                            App.targetLock = false;
                        }

                        if (App.targetLock == false && humanShipSunk != true)
                        {
                            App.targetLock = true;  // trip this the first time a hit is recorded
                            App.branch = 1;
                            App.indexPosition = 1;
                            Coordinates hit = new Coordinates(xCoord, yCoord);
                            hit.setTargetLockArray(xCoord, yCoord);
                            App.targetLockArray = hit.getTargetLockArray();
                        }

                        break;
                    case " ":
                        /*
                            * Miss - Put an X in the Coordinates.  Tell the target lock to look in another direction.							
                            */
                        App.messageOut += "missed. \n ";
                        
                        App.humanBoard[xCoord, yCoord] = "X";
                        if (App.targetLock == true)
                        {
                            /*
                                *  If the target Lock misses then it should try
                                *  another direction
                                */
                            App.branch++;
                            App.indexPosition = 1;
                            if (App.branch > 4)
                            {
                                /*
//                                     *   This is here to make sure of the neverending target
//                                     *   lock.  Especially when the search is a right search. 
//                                     *   The lock would go until the end of the board.
//                                     */
                                App.targetLock = false;
                            }
                        }

                        break;
                }
            }
        }


        public void humanGameBoardDisplay(String[,] playingSurface)
        {

            // [x,y]

            // @ = hit a boat
            // X = missed a boat
            // # = ship

            #region playingSurface[0, 0]
            if (playingSurface[0, 0] == "@")
            {
                A1.Visibility = Visibility.Collapsed;
                A1miss.Visibility = Visibility.Collapsed;
                A1hit.Visibility = Visibility.Visible;
                A1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 0] == "X")
            {
                A1.Visibility = Visibility.Collapsed;
                A1miss.Visibility = Visibility.Visible;
                A1hit.Visibility = Visibility.Collapsed;
                A1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 0] == "#")
            {
                A1.Visibility = Visibility.Collapsed;
                A1miss.Visibility = Visibility.Collapsed;
                A1hit.Visibility = Visibility.Collapsed;
                A1ship.Visibility = Visibility.Visible;
            }
            else
            {
                A1.Visibility = Visibility.Visible;
                A1miss.Visibility = Visibility.Collapsed;
                A1hit.Visibility = Visibility.Collapsed;
                A1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 1]
            if (playingSurface[0, 1] == "@")
            {
                A2.Visibility = Visibility.Collapsed;
                A2miss.Visibility = Visibility.Collapsed;
                A2hit.Visibility = Visibility.Visible;
                A2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 1] == "X")
            {
                A2.Visibility = Visibility.Collapsed;
                A2miss.Visibility = Visibility.Visible;
                A2hit.Visibility = Visibility.Collapsed;
                A2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 1] == "#")
            {
                A2.Visibility = Visibility.Collapsed;
                A2miss.Visibility = Visibility.Collapsed;
                A2hit.Visibility = Visibility.Collapsed;
                A2ship.Visibility = Visibility.Visible;
            }
            else
            {
                A2.Visibility = Visibility.Visible;
                A2miss.Visibility = Visibility.Collapsed;
                A2hit.Visibility = Visibility.Collapsed;
                A2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 2] 
            if (playingSurface[0, 2] == "@")
            {
                A3.Visibility = Visibility.Collapsed;
                A3miss.Visibility = Visibility.Collapsed;
                A3hit.Visibility = Visibility.Visible;
                A3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 2] == "X")
            {
                A3.Visibility = Visibility.Collapsed;
                A3miss.Visibility = Visibility.Visible;
                A3hit.Visibility = Visibility.Collapsed;
                A3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 2] == "#")
            {
                A3.Visibility = Visibility.Collapsed;
                A3miss.Visibility = Visibility.Collapsed;
                A3hit.Visibility = Visibility.Collapsed;
                A3ship.Visibility = Visibility.Visible;
            }
            else
            {
                A3.Visibility = Visibility.Visible;
                A3miss.Visibility = Visibility.Collapsed;
                A3hit.Visibility = Visibility.Collapsed;
                A3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region layingSurface[0, 3]
            if (playingSurface[0, 3] == "@")
            {
                A4.Visibility = Visibility.Collapsed;
                A4miss.Visibility = Visibility.Collapsed;
                A4hit.Visibility = Visibility.Visible;
                A4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 3] == "X")
            {
                A4.Visibility = Visibility.Collapsed;
                A4miss.Visibility = Visibility.Visible;
                A4hit.Visibility = Visibility.Collapsed;
                A4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 3] == "#")
            {
                A4.Visibility = Visibility.Collapsed;
                A4miss.Visibility = Visibility.Collapsed;
                A4hit.Visibility = Visibility.Collapsed;
                A4ship.Visibility = Visibility.Visible;
            }
            else
            {
                A4.Visibility = Visibility.Visible;
                A4miss.Visibility = Visibility.Collapsed;
                A4hit.Visibility = Visibility.Collapsed;
                A4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 4]
            if (playingSurface[0, 4] == "@")
            {
                A5.Visibility = Visibility.Collapsed;
                A5miss.Visibility = Visibility.Collapsed;
                A5hit.Visibility = Visibility.Visible;
                A5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 4] == "X")
            {
                A5.Visibility = Visibility.Collapsed;
                A5miss.Visibility = Visibility.Visible;
                A5hit.Visibility = Visibility.Collapsed;
                A5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 4] == "#")
            {
                A5.Visibility = Visibility.Collapsed;
                A5miss.Visibility = Visibility.Collapsed;
                A5hit.Visibility = Visibility.Collapsed;
                A5ship.Visibility = Visibility.Visible;
            }
            else
            {
                A5.Visibility = Visibility.Visible;
                A5miss.Visibility = Visibility.Collapsed;
                A5hit.Visibility = Visibility.Collapsed;
                A5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 5]
            if (playingSurface[0, 5] == "@")
            {
                A6.Visibility = Visibility.Collapsed;
                A6miss.Visibility = Visibility.Collapsed;
                A6hit.Visibility = Visibility.Visible;
                A6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 5] == "X")
            {
                A6.Visibility = Visibility.Collapsed;
                A6miss.Visibility = Visibility.Visible;
                A6hit.Visibility = Visibility.Collapsed;
                A6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 5] == "#")
            {
                A6.Visibility = Visibility.Collapsed;
                A6miss.Visibility = Visibility.Collapsed;
                A6hit.Visibility = Visibility.Collapsed;
                A6ship.Visibility = Visibility.Visible;
            }
            else
            {
                A6.Visibility = Visibility.Visible;
                A6miss.Visibility = Visibility.Collapsed;
                A6hit.Visibility = Visibility.Collapsed;
                A6ship.Visibility = Visibility.Collapsed;
            }
            #endregion 

            #region playingSurface[0, 6]
            if (playingSurface[0, 6] == "@")
            {
                A7.Visibility = Visibility.Collapsed;
                A7miss.Visibility = Visibility.Collapsed;
                A7hit.Visibility = Visibility.Visible;
                A7ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[0, 6] == "X")
            {
                A7.Visibility = Visibility.Collapsed;
                A7miss.Visibility = Visibility.Visible;
                A7hit.Visibility = Visibility.Collapsed;
                A7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 6] == "#")
            {
                A7.Visibility = Visibility.Collapsed;
                A7miss.Visibility = Visibility.Collapsed;
                A7hit.Visibility = Visibility.Collapsed;
                A7ship.Visibility = Visibility.Visible;
            }
            else
            {
                A7.Visibility = Visibility.Visible;
                A7miss.Visibility = Visibility.Collapsed;
                A7hit.Visibility = Visibility.Collapsed;
                A7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 7] 
            if (playingSurface[0, 7] == "@")
            {
                A8.Visibility = Visibility.Collapsed;
                A8miss.Visibility = Visibility.Collapsed;
                A8hit.Visibility = Visibility.Visible;
                A8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 7] == "X")
            {
                A8.Visibility = Visibility.Collapsed;
                A8miss.Visibility = Visibility.Visible;
                A8hit.Visibility = Visibility.Collapsed;
                A8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 7] == "#")
            {
                A8.Visibility = Visibility.Collapsed;
                A8miss.Visibility = Visibility.Collapsed;
                A8hit.Visibility = Visibility.Collapsed;
                A8ship.Visibility = Visibility.Visible;
            }
            else
            {
                A8.Visibility = Visibility.Visible;
                A8miss.Visibility = Visibility.Collapsed;
                A8hit.Visibility = Visibility.Collapsed;
                A8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region  playingSurface[0, 8]
            if (playingSurface[0, 8] == "@")
            {
                A9.Visibility = Visibility.Collapsed;
                A9miss.Visibility = Visibility.Collapsed;
                A9hit.Visibility = Visibility.Visible;
                A9ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[0, 8] == "X")
            {
                A9.Visibility = Visibility.Collapsed;
                A9miss.Visibility = Visibility.Visible;
                A9hit.Visibility = Visibility.Collapsed;
                A9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 8] == "#")
            {
                A9.Visibility = Visibility.Collapsed;
                A9miss.Visibility = Visibility.Collapsed;
                A9hit.Visibility = Visibility.Collapsed;
                A9ship.Visibility = Visibility.Visible;
            }
            else
            {
                A9.Visibility = Visibility.Visible;
                A9miss.Visibility = Visibility.Collapsed;
                A9hit.Visibility = Visibility.Collapsed;
                A9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[0, 9]
            if (playingSurface[0, 9] == "@")
            {
                A10.Visibility = Visibility.Collapsed;
                A10miss.Visibility = Visibility.Collapsed;
                A10hit.Visibility = Visibility.Visible;
                A10ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[0, 9] == "X")
            {
                A10.Visibility = Visibility.Collapsed;
                A10miss.Visibility = Visibility.Visible;
                A10hit.Visibility = Visibility.Collapsed;
                A10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[0, 9] == "#")
            {
                A10.Visibility = Visibility.Collapsed;
                A10miss.Visibility = Visibility.Collapsed;
                A10hit.Visibility = Visibility.Collapsed;
                A10ship.Visibility = Visibility.Visible;
            }
            else
            {
                A10.Visibility = Visibility.Visible;
                A10miss.Visibility = Visibility.Collapsed;
                A10hit.Visibility = Visibility.Collapsed;
                A10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region (playingSurface[1, 0]
            if (playingSurface[1, 0] == "@")
            {
                B1.Visibility = Visibility.Collapsed;
                B1miss.Visibility = Visibility.Collapsed;
                B1hit.Visibility = Visibility.Visible;
                B1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 0] == "X")
            {
                B1.Visibility = Visibility.Collapsed;
                B1miss.Visibility = Visibility.Visible;
                B1hit.Visibility = Visibility.Collapsed;
                B1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 0] == "#")
            {
                B1.Visibility = Visibility.Collapsed;
                B1miss.Visibility = Visibility.Collapsed;
                B1hit.Visibility = Visibility.Collapsed;
                B1ship.Visibility = Visibility.Visible;
            }
            else
            {
                B1.Visibility = Visibility.Visible;
                B1miss.Visibility = Visibility.Collapsed;
                B1hit.Visibility = Visibility.Collapsed;
                B1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 1]
            if (playingSurface[1, 1] == "@")
            {
                B2.Visibility = Visibility.Collapsed;
                B2miss.Visibility = Visibility.Collapsed;
                B2hit.Visibility = Visibility.Visible;
                B2ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[1, 1] == "X")
            {
                B2.Visibility = Visibility.Collapsed;
                B2miss.Visibility = Visibility.Visible;
                B2hit.Visibility = Visibility.Collapsed;
                B2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 1] == "#")
            {
                B2.Visibility = Visibility.Collapsed;
                B2miss.Visibility = Visibility.Collapsed;
                B2hit.Visibility = Visibility.Collapsed;
                B2ship.Visibility = Visibility.Visible;
            }
            else
            {
                B2.Visibility = Visibility.Visible;
                B2miss.Visibility = Visibility.Collapsed;
                B2hit.Visibility = Visibility.Collapsed;
                B2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 2]
            if (playingSurface[1, 2] == "@")
            {
                B3.Visibility = Visibility.Collapsed;
                B3miss.Visibility = Visibility.Collapsed;
                B3hit.Visibility = Visibility.Visible;
                B3ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[1, 2] == "X")
            {
                B3.Visibility = Visibility.Collapsed;
                B3miss.Visibility = Visibility.Visible;
                B3hit.Visibility = Visibility.Collapsed;
                B3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 2]== "#")
            {
                B3.Visibility = Visibility.Collapsed;
                B3miss.Visibility = Visibility.Collapsed;
                B3hit.Visibility = Visibility.Collapsed;
                B3ship.Visibility = Visibility.Visible;
            }
            else
            {
                B3.Visibility = Visibility.Visible;
                B3miss.Visibility = Visibility.Collapsed;
                B3hit.Visibility = Visibility.Collapsed;
                B3ship.Visibility = Visibility.Collapsed;
            }

            #endregion

            #region playingSurface[1, 3]
            if (playingSurface[1, 3] == "@")
            {
                B4.Visibility = Visibility.Collapsed;
                B4miss.Visibility = Visibility.Collapsed;
                B4hit.Visibility = Visibility.Visible;
                B4ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[1, 3] == "X")
            {
                B4.Visibility = Visibility.Collapsed;
                B4miss.Visibility = Visibility.Visible;
                B4hit.Visibility = Visibility.Collapsed;
                B4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 3] == "#")
            {
                B4.Visibility = Visibility.Collapsed;
                B4miss.Visibility = Visibility.Collapsed;
                B4hit.Visibility = Visibility.Collapsed;
                B4ship.Visibility = Visibility.Visible;
            }
            else
            {
                B4.Visibility = Visibility.Visible;
                B4miss.Visibility = Visibility.Collapsed;
                B4hit.Visibility = Visibility.Collapsed;
                B4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 4]
            if (playingSurface[1, 4] == "@")
            {
                B5.Visibility = Visibility.Collapsed;
                B5miss.Visibility = Visibility.Collapsed;
                B5hit.Visibility = Visibility.Visible;
                B5ship.Visibility = Visibility.Collapsed;
            }
            if (playingSurface[1, 4] == "X")
            {
                B5.Visibility = Visibility.Collapsed;
                B5miss.Visibility = Visibility.Visible;
                B5hit.Visibility = Visibility.Collapsed;
                B5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 4] == "#")
            {
                B5.Visibility = Visibility.Collapsed;
                B5miss.Visibility = Visibility.Collapsed;
                B5hit.Visibility = Visibility.Collapsed;
                B5ship.Visibility = Visibility.Visible;
            }
            else
            {
                B5.Visibility = Visibility.Visible;
                B5miss.Visibility = Visibility.Collapsed;
                B5hit.Visibility = Visibility.Collapsed;
                B5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 5]
            if (playingSurface[1, 5] == "@")
            {
                B6.Visibility = Visibility.Collapsed;
                B6miss.Visibility = Visibility.Collapsed;
                B6hit.Visibility = Visibility.Visible;
                B6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 5] == "X")
            {
                B6.Visibility = Visibility.Collapsed;
                B6miss.Visibility = Visibility.Visible;
                B6hit.Visibility = Visibility.Collapsed;
                B6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 5] == "#")
            {
                B6.Visibility = Visibility.Collapsed;
                B6miss.Visibility = Visibility.Collapsed;
                B6hit.Visibility = Visibility.Collapsed;
                B6ship.Visibility = Visibility.Visible;
            }
            else
            {
                B6.Visibility = Visibility.Visible;
                B6miss.Visibility = Visibility.Collapsed;
                B6hit.Visibility = Visibility.Collapsed;
                B6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 6]
            if (playingSurface[1, 6] == "@")
            {
                B7.Visibility = Visibility.Collapsed;
                B7miss.Visibility = Visibility.Collapsed;
                B7hit.Visibility = Visibility.Visible;
                B7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 6] == "X")
            {
                B7.Visibility = Visibility.Collapsed;
                B7miss.Visibility = Visibility.Visible;
                B7hit.Visibility = Visibility.Collapsed;
                B7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 6] == "#")
            {
                B7.Visibility = Visibility.Collapsed;
                B7miss.Visibility = Visibility.Collapsed;
                B7hit.Visibility = Visibility.Collapsed;
                B7ship.Visibility = Visibility.Visible;
            }
            else
            {
                B7.Visibility = Visibility.Visible;
                B7miss.Visibility = Visibility.Collapsed;
                B7hit.Visibility = Visibility.Collapsed;
                B7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 7] 
            if (playingSurface[1, 7] == "@")
            {
                B8.Visibility = Visibility.Collapsed;
                B8miss.Visibility = Visibility.Collapsed;
                B8hit.Visibility = Visibility.Visible;
                B8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 7] == "X")
            {
                B8.Visibility = Visibility.Collapsed;
                B8miss.Visibility = Visibility.Visible;
                B8hit.Visibility = Visibility.Collapsed;
                B8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 7] == "#")
            {
                B8.Visibility = Visibility.Collapsed;
                B8miss.Visibility = Visibility.Collapsed;
                B8hit.Visibility = Visibility.Collapsed;
                B8ship.Visibility = Visibility.Visible;
            }
            else
            {
                B8.Visibility = Visibility.Visible;
                B8miss.Visibility = Visibility.Collapsed;
                B8hit.Visibility = Visibility.Collapsed;
                B8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region  playingSurface[1, 8]
            if (playingSurface[1, 8] == "@")
            {
                B9.Visibility = Visibility.Collapsed;
                B9miss.Visibility = Visibility.Collapsed;
                B9hit.Visibility = Visibility.Visible;
                B9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 8] == "X")
            {
                B9.Visibility = Visibility.Collapsed;
                B9miss.Visibility = Visibility.Visible;
                B9hit.Visibility = Visibility.Collapsed;
                B9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 8] == "#")
            {
                B9.Visibility = Visibility.Collapsed;
                B9miss.Visibility = Visibility.Collapsed;
                B9hit.Visibility = Visibility.Collapsed;
                B9ship.Visibility = Visibility.Visible;
            }
            else
            {
                B9.Visibility = Visibility.Visible;
                B9miss.Visibility = Visibility.Collapsed;
                B9hit.Visibility = Visibility.Collapsed;
                B9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[1, 9]
            if (playingSurface[1, 9] == "@")
            {
                B10.Visibility = Visibility.Collapsed;
                B10miss.Visibility = Visibility.Collapsed;
                B10hit.Visibility = Visibility.Visible;
                B10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 9] == "X")
            {
                B10.Visibility = Visibility.Collapsed;
                B10miss.Visibility = Visibility.Visible;
                B10hit.Visibility = Visibility.Collapsed;
                B10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[1, 9] == "#")
            {
                B10.Visibility = Visibility.Collapsed;
                B10miss.Visibility = Visibility.Collapsed;
                B10hit.Visibility = Visibility.Collapsed;
                B10ship.Visibility = Visibility.Visible;
            }
            else
            {
                B10.Visibility = Visibility.Visible;
                B10miss.Visibility = Visibility.Collapsed;
                B10hit.Visibility = Visibility.Collapsed;
                B10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region (playingSurface[2, 0] 
            if (playingSurface[2, 0] == "@")
            {
                C1.Visibility = Visibility.Collapsed;
                C1miss.Visibility = Visibility.Collapsed;
                C1hit.Visibility = Visibility.Visible;
                C1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 0] == "X")
            {
                C1.Visibility = Visibility.Collapsed;
                C1miss.Visibility = Visibility.Visible;
                C1hit.Visibility = Visibility.Collapsed;
                C1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 0] == "#")
            {
                C1.Visibility = Visibility.Collapsed;
                C1miss.Visibility = Visibility.Collapsed;
                C1hit.Visibility = Visibility.Collapsed;
                C1ship.Visibility = Visibility.Visible;
            }
            else
            {
                C1.Visibility = Visibility.Visible;
                C1miss.Visibility = Visibility.Collapsed;
                C1hit.Visibility = Visibility.Collapsed;
                C1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 1]
            if (playingSurface[2, 1] == "@")
            {
                C2.Visibility = Visibility.Collapsed;
                C2miss.Visibility = Visibility.Collapsed;
                C2hit.Visibility = Visibility.Visible;
                C2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 1] == "X")
            {
                C2.Visibility = Visibility.Collapsed;
                C2miss.Visibility = Visibility.Visible;
                C2hit.Visibility = Visibility.Collapsed;
                C2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 1] == "#")
            {
                C2.Visibility = Visibility.Collapsed;
                C2miss.Visibility = Visibility.Collapsed;
                C2hit.Visibility = Visibility.Collapsed;
                C2ship.Visibility = Visibility.Visible;
            }
            else
            {
                C2.Visibility = Visibility.Visible;
                C2miss.Visibility = Visibility.Collapsed;
                C2hit.Visibility = Visibility.Collapsed;
                C2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 2]  
            if (playingSurface[2, 2] == "@")
            {
                C3.Visibility = Visibility.Collapsed;
                C3miss.Visibility = Visibility.Collapsed;
                C3hit.Visibility = Visibility.Visible;
                C3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 2] == "X")
            {
                C3.Visibility = Visibility.Collapsed;
                C3miss.Visibility = Visibility.Visible;
                C3hit.Visibility = Visibility.Collapsed;
                C3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 2] == "#")
            {
                C3.Visibility = Visibility.Collapsed;
                C3miss.Visibility = Visibility.Collapsed;
                C3hit.Visibility = Visibility.Collapsed;
                C3ship.Visibility = Visibility.Visible;
            }
            else
            {
                C3.Visibility = Visibility.Visible;
                C3miss.Visibility = Visibility.Collapsed;
                C3hit.Visibility = Visibility.Collapsed;
                C3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 3] 
            if (playingSurface[2, 3] == "@")
            {
                C4.Visibility = Visibility.Collapsed;
                C4miss.Visibility = Visibility.Collapsed;
                C4hit.Visibility = Visibility.Visible;
                C4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 3] == "X")
            {
                C4.Visibility = Visibility.Collapsed;
                C4miss.Visibility = Visibility.Visible;
                C4hit.Visibility = Visibility.Collapsed;
                C4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 3] == "#")
            {
                C4.Visibility = Visibility.Collapsed;
                C4miss.Visibility = Visibility.Collapsed;
                C4hit.Visibility = Visibility.Collapsed;
                C4ship.Visibility = Visibility.Visible;
            }
            else
            {
                C4.Visibility = Visibility.Visible;
                C4miss.Visibility = Visibility.Collapsed;
                C4hit.Visibility = Visibility.Collapsed;
                C4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 4]
            if (playingSurface[2, 4] == "@")
            {
                C5.Visibility = Visibility.Collapsed;
                C5miss.Visibility = Visibility.Collapsed;
                C5hit.Visibility = Visibility.Visible;
                C5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 4] == "X")
            {
                C5.Visibility = Visibility.Collapsed;
                C5miss.Visibility = Visibility.Visible;
                C5hit.Visibility = Visibility.Collapsed;
                C5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 4] == "#")
            {
                C5.Visibility = Visibility.Collapsed;
                C5miss.Visibility = Visibility.Collapsed;
                C5hit.Visibility = Visibility.Collapsed;
                C5ship.Visibility = Visibility.Visible;
            }
            else
            {
                C5.Visibility = Visibility.Visible;
                C5miss.Visibility = Visibility.Collapsed;
                C5hit.Visibility = Visibility.Collapsed;
                C5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 5]
            if (playingSurface[2, 5] == "@")
            {
                C6.Visibility = Visibility.Collapsed;
                C6miss.Visibility = Visibility.Collapsed;
                C6hit.Visibility = Visibility.Visible;
                C6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 5] == "X")
            {
                C6.Visibility = Visibility.Collapsed;
                C6miss.Visibility = Visibility.Visible;
                C6hit.Visibility = Visibility.Collapsed;
                C6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 5] == "#")
            {
                C6.Visibility = Visibility.Collapsed;
                C6miss.Visibility = Visibility.Collapsed;
                C6hit.Visibility = Visibility.Collapsed;
                C6ship.Visibility = Visibility.Visible;
            }
            else
            {
                C6.Visibility = Visibility.Visible;
                C6miss.Visibility = Visibility.Collapsed;
                C6hit.Visibility = Visibility.Collapsed;
                C6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 6]
            if (playingSurface[2, 6] == "@")
            {
                C7.Visibility = Visibility.Collapsed;
                C7miss.Visibility = Visibility.Collapsed;
                C7hit.Visibility = Visibility.Visible;
                C7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 6] == "X")
            {
                C7.Visibility = Visibility.Collapsed;
                C7miss.Visibility = Visibility.Visible;
                C7hit.Visibility = Visibility.Collapsed;
                C7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 6] == "#")
            {
                C7.Visibility = Visibility.Collapsed;
                C7miss.Visibility = Visibility.Collapsed;
                C7hit.Visibility = Visibility.Collapsed;
                C7ship.Visibility = Visibility.Visible;
            }
            else
            {
                C7.Visibility = Visibility.Visible;
                C7miss.Visibility = Visibility.Collapsed;
                C7hit.Visibility = Visibility.Collapsed;
                C7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 7] 
            if (playingSurface[2, 7] == "@")
            {
                C8.Visibility = Visibility.Collapsed;
                C8miss.Visibility = Visibility.Collapsed;
                C8hit.Visibility = Visibility.Visible;
                C8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 7] == "X")
            {
                C8.Visibility = Visibility.Collapsed;
                C8miss.Visibility = Visibility.Visible;
                C8hit.Visibility = Visibility.Collapsed;
                C8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 7] == "#")
            {
                C8.Visibility = Visibility.Collapsed;
                C8miss.Visibility = Visibility.Collapsed;
                C8hit.Visibility = Visibility.Collapsed;
                C8ship.Visibility = Visibility.Visible;
            }
            else
            {
                C8.Visibility = Visibility.Visible;
                C8miss.Visibility = Visibility.Collapsed;
                C8hit.Visibility = Visibility.Collapsed;
                C8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region  playingSurface[2, 8]
            if (playingSurface[2, 8] == "@")
            {
                C9.Visibility = Visibility.Collapsed;
                C9miss.Visibility = Visibility.Collapsed;
                C9hit.Visibility = Visibility.Visible;
                C9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 8] == "X")
            {
                C9.Visibility = Visibility.Collapsed;
                C9miss.Visibility = Visibility.Visible;
                C9hit.Visibility = Visibility.Collapsed;
                C9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 8] == "#")
            {
                C9.Visibility = Visibility.Collapsed;
                C9miss.Visibility = Visibility.Collapsed;
                C9hit.Visibility = Visibility.Collapsed;
                C9ship.Visibility = Visibility.Visible;
            }
            else
            {
                C9.Visibility = Visibility.Visible;
                C9miss.Visibility = Visibility.Collapsed;
                C9hit.Visibility = Visibility.Collapsed;
                C9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[2, 9]
            if (playingSurface[2, 9] == "@")
            {
                C10.Visibility = Visibility.Collapsed;
                C10miss.Visibility = Visibility.Collapsed;
                C10hit.Visibility = Visibility.Visible;
                C10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 9] == "X")
            {
                C10.Visibility = Visibility.Collapsed;
                C10miss.Visibility = Visibility.Visible;
                C10hit.Visibility = Visibility.Collapsed;
                C10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[2, 9] == "#")
            {
                C10.Visibility = Visibility.Collapsed;
                C10miss.Visibility = Visibility.Collapsed;
                C10hit.Visibility = Visibility.Collapsed;
                C10ship.Visibility = Visibility.Visible;
            }
            else
            {
                C10.Visibility = Visibility.Visible;
                C10miss.Visibility = Visibility.Collapsed;
                C10hit.Visibility = Visibility.Collapsed;
                C10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region (playingSurface[3, 0]
            if (playingSurface[3, 0] == "@")
            {
                D1.Visibility = Visibility.Collapsed;
                D1miss.Visibility = Visibility.Collapsed;
                D1hit.Visibility = Visibility.Visible;
                D1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 0] == "X")
            {
                D1.Visibility = Visibility.Collapsed;
                D1miss.Visibility = Visibility.Visible;
                D1hit.Visibility = Visibility.Collapsed;
                D1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 0] == "#")
            {
                D1.Visibility = Visibility.Collapsed;
                D1miss.Visibility = Visibility.Collapsed;
                D1hit.Visibility = Visibility.Collapsed;
                D1ship.Visibility = Visibility.Visible;
            }
            else
            {
                D1.Visibility = Visibility.Visible;
                D1miss.Visibility = Visibility.Collapsed;
                D1hit.Visibility = Visibility.Collapsed;
                D1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 1]
            if (playingSurface[3, 1] == "@")
            {
                D2.Visibility = Visibility.Collapsed;
                D2miss.Visibility = Visibility.Collapsed;
                D2hit.Visibility = Visibility.Visible;
                D2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 1] == "X")
            {
                D2.Visibility = Visibility.Collapsed;
                D2miss.Visibility = Visibility.Visible;
                D2hit.Visibility = Visibility.Collapsed;
                D2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 1] == "#")
            {
                D2.Visibility = Visibility.Collapsed;
                D2miss.Visibility = Visibility.Collapsed;
                D2hit.Visibility = Visibility.Collapsed;
                D2ship.Visibility = Visibility.Visible;
            }
            else
            {
                D2.Visibility = Visibility.Visible;
                D2miss.Visibility = Visibility.Collapsed;
                D2hit.Visibility = Visibility.Collapsed;
                D2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 2]
            if (playingSurface[3, 2] == "@")
            {
                D3.Visibility = Visibility.Collapsed;
                D3miss.Visibility = Visibility.Collapsed;
                D3hit.Visibility = Visibility.Visible;
                D3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 2] == "X")
            {
                D3.Visibility = Visibility.Collapsed;
                D3miss.Visibility = Visibility.Visible;
                D3hit.Visibility = Visibility.Collapsed;
                D3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 2] == "#")
            {
                D3.Visibility = Visibility.Collapsed;
                D3miss.Visibility = Visibility.Collapsed;
                D3hit.Visibility = Visibility.Collapsed;
                D3ship.Visibility = Visibility.Visible;
            }
            else
            {
                D3.Visibility = Visibility.Visible;
                D3miss.Visibility = Visibility.Collapsed;
                D3hit.Visibility = Visibility.Collapsed;
                D3ship.Visibility = Visibility.Collapsed;
            }

            #endregion

            #region playingSurface[3, 3] 
            if (playingSurface[3, 3] == "@")
            {
                D4.Visibility = Visibility.Collapsed;
                D4miss.Visibility = Visibility.Collapsed;
                D4hit.Visibility = Visibility.Visible;
                D4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 3] == "X")
            {
                D4.Visibility = Visibility.Collapsed;
                D4miss.Visibility = Visibility.Visible;
                D4hit.Visibility = Visibility.Collapsed;
                D4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 3] == "#")
            {
                D4.Visibility = Visibility.Collapsed;
                D4miss.Visibility = Visibility.Collapsed;
                D4hit.Visibility = Visibility.Collapsed;
                D4ship.Visibility = Visibility.Visible;
            }
            else
            {
                D4.Visibility = Visibility.Visible;
                D4miss.Visibility = Visibility.Collapsed;
                D4hit.Visibility = Visibility.Collapsed;
                D4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 4] 
            if (playingSurface[3, 4] == "@")
            {
                D5.Visibility = Visibility.Collapsed;
                D5miss.Visibility = Visibility.Collapsed;
                D5hit.Visibility = Visibility.Visible;
                D5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 4] == "X")
            {
                D5.Visibility = Visibility.Collapsed;
                D5miss.Visibility = Visibility.Visible;
                D5hit.Visibility = Visibility.Collapsed;
                D5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 4] == "#")
            {
                D5.Visibility = Visibility.Collapsed;
                D5miss.Visibility = Visibility.Collapsed;
                D5hit.Visibility = Visibility.Collapsed;
                D5ship.Visibility = Visibility.Visible;
            }
            else
            {
                D5.Visibility = Visibility.Visible;
                D5miss.Visibility = Visibility.Collapsed;
                D5hit.Visibility = Visibility.Collapsed;
                D5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 5]
            if (playingSurface[3, 5] == "@")
            {
                D6.Visibility = Visibility.Collapsed;
                D6miss.Visibility = Visibility.Collapsed;
                D6hit.Visibility = Visibility.Visible;
                D6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 5] == "X")
            {
                D6.Visibility = Visibility.Collapsed;
                D6miss.Visibility = Visibility.Visible;
                D6hit.Visibility = Visibility.Collapsed;
                D6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 5] == "#")
            {
                D6.Visibility = Visibility.Collapsed;
                D6miss.Visibility = Visibility.Collapsed;
                D6hit.Visibility = Visibility.Collapsed;
                D6ship.Visibility = Visibility.Visible;
            }
            else
            {
                D6.Visibility = Visibility.Visible;
                D6miss.Visibility = Visibility.Collapsed;
                D6hit.Visibility = Visibility.Collapsed;
                D6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 6]
            if (playingSurface[3, 6] == "@")
            {
                D7.Visibility = Visibility.Collapsed;
                D7miss.Visibility = Visibility.Collapsed;
                D7hit.Visibility = Visibility.Visible;
                D7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 6] == "X")
            {
                D7.Visibility = Visibility.Collapsed;
                D7miss.Visibility = Visibility.Visible;
                D7hit.Visibility = Visibility.Collapsed;
                D7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 6] == "#")
            {
                D7.Visibility = Visibility.Collapsed;
                D7miss.Visibility = Visibility.Collapsed;
                D7hit.Visibility = Visibility.Collapsed;
                D7ship.Visibility = Visibility.Visible;
            }
            else
            {
                D7.Visibility = Visibility.Visible;
                D7miss.Visibility = Visibility.Collapsed;
                D7hit.Visibility = Visibility.Collapsed;
                D7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[3, 7] 
            if (playingSurface[3, 7] == "@")
            {
                D8.Visibility = Visibility.Collapsed;
                D8miss.Visibility = Visibility.Collapsed;
                D8hit.Visibility = Visibility.Visible;
                D8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 7] == "X")
            {
                D8.Visibility = Visibility.Collapsed;
                D8miss.Visibility = Visibility.Visible;
                D8hit.Visibility = Visibility.Collapsed;
                D8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 7] == "#")
            {
                D8.Visibility = Visibility.Collapsed;
                D8miss.Visibility = Visibility.Collapsed;
                D8hit.Visibility = Visibility.Collapsed;
                D8ship.Visibility = Visibility.Visible;
            }
            else
            {
                D8.Visibility = Visibility.Visible;
                D8miss.Visibility = Visibility.Collapsed;
                D8hit.Visibility = Visibility.Collapsed;
                D8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region  playingSurface[3, 8]
            if (playingSurface[3, 8] == "@")
            {
                D9.Visibility = Visibility.Collapsed;
                D9miss.Visibility = Visibility.Collapsed;
                D9hit.Visibility = Visibility.Visible;
                D9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 8] == "X")
            {
                D9.Visibility = Visibility.Collapsed;
                D9miss.Visibility = Visibility.Visible;
                D9hit.Visibility = Visibility.Collapsed;
                D9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 8] == "#")
            {
                D9.Visibility = Visibility.Collapsed;
                D9miss.Visibility = Visibility.Collapsed;
                D9hit.Visibility = Visibility.Collapsed;
                D9ship.Visibility = Visibility.Visible;
            }
            else
            {
                D9.Visibility = Visibility.Visible;
                D9miss.Visibility = Visibility.Collapsed;
                D9hit.Visibility = Visibility.Collapsed;
                D9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region  playingSurface[3, 9]
            if (playingSurface[3, 9] == "@")
            {
                D10.Visibility = Visibility.Collapsed;
                D10miss.Visibility = Visibility.Collapsed;
                D10hit.Visibility = Visibility.Visible;
                D10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 9] == "X")
            {
                D10.Visibility = Visibility.Collapsed;
                D10miss.Visibility = Visibility.Visible;
                D10hit.Visibility = Visibility.Collapsed;
                D10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[3, 9] == "#")
            {
                D10.Visibility = Visibility.Collapsed;
                D10miss.Visibility = Visibility.Collapsed;
                D10hit.Visibility = Visibility.Collapsed;
                D10ship.Visibility = Visibility.Visible;
            }
            else
            {
                D10.Visibility = Visibility.Visible;
                D10miss.Visibility = Visibility.Collapsed;
                D10hit.Visibility = Visibility.Collapsed;
                D10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 0]
            if (playingSurface[4, 0] == "@")
            {
                E1.Visibility = Visibility.Collapsed;
                E1miss.Visibility = Visibility.Collapsed;
                E1hit.Visibility = Visibility.Visible;
                E1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 0] == "X")
            {
                E1.Visibility = Visibility.Collapsed;
                E1miss.Visibility = Visibility.Visible;
                E1hit.Visibility = Visibility.Collapsed;
                E1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 0] == "#")
            {
                E1.Visibility = Visibility.Collapsed;
                E1miss.Visibility = Visibility.Collapsed;
                E1hit.Visibility = Visibility.Collapsed;
                E1ship.Visibility = Visibility.Visible;
            }
            else
            {
                E1.Visibility = Visibility.Visible;
                E1miss.Visibility = Visibility.Collapsed;
                E1hit.Visibility = Visibility.Collapsed;
                E1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 1]
            if (playingSurface[4, 1] == "@")
            {
                E2.Visibility = Visibility.Collapsed;
                E2miss.Visibility = Visibility.Collapsed;
                E2hit.Visibility = Visibility.Visible;
                E2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 1] == "X")
            {
                E2.Visibility = Visibility.Collapsed;
                E2miss.Visibility = Visibility.Visible;
                E2hit.Visibility = Visibility.Collapsed;
                E2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 1] == "#")
            {
                E2.Visibility = Visibility.Collapsed;
                E2miss.Visibility = Visibility.Collapsed;
                E2hit.Visibility = Visibility.Collapsed;
                E2ship.Visibility = Visibility.Visible;
            }
            else
            {
                E2.Visibility = Visibility.Visible;
                E2miss.Visibility = Visibility.Collapsed;
                E2hit.Visibility = Visibility.Collapsed;
                E2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 2]
            if (playingSurface[4, 2] == "@")
            {
                E3.Visibility = Visibility.Collapsed;
                E3miss.Visibility = Visibility.Collapsed;
                E3hit.Visibility = Visibility.Visible;
                E3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 2] == "X")
            {
                E3.Visibility = Visibility.Collapsed;
                E3miss.Visibility = Visibility.Visible;
                E3hit.Visibility = Visibility.Collapsed;
                E3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 2] == "#")
            {
                E3.Visibility = Visibility.Collapsed;
                E3miss.Visibility = Visibility.Collapsed;
                E3hit.Visibility = Visibility.Collapsed;
                E3ship.Visibility = Visibility.Visible;
            }
            else
            {
                E3.Visibility = Visibility.Visible;
                E3miss.Visibility = Visibility.Collapsed;
                E3hit.Visibility = Visibility.Collapsed;
                E3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 3] 
            if (playingSurface[4, 3] == "@")
            {
                E4.Visibility = Visibility.Collapsed;
                E4miss.Visibility = Visibility.Collapsed;
                E4hit.Visibility = Visibility.Visible;
                E4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 3] == "X")
            {
                E4.Visibility = Visibility.Collapsed;
                E4miss.Visibility = Visibility.Visible;
                E4hit.Visibility = Visibility.Collapsed;
                E4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 3] == "#")
            {
                E4.Visibility = Visibility.Collapsed;
                E4miss.Visibility = Visibility.Collapsed;
                E4hit.Visibility = Visibility.Collapsed;
                E4ship.Visibility = Visibility.Visible;
            }
            else
            {
                E4.Visibility = Visibility.Visible;
                E4miss.Visibility = Visibility.Collapsed;
                E4hit.Visibility = Visibility.Collapsed;
                E4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 4] 
            if (playingSurface[4, 4] == "@")
            {
                E5.Visibility = Visibility.Collapsed;
                E5miss.Visibility = Visibility.Collapsed;
                E5hit.Visibility = Visibility.Visible;
                E5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 4] == "X")
            {
                E5.Visibility = Visibility.Collapsed;
                E5miss.Visibility = Visibility.Visible;
                E5hit.Visibility = Visibility.Collapsed;
                E5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 4] == "#")
            {
                E5.Visibility = Visibility.Collapsed;
                E5miss.Visibility = Visibility.Collapsed;
                E5hit.Visibility = Visibility.Collapsed;
                E5ship.Visibility = Visibility.Visible;
            }
            else
            {
                E5.Visibility = Visibility.Visible;
                E5miss.Visibility = Visibility.Collapsed;
                E5hit.Visibility = Visibility.Collapsed;
                E5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 5] 
            if (playingSurface[4, 5] == "@")
            {
                E6.Visibility = Visibility.Collapsed;
                E6miss.Visibility = Visibility.Collapsed;
                E6hit.Visibility = Visibility.Visible;
                E6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 5] == "X")
            {
                E6.Visibility = Visibility.Collapsed;
                E6miss.Visibility = Visibility.Visible;
                E6hit.Visibility = Visibility.Collapsed;
                E6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 5] == "#")
            {
                E6.Visibility = Visibility.Collapsed;
                E6miss.Visibility = Visibility.Collapsed;
                E6hit.Visibility = Visibility.Collapsed;
                E6ship.Visibility = Visibility.Visible;
            }
            else
            {
                E6.Visibility = Visibility.Visible;
                E6miss.Visibility = Visibility.Collapsed;
                E6hit.Visibility = Visibility.Collapsed;
                E6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 6] 
            if (playingSurface[4, 6] == "@")
            {
                E7.Visibility = Visibility.Collapsed;
                E7miss.Visibility = Visibility.Collapsed;
                E7hit.Visibility = Visibility.Visible;
                E7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 6] == "X")
            {
                E7.Visibility = Visibility.Collapsed;
                E7miss.Visibility = Visibility.Visible;
                E7hit.Visibility = Visibility.Collapsed;
                E7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 6] == "#")
            {
                E7.Visibility = Visibility.Collapsed;
                E7miss.Visibility = Visibility.Collapsed;
                E7hit.Visibility = Visibility.Collapsed;
                E7ship.Visibility = Visibility.Visible;
            }
            else
            {
                E7.Visibility = Visibility.Visible;
                E7miss.Visibility = Visibility.Collapsed;
                E7hit.Visibility = Visibility.Collapsed;
                E7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 7] 
            if (playingSurface[4, 7] == "@")
            {
                E8.Visibility = Visibility.Collapsed;
                E8miss.Visibility = Visibility.Collapsed;
                E8hit.Visibility = Visibility.Visible;
                E8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 7] == "X")
            {
                E8.Visibility = Visibility.Collapsed;
                E8miss.Visibility = Visibility.Visible;
                E8hit.Visibility = Visibility.Collapsed;
                E8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 7] == "#")
            {
                E8.Visibility = Visibility.Collapsed;
                E8miss.Visibility = Visibility.Collapsed;
                E8hit.Visibility = Visibility.Collapsed;
                E8ship.Visibility = Visibility.Visible;
            }
            else
            {
                E8.Visibility = Visibility.Visible;
                E8miss.Visibility = Visibility.Collapsed;
                E8hit.Visibility = Visibility.Collapsed;
                E8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 8]
            if (playingSurface[4, 8] == "@")
            {
                E9.Visibility = Visibility.Collapsed;
                E9miss.Visibility = Visibility.Collapsed;
                E9hit.Visibility = Visibility.Visible;
                E9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 8] == "X")
            {
                E9.Visibility = Visibility.Collapsed;
                E9miss.Visibility = Visibility.Visible;
                E9hit.Visibility = Visibility.Collapsed;
                E9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 8] == "#")
            {
                E9.Visibility = Visibility.Collapsed;
                E9miss.Visibility = Visibility.Collapsed;
                E9hit.Visibility = Visibility.Collapsed;
                E9ship.Visibility = Visibility.Visible;
            }
            else
            {
                E9.Visibility = Visibility.Visible;
                E9miss.Visibility = Visibility.Collapsed;
                E9hit.Visibility = Visibility.Collapsed;
                E9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[4, 9]
            if (playingSurface[4, 9] == "@")
            {
                E10.Visibility = Visibility.Collapsed;
                E10miss.Visibility = Visibility.Collapsed;
                E10hit.Visibility = Visibility.Visible;
                E10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 9] == "X")
            {
                E10.Visibility = Visibility.Collapsed;
                E10miss.Visibility = Visibility.Visible;
                E10hit.Visibility = Visibility.Collapsed;
                E10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[4, 9] == "#")
            {
                E10.Visibility = Visibility.Collapsed;
                E10miss.Visibility = Visibility.Collapsed;
                E10hit.Visibility = Visibility.Collapsed;
                E10ship.Visibility = Visibility.Visible;
            }
            else
            {
                E10.Visibility = Visibility.Visible;
                E10miss.Visibility = Visibility.Collapsed;
                E10hit.Visibility = Visibility.Collapsed;
                E10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 0]
            if (playingSurface[5, 0] == "@")
            {
                F1.Visibility = Visibility.Collapsed;
                F1miss.Visibility = Visibility.Collapsed;
                F1hit.Visibility = Visibility.Visible;
                F1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 0] == "X")
            {
                F1.Visibility = Visibility.Collapsed;
                F1miss.Visibility = Visibility.Visible;
                F1hit.Visibility = Visibility.Collapsed;
                F1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 0] == "#")
            {
                F1.Visibility = Visibility.Collapsed;
                F1miss.Visibility = Visibility.Collapsed;
                F1hit.Visibility = Visibility.Collapsed;
                F1ship.Visibility = Visibility.Visible;
            }
            else
            {
                F1.Visibility = Visibility.Visible;
                F1miss.Visibility = Visibility.Collapsed;
                F1hit.Visibility = Visibility.Collapsed;
                F1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 1]
            if (playingSurface[5, 1] == "@")
            {
                F2.Visibility = Visibility.Collapsed;
                F2miss.Visibility = Visibility.Collapsed;
                F2hit.Visibility = Visibility.Visible;
                F2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 1] == "X")
            {
                F2.Visibility = Visibility.Collapsed;
                F2miss.Visibility = Visibility.Visible;
                F2hit.Visibility = Visibility.Collapsed;
                F2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 1] == "#")
            {
                F2.Visibility = Visibility.Collapsed;
                F2miss.Visibility = Visibility.Collapsed;
                F2hit.Visibility = Visibility.Collapsed;
                F2ship.Visibility = Visibility.Visible;
            }
            else
            {
                F2.Visibility = Visibility.Visible;
                F2miss.Visibility = Visibility.Collapsed;
                F2hit.Visibility = Visibility.Collapsed;
                F2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 2] 
            if (playingSurface[5, 2] == "@")
            {
                F3.Visibility = Visibility.Collapsed;
                F3miss.Visibility = Visibility.Collapsed;
                F3hit.Visibility = Visibility.Visible;
                F3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 2] == "X")
            {
                F3.Visibility = Visibility.Collapsed;
                F3miss.Visibility = Visibility.Visible;
                F3hit.Visibility = Visibility.Collapsed;
                F3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 2] == "#")
            {
                F3.Visibility = Visibility.Collapsed;
                F3miss.Visibility = Visibility.Collapsed;
                F3hit.Visibility = Visibility.Collapsed;
                F3ship.Visibility = Visibility.Visible;
            }
            else
            {
                F3.Visibility = Visibility.Visible;
                F3miss.Visibility = Visibility.Collapsed;
                F3hit.Visibility = Visibility.Collapsed;
                F3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 3]
            if (playingSurface[5, 3] == "@")
            {
                F4.Visibility = Visibility.Collapsed;
                F4miss.Visibility = Visibility.Collapsed;
                F4hit.Visibility = Visibility.Visible;
                F4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 3] == "X")
            {
                F4.Visibility = Visibility.Collapsed;
                F4miss.Visibility = Visibility.Visible;
                F4hit.Visibility = Visibility.Collapsed;
                F4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 3] == "#")
            {
                F4.Visibility = Visibility.Collapsed;
                F4miss.Visibility = Visibility.Collapsed;
                F4hit.Visibility = Visibility.Collapsed;
                F4ship.Visibility = Visibility.Visible;
            }
            else
            {
                F4.Visibility = Visibility.Visible;
                F4miss.Visibility = Visibility.Collapsed;
                F4hit.Visibility = Visibility.Collapsed;
                F4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 4]
            if (playingSurface[5, 4] == "@")
            {
                F5.Visibility = Visibility.Collapsed;
                F5miss.Visibility = Visibility.Collapsed;
                F5hit.Visibility = Visibility.Visible;
                F5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 4] == "X")
            {
                F5.Visibility = Visibility.Collapsed;
                F5miss.Visibility = Visibility.Visible;
                F5hit.Visibility = Visibility.Collapsed;
                F5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 4] == "#")
            {
                F5.Visibility = Visibility.Collapsed;
                F5miss.Visibility = Visibility.Collapsed;
                F5hit.Visibility = Visibility.Collapsed;
                F5ship.Visibility = Visibility.Visible;
            }
            else
            {
                F5.Visibility = Visibility.Visible;
                F5miss.Visibility = Visibility.Collapsed;
                F5hit.Visibility = Visibility.Collapsed;
                F5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 5] 
            if (playingSurface[5, 5] == "@")
            {
                F6.Visibility = Visibility.Collapsed;
                F6miss.Visibility = Visibility.Collapsed;
                F6hit.Visibility = Visibility.Visible;
                F6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 5] == "X")
            {
                F6.Visibility = Visibility.Collapsed;
                F6miss.Visibility = Visibility.Visible;
                F6hit.Visibility = Visibility.Collapsed;
                F6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 5] == "#")
            {
                F6.Visibility = Visibility.Collapsed;
                F6miss.Visibility = Visibility.Collapsed;
                F6hit.Visibility = Visibility.Collapsed;
                F6ship.Visibility = Visibility.Visible;
            }
            else
            {
                F6.Visibility = Visibility.Visible;
                F6miss.Visibility = Visibility.Collapsed;
                F6hit.Visibility = Visibility.Collapsed;
                F6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 6]
            if (playingSurface[5, 6] == "@")
            {
                F7.Visibility = Visibility.Collapsed;
                F7miss.Visibility = Visibility.Collapsed;
                F7hit.Visibility = Visibility.Visible;
                F7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 6] == "X")
            {
                F7.Visibility = Visibility.Collapsed;
                F7miss.Visibility = Visibility.Visible;
                F7hit.Visibility = Visibility.Collapsed;
                F7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 6] == "#")
            {
                F7.Visibility = Visibility.Collapsed;
                F7miss.Visibility = Visibility.Collapsed;
                F7hit.Visibility = Visibility.Collapsed;
                F7ship.Visibility = Visibility.Visible;
            }
            else
            {
                F7.Visibility = Visibility.Visible;
                F7miss.Visibility = Visibility.Collapsed;
                F7hit.Visibility = Visibility.Collapsed;
                F7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 7]
            if (playingSurface[5, 7] == "@")
            {
                F8.Visibility = Visibility.Collapsed;
                F8miss.Visibility = Visibility.Collapsed;
                F8hit.Visibility = Visibility.Visible;
                F8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 7] == "X")
            {
                F8.Visibility = Visibility.Collapsed;
                F8miss.Visibility = Visibility.Visible;
                F8hit.Visibility = Visibility.Collapsed;
                F8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 7] == "#")
            {
                F8.Visibility = Visibility.Collapsed;
                F8miss.Visibility = Visibility.Collapsed;
                F8hit.Visibility = Visibility.Collapsed;
                F8ship.Visibility = Visibility.Visible;
            }
            else
            {
                F8.Visibility = Visibility.Visible;
                F8miss.Visibility = Visibility.Collapsed;
                F8hit.Visibility = Visibility.Collapsed;
                F8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 8]
            if (playingSurface[5, 8] == "@")
            {
                F9.Visibility = Visibility.Collapsed;
                F9miss.Visibility = Visibility.Collapsed;
                F9hit.Visibility = Visibility.Visible;
                F9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 8] == "X")
            {
                F9.Visibility = Visibility.Collapsed;
                F9miss.Visibility = Visibility.Visible;
                F9hit.Visibility = Visibility.Collapsed;
                F9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 8] == "#")
            {
                F9.Visibility = Visibility.Collapsed;
                F9miss.Visibility = Visibility.Collapsed;
                F9hit.Visibility = Visibility.Collapsed;
                F9ship.Visibility = Visibility.Visible;
            }
            else
            {
                F9.Visibility = Visibility.Visible;
                F9miss.Visibility = Visibility.Collapsed;
                F9hit.Visibility = Visibility.Collapsed;
                F9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[5, 9]
            if (playingSurface[5, 9] == "@")
            {
                F10.Visibility = Visibility.Collapsed;
                F10miss.Visibility = Visibility.Collapsed;
                F10hit.Visibility = Visibility.Visible;
                F10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 9] == "X")
            {
                F10.Visibility = Visibility.Collapsed;
                F10miss.Visibility = Visibility.Visible;
                F10hit.Visibility = Visibility.Collapsed;
                F10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[5, 9] == "#")
            {
                F10.Visibility = Visibility.Collapsed;
                F10miss.Visibility = Visibility.Collapsed;
                F10hit.Visibility = Visibility.Collapsed;
                F10ship.Visibility = Visibility.Visible;
            }
            else
            {
                F10.Visibility = Visibility.Visible;
                F10miss.Visibility = Visibility.Collapsed;
                F10hit.Visibility = Visibility.Collapsed;
                F10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 0]
            if (playingSurface[6, 0] == "@")
            {
                G1.Visibility = Visibility.Collapsed;
                G1miss.Visibility = Visibility.Collapsed;
                G1hit.Visibility = Visibility.Visible;
                G1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 0] == "X")
            {
                G1.Visibility = Visibility.Collapsed;
                G1miss.Visibility = Visibility.Visible;
                G1hit.Visibility = Visibility.Collapsed;
                G1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 0] == "#")
            {
                G1.Visibility = Visibility.Collapsed;
                G1miss.Visibility = Visibility.Collapsed;
                G1hit.Visibility = Visibility.Collapsed;
                G1ship.Visibility = Visibility.Visible;
            }
            else
            {
                G1.Visibility = Visibility.Visible;
                G1miss.Visibility = Visibility.Collapsed;
                G1hit.Visibility = Visibility.Collapsed;
                G1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 1] 
            if (playingSurface[6, 1] == "@")
            {
                G2.Visibility = Visibility.Collapsed;
                G2miss.Visibility = Visibility.Collapsed;
                G2hit.Visibility = Visibility.Visible;
                G2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 1] == "X")
            {
                G2.Visibility = Visibility.Collapsed;
                G2miss.Visibility = Visibility.Visible;
                G2hit.Visibility = Visibility.Collapsed;
                G2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 1] == "#")
            {
                G2.Visibility = Visibility.Collapsed;
                G2miss.Visibility = Visibility.Collapsed;
                G2hit.Visibility = Visibility.Collapsed;
                G2ship.Visibility = Visibility.Visible;
            }
            else
            {
                G2.Visibility = Visibility.Visible;
                G2miss.Visibility = Visibility.Collapsed;
                G2hit.Visibility = Visibility.Collapsed;
                G2ship.Visibility = Visibility.Collapsed;
            }
            #endregion 

            #region playingSurface[6, 2]
            if (playingSurface[6, 2] == "@")
            {
                G3.Visibility = Visibility.Collapsed;
                G3miss.Visibility = Visibility.Collapsed;
                G3hit.Visibility = Visibility.Visible;
                G3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 2] == "X")
            {
                G3.Visibility = Visibility.Collapsed;
                G3miss.Visibility = Visibility.Visible;
                G3hit.Visibility = Visibility.Collapsed;
                G3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 2] == "#")
            {
                G3.Visibility = Visibility.Collapsed;
                G3miss.Visibility = Visibility.Collapsed;
                G3hit.Visibility = Visibility.Collapsed;
                G3ship.Visibility = Visibility.Visible;
            }
            else
            {
                G3.Visibility = Visibility.Visible;
                G3miss.Visibility = Visibility.Collapsed;
                G3hit.Visibility = Visibility.Collapsed;
                G3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 3] 
            if (playingSurface[6, 3] == "@")
            {
                G4.Visibility = Visibility.Collapsed;
                G4miss.Visibility = Visibility.Collapsed;
                G4hit.Visibility = Visibility.Visible;
                G4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 3] == "X")
            {
                G4.Visibility = Visibility.Collapsed;
                G4miss.Visibility = Visibility.Visible;
                G4hit.Visibility = Visibility.Collapsed;
                G4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 3] == "#")
            {
                G4.Visibility = Visibility.Collapsed;
                G4miss.Visibility = Visibility.Collapsed;
                G4hit.Visibility = Visibility.Collapsed;
                G4ship.Visibility = Visibility.Visible;
            }
            else
            {
                G4.Visibility = Visibility.Visible;
                G4miss.Visibility = Visibility.Collapsed;
                G4hit.Visibility = Visibility.Collapsed;
                G4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 4]
            if (playingSurface[6, 4] == "@")
            {
                G5.Visibility = Visibility.Collapsed;
                G5miss.Visibility = Visibility.Collapsed;
                G5hit.Visibility = Visibility.Visible;
                G5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 4] == "X")
            {
                G5.Visibility = Visibility.Collapsed;
                G5miss.Visibility = Visibility.Visible;
                G5hit.Visibility = Visibility.Collapsed;
                G5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 4] == "#")
            {
                G5.Visibility = Visibility.Collapsed;
                G5miss.Visibility = Visibility.Collapsed;
                G5hit.Visibility = Visibility.Collapsed;
                G5ship.Visibility = Visibility.Visible;
            }
            else
            {
                G5.Visibility = Visibility.Visible;
                G5miss.Visibility = Visibility.Collapsed;
                G5hit.Visibility = Visibility.Collapsed;
                G5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 5]
            if (playingSurface[6, 5] == "@")
            {
                G6.Visibility = Visibility.Collapsed;
                G6miss.Visibility = Visibility.Collapsed;
                G6hit.Visibility = Visibility.Visible;
                G6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 5] == "X")
            {
                G6.Visibility = Visibility.Collapsed;
                G6miss.Visibility = Visibility.Visible;
                G6hit.Visibility = Visibility.Collapsed;
                G6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 5] == "#")
            {
                G6.Visibility = Visibility.Collapsed;
                G6miss.Visibility = Visibility.Collapsed;
                G6hit.Visibility = Visibility.Collapsed;
                G6ship.Visibility = Visibility.Visible;
            }
            else
            {
                G6.Visibility = Visibility.Visible;
                G6miss.Visibility = Visibility.Collapsed;
                G6hit.Visibility = Visibility.Collapsed;
                G6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 6]
            if (playingSurface[6, 6] == "@")
            {
                G7.Visibility = Visibility.Collapsed;
                G7miss.Visibility = Visibility.Collapsed;
                G7hit.Visibility = Visibility.Visible;
                G7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 6] == "X")
            {
                G7.Visibility = Visibility.Collapsed;
                G7miss.Visibility = Visibility.Visible;
                G7hit.Visibility = Visibility.Collapsed;
                G7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 6] == "#")
            {
                G7.Visibility = Visibility.Collapsed;
                G7miss.Visibility = Visibility.Collapsed;
                G7hit.Visibility = Visibility.Collapsed;
                G7ship.Visibility = Visibility.Visible;
            }
            else
            {
                G7.Visibility = Visibility.Visible;
                G7miss.Visibility = Visibility.Collapsed;
                G7hit.Visibility = Visibility.Collapsed;
                G7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 7] 
            if (playingSurface[6, 7] == "@")
            {
                G8.Visibility = Visibility.Collapsed;
                G8miss.Visibility = Visibility.Collapsed;
                G8hit.Visibility = Visibility.Visible;
                G8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 7] == "X")
            {
                G8.Visibility = Visibility.Collapsed;
                G8miss.Visibility = Visibility.Visible;
                G8hit.Visibility = Visibility.Collapsed;
                G8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 7] == "#")
            {
                G8.Visibility = Visibility.Collapsed;
                G8miss.Visibility = Visibility.Collapsed;
                G8hit.Visibility = Visibility.Collapsed;
                G8ship.Visibility = Visibility.Visible;
            }
            else
            {
                G8.Visibility = Visibility.Visible;
                G8miss.Visibility = Visibility.Collapsed;
                G8hit.Visibility = Visibility.Collapsed;
                G8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region (playingSurface[6, 8]
            if (playingSurface[6, 8] == "@")
            {
                G9.Visibility = Visibility.Collapsed;
                G9miss.Visibility = Visibility.Collapsed;
                G9hit.Visibility = Visibility.Visible;
                G9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 8] == "X")
            {
                G9.Visibility = Visibility.Collapsed;
                G9miss.Visibility = Visibility.Visible;
                G9hit.Visibility = Visibility.Collapsed;
                G9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 8] == "#")
            {
                G9.Visibility = Visibility.Collapsed;
                G9miss.Visibility = Visibility.Collapsed;
                G9hit.Visibility = Visibility.Collapsed;
                G9ship.Visibility = Visibility.Visible;
            }
            else
            {
                G9.Visibility = Visibility.Visible;
                G9miss.Visibility = Visibility.Collapsed;
                G9hit.Visibility = Visibility.Collapsed;
                G9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[6, 9]
            if (playingSurface[6, 9] == "@")
            {
                G10.Visibility = Visibility.Collapsed;
                G10miss.Visibility = Visibility.Collapsed;
                G10hit.Visibility = Visibility.Visible;
                G10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 9] == "X")
            {
                G10.Visibility = Visibility.Collapsed;
                G10miss.Visibility = Visibility.Visible;
                G10hit.Visibility = Visibility.Collapsed;
                G10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[6, 9] == "#")
            {
                G10.Visibility = Visibility.Collapsed;
                G10miss.Visibility = Visibility.Collapsed;
                G10hit.Visibility = Visibility.Collapsed;
                G10ship.Visibility = Visibility.Visible;
            }
            else
            {
                G10.Visibility = Visibility.Visible;
                G10miss.Visibility = Visibility.Collapsed;
                G10hit.Visibility = Visibility.Collapsed;
                G10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 0] 
            if (playingSurface[7, 0] == "@")
            {
                H1.Visibility = Visibility.Collapsed;
                H1miss.Visibility = Visibility.Collapsed;
                H1hit.Visibility = Visibility.Visible;
                H1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 0] == "X")
            {
                H1.Visibility = Visibility.Collapsed;
                H1miss.Visibility = Visibility.Visible;
                H1hit.Visibility = Visibility.Collapsed;
                H1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 0] == "#")
            {
                H1.Visibility = Visibility.Collapsed;
                H1miss.Visibility = Visibility.Collapsed;
                H1hit.Visibility = Visibility.Collapsed;
                H1ship.Visibility = Visibility.Visible;
            }
            else
            {
                H1.Visibility = Visibility.Visible;
                H1miss.Visibility = Visibility.Collapsed;
                H1hit.Visibility = Visibility.Collapsed;
                H1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 1] 
            if (playingSurface[7, 1] == "@")
            {
                H2.Visibility = Visibility.Collapsed;
                H2miss.Visibility = Visibility.Collapsed;
                H2hit.Visibility = Visibility.Visible;
                H2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 1] == "X")
            {
                H2.Visibility = Visibility.Collapsed;
                H2miss.Visibility = Visibility.Visible;
                H2hit.Visibility = Visibility.Collapsed;
                H2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 1] == "#")
            {
                H2.Visibility = Visibility.Collapsed;
                H2miss.Visibility = Visibility.Collapsed;
                H2hit.Visibility = Visibility.Collapsed;
                H2ship.Visibility = Visibility.Visible;
            }
            else
            {
                H2.Visibility = Visibility.Visible;
                H2miss.Visibility = Visibility.Collapsed;
                H2hit.Visibility = Visibility.Collapsed;
                H2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 2]
            if (playingSurface[7, 2] == "@")
            {
                H3.Visibility = Visibility.Collapsed;
                H3miss.Visibility = Visibility.Collapsed;
                H3hit.Visibility = Visibility.Visible;
                H3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 2] == "X")
            {
                H3.Visibility = Visibility.Collapsed;
                H3miss.Visibility = Visibility.Visible;
                H3hit.Visibility = Visibility.Collapsed;
                H3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 2] == "#")
            {
                H3.Visibility = Visibility.Collapsed;
                H3miss.Visibility = Visibility.Collapsed;
                H3hit.Visibility = Visibility.Collapsed;
                H3ship.Visibility = Visibility.Visible;
            }
            else
            {
                H3.Visibility = Visibility.Visible;
                H3miss.Visibility = Visibility.Collapsed;
                H3hit.Visibility = Visibility.Collapsed;
                H3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 3]
            if (playingSurface[7, 3] == "@")
            {
                H4.Visibility = Visibility.Collapsed;
                H4miss.Visibility = Visibility.Collapsed;
                H4hit.Visibility = Visibility.Visible;
                H4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 3] == "X")
            {
                H4.Visibility = Visibility.Collapsed;
                H4miss.Visibility = Visibility.Visible;
                H4hit.Visibility = Visibility.Collapsed;
                H4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 3] == "#")
            {
                H4.Visibility = Visibility.Collapsed;
                H4miss.Visibility = Visibility.Collapsed;
                H4hit.Visibility = Visibility.Collapsed;
                H4ship.Visibility = Visibility.Visible;
            }
            else
            {
                H4.Visibility = Visibility.Visible;
                H4miss.Visibility = Visibility.Collapsed;
                H4hit.Visibility = Visibility.Collapsed;
                H4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 4]
            if (playingSurface[7, 4] == "@")
            {
                H5.Visibility = Visibility.Collapsed;
                H5miss.Visibility = Visibility.Collapsed;
                H5hit.Visibility = Visibility.Visible;
                H5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 4] == "X")
            {
                H5.Visibility = Visibility.Collapsed;
                H5miss.Visibility = Visibility.Visible;
                H5hit.Visibility = Visibility.Collapsed;
                H5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 4] == "#")
            {
                H5.Visibility = Visibility.Collapsed;
                H5miss.Visibility = Visibility.Collapsed;
                H5hit.Visibility = Visibility.Collapsed;
                H5ship.Visibility = Visibility.Visible;
            }
            else
            {
                H5.Visibility = Visibility.Visible;
                H5miss.Visibility = Visibility.Collapsed;
                H5hit.Visibility = Visibility.Collapsed;
                H5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 5] 
            if (playingSurface[7, 5] == "@")
            {
                H6.Visibility = Visibility.Collapsed;
                H6miss.Visibility = Visibility.Collapsed;
                H6hit.Visibility = Visibility.Visible;
                H6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 5] == "X")
            {
                H6.Visibility = Visibility.Collapsed;
                H6miss.Visibility = Visibility.Visible;
                H6hit.Visibility = Visibility.Collapsed;
                H6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 5] == "#")
            {
                H6.Visibility = Visibility.Collapsed;
                H6miss.Visibility = Visibility.Collapsed;
                H6hit.Visibility = Visibility.Collapsed;
                H6ship.Visibility = Visibility.Visible;
            }
            else
            {
                H6.Visibility = Visibility.Visible;
                H6miss.Visibility = Visibility.Collapsed;
                H6hit.Visibility = Visibility.Collapsed;
                H6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 6]
            if (playingSurface[7, 6] == "@")
            {
                H7.Visibility = Visibility.Collapsed;
                H7miss.Visibility = Visibility.Collapsed;
                H7hit.Visibility = Visibility.Visible;
                H7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 6] == "X")
            {
                H7.Visibility = Visibility.Collapsed;
                H7miss.Visibility = Visibility.Visible;
                H7hit.Visibility = Visibility.Collapsed;
                H7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 6] == "#")
            {
                H7.Visibility = Visibility.Collapsed;
                H7miss.Visibility = Visibility.Collapsed;
                H7hit.Visibility = Visibility.Collapsed;
                H7ship.Visibility = Visibility.Visible;
            }
            else
            {
                H7.Visibility = Visibility.Visible;
                H7miss.Visibility = Visibility.Collapsed;
                H7hit.Visibility = Visibility.Collapsed;
                H7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 7]
            if (playingSurface[7, 7] == "@")
            {
                H8.Visibility = Visibility.Collapsed;
                H8miss.Visibility = Visibility.Collapsed;
                H8hit.Visibility = Visibility.Visible;
                H8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 7] == "X")
            {
                H8.Visibility = Visibility.Collapsed;
                H8miss.Visibility = Visibility.Visible;
                H8hit.Visibility = Visibility.Collapsed;
                H8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 7] == "#")
            {
                H8.Visibility = Visibility.Collapsed;
                H8miss.Visibility = Visibility.Collapsed;
                H8hit.Visibility = Visibility.Collapsed;
                H8ship.Visibility = Visibility.Visible;
            }
            else
            {
                H8.Visibility = Visibility.Visible;
                H8miss.Visibility = Visibility.Collapsed;
                H8hit.Visibility = Visibility.Collapsed;
                H8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 8]
            if (playingSurface[7, 8] == "@")
            {
                H9.Visibility = Visibility.Collapsed;
                H9miss.Visibility = Visibility.Collapsed;
                H9hit.Visibility = Visibility.Visible;
                H9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 8] == "X")
            {
                H9.Visibility = Visibility.Collapsed;
                H9miss.Visibility = Visibility.Visible;
                H9hit.Visibility = Visibility.Collapsed;
                H9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 8] == "#")
            {
                H9.Visibility = Visibility.Collapsed;
                H9miss.Visibility = Visibility.Collapsed;
                H9hit.Visibility = Visibility.Collapsed;
                H9ship.Visibility = Visibility.Visible;
            }
            else
            {
                H9.Visibility = Visibility.Visible;
                H9miss.Visibility = Visibility.Collapsed;
                H9hit.Visibility = Visibility.Collapsed;
                H9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[7, 9]
            if (playingSurface[7, 9] == "@")
            {
                H10.Visibility = Visibility.Collapsed;
                H10miss.Visibility = Visibility.Collapsed;
                H10hit.Visibility = Visibility.Visible;
                H10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 9] == "X")
            {
                H10.Visibility = Visibility.Collapsed;
                H10miss.Visibility = Visibility.Visible;
                H10hit.Visibility = Visibility.Collapsed;
                H10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[7, 9] == "#")
            {
                H10.Visibility = Visibility.Collapsed;
                H10miss.Visibility = Visibility.Collapsed;
                H10hit.Visibility = Visibility.Collapsed;
                H10ship.Visibility = Visibility.Visible;
            }
            else
            {
                H10.Visibility = Visibility.Visible;
                H10miss.Visibility = Visibility.Collapsed;
                H10hit.Visibility = Visibility.Collapsed;
                H10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 0]
            if (playingSurface[8, 0] == "@")
            {
                I1.Visibility = Visibility.Collapsed;
                I1miss.Visibility = Visibility.Collapsed;
                I1hit.Visibility = Visibility.Visible;
                I1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 0] == "X")
            {
                I1.Visibility = Visibility.Collapsed;
                I1miss.Visibility = Visibility.Visible;
                I1hit.Visibility = Visibility.Collapsed;
                I1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 0] == "#")
            {
                I1.Visibility = Visibility.Collapsed;
                I1miss.Visibility = Visibility.Collapsed;
                I1hit.Visibility = Visibility.Collapsed;
                I1ship.Visibility = Visibility.Visible;
            }
            else
            {
                I1.Visibility = Visibility.Visible;
                I1miss.Visibility = Visibility.Collapsed;
                I1hit.Visibility = Visibility.Collapsed;
                I1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 1]
            if (playingSurface[8, 1] == "@")
            {
                I2.Visibility = Visibility.Collapsed;
                I2miss.Visibility = Visibility.Collapsed;
                I2hit.Visibility = Visibility.Visible;
                I2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 1] == "X")
            {
                I2.Visibility = Visibility.Collapsed;
                I2miss.Visibility = Visibility.Visible;
                I2hit.Visibility = Visibility.Collapsed;
                I2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 1] == "#")
            {
                I2.Visibility = Visibility.Collapsed;
                I2miss.Visibility = Visibility.Collapsed;
                I2hit.Visibility = Visibility.Collapsed;
                I2ship.Visibility = Visibility.Visible;
            }
            else
            {
                I2.Visibility = Visibility.Visible;
                I2miss.Visibility = Visibility.Collapsed;
                I2hit.Visibility = Visibility.Collapsed;
                I2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 2] 
            if (playingSurface[8, 2] == "@")
            {
                I3.Visibility = Visibility.Collapsed;
                I3miss.Visibility = Visibility.Collapsed;
                I3hit.Visibility = Visibility.Visible;
                I3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 2] == "X")
            {
                I3.Visibility = Visibility.Collapsed;
                I3miss.Visibility = Visibility.Visible;
                I3hit.Visibility = Visibility.Collapsed;
                I3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 2] == "#")
            {
                I3.Visibility = Visibility.Collapsed;
                I3miss.Visibility = Visibility.Collapsed;
                I3hit.Visibility = Visibility.Collapsed;
                I3ship.Visibility = Visibility.Visible;
            }
            else
            {
                I3.Visibility = Visibility.Visible;
                I3miss.Visibility = Visibility.Collapsed;
                I3hit.Visibility = Visibility.Collapsed;
                I3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 3]
            if (playingSurface[8, 3] == "@")
            {
                I4.Visibility = Visibility.Collapsed;
                I4miss.Visibility = Visibility.Collapsed;
                I4hit.Visibility = Visibility.Visible;
                I4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 3] == "X")
            {
                I4.Visibility = Visibility.Collapsed;
                I4miss.Visibility = Visibility.Visible;
                I4hit.Visibility = Visibility.Collapsed;
                I4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 3] == "#")
            {
                I4.Visibility = Visibility.Collapsed;
                I4miss.Visibility = Visibility.Collapsed;
                I4hit.Visibility = Visibility.Collapsed;
                I4ship.Visibility = Visibility.Visible;
            }
            else
            {
                I4.Visibility = Visibility.Visible;
                I4miss.Visibility = Visibility.Collapsed;
                I4hit.Visibility = Visibility.Collapsed;
                I4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 4]
            if (playingSurface[8, 4] == "@")
            {
                I5.Visibility = Visibility.Collapsed;
                I5miss.Visibility = Visibility.Collapsed;
                I5hit.Visibility = Visibility.Visible;
                I5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 4] == "X")
            {
                I5.Visibility = Visibility.Collapsed;
                I5miss.Visibility = Visibility.Visible;
                I5hit.Visibility = Visibility.Collapsed;
                I5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 4] == "#")
            {
                I5.Visibility = Visibility.Collapsed;
                I5miss.Visibility = Visibility.Collapsed;
                I5hit.Visibility = Visibility.Collapsed;
                I5ship.Visibility = Visibility.Visible;
            }
            else
            {
                I5.Visibility = Visibility.Visible;
                I5miss.Visibility = Visibility.Collapsed;
                I5hit.Visibility = Visibility.Collapsed;
                I5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 5] 
            if (playingSurface[8, 5] == "@")
            {
                I6.Visibility = Visibility.Collapsed;
                I6miss.Visibility = Visibility.Collapsed;
                I6hit.Visibility = Visibility.Visible;
                I6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 5] == "X")
            {
                I6.Visibility = Visibility.Collapsed;
                I6miss.Visibility = Visibility.Visible;
                I6hit.Visibility = Visibility.Collapsed;
                I6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 5] == "#")
            {
                I6.Visibility = Visibility.Collapsed;
                I6miss.Visibility = Visibility.Collapsed;
                I6hit.Visibility = Visibility.Collapsed;
                I6ship.Visibility = Visibility.Visible;
            }
            else
            {
                I6.Visibility = Visibility.Visible;
                I6miss.Visibility = Visibility.Collapsed;
                I6hit.Visibility = Visibility.Collapsed;
                I6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 6]
            if (playingSurface[8, 6] == "@")
            {
                I7.Visibility = Visibility.Collapsed;
                I7miss.Visibility = Visibility.Collapsed;
                I7hit.Visibility = Visibility.Visible;
                I7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 6] == "X")
            {
                I7.Visibility = Visibility.Collapsed;
                I7miss.Visibility = Visibility.Visible;
                I7hit.Visibility = Visibility.Collapsed;
                I7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 6] == "#")
            {
                I7.Visibility = Visibility.Collapsed;
                I7miss.Visibility = Visibility.Collapsed;
                I7hit.Visibility = Visibility.Collapsed;
                I7ship.Visibility = Visibility.Visible;
            }
            else
            {
                I7.Visibility = Visibility.Visible;
                I7miss.Visibility = Visibility.Collapsed;
                I7hit.Visibility = Visibility.Collapsed;
                I7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 7]
            if (playingSurface[8, 7] == "@")
            {
                I8.Visibility = Visibility.Collapsed;
                I8miss.Visibility = Visibility.Collapsed;
                I8hit.Visibility = Visibility.Visible;
                I8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 7] == "X")
            {
                I8.Visibility = Visibility.Collapsed;
                I8miss.Visibility = Visibility.Visible;
                I8hit.Visibility = Visibility.Collapsed;
                I8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 7] == "#")
            {
                I8.Visibility = Visibility.Collapsed;
                I8miss.Visibility = Visibility.Collapsed;
                I8hit.Visibility = Visibility.Collapsed;
                I8ship.Visibility = Visibility.Visible;
            }
            else
            {
                I8.Visibility = Visibility.Visible;
                I8miss.Visibility = Visibility.Collapsed;
                I8hit.Visibility = Visibility.Collapsed;
                I8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 8]
            if (playingSurface[8, 8] == "@")
            {
                I9.Visibility = Visibility.Collapsed;
                I9miss.Visibility = Visibility.Collapsed;
                I9hit.Visibility = Visibility.Visible;
                I9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 8] == "X")
            {
                I9.Visibility = Visibility.Collapsed;
                I9miss.Visibility = Visibility.Visible;
                I9hit.Visibility = Visibility.Collapsed;
                I9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 8] == "#")
            {
                I9.Visibility = Visibility.Collapsed;
                I9miss.Visibility = Visibility.Collapsed;
                I9hit.Visibility = Visibility.Collapsed;
                I9ship.Visibility = Visibility.Visible;
            }
            else
            {
                I9.Visibility = Visibility.Visible;
                I9miss.Visibility = Visibility.Collapsed;
                I9hit.Visibility = Visibility.Collapsed;
                I9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[8, 9]
            if (playingSurface[8, 9] == "@")
            {
                I10.Visibility = Visibility.Collapsed;
                I10miss.Visibility = Visibility.Collapsed;
                I10hit.Visibility = Visibility.Visible;
                I10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 9] == "X")
            {
                I10.Visibility = Visibility.Collapsed;
                I10miss.Visibility = Visibility.Visible;
                I10hit.Visibility = Visibility.Collapsed;
                I10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[8, 9] == "#")
            {
                I10.Visibility = Visibility.Collapsed;
                I10miss.Visibility = Visibility.Collapsed;
                I10hit.Visibility = Visibility.Collapsed;
                I10ship.Visibility = Visibility.Visible;
            }
            else
            {
                I10.Visibility = Visibility.Visible;
                I10miss.Visibility = Visibility.Collapsed;
                I10hit.Visibility = Visibility.Collapsed;
                I10ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 0]
            if (playingSurface[9, 0] == "@")
            {
                J1.Visibility = Visibility.Collapsed;
                J1miss.Visibility = Visibility.Collapsed;
                J1hit.Visibility = Visibility.Visible;
                J1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 0] == "X")
            {
                J1.Visibility = Visibility.Collapsed;
                J1miss.Visibility = Visibility.Visible;
                J1hit.Visibility = Visibility.Collapsed;
                J1ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 0] == "#")
            {
                J1.Visibility = Visibility.Collapsed;
                J1miss.Visibility = Visibility.Collapsed;
                J1hit.Visibility = Visibility.Collapsed;
                J1ship.Visibility = Visibility.Visible;
            }
            else
            {
                J1.Visibility = Visibility.Visible;
                J1miss.Visibility = Visibility.Collapsed;
                J1hit.Visibility = Visibility.Collapsed;
                J1ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 1]
            if (playingSurface[9, 1] == "@")
            {
                J2.Visibility = Visibility.Collapsed;
                J2miss.Visibility = Visibility.Collapsed;
                J2hit.Visibility = Visibility.Visible;
                J2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 1] == "X")
            {
                J2.Visibility = Visibility.Collapsed;
                J2miss.Visibility = Visibility.Visible;
                J2hit.Visibility = Visibility.Collapsed;
                J2ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 1] == "#")
            {
                J2.Visibility = Visibility.Collapsed;
                J2miss.Visibility = Visibility.Collapsed;
                J2hit.Visibility = Visibility.Collapsed;
                J2ship.Visibility = Visibility.Visible;
            }
            else
            {
                J2.Visibility = Visibility.Visible;
                J2miss.Visibility = Visibility.Collapsed;
                J2hit.Visibility = Visibility.Collapsed;
                J2ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 2]
            if (playingSurface[9, 2] == "@")
            {
                J3.Visibility = Visibility.Collapsed;
                J3miss.Visibility = Visibility.Collapsed;
                J3hit.Visibility = Visibility.Visible;
                J3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 2] == "X")
            {
                J3.Visibility = Visibility.Collapsed;
                J3miss.Visibility = Visibility.Visible;
                J3hit.Visibility = Visibility.Collapsed;
                J3ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 2] == "#")
            {
                J3.Visibility = Visibility.Collapsed;
                J3miss.Visibility = Visibility.Collapsed;
                J3hit.Visibility = Visibility.Collapsed;
                J3ship.Visibility = Visibility.Visible;
            }
            else
            {
                J3.Visibility = Visibility.Visible;
                J3miss.Visibility = Visibility.Collapsed;
                J3hit.Visibility = Visibility.Collapsed;
                J3ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 3]
            if (playingSurface[9, 3] == "@")
            {
                J4.Visibility = Visibility.Collapsed;
                J4miss.Visibility = Visibility.Collapsed;
                J4hit.Visibility = Visibility.Visible;
                J4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 3] == "X")
            {
                J4.Visibility = Visibility.Collapsed;
                J4miss.Visibility = Visibility.Visible;
                J4hit.Visibility = Visibility.Collapsed;
                J4ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 3] == "#")
            {
                J4.Visibility = Visibility.Collapsed;
                J4miss.Visibility = Visibility.Collapsed;
                J4hit.Visibility = Visibility.Collapsed;
                J4ship.Visibility = Visibility.Visible;
            }
            else
            {
                J4.Visibility = Visibility.Visible;
                J4miss.Visibility = Visibility.Collapsed;
                J4hit.Visibility = Visibility.Collapsed;
                J4ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 4] 
            if (playingSurface[9, 4] == "@")
            {
                J5.Visibility = Visibility.Collapsed;
                J5miss.Visibility = Visibility.Collapsed;
                J5hit.Visibility = Visibility.Visible;
                J5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 4] == "X")
            {
                J5.Visibility = Visibility.Collapsed;
                J5miss.Visibility = Visibility.Visible;
                J5hit.Visibility = Visibility.Collapsed;
                J5ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 4] == "#")
            {
                J5.Visibility = Visibility.Collapsed;
                J5miss.Visibility = Visibility.Collapsed;
                J5hit.Visibility = Visibility.Collapsed;
                J5ship.Visibility = Visibility.Visible;
            }
            else
            {
                J5.Visibility = Visibility.Visible;
                J5miss.Visibility = Visibility.Collapsed;
                J5hit.Visibility = Visibility.Collapsed;
                J5ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 5]
            if (playingSurface[9, 5] == "@")
            {
                J6.Visibility = Visibility.Collapsed;
                J6miss.Visibility = Visibility.Collapsed;
                J6hit.Visibility = Visibility.Visible;
                J6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 5] == "X")
            {
                J6.Visibility = Visibility.Collapsed;
                J6miss.Visibility = Visibility.Visible;
                J6hit.Visibility = Visibility.Collapsed;
                J6ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 5] == "#")
            {
                J6.Visibility = Visibility.Collapsed;
                J6miss.Visibility = Visibility.Collapsed;
                J6hit.Visibility = Visibility.Collapsed;
                J6ship.Visibility = Visibility.Visible;
            }
            else
            {
                J6.Visibility = Visibility.Visible;
                J6miss.Visibility = Visibility.Collapsed;
                J6hit.Visibility = Visibility.Collapsed;
                J6ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 6]
            if (playingSurface[9, 6] == "@")
            {
                J7.Visibility = Visibility.Collapsed;
                J7miss.Visibility = Visibility.Collapsed;
                J7hit.Visibility = Visibility.Visible;
                J7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 6] == "X")
            {
                J7.Visibility = Visibility.Collapsed;
                J7miss.Visibility = Visibility.Visible;
                J7hit.Visibility = Visibility.Collapsed;
                J7ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 6] == "#")
            {
                J7.Visibility = Visibility.Collapsed;
                J7miss.Visibility = Visibility.Collapsed;
                J7hit.Visibility = Visibility.Collapsed;
                J7ship.Visibility = Visibility.Visible;
            }
            else
            {
                J7.Visibility = Visibility.Visible;
                J7miss.Visibility = Visibility.Collapsed;
                J7hit.Visibility = Visibility.Collapsed;
                J7ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 7]
            if (playingSurface[9, 7] == "@")
            {
                J8.Visibility = Visibility.Collapsed;
                J8miss.Visibility = Visibility.Collapsed;
                J8hit.Visibility = Visibility.Visible;
                J8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 7] == "X")
            {
                J8.Visibility = Visibility.Collapsed;
                J8miss.Visibility = Visibility.Visible;
                J8hit.Visibility = Visibility.Collapsed;
                J8ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 7] == "#")
            {
                J8.Visibility = Visibility.Collapsed;
                J8miss.Visibility = Visibility.Collapsed;
                J8hit.Visibility = Visibility.Collapsed;
                J8ship.Visibility = Visibility.Visible;
            }
            else
            {
                J8.Visibility = Visibility.Visible;
                J8miss.Visibility = Visibility.Collapsed;
                J8hit.Visibility = Visibility.Collapsed;
                J8ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region playingSurface[9, 8]
            if (playingSurface[9, 8] == "@")
            {
                J9.Visibility = Visibility.Collapsed;
                J9miss.Visibility = Visibility.Collapsed;
                J9hit.Visibility = Visibility.Visible;
                J9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 8] == "X")
            {
                J9.Visibility = Visibility.Collapsed;
                J9miss.Visibility = Visibility.Visible;
                J9hit.Visibility = Visibility.Collapsed;
                J9ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 8] == "#")
            {
                J9.Visibility = Visibility.Collapsed;
                J9miss.Visibility = Visibility.Collapsed;
                J9hit.Visibility = Visibility.Collapsed;
                J9ship.Visibility = Visibility.Visible;
            }
            else
            {
                J9.Visibility = Visibility.Visible;
                J9miss.Visibility = Visibility.Collapsed;
                J9hit.Visibility = Visibility.Collapsed;
                J9ship.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region (playingSurface[9, 9]
            if (playingSurface[9, 9] == "@")
            {
                J10.Visibility = Visibility.Collapsed;
                J10miss.Visibility = Visibility.Collapsed;
                J10hit.Visibility = Visibility.Visible;
                J10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 9] == "X")
            {
                J10.Visibility = Visibility.Collapsed;
                J10miss.Visibility = Visibility.Visible;
                J10hit.Visibility = Visibility.Collapsed;
                J10ship.Visibility = Visibility.Collapsed;
            }
            else if (playingSurface[9, 9] == "#")
            {
                J10.Visibility = Visibility.Collapsed;
                J10miss.Visibility = Visibility.Collapsed;
                J10hit.Visibility = Visibility.Collapsed;
                J10ship.Visibility = Visibility.Visible;
            }
            else
            {
                J10.Visibility = Visibility.Visible;
                J10miss.Visibility = Visibility.Collapsed;
                J10hit.Visibility = Visibility.Collapsed;
                J10ship.Visibility = Visibility.Collapsed;
            }
            #endregion


        }
    

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                messageForGame.Text = (String)e.Parameter;
            }

            base.OnNavigatedTo(e);
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(GamePlayHuman), App.messageOut);
            Application.Current.Exit();
            
        }

        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GameSetup));
        }

        private void highScores_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HighScore));
        }
    }
}
