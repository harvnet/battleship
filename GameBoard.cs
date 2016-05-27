using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class GameBoard
    {
        private String[,] gameBoard = new String[10,10];  
        public GameBoard()
        {
        }

        /** @author Paul Harvey
         *  setGameBoard is used to initialize the internal gameboard and 
         *  fill it with blanks.
         *  setGameBoard is paired with getGameBoard	
         */
        public void setGameBoard()
        {
            for (int xfill = 0; xfill < 10; ++xfill)
            {
                for (int yfill = 0; yfill < 10; ++yfill)
                {
                    gameBoard[xfill,yfill] = " ";
                }
            }
        }
        /** @author Paul Harvey
         *  getGameBoard is paired with setGameBoard	
         */
        public String[,] getGameBoard()
        {
            return gameBoard;
        }
        /** @author Paul Harvey
         *  displayPlayingSurface is a procedure that displays the entire
         *  game board in play with the side borders.  It will display 
         *  hits and misses.	
         */
        
    }


}
