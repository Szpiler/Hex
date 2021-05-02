using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Hex
{
    class DijkstraSolver: Board
    {
        private int[] prevBlue = new int[boardSize * boardSize + 2];
        private int[] prevRed = new int[boardSize * boardSize + 2];

        // function that checks who is the closest neighbour to the chcking field
        // and returns index of the closest field

        public void ClosestNeighbour(int current, List<int> Q, List<int> S, ref int[] distance, int player, int currentIndx)
        {
            List<int> pomQ = new List<int>();
            for (int i = 0; i < Q.Count; i++)
            {
                if (player == 1)
                {
                    if (Q[i] == 1)
                        pomQ.Add(0);
                    else if (Q[i] == 2)
                        pomQ.Add(boardSize * boardSize);
                    else
                        pomQ.Add(1);
                }
                else
                {
                    if (Q[i] == 2)
                        pomQ.Add(0);
                    else if (Q[i] == 1)
                        pomQ.Add(boardSize * boardSize);
                    else
                        pomQ.Add(1);
                }
            }
            if (currentIndx < boardSize * boardSize)
            {
                if (currentIndx - boardSize >= 0 && currentIndx - boardSize < boardSize * boardSize && !S.Contains(currentIndx - boardSize))
                {
                    if (distance[currentIndx - boardSize] > distance[currentIndx] + pomQ[currentIndx - boardSize])
                    {
                        distance[currentIndx - boardSize] = distance[currentIndx] + pomQ[currentIndx - boardSize];
                        prevBlue[currentIndx - boardSize] = currentIndx;
                    }
                }
                if (currentIndx - boardSize + 1 >= 0 && currentIndx - boardSize + 1 < boardSize * boardSize && !S.Contains(currentIndx - boardSize + 1) && (currentIndx + 1) % boardSize != 0)
                {
                    if (distance[currentIndx - boardSize + 1] > distance[currentIndx] + pomQ[currentIndx - boardSize + 1])
                    {
                        distance[currentIndx - boardSize + 1] = distance[currentIndx] + pomQ[currentIndx - boardSize + 1];
                        prevBlue[currentIndx - boardSize + 1] = currentIndx;
                    }
                }
                if (currentIndx - 1 >= 0 && currentIndx - 1 < boardSize * boardSize && !S.Contains(currentIndx - 1) && currentIndx % boardSize != 0)
                {
                    if (distance[currentIndx - 1] > distance[currentIndx] + pomQ[currentIndx - 1])
                    {
                        distance[currentIndx - 1] = distance[currentIndx] + pomQ[currentIndx - 1];
                        prevBlue[currentIndx - 1] = currentIndx;
                    }
                }
                if (currentIndx + 1 >= 0 && currentIndx + 1 < boardSize * boardSize && !S.Contains(currentIndx + 1) && (currentIndx + 1) % boardSize != 0)
                {
                    if (distance[currentIndx + 1] > distance[currentIndx] + pomQ[currentIndx + 1])
                    {
                        distance[currentIndx + 1] = distance[currentIndx] + pomQ[currentIndx + 1];
                        prevBlue[currentIndx + 1] = currentIndx;
                    }
                }
                if (currentIndx + boardSize - 1 >= 0 && currentIndx + boardSize - 1 < boardSize * boardSize && !S.Contains(currentIndx + boardSize - 1) && currentIndx % boardSize != 0)
                {
                    if (distance[currentIndx + boardSize - 1] > distance[currentIndx] + pomQ[currentIndx + boardSize - 1])
                    {
                        distance[currentIndx + boardSize - 1] = distance[currentIndx] + pomQ[currentIndx + boardSize - 1];
                        prevBlue[currentIndx + boardSize - 1] = currentIndx;
                    }
                }
                if (currentIndx + boardSize >= 0 && currentIndx + boardSize < boardSize * boardSize && !S.Contains(currentIndx + boardSize))
                {
                    if (distance[currentIndx + boardSize] > distance[currentIndx] + pomQ[currentIndx + boardSize])
                    {
                        distance[currentIndx + boardSize] = distance[currentIndx] + pomQ[currentIndx + boardSize];
                        prevBlue[currentIndx + boardSize] = currentIndx;
                    }
                }
                if (player == 1 && currentIndx >= boardSize * boardSize - boardSize)
                {
                    prevBlue[boardSize * boardSize + 1] = currentIndx;
                    distance[boardSize * boardSize + 1] = distance[currentIndx];
                }
                else if (player == 2 && currentIndx % boardSize == 0)
                {
                    prevBlue[boardSize * boardSize + 1] = currentIndx;
                    distance[boardSize * boardSize + 1] = distance[currentIndx];
                }
            }
            else
            {
                // red player starting fields
                if (current == -1 && player == 1)
                {
                    for (int i = 0; i < boardSize; i++)
                    {
                        if (Q[i] == 0 && !S.Contains(i))
                        {
                            distance[i] = 1;
                            prevBlue[i] = currentIndx;
                        }
                        else if (Q[i] == 1 && !S.Contains(i))
                        {
                            distance[i] = 0;
                            prevBlue[i] = currentIndx;
                        }
                        else if (Q[i] == 2 && !S.Contains(i))
                        {
                            distance[i] = boardSize * boardSize;
                            prevBlue[i] = currentIndx;
                        }
                    }
                }

                // blue player starting fields
                else if (current == -1 && player == 2)
                {
                    for (int i = boardSize - 1; i < boardSize * boardSize; i += boardSize)
                    {
                        if (Q[i] == 0 && !S.Contains(i))
                        {
                            distance[i] = 1;
                            prevBlue[i] = currentIndx;
                        }
                        else if (Q[i] == 2 && !S.Contains(i))
                        {
                            distance[i] = 0;
                            prevBlue[i] = currentIndx;
                        }
                        else if (Q[i] == 1 && !S.Contains(i))
                        {
                            distance[i] = boardSize * boardSize;
                            prevBlue[i] = currentIndx;
                        }
                    }
                }
            }
        }

        //function taht returns index of element of list Q that is the closest to get and isn't in list S
        public int ClosestElementInQ(int[] distance, List<int> S)
        {
            int closestElement = -3;
            int min = int.MaxValue;
            for (int i = 0; i < distance.Count(); i++)
            {
                if (min > distance[i] && !S.Contains(i))
                {
                    min = distance[i];
                    closestElement = i;
                }
            }
            return closestElement;
        }

        // function that checks how many moves player have to made to win by using Dijkstra algorithm
        // returns count of moves

        public int DijkstraAlgorithm(List<int> Q, List<int> S, int[] distance, int currentIndx, int player)
        {
            while (Q.Count > S.Count)
            {
                currentIndx = ClosestElementInQ(distance, S);
                S.Add(currentIndx);
                int current = Q[currentIndx];
                if (current == -2)
                {
                    return distance[boardSize * boardSize + 1];
                }
                else
                {
                    ClosestNeighbour(current, Q, S, ref distance, player, currentIndx);
                }

            }
            return distance[boardSize * boardSize + 1];
        }

        // function that checks who is closer to win by using Dijkstra algorithm   
        // returns diffrends between player that moves and his opponent
        // and count of moves needed to win

        public int[] DistanceChecker(int player)
        {
            int start = -1;
            int end = -2;
            int[] diffCountMoves = new int[2];
            int[] distance = new int[boardSize * boardSize + 2];
            distance.Fill(int.MaxValue);
            prevBlue.Fill(-1);
            prevRed.Fill(-1);
            distance[boardSize * boardSize] = 0;
            List<int> Q = new List<int>();
            List<int> S = new List<int>();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Q.Add(board[i, j].color);
                }
            }
            Q.Add(start);
            Q.Add(end);
            int currentIndx = boardSize * boardSize;
            int playerRed = DijkstraAlgorithm(Q, S, distance, currentIndx, 1);
            Array.Copy(prevBlue, prevRed, prevBlue.Count());
            prevBlue.Fill(-1);
            S.Clear();
            distance.Fill(int.MaxValue);
            distance[boardSize * boardSize] = 0;
            currentIndx = boardSize * boardSize;
            int playerBlue = DijkstraAlgorithm(Q, S, distance, currentIndx, 2);
            if (player == 1)
            {
                diffCountMoves[0] = playerBlue - playerRed;
                diffCountMoves[1] = playerRed;
                return diffCountMoves;
            }
            else if (player == 2)
            {
                diffCountMoves[0] = playerRed - playerBlue;
                diffCountMoves[1] = playerBlue;
                return diffCountMoves;
            }
            else
            {
                diffCountMoves[0] = 0;
                diffCountMoves[1] = 0;
                return diffCountMoves;
            }         
        }

        // function that returns the fastest path
        // player = 1 - red,
        // player = 2 - blue
        public List<int> GetShortestPath(int player)
        {
            List<int> shortestPath = new List<int>();
            if (player == 1)
            {
                int curr = prevRed[boardSize * boardSize + 1];
                while (curr != -1)
                {
                    shortestPath.Add(curr);
                    curr = prevRed[curr];
                }
            }
            else
            {
                int curr = prevBlue[boardSize * boardSize + 1];
                while (curr != -1)
                {
                    shortestPath.Add(curr);
                    curr = prevBlue[curr];
                }
            }
            return shortestPath;
        }
    }
}
