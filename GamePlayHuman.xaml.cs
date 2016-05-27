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
    /// 

    public sealed partial class GamePlayHuman : Page
    {
        public GamePlayHuman()
        {
            this.InitializeComponent();
            computerGameBoardDisplay(App.parallelBoard);

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Need to save all data
        }

        private void NextTurn_Click(object sender, RoutedEventArgs e)
        {

            // do validation here
            string cCoord = colCoord.Text;
            string rCoord = rowCoord.Text;
            Byte result = 0;
            Boolean success = false;

            success = Byte.TryParse(rCoord, out result);
            if (success)
            {
                if (result >= 1 && result <= 10)
                {

                    //check player name
                    if (cCoord.Equals("A") || cCoord.Equals("B") || cCoord.Equals("C") || cCoord.Equals("D") || cCoord.Equals("E") || cCoord.Equals("F") || cCoord.Equals("G") || cCoord.Equals("H") || cCoord.Equals("I") || cCoord.Equals("J"))
                    {
                        goodValidation(cCoord, result);  // if good then prepare the player

                        // do not do next lines if game over.
                        App.messageOut += "It is now " + App.aiPlayerName + "'s turn.";
                        Frame.Navigate(typeof(GamePlayAI), App.messageOut);
                    }
                    else
                    {
                        messageArea.Text = "Please enter a proper Column Coordinate (A-Z).";

                    }

                }
                else
                {
                    messageArea.Text = "Please enter a proper Row Coordinate (1-10).";
                }
            }
            else
            {
                messageArea.Text = "Please enter number between 2 and 5.";
                // display in error message area.  You need to enter a good number.

            }
        }

        private void goodValidation(String colCoord, Byte rowCoord)
        {

            String torpedoCoordinate = "" + colCoord + rowCoord.ToString();

            Coordinates testCoord = new Coordinates(torpedoCoordinate);
            testCoord.setInputtedXCoordinate(torpedoCoordinate);
            testCoord.setInputtedYCoordinate(torpedoCoordinate);
            Byte xCoord = testCoord.getInputtedXCoordinate();
            Byte yCoord = testCoord.getInputtedYCoordinate();
            String shipClassification = "";
            Byte shipSize = 0;
            String hitShip = "";


            if (App.computerBoard[xCoord, yCoord] == "#" || App.computerBoard[xCoord, yCoord] == " ")
            {
                switch (App.computerBoard[xCoord, yCoord])
                {
                    case "#":
                        /*
                            *  Hit - What happens?	1. Change marker to @ on both computer boards
                            *  					2. Figure out which ship was hit
                            *  					3. Set Hit counter on Ship
                            *  					4. See if Ship is Sunk	
                            *  					5. See if all Ships were Sunk					
                            */

                        App.messageOut = App.humanPlayerName + ", you have hit a ";
                        App.parallelBoard[xCoord, yCoord] = "@";
                        App.computerBoard[xCoord, yCoord] = "@";
                        Debug.WriteLine("HIT Coords = " + xCoord + " " + yCoord);


                        int shipNumber = 0;
                        String[] shipNumberCoord = new String[App.aiNumOfShips];

                        

                        Boolean gotShipNumber = false;
                        do
                        {
                            for (Byte loop = 0; loop < App.aiNumOfShips; ++loop)
                            {
                               
                                hitShip = "" + xCoord.ToString() + yCoord.ToString();
                                //shipSize = (Byte)(App.computerShips[loop].getShipSize() * 2);
                                //App.computerShips[loop].setShipClassification((Byte)(shipSize / 2));
                                //shipClassification = App.computerShips[loop].getShipClassification();

                                shipNumberCoord = App.computerShips[loop].getShipNumber();
                                shipSize = (Byte)(shipNumberCoord[loop].Length);

                                Debug.WriteLine("shipNumberCoord=" + shipNumberCoord[loop]);

                                for (int i = 0; i < shipSize; i = i + 2)
                                {

                                    // need to get the ship number of the ship that was hit  
                                    // looks like  141516
                                    Debug.WriteLine("Shipsize= " + shipSize);
                                    Debug.WriteLine("shipNumberCoord[loop].Substring(i, 2)=" + shipNumberCoord[loop].Substring(i, 2));
                                    Debug.WriteLine("hitShip=" + hitShip);


                                    if (shipNumberCoord[loop].Substring(i, 2).Equals(hitShip))
                                    {
                                        shipNumber = loop;
                                        gotShipNumber = true;
                                        App.computerShips[loop].setShipClassification((Byte)(shipSize / 2));
                                        shipClassification = App.computerShips[loop].getShipClassification();
                                    }
                                }
                            }
                        } while (gotShipNumber == false);

                        App.computerShips[shipNumber].setHits();  // set hits counter on ship
                        App.computerShips[shipNumber].setSunk((Byte)(shipSize / 2));  // Check to see if ship is sunk
                        Boolean computerShipSunk = App.computerShips[shipNumber].getSunk();  // is the ship truly sunk

                        App.messageOut += shipClassification + " at Coordinates " + torpedoCoordinate;
                        App.messageOut += "\n";

                        if (computerShipSunk == true)
                        { // now we check and see if all ships are sunk
                            App.computerShipsSunk++;
                            if (App.computerShipsSunk == App.aiNumOfShips)
                            {
                                App.messageOut += "All of the computer ships have sunk.  You have won!";

                                // Change buttons at bottom of screen
                                nextTurn.Visibility = Visibility.Collapsed;
                                quit.Visibility = Visibility.Visible;
                                newGame.Visibility = Visibility.Visible;
                                highScores.Visibility = Visibility.Visible;

                                App.gameOver = true;
                            }
                        }

                        break;
                    case " ":
                        /*
                        * Miss - place a X on the board so that the dumb human doesn't pick this coordinate again.							
                        */
                        String missShip = "" + xCoord.ToString() + yCoord.ToString();
                        App.messageOut = App.humanPlayerName + ", you have missed at Coordinates " + torpedoCoordinate;
                        App.messageOut += "\n";

                        App.parallelBoard[xCoord, yCoord] = "X";
                        App.computerBoard[xCoord, yCoord] = "X";
                        break;
                }
            }
        }

        public void computerGameBoardDisplay(String[,] playingSurface)
        {

            if (playingSurface[0, 0] == "@")
            {
                A1.Visibility = Visibility.Collapsed;
                A1miss.Visibility = Visibility.Collapsed;
                A1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 0] == "X")
            {
                A1.Visibility = Visibility.Collapsed;
                A1miss.Visibility = Visibility.Visible;
                A1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A1.Visibility = Visibility.Visible;
                A1miss.Visibility = Visibility.Collapsed;
                A1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 1] == "@")
            {
                A2.Visibility = Visibility.Collapsed;
                A2miss.Visibility = Visibility.Collapsed;
                A2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 1] == "X")
            {
                A2.Visibility = Visibility.Collapsed;
                A2miss.Visibility = Visibility.Visible;
                A2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A2.Visibility = Visibility.Visible;
                A2miss.Visibility = Visibility.Collapsed;
                A2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 2] == "@")
            {
                A3.Visibility = Visibility.Collapsed;
                A3miss.Visibility = Visibility.Collapsed;
                A3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 2] == "X")
            {
                A3.Visibility = Visibility.Collapsed;
                A3miss.Visibility = Visibility.Visible;
                A3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A3.Visibility = Visibility.Visible;
                A3miss.Visibility = Visibility.Collapsed;
                A3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 3] == "@")
            {
                A4.Visibility = Visibility.Collapsed;
                A4miss.Visibility = Visibility.Collapsed;
                A4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 3] == "X")
            {
                A4.Visibility = Visibility.Collapsed;
                A4miss.Visibility = Visibility.Visible;
                A4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A4.Visibility = Visibility.Visible;
                A4miss.Visibility = Visibility.Collapsed;
                A4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 4] == "@")
            {
                A5.Visibility = Visibility.Collapsed;
                A5miss.Visibility = Visibility.Collapsed;
                A5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 4] == "X")
            {
                A5.Visibility = Visibility.Collapsed;
                A5miss.Visibility = Visibility.Visible;
                A5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A5.Visibility = Visibility.Visible;
                A5miss.Visibility = Visibility.Collapsed;
                A5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 5] == "@")
            {
                A6.Visibility = Visibility.Collapsed;
                A6miss.Visibility = Visibility.Collapsed;
                A6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 5] == "X")
            {
                A6.Visibility = Visibility.Collapsed;
                A6miss.Visibility = Visibility.Visible;
                A6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A6.Visibility = Visibility.Visible;
                A6miss.Visibility = Visibility.Collapsed;
                A6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 6] == "@")
            {
                A7.Visibility = Visibility.Collapsed;
                A7miss.Visibility = Visibility.Collapsed;
                A7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 6] == "X")
            {
                A7.Visibility = Visibility.Collapsed;
                A7miss.Visibility = Visibility.Visible;
                A7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A7.Visibility = Visibility.Visible;
                A7miss.Visibility = Visibility.Collapsed;
                A7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 7] == "@")
            {
                A8.Visibility = Visibility.Collapsed;
                A8miss.Visibility = Visibility.Collapsed;
                A8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 7] == "X")
            {
                A8.Visibility = Visibility.Collapsed;
                A8miss.Visibility = Visibility.Visible;
                A8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A8.Visibility = Visibility.Visible;
                A8miss.Visibility = Visibility.Collapsed;
                A8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 8] == "@")
            {
                A9.Visibility = Visibility.Collapsed;
                A9miss.Visibility = Visibility.Collapsed;
                A9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 8] == "X")
            {
                A9.Visibility = Visibility.Collapsed;
                A9miss.Visibility = Visibility.Visible;
                A9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A9.Visibility = Visibility.Visible;
                A9miss.Visibility = Visibility.Collapsed;
                A9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[0, 9] == "@")
            {
                A10.Visibility = Visibility.Collapsed;
                A10miss.Visibility = Visibility.Collapsed;
                A10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[0, 9] == "X")
            {
                A10.Visibility = Visibility.Collapsed;
                A10miss.Visibility = Visibility.Visible;
                A10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                A10.Visibility = Visibility.Visible;
                A10miss.Visibility = Visibility.Collapsed;
                A10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 0] == "@")
            {
                B1.Visibility = Visibility.Collapsed;
                B1miss.Visibility = Visibility.Collapsed;
                B1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 0] == "X")
            {
                B1.Visibility = Visibility.Collapsed;
                B1miss.Visibility = Visibility.Visible;
                B1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B1.Visibility = Visibility.Visible;
                B1miss.Visibility = Visibility.Collapsed;
                B1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 1] == "@")
            {
                B2.Visibility = Visibility.Collapsed;
                B2miss.Visibility = Visibility.Collapsed;
                B2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 1] == "X")
            {
                B2.Visibility = Visibility.Collapsed;
                B2miss.Visibility = Visibility.Visible;
                B2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B2.Visibility = Visibility.Visible;
                B2miss.Visibility = Visibility.Collapsed;
                B2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 2] == "@")
            {
                B3.Visibility = Visibility.Collapsed;
                B3miss.Visibility = Visibility.Collapsed;
                B3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 2] == "X")
            {
                B3.Visibility = Visibility.Collapsed;
                B3miss.Visibility = Visibility.Visible;
                B3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B3.Visibility = Visibility.Visible;
                B3miss.Visibility = Visibility.Collapsed;
                B3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 3] == "@")
            {
                B4.Visibility = Visibility.Collapsed;
                B4miss.Visibility = Visibility.Collapsed;
                B4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 3] == "X")
            {
                B4.Visibility = Visibility.Collapsed;
                B4miss.Visibility = Visibility.Visible;
                B4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B4.Visibility = Visibility.Visible;
                B4miss.Visibility = Visibility.Collapsed;
                B4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 4] == "@")
            {
                B5.Visibility = Visibility.Collapsed;
                B5miss.Visibility = Visibility.Collapsed;
                B5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 4] == "X")
            {
                B5.Visibility = Visibility.Collapsed;
                B5miss.Visibility = Visibility.Visible;
                B5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B5.Visibility = Visibility.Visible;
                B5miss.Visibility = Visibility.Collapsed;
                B5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 5] == "@")
            {
                B6.Visibility = Visibility.Collapsed;
                B6miss.Visibility = Visibility.Collapsed;
                B6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 5] == "X")
            {
                B6.Visibility = Visibility.Collapsed;
                B6miss.Visibility = Visibility.Visible;
                B6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B6.Visibility = Visibility.Visible;
                B6miss.Visibility = Visibility.Collapsed;
                B6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 6] == "@")
            {
                B7.Visibility = Visibility.Collapsed;
                B7miss.Visibility = Visibility.Collapsed;
                B7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 6] == "X")
            {
                B7.Visibility = Visibility.Collapsed;
                B7miss.Visibility = Visibility.Visible;
                B7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B7.Visibility = Visibility.Visible;
                B7miss.Visibility = Visibility.Collapsed;
                B7hit.Visibility = Visibility.Collapsed;
            }


            if (playingSurface[1, 7] == "@")
            {
                B8.Visibility = Visibility.Collapsed;
                B8miss.Visibility = Visibility.Collapsed;
                B8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 7] == "X")
            {
                B8.Visibility = Visibility.Collapsed;
                B8miss.Visibility = Visibility.Visible;
                B8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B8.Visibility = Visibility.Visible;
                B8miss.Visibility = Visibility.Collapsed;
                B8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 8] == "@")
            {
                B9.Visibility = Visibility.Collapsed;
                B9miss.Visibility = Visibility.Collapsed;
                B9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 8] == "X")
            {
                B9.Visibility = Visibility.Collapsed;
                B9miss.Visibility = Visibility.Visible;
                B9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B9.Visibility = Visibility.Visible;
                B9miss.Visibility = Visibility.Collapsed;
                B9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[1, 9] == "@")
            {
                B10.Visibility = Visibility.Collapsed;
                B10miss.Visibility = Visibility.Collapsed;
                B10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[1, 9] == "X")
            {
                B10.Visibility = Visibility.Collapsed;
                B10miss.Visibility = Visibility.Visible;
                B10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                B10.Visibility = Visibility.Visible;
                B10miss.Visibility = Visibility.Collapsed;
                B10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 0] == "@")
            {
                C1.Visibility = Visibility.Collapsed;
                C1miss.Visibility = Visibility.Collapsed;
                C1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 0] == "X")
            {
                C1.Visibility = Visibility.Collapsed;
                C1miss.Visibility = Visibility.Visible;
                C1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C1.Visibility = Visibility.Visible;
                C1miss.Visibility = Visibility.Collapsed;
                C1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 1] == "@")
            {
                C2.Visibility = Visibility.Collapsed;
                C2miss.Visibility = Visibility.Collapsed;
                C2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 1] == "X")
            {
                C2.Visibility = Visibility.Collapsed;
                C2miss.Visibility = Visibility.Visible;
                C2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C2.Visibility = Visibility.Visible;
                C2miss.Visibility = Visibility.Collapsed;
                C2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 2] == "@")
            {
                C3.Visibility = Visibility.Collapsed;
                C3miss.Visibility = Visibility.Collapsed;
                C3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 2] == "X")
            {
                C3.Visibility = Visibility.Collapsed;
                C3miss.Visibility = Visibility.Visible;
                C3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C3.Visibility = Visibility.Visible;
                C3miss.Visibility = Visibility.Collapsed;
                C3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 3] == "@")
            {
                C4.Visibility = Visibility.Collapsed;
                C4miss.Visibility = Visibility.Collapsed;
                C4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 3] == "X")
            {
                C4.Visibility = Visibility.Collapsed;
                C4miss.Visibility = Visibility.Visible;
                C4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C4.Visibility = Visibility.Visible;
                C4miss.Visibility = Visibility.Collapsed;
                C4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 4] == "@")
            {
                C5.Visibility = Visibility.Collapsed;
                C5miss.Visibility = Visibility.Collapsed;
                C5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 4] == "X")
            {
                C5.Visibility = Visibility.Collapsed;
                C5miss.Visibility = Visibility.Visible;
                C5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C5.Visibility = Visibility.Visible;
                C5miss.Visibility = Visibility.Collapsed;
                C5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 5] == "@")
            {
                C6.Visibility = Visibility.Collapsed;
                C6miss.Visibility = Visibility.Collapsed;
                C6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 5] == "X")
            {
                C6.Visibility = Visibility.Collapsed;
                C6miss.Visibility = Visibility.Visible;
                C6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C6.Visibility = Visibility.Visible;
                C6miss.Visibility = Visibility.Collapsed;
                C6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 6] == "@")
            {
                C7.Visibility = Visibility.Collapsed;
                C7miss.Visibility = Visibility.Collapsed;
                C7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 6] == "X")
            {
                C7.Visibility = Visibility.Collapsed;
                C7miss.Visibility = Visibility.Visible;
                C7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C7.Visibility = Visibility.Visible;
                C7miss.Visibility = Visibility.Collapsed;
                C7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 7] == "@")
            {
                C8.Visibility = Visibility.Collapsed;
                C8miss.Visibility = Visibility.Collapsed;
                C8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 7] == "X")
            {
                C8.Visibility = Visibility.Collapsed;
                C8miss.Visibility = Visibility.Visible;
                C8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C8.Visibility = Visibility.Visible;
                C8miss.Visibility = Visibility.Collapsed;
                C8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 8] == "@")
            {
                C9.Visibility = Visibility.Collapsed;
                C9miss.Visibility = Visibility.Collapsed;
                C9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 8] == "X")
            {
                C9.Visibility = Visibility.Collapsed;
                C9miss.Visibility = Visibility.Visible;
                C9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C9.Visibility = Visibility.Visible;
                C9miss.Visibility = Visibility.Collapsed;
                C9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[2, 9] == "@")
            {
                C10.Visibility = Visibility.Collapsed;
                C10miss.Visibility = Visibility.Collapsed;
                C10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[2, 9] == "X")
            {
                C10.Visibility = Visibility.Collapsed;
                C10miss.Visibility = Visibility.Visible;
                C10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                C10.Visibility = Visibility.Visible;
                C10miss.Visibility = Visibility.Collapsed;
                C10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 0] == "@")
            {
                D1.Visibility = Visibility.Collapsed;
                D1miss.Visibility = Visibility.Collapsed;
                D1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 0] == "X")
            {
                D1.Visibility = Visibility.Collapsed;
                D1miss.Visibility = Visibility.Visible;
                D1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D1.Visibility = Visibility.Visible;
                D1miss.Visibility = Visibility.Collapsed;
                D1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 1] == "@")
            {
                D2.Visibility = Visibility.Collapsed;
                D2miss.Visibility = Visibility.Collapsed;
                D2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 1] == "X")
            {
                D2.Visibility = Visibility.Collapsed;
                D2miss.Visibility = Visibility.Visible;
                D2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D2.Visibility = Visibility.Visible;
                D2miss.Visibility = Visibility.Collapsed;
                D2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 2] == "@")
            {
                D3.Visibility = Visibility.Collapsed;
                D3miss.Visibility = Visibility.Collapsed;
                D3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 2] == "X")
            {
                D3.Visibility = Visibility.Collapsed;
                D3miss.Visibility = Visibility.Visible;
                D3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D3.Visibility = Visibility.Visible;
                D3miss.Visibility = Visibility.Collapsed;
                D3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 3] == "@")
            {
                D4.Visibility = Visibility.Collapsed;
                D4miss.Visibility = Visibility.Collapsed;
                D4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 3] == "X")
            {
                D4.Visibility = Visibility.Collapsed;
                D4miss.Visibility = Visibility.Visible;
                D4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D4.Visibility = Visibility.Visible;
                D4miss.Visibility = Visibility.Collapsed;
                D4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 4] == "@")
            {
                D5.Visibility = Visibility.Collapsed;
                D5miss.Visibility = Visibility.Collapsed;
                D5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 4] == "X")
            {
                D5.Visibility = Visibility.Collapsed;
                D5miss.Visibility = Visibility.Visible;
                D5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D5.Visibility = Visibility.Visible;
                D5miss.Visibility = Visibility.Collapsed;
                D5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 5] == "@")
            {
                D6.Visibility = Visibility.Collapsed;
                D6miss.Visibility = Visibility.Collapsed;
                D6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 5] == "X")
            {
                D6.Visibility = Visibility.Collapsed;
                D6miss.Visibility = Visibility.Visible;
                D6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D6.Visibility = Visibility.Visible;
                D6miss.Visibility = Visibility.Collapsed;
                D6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 6] == "@")
            {
                D7.Visibility = Visibility.Collapsed;
                D7miss.Visibility = Visibility.Collapsed;
                D7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 6] == "X")
            {
                D7.Visibility = Visibility.Collapsed;
                D7miss.Visibility = Visibility.Visible;
                D7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D7.Visibility = Visibility.Visible;
                D7miss.Visibility = Visibility.Collapsed;
                D7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 7] == "@")
            {
                D8.Visibility = Visibility.Collapsed;
                D8miss.Visibility = Visibility.Collapsed;
                D8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 7] == "X")
            {
                D8.Visibility = Visibility.Collapsed;
                D8miss.Visibility = Visibility.Visible;
                D8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D8.Visibility = Visibility.Visible;
                D8miss.Visibility = Visibility.Collapsed;
                D8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 8] == "@")
            {
                D9.Visibility = Visibility.Collapsed;
                D9miss.Visibility = Visibility.Collapsed;
                D9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 8] == "X")
            {
                D9.Visibility = Visibility.Collapsed;
                D9miss.Visibility = Visibility.Visible;
                D9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D9.Visibility = Visibility.Visible;
                D9miss.Visibility = Visibility.Collapsed;
                D9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[3, 9] == "@")
            {
                D10.Visibility = Visibility.Collapsed;
                D10miss.Visibility = Visibility.Collapsed;
                D10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[3, 9] == "X")
            {
                D10.Visibility = Visibility.Collapsed;
                D10miss.Visibility = Visibility.Visible;
                D10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                D10.Visibility = Visibility.Visible;
                D10miss.Visibility = Visibility.Collapsed;
                D10hit.Visibility = Visibility.Collapsed;
            }


            if (playingSurface[4, 0] == "@")
            {
                E1.Visibility = Visibility.Collapsed;
                E1miss.Visibility = Visibility.Collapsed;
                E1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 0] == "X")
            {
                E1.Visibility = Visibility.Collapsed;
                E1miss.Visibility = Visibility.Visible;
                E1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E1.Visibility = Visibility.Visible;
                E1miss.Visibility = Visibility.Collapsed;
                E1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 1] == "@")
            {
                E2.Visibility = Visibility.Collapsed;
                E2miss.Visibility = Visibility.Collapsed;
                E2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 1] == "X")
            {
                E2.Visibility = Visibility.Collapsed;
                E2miss.Visibility = Visibility.Visible;
                E2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E2.Visibility = Visibility.Visible;
                E2miss.Visibility = Visibility.Collapsed;
                E2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 2] == "@")
            {
                E3.Visibility = Visibility.Collapsed;
                E3miss.Visibility = Visibility.Collapsed;
                E3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 2] == "X")
            {
                E3.Visibility = Visibility.Collapsed;
                E3miss.Visibility = Visibility.Visible;
                E3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E3.Visibility = Visibility.Visible;
                E3miss.Visibility = Visibility.Collapsed;
                E3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 3] == "@")
            {
                E4.Visibility = Visibility.Collapsed;
                E4miss.Visibility = Visibility.Collapsed;
                E4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 3] == "X")
            {
                E4.Visibility = Visibility.Collapsed;
                E4miss.Visibility = Visibility.Visible;
                E4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E4.Visibility = Visibility.Visible;
                E4miss.Visibility = Visibility.Collapsed;
                E4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 4] == "@")
            {
                E5.Visibility = Visibility.Collapsed;
                E5miss.Visibility = Visibility.Collapsed;
                E5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 4] == "X")
            {
                E5.Visibility = Visibility.Collapsed;
                E5miss.Visibility = Visibility.Visible;
                E5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E5.Visibility = Visibility.Visible;
                E5miss.Visibility = Visibility.Collapsed;
                E5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 5] == "@")
            {
                E6.Visibility = Visibility.Collapsed;
                E6miss.Visibility = Visibility.Collapsed;
                E6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 5] == "X")
            {
                E6.Visibility = Visibility.Collapsed;
                E6miss.Visibility = Visibility.Visible;
                E6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E6.Visibility = Visibility.Visible;
                E6miss.Visibility = Visibility.Collapsed;
                E6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 6] == "@")
            {
                E7.Visibility = Visibility.Collapsed;
                E7miss.Visibility = Visibility.Collapsed;
                E7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 6] == "X")
            {
                E7.Visibility = Visibility.Collapsed;
                E7miss.Visibility = Visibility.Visible;
                E7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E7.Visibility = Visibility.Visible;
                E7miss.Visibility = Visibility.Collapsed;
                E7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 7] == "@")
            {
                E8.Visibility = Visibility.Collapsed;
                E8miss.Visibility = Visibility.Collapsed;
                E8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 7] == "X")
            {
                E8.Visibility = Visibility.Collapsed;
                E8miss.Visibility = Visibility.Visible;
                E8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E8.Visibility = Visibility.Visible;
                E8miss.Visibility = Visibility.Collapsed;
                E8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 8] == "@")
            {
                E9.Visibility = Visibility.Collapsed;
                E9miss.Visibility = Visibility.Collapsed;
                E9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 8] == "X")
            {
                E9.Visibility = Visibility.Collapsed;
                E9miss.Visibility = Visibility.Visible;
                E9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E9.Visibility = Visibility.Visible;
                E9miss.Visibility = Visibility.Collapsed;
                E9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[4, 9] == "@")
            {
                E10.Visibility = Visibility.Collapsed;
                E10miss.Visibility = Visibility.Collapsed;
                E10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[4, 9] == "X")
            {
                E10.Visibility = Visibility.Collapsed;
                E10miss.Visibility = Visibility.Visible;
                E10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                E10.Visibility = Visibility.Visible;
                E10miss.Visibility = Visibility.Collapsed;
                E10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 0] == "@")
            {
                F1.Visibility = Visibility.Collapsed;
                F1miss.Visibility = Visibility.Collapsed;
                F1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 0] == "X")
            {
                F1.Visibility = Visibility.Collapsed;
                F1miss.Visibility = Visibility.Visible;
                F1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F1.Visibility = Visibility.Visible;
                F1miss.Visibility = Visibility.Collapsed;
                F1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 1] == "@")
            {
                F2.Visibility = Visibility.Collapsed;
                F2miss.Visibility = Visibility.Collapsed;
                F2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 1] == "X")
            {
                F2.Visibility = Visibility.Collapsed;
                F2miss.Visibility = Visibility.Visible;
                F2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F2.Visibility = Visibility.Visible;
                F2miss.Visibility = Visibility.Collapsed;
                F2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 2] == "@")
            {
                F3.Visibility = Visibility.Collapsed;
                F3miss.Visibility = Visibility.Collapsed;
                F3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 2] == "X")
            {
                F3.Visibility = Visibility.Collapsed;
                F3miss.Visibility = Visibility.Visible;
                F3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F3.Visibility = Visibility.Visible;
                F3miss.Visibility = Visibility.Collapsed;
                F3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 3] == "@")
            {
                F4.Visibility = Visibility.Collapsed;
                F4miss.Visibility = Visibility.Collapsed;
                F4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 3] == "X")
            {
                F4.Visibility = Visibility.Collapsed;
                F4miss.Visibility = Visibility.Visible;
                F4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F4.Visibility = Visibility.Visible;
                F4miss.Visibility = Visibility.Collapsed;
                F4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 4] == "@")
            {
                F5.Visibility = Visibility.Collapsed;
                F5miss.Visibility = Visibility.Collapsed;
                F5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 4] == "X")
            {
                F5.Visibility = Visibility.Collapsed;
                F5miss.Visibility = Visibility.Visible;
                F5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F5.Visibility = Visibility.Visible;
                F5miss.Visibility = Visibility.Collapsed;
                F5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 5] == "@")
            {
                F6.Visibility = Visibility.Collapsed;
                F6miss.Visibility = Visibility.Collapsed;
                F6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 5] == "X")
            {
                F6.Visibility = Visibility.Collapsed;
                F6miss.Visibility = Visibility.Visible;
                F6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F6.Visibility = Visibility.Visible;
                F6miss.Visibility = Visibility.Collapsed;
                F6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 6] == "@")
            {
                F7.Visibility = Visibility.Collapsed;
                F7miss.Visibility = Visibility.Collapsed;
                F7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 6] == "X")
            {
                F7.Visibility = Visibility.Collapsed;
                F7miss.Visibility = Visibility.Visible;
                F7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F7.Visibility = Visibility.Visible;
                F7miss.Visibility = Visibility.Collapsed;
                F7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 7] == "@")
            {
                F8.Visibility = Visibility.Collapsed;
                F8miss.Visibility = Visibility.Collapsed;
                F8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 7] == "X")
            {
                F8.Visibility = Visibility.Collapsed;
                F8miss.Visibility = Visibility.Visible;
                F8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F8.Visibility = Visibility.Visible;
                F8miss.Visibility = Visibility.Collapsed;
                F8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 8] == "@")
            {
                F9.Visibility = Visibility.Collapsed;
                F9miss.Visibility = Visibility.Collapsed;
                F9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 8] == "X")
            {
                F9.Visibility = Visibility.Collapsed;
                F9miss.Visibility = Visibility.Visible;
                F9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F9.Visibility = Visibility.Visible;
                F9miss.Visibility = Visibility.Collapsed;
                F9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[5, 9] == "@")
            {
                F10.Visibility = Visibility.Collapsed;
                F10miss.Visibility = Visibility.Collapsed;
                F10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[5, 9] == "X")
            {
                F10.Visibility = Visibility.Collapsed;
                F10miss.Visibility = Visibility.Visible;
                F10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                F10.Visibility = Visibility.Visible;
                F10miss.Visibility = Visibility.Collapsed;
                F10hit.Visibility = Visibility.Collapsed;
            }


            if (playingSurface[6, 0] == "@")
            {
                G1.Visibility = Visibility.Collapsed;
                G1miss.Visibility = Visibility.Collapsed;
                G1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 0] == "X")
            {
                G1.Visibility = Visibility.Collapsed;
                G1miss.Visibility = Visibility.Visible;
                G1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G1.Visibility = Visibility.Visible;
                G1miss.Visibility = Visibility.Collapsed;
                G1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 1] == "@")
            {
                G2.Visibility = Visibility.Collapsed;
                G2miss.Visibility = Visibility.Collapsed;
                G2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 1] == "X")
            {
                G2.Visibility = Visibility.Collapsed;
                G2miss.Visibility = Visibility.Visible;
                G2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G2.Visibility = Visibility.Visible;
                G2miss.Visibility = Visibility.Collapsed;
                G2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 2] == "@")
            {
                G3.Visibility = Visibility.Collapsed;
                G3miss.Visibility = Visibility.Collapsed;
                G3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 2] == "X")
            {
                G3.Visibility = Visibility.Collapsed;
                G3miss.Visibility = Visibility.Visible;
                G3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G3.Visibility = Visibility.Visible;
                G3miss.Visibility = Visibility.Collapsed;
                G3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 3] == "@")
            {
                G4.Visibility = Visibility.Collapsed;
                G4miss.Visibility = Visibility.Collapsed;
                G4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 3] == "X")
            {
                G4.Visibility = Visibility.Collapsed;
                G4miss.Visibility = Visibility.Visible;
                G4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G4.Visibility = Visibility.Visible;
                G4miss.Visibility = Visibility.Collapsed;
                G4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 4] == "@")
            {
                G5.Visibility = Visibility.Collapsed;
                G5miss.Visibility = Visibility.Collapsed;
                G5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 4] == "X")
            {
                G5.Visibility = Visibility.Collapsed;
                G5miss.Visibility = Visibility.Visible;
                G5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G5.Visibility = Visibility.Visible;
                G5miss.Visibility = Visibility.Collapsed;
                G5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 5] == "@")
            {
                G6.Visibility = Visibility.Collapsed;
                G6miss.Visibility = Visibility.Collapsed;
                G6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 5] == "X")
            {
                G6.Visibility = Visibility.Collapsed;
                G6miss.Visibility = Visibility.Visible;
                G6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G6.Visibility = Visibility.Visible;
                G6miss.Visibility = Visibility.Collapsed;
                G6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 6] == "@")
            {
                G7.Visibility = Visibility.Collapsed;
                G7miss.Visibility = Visibility.Collapsed;
                G7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 6] == "X")
            {
                G7.Visibility = Visibility.Collapsed;
                G7miss.Visibility = Visibility.Visible;
                G7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G7.Visibility = Visibility.Visible;
                G7miss.Visibility = Visibility.Collapsed;
                G7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 7] == "@")
            {
                G8.Visibility = Visibility.Collapsed;
                G8miss.Visibility = Visibility.Collapsed;
                G8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 7] == "X")
            {
                G8.Visibility = Visibility.Collapsed;
                G8miss.Visibility = Visibility.Visible;
                G8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G8.Visibility = Visibility.Visible;
                G8miss.Visibility = Visibility.Collapsed;
                G8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 8] == "@")
            {
                G9.Visibility = Visibility.Collapsed;
                G9miss.Visibility = Visibility.Collapsed;
                G9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 8] == "X")
            {
                G9.Visibility = Visibility.Collapsed;
                G9miss.Visibility = Visibility.Visible;
                G9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G9.Visibility = Visibility.Visible;
                G9miss.Visibility = Visibility.Collapsed;
                G9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[6, 9] == "@")
            {
                G10.Visibility = Visibility.Collapsed;
                G10miss.Visibility = Visibility.Collapsed;
                G10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[6, 9] == "X")
            {
                G10.Visibility = Visibility.Collapsed;
                G10miss.Visibility = Visibility.Visible;
                G10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                G10.Visibility = Visibility.Visible;
                G10miss.Visibility = Visibility.Collapsed;
                G10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 0] == "@")
            {
                H1.Visibility = Visibility.Collapsed;
                H1miss.Visibility = Visibility.Collapsed;
                H1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 0] == "X")
            {
                H1.Visibility = Visibility.Collapsed;
                H1miss.Visibility = Visibility.Visible;
                H1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H1.Visibility = Visibility.Visible;
                H1miss.Visibility = Visibility.Collapsed;
                H1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 1] == "@")
            {
                H2.Visibility = Visibility.Collapsed;
                H2miss.Visibility = Visibility.Collapsed;
                H2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 1] == "X")
            {
                H2.Visibility = Visibility.Collapsed;
                H2miss.Visibility = Visibility.Visible;
                H2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H2.Visibility = Visibility.Visible;
                H2miss.Visibility = Visibility.Collapsed;
                H2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 2] == "@")
            {
                H3.Visibility = Visibility.Collapsed;
                H3miss.Visibility = Visibility.Collapsed;
                H3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 2] == "X")
            {
                H3.Visibility = Visibility.Collapsed;
                H3miss.Visibility = Visibility.Visible;
                H3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H3.Visibility = Visibility.Visible;
                H3miss.Visibility = Visibility.Collapsed;
                H3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 3] == "@")
            {
                H4.Visibility = Visibility.Collapsed;
                H4miss.Visibility = Visibility.Collapsed;
                H4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 3] == "X")
            {
                H4.Visibility = Visibility.Collapsed;
                H4miss.Visibility = Visibility.Visible;
                H4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H4.Visibility = Visibility.Visible;
                H4miss.Visibility = Visibility.Collapsed;
                H4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 4] == "@")
            {
                H5.Visibility = Visibility.Collapsed;
                H5miss.Visibility = Visibility.Collapsed;
                H5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 4] == "X")
            {
                H5.Visibility = Visibility.Collapsed;
                H5miss.Visibility = Visibility.Visible;
                H5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H5.Visibility = Visibility.Visible;
                H5miss.Visibility = Visibility.Collapsed;
                H5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 5] == "@")
            {
                H6.Visibility = Visibility.Collapsed;
                H6miss.Visibility = Visibility.Collapsed;
                H6hit.Visibility = Visibility.Visible;
            }
            else if  (playingSurface[7, 5] == "X")
            {
                H6.Visibility = Visibility.Collapsed;
                H6miss.Visibility = Visibility.Visible;
                H6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H6.Visibility = Visibility.Visible;
                H6miss.Visibility = Visibility.Collapsed;
                H6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 6] == "@")
            {
                H7.Visibility = Visibility.Collapsed;
                H7miss.Visibility = Visibility.Collapsed;
                H7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 6] == "X")
            {
                H7.Visibility = Visibility.Collapsed;
                H7miss.Visibility = Visibility.Visible;
                H7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H7.Visibility = Visibility.Visible;
                H7miss.Visibility = Visibility.Collapsed;
                H7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 7] == "@")
            {
                H8.Visibility = Visibility.Collapsed;
                H8miss.Visibility = Visibility.Collapsed;
                H8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 7] == "X")
            {
                H8.Visibility = Visibility.Collapsed;
                H8miss.Visibility = Visibility.Visible;
                H8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H8.Visibility = Visibility.Visible;
                H8miss.Visibility = Visibility.Collapsed;
                H8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 8] == "@")
            {
                H9.Visibility = Visibility.Collapsed;
                H9miss.Visibility = Visibility.Collapsed;
                H9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 8] == "X")
            {
                H9.Visibility = Visibility.Collapsed;
                H9miss.Visibility = Visibility.Visible;
                H9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H9.Visibility = Visibility.Visible;
                H9miss.Visibility = Visibility.Collapsed;
                H9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[7, 9] == "@")
            {
                H10.Visibility = Visibility.Collapsed;
                H10miss.Visibility = Visibility.Collapsed;
                H10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[7, 9] == "X")
            {
                H10.Visibility = Visibility.Collapsed;
                H10miss.Visibility = Visibility.Visible;
                H10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                H10.Visibility = Visibility.Visible;
                H10miss.Visibility = Visibility.Collapsed;
                H10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 0] == "@")
            {
                I1.Visibility = Visibility.Collapsed;
                I1miss.Visibility = Visibility.Collapsed;
                I1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 0] == "X")
            {
                I1.Visibility = Visibility.Collapsed;
                I1miss.Visibility = Visibility.Visible;
                I1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I1.Visibility = Visibility.Visible;
                I1miss.Visibility = Visibility.Collapsed;
                I1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 1] == "@")
            {
                I2.Visibility = Visibility.Collapsed;
                I2miss.Visibility = Visibility.Collapsed;
                I2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 1] == "X")
            {
                I2.Visibility = Visibility.Collapsed;
                I2miss.Visibility = Visibility.Visible;
                I2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I2.Visibility = Visibility.Visible;
                I2miss.Visibility = Visibility.Collapsed;
                I2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 2] == "@")
            {
                I3.Visibility = Visibility.Collapsed;
                I3miss.Visibility = Visibility.Collapsed;
                I3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 2] == "X")
            {
                I3.Visibility = Visibility.Collapsed;
                I3miss.Visibility = Visibility.Visible;
                I3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I3.Visibility = Visibility.Visible;
                I3miss.Visibility = Visibility.Collapsed;
                I3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 3] == "@")
            {
                I4.Visibility = Visibility.Collapsed;
                I4miss.Visibility = Visibility.Collapsed;
                I4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 3] == "X")
            {
                I4.Visibility = Visibility.Collapsed;
                I4miss.Visibility = Visibility.Visible;
                I4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I4.Visibility = Visibility.Visible;
                I4miss.Visibility = Visibility.Collapsed;
                I4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 4] == "@")
            {
                I5.Visibility = Visibility.Collapsed;
                I5miss.Visibility = Visibility.Collapsed;
                I5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 4] == "X")
            {
                I5.Visibility = Visibility.Collapsed;
                I5miss.Visibility = Visibility.Visible;
                I5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I5.Visibility = Visibility.Visible;
                I5miss.Visibility = Visibility.Collapsed;
                I5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 5] == "@")
            {
                I6.Visibility = Visibility.Collapsed;
                I6miss.Visibility = Visibility.Collapsed;
                I6hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 5] == "X")
            {
                I6.Visibility = Visibility.Collapsed;
                I6miss.Visibility = Visibility.Visible;
                I6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I6.Visibility = Visibility.Visible;
                I6miss.Visibility = Visibility.Collapsed;
                I6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 6] == "@")
            {
                I7.Visibility = Visibility.Collapsed;
                I7miss.Visibility = Visibility.Collapsed;
                I7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 6] == "X")
            {
                I7.Visibility = Visibility.Collapsed;
                I7miss.Visibility = Visibility.Visible;
                I7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I7.Visibility = Visibility.Visible;
                I7miss.Visibility = Visibility.Collapsed;
                I7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 7] == "@")
            {
                I8.Visibility = Visibility.Collapsed;
                I8miss.Visibility = Visibility.Collapsed;
                I8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 7] == "X")
            {
                I8.Visibility = Visibility.Collapsed;
                I8miss.Visibility = Visibility.Visible;
                I8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I8.Visibility = Visibility.Visible;
                I8miss.Visibility = Visibility.Collapsed;
                I8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 8] == "@")
            {
                I9.Visibility = Visibility.Collapsed;
                I9miss.Visibility = Visibility.Collapsed;
                I9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 8] == "X")
            {
                I9.Visibility = Visibility.Collapsed;
                I9miss.Visibility = Visibility.Visible;
                I9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I9.Visibility = Visibility.Visible;
                I9miss.Visibility = Visibility.Collapsed;
                I9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[8, 9] == "@")
            {
                I10.Visibility = Visibility.Collapsed;
                I10miss.Visibility = Visibility.Collapsed;
                I10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[8, 9] == "X")
            {
                I10.Visibility = Visibility.Collapsed;
                I10miss.Visibility = Visibility.Visible;
                I10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                I10.Visibility = Visibility.Visible;
                I10miss.Visibility = Visibility.Collapsed;
                I10hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 0] == "@")
            {
                J1.Visibility = Visibility.Collapsed;
                J1miss.Visibility = Visibility.Collapsed;
                J1hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 0] == "X")
            {
                J1.Visibility = Visibility.Collapsed;
                J1miss.Visibility = Visibility.Visible;
                J1hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J1.Visibility = Visibility.Visible;
                J1miss.Visibility = Visibility.Collapsed;
                J1hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 1] == "@")
            {
                J2.Visibility = Visibility.Collapsed;
                J2miss.Visibility = Visibility.Collapsed;
                J2hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 1] == "X")
            {
                J2.Visibility = Visibility.Collapsed;
                J2miss.Visibility = Visibility.Visible;
                J2hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J2.Visibility = Visibility.Visible;
                J2miss.Visibility = Visibility.Collapsed;
                J2hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 2] == "@")
            {
                J3.Visibility = Visibility.Collapsed;
                J3miss.Visibility = Visibility.Collapsed;
                J3hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 2] == "X")
            {
                J3.Visibility = Visibility.Collapsed;
                J3miss.Visibility = Visibility.Visible;
                J3hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J3.Visibility = Visibility.Visible;
                J3miss.Visibility = Visibility.Collapsed;
                J3hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 3] == "@")
            {
                J4.Visibility = Visibility.Collapsed;
                J4miss.Visibility = Visibility.Collapsed;
                J4hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 3] == "X")
            {
                J4.Visibility = Visibility.Collapsed;
                J4miss.Visibility = Visibility.Visible;
                J4hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J4.Visibility = Visibility.Visible;
                J4miss.Visibility = Visibility.Collapsed;
                J4hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 4] == "@")
            {
                J5.Visibility = Visibility.Collapsed;
                J5miss.Visibility = Visibility.Collapsed;
                J5hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 4] == "X")
            {
                J5.Visibility = Visibility.Collapsed;
                J5miss.Visibility = Visibility.Visible;
                J5hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J5.Visibility = Visibility.Visible;
                J5miss.Visibility = Visibility.Collapsed;
                J5hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 5] == "@")
            {
                J6.Visibility = Visibility.Collapsed;
                J6miss.Visibility = Visibility.Collapsed;
                J6hit.Visibility = Visibility.Visible;
            }
           else if (playingSurface[9, 5] == "X")
            {
                J6.Visibility = Visibility.Collapsed;
                J6miss.Visibility = Visibility.Visible;
                J6hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J6.Visibility = Visibility.Visible;
                J6miss.Visibility = Visibility.Collapsed;
                J6hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 6] == "@")
            {
                J7.Visibility = Visibility.Collapsed;
                J7miss.Visibility = Visibility.Collapsed;
                J7hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 6] == "X")
            {
                J7.Visibility = Visibility.Collapsed;
                J7miss.Visibility = Visibility.Visible;
                J7hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J7.Visibility = Visibility.Visible;
                J7miss.Visibility = Visibility.Collapsed;
                J7hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 7] == "@")
            {
                J8.Visibility = Visibility.Collapsed;
                J8miss.Visibility = Visibility.Collapsed;
                J8hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 7] == "X")
            {
                J8.Visibility = Visibility.Collapsed;
                J8miss.Visibility = Visibility.Visible;
                J8hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J8.Visibility = Visibility.Visible;
                J8miss.Visibility = Visibility.Collapsed;
                J8hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 8] == "@")
            {
                J9.Visibility = Visibility.Collapsed;
                J9miss.Visibility = Visibility.Collapsed;
                J9hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 8] == "X")
            {
                J9.Visibility = Visibility.Collapsed;
                J9miss.Visibility = Visibility.Visible;
                J9hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J9.Visibility = Visibility.Visible;
                J9miss.Visibility = Visibility.Collapsed;
                J9hit.Visibility = Visibility.Collapsed;
            }

            if (playingSurface[9, 9] == "@")
            {
                J10.Visibility = Visibility.Collapsed;
                J10miss.Visibility = Visibility.Collapsed;
                J10hit.Visibility = Visibility.Visible;
            }
            else if (playingSurface[9, 9] == "X")
            {
                J10.Visibility = Visibility.Collapsed;
                J10miss.Visibility = Visibility.Visible;
                J10hit.Visibility = Visibility.Collapsed;
            }
            else
            {
                J10.Visibility = Visibility.Visible;
                J10miss.Visibility = Visibility.Collapsed;
                J10hit.Visibility = Visibility.Collapsed;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                messageArea.Text = (String)e.Parameter;
            }

            base.OnNavigatedTo(e);
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {
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

    