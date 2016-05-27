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
    public sealed partial class GameSetup : Page
    {

        private RangedRandomNumber numComputerShipsGenerator;
        private RangedRandomNumber computerNameGenerator;
        private RangedRandomNumber firstTurnGenerator;

        public GameSetup()
        {
            this.InitializeComponent();
            numComputerShipsGenerator = new RangedRandomNumber();
            computerNameGenerator = new RangedRandomNumber();
            firstTurnGenerator = new RangedRandomNumber();
        }

        private void GamePlayAI_Click(object sender, RoutedEventArgs e)
        {
            // Generate Random Number to see who goes first (flip coin)
            Byte firstTurn = setFirstTurn();

            if (firstTurn == 0)
            {
                Frame.Navigate(typeof(GamePlayAI));
            } else
            {
                Frame.Navigate(typeof(GamePlayHuman));
            }
        }

        private void MainPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void PlaceShips_Click(object sender, RoutedEventArgs e)
        {
            string pName = playerName.Text;
            string numberOfShips = numOfShips.Text;
            Byte result = 0;
            Boolean success = false;

            success = Byte.TryParse(numberOfShips, out result);
            if (success)
            {
                if (result >= 2 && result <= 5)
                {

                    //check player name
                    if (pName.Length > 0)
                    {
                        goodValidation(pName, result);  // if good then prepare the player
                    }
                    else
                    {
                        messageArea.Text = "Please enter a player name.";

                    }

                }
                else
                {
                    messageArea.Text = "Please enter number between 2 and 5.";
                }
            }
            else
            {
                messageArea.Text = "Please enter number between 2 and 5.";
                // display in error message area.  You need to enter a good number.

            }
        }
        private void goodValidation(String name, Byte numShips)
        {
            // set up global variables
            App.humanPlayerName = name;
            App.humanNumOfShips = numShips;

            // set up human player (player class)
            Player humanPlayer = new Player(name, numShips);

            // create game board for human player (gameboard class)
            GameBoard Boardhuman = new GameBoard();
            Boardhuman.setGameBoard();
            App.humanBoard = Boardhuman.getGameBoard();

            // place ships on internal human game board (ship class)

            Boolean placeflag = false;
            //Ship[] humanShips = new Ship[numShips];
            Array.Resize(ref App.humanShips, App.humanNumOfShips);


            for (int i=0;i<App.humanNumOfShips;++i) {
                App.humanShips[i] = new Ship();
                Byte shipSize = App.humanShips[i].setShipSize(2,6);
            
                do
                {
            
                    String shipFirstCoord = App.humanShips[i].setFirstPosition();
                    String shipSecondCoord = App.humanShips[i].setSecondPosition(shipSize, shipFirstCoord, App.humanBoard);

                    Debug.WriteLine("ShipSecondCoord=" + shipSecondCoord);

                    if (!String.IsNullOrEmpty(shipSecondCoord))
                    {
                        if (!shipSecondCoord.Trim().Equals("XX") )
                        {
                            placeflag = false;
                            placeflag = App.humanShips[i].checkOverlap(shipFirstCoord, shipSecondCoord, App.humanBoard);
                            if (placeflag == true)
                            {
                                App.humanBoard = App.humanShips[i].placeShip(shipFirstCoord, shipSecondCoord);
                                App.humanShips[i].setShipNumber(i);
                            }
                        }
                    }

                } while (placeflag == false);
            }

            // set up computer player (player class)
            App.aiPlayerName = setComputerName();
            App.aiNumOfShips = setNumComputerShips();
            
            Player computerPlayer = new Player(App.aiPlayerName, App.aiNumOfShips);

            // create game board for computer player.  This board will show all 
            // actions such as hit, miss, and ship positions.  The game will check
            // against this board all the time but this will not be used for UI. 
            // the parallel game board will be used for UI
            GameBoard Boardcomputer = new GameBoard();
            Boardcomputer.setGameBoard();
            App.computerBoard = Boardcomputer.getGameBoard();

            // this is a board that is used by the computer because the player is not 
            // allowed to see where the ships are (cheating).  So we need a board that 
            // will be used for displaying to the UI
            GameBoard Boardparallel = new GameBoard();
            Boardparallel.setGameBoard();
            App.parallelBoard = Boardparallel.getGameBoard();

            // place ships on internal computer game board (ship class)

            placeflag = false;
            //Ship[] computerShips = new Ship[App.aiNumOfShips];
            Array.Resize(ref App.computerShips, App.aiNumOfShips);

            for (int i = 0; i < App.aiNumOfShips; ++i)
            {
                App.computerShips[i] = new Ship();
                Byte shipSize = App.computerShips[i].setShipSize(2, 6);
                Boolean badSecondCoord = true;
                do
                {
                    if (badSecondCoord)
                    {
                        //redo first and second coordinates

                        String shipFirstCoord = App.computerShips[i].setFirstPosition();
                        String shipSecondCoord = App.computerShips[i].setSecondPosition(shipSize, shipFirstCoord, App.computerBoard);

                        Debug.WriteLine("ShipSecondCoord=" + shipSecondCoord);

                        if (!String.IsNullOrEmpty(shipSecondCoord))
                        {
                            if (!shipSecondCoord.Trim().Equals("XX"))
                            {
                                badSecondCoord = false;
                                placeflag = false;
                                placeflag = App.computerShips[i].checkOverlap(shipFirstCoord, shipSecondCoord, App.computerBoard);
                                if (placeflag == true)
                                {
                                    App.computerBoard = App.computerShips[i].placeShip(shipFirstCoord, shipSecondCoord);
                                    App.computerShips[i].setShipNumber(i);
                                }
                            }
                            else
                            {
                                badSecondCoord = true;   //bad second coordinates
                            }
                        }
                    }
                } while (placeflag == false);
            }

            humanInitialShipPlacement(App.humanBoard);
            // display game board

            // disable placeships button
            placeShips.Visibility = Visibility.Collapsed;

            // disable name and placeships control
            playerName.IsReadOnly = true;
            numOfShips.IsReadOnly = true;

            // enable gamePlayAI button
            startGame.Visibility = Visibility.Visible;



        }

    public void humanInitialShipPlacement(String[,] playingSurface)
        {


            if (playingSurface[0, 0] == "#")
            {
                A1.Visibility = Visibility.Collapsed;
                A1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 1] == "#")
            {
                A2.Visibility = Visibility.Collapsed;
                A2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 2] == "#")
            {
                A3.Visibility = Visibility.Collapsed;
                A3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 3] == "#")
            {
                A4.Visibility = Visibility.Collapsed;
                A4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 4] == "#")
            {
                A5.Visibility = Visibility.Collapsed;
                A5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 5] == "#")
            {
                A6.Visibility = Visibility.Collapsed;
                A6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 6] == "#")
            {
                A7.Visibility = Visibility.Collapsed;
                A7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 7] == "#")
            {
                A8.Visibility = Visibility.Collapsed;
                A8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 8] == "#")
            {
                A9.Visibility = Visibility.Collapsed;
                A9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[0, 9] == "#")
            {
                A10.Visibility = Visibility.Collapsed;
                A10ship.Visibility = Visibility.Visible;
            }

            if (playingSurface[1, 0] == "#")
            {
                B1.Visibility = Visibility.Collapsed;
                B1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 1] == "#")
            {
                B2.Visibility = Visibility.Collapsed;
                B2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 2] == "#")
            {
                B3.Visibility = Visibility.Collapsed;
                B3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 3] == "#")
            {
                B4.Visibility = Visibility.Collapsed;
                B4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 4] == "#")
            {
                B5.Visibility = Visibility.Collapsed;
                B5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 5] == "#")
            {
                B6.Visibility = Visibility.Collapsed;
                B6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 6] == "#")
            {
                B7.Visibility = Visibility.Collapsed;
                B7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 7] == "#")
            {
                B8.Visibility = Visibility.Collapsed;
                B8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 8] == "#")
            {
                B9.Visibility = Visibility.Collapsed;
                B9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[1, 9] == "#")
            {
                B10.Visibility = Visibility.Collapsed;
                B10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 0] == "#")
            {
                C1.Visibility = Visibility.Collapsed;
                C1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 1] == "#")
            {
                C2.Visibility = Visibility.Collapsed;
                C2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 2] == "#")
            {
                C3.Visibility = Visibility.Collapsed;
                C3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 3] == "#")
            {
                C4.Visibility = Visibility.Collapsed;
                C4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 4] == "#")
            {
                C5.Visibility = Visibility.Collapsed;
                C5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 5] == "#")
            {
                C6.Visibility = Visibility.Collapsed;
                C6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 6] == "#")
            {
                C7.Visibility = Visibility.Collapsed;
                C7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 7] == "#")
            {
                C8.Visibility = Visibility.Collapsed;
                C8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 8] == "#")
            {
                C9.Visibility = Visibility.Collapsed;
                C9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[2, 9] == "#")
            {
                C10.Visibility = Visibility.Collapsed;
                C10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 0] == "#")
            {
                D1.Visibility = Visibility.Collapsed;
                D1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 1] == "#")
            {
                D2.Visibility = Visibility.Collapsed;
                D2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 2] == "#")
            {
                D3.Visibility = Visibility.Collapsed;
                D3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 3] == "#")
            {
                D4.Visibility = Visibility.Collapsed;
                D4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 4] == "#")
            {
                D5.Visibility = Visibility.Collapsed;
                D5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 5] == "#")
            {
                D6.Visibility = Visibility.Collapsed;
                D6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 6] == "#")
            {
                D7.Visibility = Visibility.Collapsed;
                D7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 7] == "#")
            {
                D8.Visibility = Visibility.Collapsed;
                D8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 8] == "#")
            {
                D9.Visibility = Visibility.Collapsed;
                D9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[3, 9] == "#")
            {
                D10.Visibility = Visibility.Collapsed;
                D10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 0] == "#")
            {
                E1.Visibility = Visibility.Collapsed;
                E1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 1] == "#")
            {
                E2.Visibility = Visibility.Collapsed;
                E2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 2] == "#")
            {
                E3.Visibility = Visibility.Collapsed;
                E3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 3] == "#")
            {
                E4.Visibility = Visibility.Collapsed;
                E4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 4] == "#")
            {
                E5.Visibility = Visibility.Collapsed;
                E5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 5] == "#")
            {
                E6.Visibility = Visibility.Collapsed;
                E6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 6] == "#")
            {
                E7.Visibility = Visibility.Collapsed;
                E7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 7] == "#")
            {
                E8.Visibility = Visibility.Collapsed;
                E8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 8] == "#")
            {
                E9.Visibility = Visibility.Collapsed;
                E9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[4, 9] == "#")
            {
                E10.Visibility = Visibility.Collapsed;
                E10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 0] == "#")
            {
                F1.Visibility = Visibility.Collapsed;
                F1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 1] == "#")
            {
                F2.Visibility = Visibility.Collapsed;
                F2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 2] == "#")
            {
                F3.Visibility = Visibility.Collapsed;
                F3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 3] == "#")
            {
                F4.Visibility = Visibility.Collapsed;
                F4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 4] == "#")
            {
                F5.Visibility = Visibility.Collapsed;
                F5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 5] == "#")
            {
                F6.Visibility = Visibility.Collapsed;
                F6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 6] == "#")
            {
                F7.Visibility = Visibility.Collapsed;
                F7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 7] == "#")
            {
                F8.Visibility = Visibility.Collapsed;
                F8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 8] == "#")
            {
                F9.Visibility = Visibility.Collapsed;
                F9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[5, 9] == "#")
            {
                F10.Visibility = Visibility.Collapsed;
                F10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 0] == "#")
            {
                G1.Visibility = Visibility.Collapsed;
                G1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 1] == "#")
            {
                G2.Visibility = Visibility.Collapsed;
                G2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 2] == "#")
            {
                G3.Visibility = Visibility.Collapsed;
                G3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 3] == "#")
            {
                G4.Visibility = Visibility.Collapsed;
                G4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 4] == "#")
            {
                G5.Visibility = Visibility.Collapsed;
                G5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 5] == "#")
            {
                G6.Visibility = Visibility.Collapsed;
                G6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 6] == "#")
            {
                G7.Visibility = Visibility.Collapsed;
                G7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 7] == "#")
            {
                G8.Visibility = Visibility.Collapsed;
                G8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 8] == "#")
            {
                G9.Visibility = Visibility.Collapsed;
                G9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[6, 9] == "#")
            {
                G10.Visibility = Visibility.Collapsed;
                G10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 0] == "#")
            {
                H1.Visibility = Visibility.Collapsed;
                H1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 1] == "#")
            {
                H2.Visibility = Visibility.Collapsed;
                H2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 2] == "#")
            {
                H3.Visibility = Visibility.Collapsed;
                H3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 3] == "#")
            {
                H4.Visibility = Visibility.Collapsed;
                H4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 4] == "#")
            {
                H5.Visibility = Visibility.Collapsed;
                H5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 5] == "#")
            {
                H6.Visibility = Visibility.Collapsed;
                H6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 6] == "#")
            {
                H7.Visibility = Visibility.Collapsed;
                H7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 7] == "#")
            {
                H8.Visibility = Visibility.Collapsed;
                H8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 8] == "#")
            {
                H9.Visibility = Visibility.Collapsed;
                H9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[7, 9] == "#")
            {
                H10.Visibility = Visibility.Collapsed;
                H10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 0] == "#")
            {
                I1.Visibility = Visibility.Collapsed;
                I1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 1] == "#")
            {
                I2.Visibility = Visibility.Collapsed;
                I2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 2] == "#")
            {
                I3.Visibility = Visibility.Collapsed;
                I3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 3] == "#")
            {
                I4.Visibility = Visibility.Collapsed;
                I4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 4] == "#")
            {
                I5.Visibility = Visibility.Collapsed;
                I5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 5] == "#")
            {
                I6.Visibility = Visibility.Collapsed;
                I6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 6] == "#")
            {
                I7.Visibility = Visibility.Collapsed;
                I7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 7] == "#")
            {
                I8.Visibility = Visibility.Collapsed;
                I8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 8] == "#")
            {
                I9.Visibility = Visibility.Collapsed;
                I9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[8, 9] == "#")
            {
                I10.Visibility = Visibility.Collapsed;
                I10ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 0] == "#")
            {
                J1.Visibility = Visibility.Collapsed;
                J1ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 1] == "#")
            {
                J2.Visibility = Visibility.Collapsed;
                J2ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 2] == "#")
            {
                J3.Visibility = Visibility.Collapsed;
                J3ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 3] == "#")
            {
                J4.Visibility = Visibility.Collapsed;
                J4ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 4] == "#")
            {
                J5.Visibility = Visibility.Collapsed;
                J5ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 5] == "#")
            {
                J6.Visibility = Visibility.Collapsed;
                J6ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 6] == "#")
            {
                J7.Visibility = Visibility.Collapsed;
                J7ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 7] == "#")
            {
                J8.Visibility = Visibility.Collapsed;
                J8ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 8] == "#")
            {
                J9.Visibility = Visibility.Collapsed;
                J9ship.Visibility = Visibility.Visible;
            }
            if (playingSurface[9, 9] == "#")
            {
                J10.Visibility = Visibility.Collapsed;
                J10ship.Visibility = Visibility.Visible;
            }
        }

        public Byte setNumComputerShips()
        {

            numComputerShipsGenerator.SetMinimum(2);
            numComputerShipsGenerator.SetMaximum(5);
            Byte numComputerShips = Convert.ToByte(numComputerShipsGenerator.GenerateRandomNumber());

            return numComputerShips;

        }

        public String setComputerName()
        {
            String[] computerNames = { "R2-D2", "Awesome-O", "Robbie the Robot", "C3PO", "Marvin", "Deep Thought", "Hal-9000", "Terminator", "The Borg", "Milton", "Tony the Robot", "Sonny", "Bender", "Calculon" };
            computerNameGenerator.SetMinimum(0);
            computerNameGenerator.SetMaximum(13);
            Byte indexNumber = Convert.ToByte(computerNameGenerator.GenerateRandomNumber());
            
            return computerNames[indexNumber];

        }

        public Byte setFirstTurn()
        {
            computerNameGenerator.SetMinimum(0);
            computerNameGenerator.SetMaximum(1);
            Byte firstTurn = Convert.ToByte(firstTurnGenerator.GenerateRandomNumber());

            return firstTurn;
        }
    }
}
