﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    enum tileState
    {
        WHITE,
        BLACK,
        EMPTY
    }

    [Serializable()]
    class OthelloBoard : IPlayable
    {
        private const int BOARDSIZE = 8;

        private tileState[,] board;     // X = Line     Y = Column

        private List<Tuple<int, int>> canMove;

        //stopwatch to measure time of each player
        private Stopwatch watch1;
        private Stopwatch watch2;

        /**
         *  Constructor
         */
        public OthelloBoard()
        {
            board = new tileState[BOARDSIZE, BOARDSIZE];
            canMove = new List<Tuple<int, int>>();

            watch1 = new Stopwatch();
            watch2 = new Stopwatch();

            // Make the board empty
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i, j] = tileState.EMPTY;
                }
            }
            

            // Setup first pieces
            board[3, 3] = tileState.BLACK;
            board[3, 4] = tileState.WHITE;
            board[4, 4] = tileState.BLACK;
            board[4, 3] = tileState.WHITE;

            // Get the possible moves (black always start)
            possibleMoves(false);
        }

       public TimeSpan elapsedWatch1()
       {
            TimeSpan ts = watch1.Elapsed;
            return ts;
       }
        
        public TimeSpan elapsedWatch2()
        {
            TimeSpan ts = watch2.Elapsed;
            return ts;
        }
        /*-------------------------------------------------------
         * Iplayable functions
         -------------------------------------------------------- */
        public bool isPlayable(int column, int line, bool isWhite)
        {
            Tuple<int, int> pos = new Tuple<int, int>(column, line);
            possibleMoves(isWhite);
            return canMove.Contains(pos);
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            int x = column;
            int y = line;

            Tuple<int, int> pos = new Tuple<int, int>(x, y);

            if (isWhite)
            { 
                possibleMoves(isWhite);
                if (canMove.Contains(pos))
                {
                    board[x, y] = tileState.WHITE;
                    turnPieces(x, y, isWhite);
                    possibleMoves(!isWhite);

                    //start stopwatch of black player
                    watch2.Start();
                    watch1.Stop();

                    return true;
                }
                else
                    return false;
            }
            else
            {
                possibleMoves(isWhite);
                if (canMove.Contains(pos))
                {
                    board[x, y] = tileState.BLACK;
                    turnPieces(x, y, isWhite);
                    possibleMoves(!isWhite);

                    //start stopwatch of white player
                    watch1.Start();
                    watch2.Stop();

                    return true;
                }
                else
                    return false;
            }
        }

        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int getBlackScore()
        {
            return calculateScore(tileState.BLACK);
        }

        public int getWhiteScore()
        {
            return calculateScore(tileState.WHITE);
        }

        /*-------------------------------------------------------
         * Class functions
         -------------------------------------------------------- */

        private void turnPieces(int x, int y, bool isWhite)
        {
            tileState color;
            tileState ennemyColor;

            int laX;
            int laY;
            bool wentTroughEnnemy;

            bool turnNorth = false;
            bool turnSouth = false;
            bool turnWest = false;
            bool turnEast = false;
            bool turnNorthEast = false;
            bool turnNorthWest = false;
            bool turnSouthEast = false;
            bool turnSouthWest = false;

            // Setup the colors we need to check
            if (!isWhite)
            {
                color = tileState.BLACK;
                ennemyColor = tileState.WHITE;
            }
            else
            {
                color = tileState.WHITE;
                ennemyColor = tileState.BLACK;
            }

            /*
             * CHECK WICH LINES TO TURN
             */

            //check NORTH
            laX = x - 1;
            wentTroughEnnemy = false;
            while (laX >= 0)
            {
                if (board[laX, y] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX -= 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, y] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnNorth = true;
                    break;
                }
                else
                {
                    //empty cell
                    break;
                }
            }

            // check SOUTH
            laX = x + 1;
            wentTroughEnnemy = false;
            while (laX < BOARDSIZE)
            {
                if (board[laX, y] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX += 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, y] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnSouth = true;
                    break;
                }
                else
                {
                    break;
                }
            }


            // check WEST
            laY = y - 1;
            wentTroughEnnemy = false;
            while (laY >= 0)
            {
                if (board[x, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laY -= 1;
                    wentTroughEnnemy = true;
                }
                else if (board[x, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnWest = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            // check EAST
            laY = y + 1;
            wentTroughEnnemy = false;
            while (laY < BOARDSIZE)
            {
                if (board[x, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laY += 1;
                    wentTroughEnnemy = true;
                }
                else if (board[x, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnEast = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            // check NORTH EAST
            laX = x - 1;
            laY = y + 1;
            wentTroughEnnemy = false;
            while (laX >= 0 && laY < BOARDSIZE)
            {
                if (board[laX, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX -= 1;
                    laY += 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnNorthEast = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            // check NORTH WEST
            laX = x - 1;
            laY = y - 1;
            wentTroughEnnemy = false;
            while (laX >= 0 && laY >= 0)
            {
                if (board[laX, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX -= 1;
                    laY -= 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnNorthWest = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            // check SOUTH EAST
            laX = x + 1;
            laY = y + 1;
            wentTroughEnnemy = false;
            while (laX < BOARDSIZE && laY < BOARDSIZE)
            {
                if (board[laX, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX += 1;
                    laY += 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnSouthEast = true;
                    break;
                }
                else
                {
                    break;
                }
            }

            // check SOUTH WEST
            laX = x + 1;
            laY = y - 1;
            wentTroughEnnemy = false;
            while (laX < BOARDSIZE && laY >= 0)
            {
                if (board[laX, laY] == ennemyColor)
                {
                    // if its ennemy, look after
                    laX += 1;
                    laY -= 1;
                    wentTroughEnnemy = true;
                }
                else if (board[laX, laY] == color)
                {
                    // if its our color and we eat ennemies, we gonna turn the line
                    if (wentTroughEnnemy)
                        turnSouthWest = true;
                    break;
                }
                else
                {
                    break;
                }
            }


            /*
             *  TURN PIECES 
             */
            int tempx = x;
            int tempy = y;

            if (turnNorth)
            {
                tempx = x - 1;
                while (board[tempx, y] == ennemyColor)
                {
                    board[tempx, y] = color;
                    tempx -= 1;
                }
            }
            if (turnSouth)
            {
                tempx = x + 1;
                while (board[tempx, y] == ennemyColor)
                {
                    board[tempx, y] = color;
                    tempx += 1;
                }
            }
            if (turnEast)
            {
                tempy = y + 1;
                while (board[x, tempy] == ennemyColor)
                {
                    board[x, tempy] = color;
                    tempy += 1;
                }
            }
            if (turnWest)
            {
                tempy = y - 1;
                while (board[x, tempy] == ennemyColor)
                {
                    board[x, tempy] = color;
                    tempy -= 1;
                }
            }
            if (turnNorthEast)
            {
                tempx = x - 1;
                tempy = y + 1;
                while (board[tempx, tempy] == ennemyColor)
                {
                    board[tempx, tempy] = color;
                    tempx -= 1;
                    tempy += 1;
                }
            }
            if (turnNorthWest)
            {
                tempx = x - 1;
                tempy = y - 1;
                while (board[tempx, tempy] == ennemyColor)
                {
                    board[tempx, tempy] = color;
                    tempx -= 1;
                    tempy -= 1;
                }
            }
            if (turnSouthEast)
            {
                tempx = x + 1;
                tempy = y + 1;
                while (board[tempx, tempy] == ennemyColor)
                {
                    board[tempx, tempy] = color;
                    tempx += 1;
                    tempy += 1;
                }
            }
            if (turnSouthWest)
            {
                tempx = x + 1;
                tempy = y - 1;
                while (board[tempx, tempy] == ennemyColor)
                {
                    board[tempx, tempy] = color;
                    tempx += 1;
                    tempy -= 1;
                }
            }
        }

        public void possibleMoves(bool isWhite)
        {
            tileState color;
            tileState ennemyColor;
            List<Tuple<int, int>> colorList = new List<Tuple<int, int>>();

            // Reset the canMove list
            canMove.Clear();

            // Setup the colors we need to check
            if (!isWhite)
            {
                color = tileState.BLACK;
                ennemyColor = tileState.WHITE;
            }
            else
            {
                color = tileState.WHITE;
                ennemyColor = tileState.BLACK;
            }

            //Get all the color pieces on board
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j] == color)
                    {
                        colorList.Add(new Tuple<int, int>(i, j));
                    }
                }
            }


            foreach (Tuple<int, int> pos in colorList)
            {
                int x = pos.Item1;
                int y = pos.Item2;
                int laX;
                int laY;
                bool wentTroughEnnemy;

                //check NORTH
                laX = x - 1;
                wentTroughEnnemy = false;
                while (laX >= 0)
                {
                    if (board[laX, y] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX -= 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, y] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, y));
                        break;
                    }
                }

                // check SOUTH
                laX = x + 1;
                wentTroughEnnemy = false;
                while (laX < BOARDSIZE)
                {
                    if (board[laX, y] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX += 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, y] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, y));
                        break;
                    }
                }


                // check WEST
                laY = y - 1;
                wentTroughEnnemy = false;
                while (laY >= 0)
                {
                    if (board[x, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laY -= 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[x, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(x, laY));
                        break;
                    }
                }

                // check EAST
                laY = y + 1;
                wentTroughEnnemy = false;
                while (laY < BOARDSIZE)
                {
                    if (board[x, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laY += 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[x, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(x, laY));
                        break;
                    }
                }

                // check NORTH EAST
                laX = x - 1;
                laY = y + 1;
                wentTroughEnnemy = false;
                while (laX >= 0 && laY < BOARDSIZE)
                {
                    if (board[laX, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX -= 1;
                        laY += 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, laY));
                        break;
                    }
                }

                // check NORTH WEST
                laX = x - 1;
                laY = y - 1;
                wentTroughEnnemy = false;
                while (laX >= 0 && laY >= 0)
                {
                    if (board[laX, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX -= 1;
                        laY -= 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, laY));
                        break;
                    }
                }

                // check SOUTH EAST
                laX = x + 1;
                laY = y + 1;
                wentTroughEnnemy = false;
                while (laX < BOARDSIZE && laY < BOARDSIZE)
                {
                    if (board[laX, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX += 1;
                        laY += 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, laY));
                        break;
                    }
                }

                // check SOUTH WEST
                laX = x + 1;
                laY = y - 1;
                wentTroughEnnemy = false;
                while (laX < BOARDSIZE && laY >= 0)
                {
                    if (board[laX, laY] == ennemyColor)
                    {
                        // if its ennemy, look after
                        laX += 1;
                        laY -= 1;
                        wentTroughEnnemy = true;
                    }
                    else if (board[laX, laY] == color)
                    {
                        // if its our color, end
                        break;
                    }
                    else
                    {
                        // if its empty and we eat an enenmy, add it to canMove
                        if (wentTroughEnnemy)
                            canMove.Add(new Tuple<int, int>(laX, laY));
                        break;
                    }
                }
            }
        }
        private int calculateScore(tileState tileColor)
        {
            int score = 0;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j] == tileColor)
                        score++;
                }
            }

            return score;
        }

        public void save(bool isWhite)
        {
            // board + temps + isWhite
            // convert multidimensionnal array to jagged array

        }

        public void load(string path)
        {

        }


        /*-------------------------------------------------------
         * Getters and Setters
         -------------------------------------------------------- */
        //test method that return state of the board (to update pieces after a move)
        public tileState[,] getState()
        {
            return board;
        }

        public List<Tuple<int, int>> getCanMove()
        {
            return canMove;
        }
    }
}
