using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class ArrayExtensions
{
    public static void Fill<T>(this T[] originalArray, T with)
    {
        for (int i = 0; i < originalArray.Length; i++)
        {
            originalArray[i] = with;
        }
    }
}

namespace Project_Hex
{
    class Board : Panel
    {
        public static int boardSize = 11;
        public static int depth = 5;
        public static int alpha = int.MinValue;
        public static int beta = int.MaxValue;

        private int turnCounter = 0;
        private readonly int buttonSize = 50;
        private Button resetBtn;
        private Button startBtn;
        private Label winText;      

        public static HexagonButton[,] board = new HexagonButton[boardSize, boardSize];      

        public void InitialBoard()
        {           
            int blackSpace = 100;
            this.Height = buttonSize + (boardSize - 1) * (buttonSize / 4 * 3) + blackSpace;
            this.Width = boardSize * buttonSize + (boardSize - 1) * (buttonSize / 2) + blackSpace;
            this.Location = new Point((1200 - this.Width) / 2, (750 - this.Height) / 2);
            // reset button
            resetBtn = new Button();
            resetBtn.Height = 25;
            resetBtn.Width = 75;
            resetBtn.Location = new Point(0, this.Height - 75);           
            resetBtn.BackColor = Color.Green;
            resetBtn.Text = "Reset";
            resetBtn.ForeColor = Color.White;
            resetBtn.Click += Reset_Button_Click;
            Controls.Add(resetBtn);
            // winnig label
            winText = new Label();
            winText.Height = 16;
            winText.Width = 100;
            winText.ForeColor = Color.White;
            winText.Location = new Point(0, this.Height - 45);          
            Controls.Add(winText);
            // start button
            startBtn = new Button();
            startBtn.Height = 25;
            startBtn.Width = 75;
            startBtn.Location = new Point(this.Width - startBtn.Width, 0);
            startBtn.BackColor = Color.Green;
            startBtn.Text = "Start";
            startBtn.ForeColor = Color.White;
            startBtn.Click += Start_Button_Click;
            Controls.Add(startBtn);

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    board[i, j] = new HexagonButton();
                    board[i, j].Size = new Size(buttonSize, buttonSize);
                    board[i, j].Location = new Point((j * (buttonSize + 1) + i * (buttonSize / 2) + blackSpace / 2 - (boardSize - 1) / 2), i * (buttonSize / 4 * 3 + 1) + blackSpace / 2 - (boardSize - 1) / 2);
                    board[i, j].BackColor = Color.Gray;
                    board[i, j].FlatAppearance.BorderSize = 0;
                    board[i, j].FlatStyle = FlatStyle.Flat;
                    board[i, j].Click += Panel_Click;
                    Controls.Add(board[i, j]);
                }
            }           
        }

        public void GetGraphicData(out List<float> x, out List<float> y)
        {
            float xValue;
            float yValue;
            x = new List<float>();
            y = new List<float>();
            x.Add(Convert.ToSingle(board[0, 0].Location.X));
            y.Add(Convert.ToSingle(board[0, 0].Location.Y) + buttonSize / 4);
            xValue = Convert.ToSingle(board[0, 0].Location.X);
            yValue = Convert.ToSingle(board[0, 0].Location.Y) + buttonSize / 4;
            int vertices = (boardSize - 2) * 8 + 15;
            for (int i = 0; i < vertices; i++)
            {
                if (i < ((vertices + 1) / 4))
                {
                    if ((i % 2) == 0)
                    {
                        x.Add(xValue);
                        yValue += buttonSize / 2;
                        y.Add(yValue);
                    }
                    else
                    {
                        xValue += buttonSize / 2;
                        x.Add(xValue);
                        yValue += buttonSize / 4;
                        y.Add(yValue);
                    }
                }
                else if (i < ((vertices + 1) / 2 - 1))
                {
                    if ((i % 2) == 0)
                    {
                        xValue += buttonSize / 2 + 1;
                        x.Add(xValue);
                        yValue -= buttonSize / 4;
                        y.Add(yValue);
                    }
                    else
                    {
                        xValue += buttonSize / 2;
                        x.Add(xValue);
                        yValue += buttonSize / 4;
                        y.Add(yValue);
                    }
                }
                else if (i < ((vertices + 1) / 4 * 3 - 1))
                {
                    if ((i % 2) != 0)
                    {
                        x.Add(xValue);
                        yValue -= buttonSize / 2;
                        y.Add(yValue);
                    }
                    else
                    {
                        xValue -= buttonSize / 2;
                        x.Add(xValue);
                        yValue -= buttonSize / 4;
                        y.Add(yValue);
                    }
                }
                else
                {
                    if ((i % 2) != 0)
                    {
                        xValue -= buttonSize / 2 + 1;
                        x.Add(xValue);
                        yValue += buttonSize / 4;
                        y.Add(yValue);
                    }
                    else
                    {
                        xValue -= buttonSize / 2;
                        x.Add(xValue);
                        yValue -= buttonSize / 4;
                        y.Add(yValue);
                    }
                }
            }
        }      

        // function that checks if last player who moved won.
        // function returns bool type variable (true - if last player moved won, false - if last player moved didn't win).
        // functions argument decides which player is being checking. If it's 1 - Red player, if - 2 Blue player.

        public bool IsWinner(int checkedPlayer)
        {
            bool isWin = false;
            DijkstraSolver dijkstra = new DijkstraSolver();
            int start = -1;
            int end = -2;
            int[] distance = new int[boardSize * boardSize + 2];
            distance.Fill(int.MaxValue);
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
            if (dijkstra.DijkstraAlgorithm(Q, S, distance, currentIndx, checkedPlayer) == 0)
                isWin = true;
            return isWin;
        }
    

        // heuristic function that evaluates fields
        // player = 1 - red, 
        // player = 2 - blue
        public int[] Evaluate(int player)
        {
            DijkstraSolver dijkstra = new DijkstraSolver();
            int[] evaluateCells = new int[boardSize * boardSize];
            evaluateCells.Fill(int.MinValue);
            int[] biggestCells = new int[boardSize * boardSize];
            int[] fieldsToCheck = new int[6];
            int[] distanceCheckerFields = new int[2];
            List<int> redPath = new List<int>();
            List<int> bluePath = new List<int>();
            var rand = new Random();
            int excludedFields = 0;
            int takenFields = 0;
            int numberOfMovesToWin;
            int temp;
            int center = boardSize / 2;
            distanceCheckerFields = dijkstra.DistanceChecker(player);
            numberOfMovesToWin = distanceCheckerFields[1];
            redPath = dijkstra.GetShortestPath(1);
            bluePath = dijkstra.GetShortestPath(2);
            for (int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    if (board[i, j].color == 0)
                    {
                        int distance = 0, friends = 0, enemies = 0, emptyCells = 0;
                        // distance from center
                        if (boardSize % 2 == 0)
                        {
                            List<int> pom = new List<int>{
                                Math.Abs(i - center) + Math.Abs(j - center),
                                Math.Abs(i - center) + Math.Abs(j - center - 1),
                                Math.Abs(i - center - 1) + Math.Abs(j - center - 1),
                                Math.Abs(i - center - 1) + Math.Abs(j - center) };
                            distance = pom.Min();
                        }
                        else
                        {
                            distance = Math.Abs(i - center) + Math.Abs(j - center);
                        }

                        // top
                        if (i - 1 >= 0)
                        {
                            if (board[i - 1, j].color == player)
                                friends++;
                            else if (board[i - 1, j].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        // top right
                        if (i - 1 >= 0 && j + 1 < boardSize)
                        {
                            if (board[i - 1, j + 1].color == player)
                                friends++;
                            else if (board[i - 1, j + 1].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        // left
                        if (j - 1 >= 0)
                        {
                            if (board[i, j - 1].color == player)
                                friends++;
                            else if (board[i, j - 1].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        // right
                        if (j + 1 < boardSize)
                        {
                            if (board[i, j + 1].color == player)
                                friends++;
                            else if (board[i, j + 1].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        // down left
                        if (i + 1 < boardSize && j - 1 >= 0)
                        {
                            if (board[i + 1, j - 1].color == player)
                                friends++;
                            else if (board[i + 1, j - 1].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        // down
                        if (i + 1 < boardSize)
                        {
                            if (board[i + 1, j].color == player)
                                friends++;
                            else if (board[i + 1, j].color == 0)
                                emptyCells++;
                            else
                                enemies++;
                        }
                        evaluateCells[i * boardSize + j] = friends * numberOfMovesToWin * -1;
                        if (redPath.Contains(i * boardSize + j) || bluePath.Contains(i * boardSize + j))
                        {
                            if (redPath.Contains(i * boardSize + j) && bluePath.Contains(i * boardSize + j))
                                evaluateCells[i * boardSize + j] = int.MaxValue;
                            else
                                evaluateCells[i * boardSize + j] = evaluateCells[i * boardSize + j] + 100;
                        }
                    }
                    else
                    {
                        excludedFields++;
                        takenFields++;
                    }                        
                }
            }           
            Array.Copy(evaluateCells, 0, biggestCells, 0, evaluateCells.Length);
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);
            fieldsToCheck[0] = Array.IndexOf(evaluateCells, biggestCells[0]);
            evaluateCells[Array.IndexOf(evaluateCells, biggestCells[0])] = int.MinValue;
            biggestCells[0] = int.MinValue;
            excludedFields++;
            if (fieldsToCheck[0] - boardSize >= 0 && fieldsToCheck[0] - boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] - boardSize]) &&
                evaluateCells[fieldsToCheck[0] - boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] - boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] - boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[0] - boardSize + 1 >= 0 && fieldsToCheck[0] - boardSize + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] - boardSize + 1]) && (fieldsToCheck[0] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[0] - boardSize + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] - boardSize + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] - boardSize + 1] = int.MinValue + 1;         
                excludedFields++;
            }
            if (fieldsToCheck[0] - 1 >= 0 && fieldsToCheck[0] - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] - 1]) && fieldsToCheck[0] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[0] - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] - 1] = int.MinValue + 1;           
                excludedFields++;
            }
            if (fieldsToCheck[0] + 1 >= 0 && fieldsToCheck[0] + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] + 1]) && fieldsToCheck[0] + 1 % boardSize != 0 &&
                evaluateCells[fieldsToCheck[0] + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[0] + boardSize - 1 >= 0 && fieldsToCheck[0] + boardSize - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] + boardSize - 1]) && fieldsToCheck[0] % boardSize != 0 &&
                 evaluateCells[fieldsToCheck[0] + boardSize - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] + boardSize - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] + boardSize - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[0] + boardSize >= 0 && fieldsToCheck[0] + boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[0] + boardSize]) &&
                evaluateCells[fieldsToCheck[0] + boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[0] + boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[0] + boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);
            fieldsToCheck[1] = Array.IndexOf(evaluateCells, biggestCells[0]);
            evaluateCells[Array.IndexOf(evaluateCells, biggestCells[0])] = int.MinValue;
            biggestCells[0] = int.MinValue;
            excludedFields++;
            if (fieldsToCheck[1] - boardSize >= 0 && fieldsToCheck[1] - boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] - boardSize]) &&
                evaluateCells[fieldsToCheck[1] - boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] - boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] - boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[1] - boardSize + 1 >= 0 && fieldsToCheck[1] - boardSize + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] - boardSize + 1]) && (fieldsToCheck[1] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[1] - boardSize + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] - boardSize + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] - boardSize + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[1] - 1 >= 0 && fieldsToCheck[1] - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] - 1]) && fieldsToCheck[1] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[1] - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[1] + 1 >= 0 && fieldsToCheck[1] + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] + 1]) && (fieldsToCheck[1] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[1] + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[1] + boardSize - 1 >= 0 && fieldsToCheck[1] + boardSize - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] + boardSize - 1]) && fieldsToCheck[1] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[1] + boardSize - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] + boardSize - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] + boardSize - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[1] + boardSize >= 0 && fieldsToCheck[1] + boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[1] + boardSize]) &&
                evaluateCells[fieldsToCheck[1] + boardSize] != int.MinValue) 
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[1] + boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[1] + boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);
            fieldsToCheck[2] = Array.IndexOf(evaluateCells, biggestCells[0]);
            evaluateCells[Array.IndexOf(evaluateCells, biggestCells[0])] = int.MinValue;
            biggestCells[0] = int.MinValue;
            excludedFields++;
            if (fieldsToCheck[2] - boardSize >= 0 && fieldsToCheck[2] - boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] - boardSize]) &&
                evaluateCells[fieldsToCheck[2] - boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] - boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] - boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[2] - boardSize + 1 >= 0 && fieldsToCheck[2] - boardSize + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] - boardSize + 1]) && (fieldsToCheck[2] + 1) % boardSize != 0 &&
                 evaluateCells[fieldsToCheck[2] - boardSize + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] - boardSize + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] - boardSize + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[2] - 1 >= 0 && fieldsToCheck[2] - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] - 1]) && fieldsToCheck[2] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[2] - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] - 1] = int.MinValue + 1;
                excludedFields++; 
            }
            if (fieldsToCheck[2] + 1 >= 0 && fieldsToCheck[2] + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] + 1]) && (fieldsToCheck[2] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[2] + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[2] + boardSize - 1 >= 0 && fieldsToCheck[2] + boardSize - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] + boardSize - 1]) && fieldsToCheck[2] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[2] + boardSize - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] + boardSize - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] + boardSize - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[2] + boardSize >= 0 && fieldsToCheck[2] + boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[2] + boardSize]) &&
                evaluateCells[fieldsToCheck[2] + boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[2] + boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[2] + boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);
            fieldsToCheck[3] = Array.IndexOf(evaluateCells, biggestCells[0]);
            evaluateCells[Array.IndexOf(evaluateCells, biggestCells[0])] = int.MinValue;
            biggestCells[0] = int.MinValue;
            excludedFields++;
            if (fieldsToCheck[3] - boardSize >= 0 && fieldsToCheck[3] - boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] - boardSize]) &&
                evaluateCells[fieldsToCheck[3] - boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] - boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] - boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[3] - boardSize + 1 >= 0 && fieldsToCheck[3] - boardSize + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] - boardSize + 1]) && (fieldsToCheck[3] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[3] - boardSize + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] - boardSize + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] - boardSize + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[3] - 1 >= 0 && fieldsToCheck[3] - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] - 1]) && fieldsToCheck[3] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[3] - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[3] + 1 >= 0 && fieldsToCheck[3] + 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] + 1]) && (fieldsToCheck[3] + 1) % boardSize != 0 &&
                evaluateCells[fieldsToCheck[3] + 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] + 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] + 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[3] + boardSize - 1 >= 0 && fieldsToCheck[3] + boardSize - 1 < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] + boardSize - 1]) && fieldsToCheck[3] % boardSize != 0 &&
                evaluateCells[fieldsToCheck[3] + boardSize - 1] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] + boardSize - 1])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] + boardSize - 1] = int.MinValue + 1;
                excludedFields++;
            }
            if (fieldsToCheck[3] + boardSize >= 0 && fieldsToCheck[3] + boardSize < boardSize * boardSize && Array.Exists(biggestCells, elem => elem == evaluateCells[fieldsToCheck[3] + boardSize]) &&
                evaluateCells[fieldsToCheck[3] + boardSize] != int.MinValue)
            {
                biggestCells[Array.IndexOf(biggestCells, evaluateCells[fieldsToCheck[3] + boardSize])] = int.MinValue + 1;
                evaluateCells[fieldsToCheck[3] + boardSize] = int.MinValue + 1;
                excludedFields++;
            }
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);           
            if(evaluateCells.Count() > excludedFields)
            {
                temp = rand.Next(boardSize * boardSize - excludedFields);              
            }
            else
            {
                temp = rand.Next(boardSize * boardSize - 4 - takenFields);                                 
            }
            fieldsToCheck[4] = Array.IndexOf(evaluateCells, biggestCells[temp]);
            evaluateCells[fieldsToCheck[4]] = int.MinValue;
            biggestCells[temp] = int.MinValue;
            excludedFields++;
            Array.Sort(biggestCells);
            Array.Reverse(biggestCells);
            if (evaluateCells.Count() > excludedFields)
            {
                temp = rand.Next(boardSize * boardSize - excludedFields);
            }
            else
            {
                temp = rand.Next(boardSize * boardSize - 5 - takenFields);
            }
            fieldsToCheck[5] = Array.IndexOf(evaluateCells, biggestCells[temp]);
            return fieldsToCheck;
        }

        public int MinMax(int player, int turn, int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                DijkstraSolver dijkstra = new DijkstraSolver();
                int[] dC = dijkstra.DistanceChecker(player);
                int heuristicValue = boardSize - dC[1] + dC[0] * boardSize;
                return heuristicValue;
            }
            depth--;
            int freeCells = 0;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {                   
                    if (board[i, j].color == 0)
                        freeCells++;
                }
            }
            if (freeCells > 5)
            {
                int[] bestCells = new int[6];
                bestCells = Evaluate(player);
                for(int i = 0; i < bestCells.Count(); i++)
                {
                    if(turn == 1 && board[bestCells[i] / boardSize, bestCells[i] % boardSize].color == 0)
                    {
                        board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 1;
                        if (player == 1)
                        {
                            if (IsWinner(turn))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MaxValue;
                            }
                            else if (IsWinner(turn + 1))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MinValue + 1;
                            }
                            else
                            {
                                int pom = MinMax(player, turn + 1, depth, alpha, beta);
                                if (pom > alpha)
                                    alpha = pom;
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                if (alpha >= beta)
                                    return alpha;
                            }
                        }
                        else if (player == 2)
                        {
                            if (IsWinner(turn))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MinValue + 1;
                            }
                            else if (IsWinner(turn + 1))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MaxValue;
                            }
                            else
                            {
                                int pom = MinMax(player, turn + 1, depth, alpha, beta);
                                if (pom < beta)
                                    beta = pom;
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                if (alpha >= beta)
                                    return beta;
                            }
                        }
                    }
                    else if(turn == 2 && board[bestCells[i] / boardSize, bestCells[i] % boardSize].color == 0)
                    {
                        board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 2;
                        if (player == 1)
                        {
                            if (IsWinner(turn))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MinValue + 1;
                            }
                            else if (IsWinner(turn - 1))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MaxValue;
                            }
                            else
                            {
                                int pom = MinMax(player, turn - 1, depth, alpha, beta);
                                if (pom < beta)
                                    beta = pom;
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                if (alpha >= beta)
                                    return beta;
                            }
                        }
                        else if (player == 2)
                        {
                            if (IsWinner(turn))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MaxValue;
                            }
                            else if (IsWinner(turn - 1))
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                return int.MinValue + 1;
                            }
                            else
                            {
                                int pom = MinMax(player, turn - 1, depth, alpha, beta);
                                if (pom > alpha)
                                    alpha = pom;
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                                if (alpha >= beta)
                                    return alpha;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (turn == 1)
                        {
                            if (board[i, j].color == 0)
                            {
                                board[i, j].color = 1;
                                if (player == 1)
                                {
                                    if (IsWinner(turn))
                                    {
                                        board[i, j].color = 0;
                                        return int.MaxValue;
                                    }
                                    else if (IsWinner(turn + 1))
                                    {
                                        board[i, j].color = 0;
                                        return int.MinValue + 1;
                                    }
                                    else
                                    {
                                        int pom = MinMax(player, turn + 1, depth, alpha, beta);
                                        if (pom > alpha)
                                            alpha = pom;
                                        board[i, j].color = 0;
                                        if (alpha >= beta)
                                            return alpha;
                                    }
                                }
                                else if (player == 2)
                                {
                                    if (IsWinner(turn))
                                    {
                                        board[i, j].color = 0;
                                        return int.MinValue + 1;
                                    }
                                    else if (IsWinner(turn + 1))
                                    {
                                        board[i, j].color = 0;
                                        return int.MaxValue;
                                    }
                                    else
                                    {
                                        int pom = MinMax(player, turn + 1, depth, alpha, beta);
                                        if (pom < beta)
                                            beta = pom;
                                        board[i, j].color = 0;
                                        if (alpha >= beta)
                                            return beta;
                                    }
                                }
                            }
                        }
                        else if (turn == 2)
                        {
                            if (board[i, j].color == 0)
                            {
                                board[i, j].color = 2;
                                if (player == 1)
                                {
                                    if (IsWinner(turn))
                                    {
                                        board[i, j].color = 0;
                                        return int.MinValue + 1;
                                    }
                                    else if (IsWinner(turn - 1))
                                    {
                                        board[i, j].color = 0;
                                        return int.MaxValue;
                                    }
                                    else
                                    {
                                        int pom = MinMax(player, turn - 1, depth, alpha, beta);
                                        if (pom < beta)
                                            beta = pom;
                                        board[i, j].color = 0;
                                        if (alpha >= beta)
                                            return beta;
                                    }
                                }
                                else if (player == 2)
                                {
                                    if (IsWinner(turn))
                                    {
                                        board[i, j].color = 0;
                                        return int.MaxValue;
                                    }
                                    else if (IsWinner(turn - 1))
                                    {
                                        board[i, j].color = 0;
                                        return int.MinValue + 1;
                                    }
                                    else
                                    {
                                        int pom = MinMax(player, turn - 1, depth, alpha, beta);
                                        if (pom > alpha)
                                            alpha = pom;
                                        board[i, j].color = 0;
                                        if (alpha >= beta)
                                            return alpha;
                                    }
                                }
                            }
                        }
                    }
                }
            }           
            if (turn == player)
                return alpha;
            else
                return beta;
        }       

        public void Panel_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            if (args.Button == MouseButtons.Left)
            {
                bool IsEnd = false;
                HexagonButton btn = (HexagonButton)sender;
                if (btn.color == 0 && (turnCounter % 2) == 0)
                {
                    btn.BackColor = Color.Red;
                    btn.color = 1;
                    if (IsWinner(btn.color))
                    {
                        winText.Text = "Red player won";
                        IsEnd = true;
                    }                      
                    turnCounter++;

                    // computer move (Blue player == 2)
                    if(!IsEnd)              
                    {
                        int maxI1 = 0;
                        int maxI2 = 0;
                        int maxV = int.MinValue;
                        int[,] bestMove = new int[boardSize, boardSize];
                        int freeCells = 0;
                        for (int i = 0; i < boardSize; i++)
                        {
                            for (int j = 0; j < boardSize; j++)
                            {
                                bestMove[i, j] = int.MinValue;
                                if (board[i, j].color == 0)
                                    freeCells++;
                            }
                        }
                        if (freeCells > 5)
                        {
                            int[] bestCells = new int[6];
                            int blue_player = 2;
                            bestCells = Evaluate(blue_player);
                            for(int i = 0; i < 6; i++)
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 2;
                                bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] = MinMax(2, 1, depth, alpha, beta);
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                            }                          
                            for (int i = 0; i < 6; i++)
                            {
                                if (bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] > maxV)
                                {
                                    maxV = bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize];
                                    maxI1 = bestCells[i] / boardSize;
                                    maxI2 = bestCells[i] % boardSize;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < boardSize; i++)
                            {
                                for (int j = 0; j < boardSize; j++)
                                {
                                    if (board[i, j].color == 0)
                                    {
                                        board[i, j].color = 2;
                                        bestMove[i, j] = MinMax(2, 1, depth, alpha, beta);
                                        board[i, j].color = 0;
                                    }
                                }
                            }
                            for (int i = 0; i < boardSize; i++)
                            {
                                for (int j = 0; j < boardSize; j++)
                                {
                                    if (bestMove[i, j] > maxV)
                                    {
                                        maxV = bestMove[i, j];
                                        maxI1 = i;
                                        maxI2 = j;
                                    }
                                }
                            }
                        }                                              
                       
                        //Console.WriteLine(maxI1 + " " + maxI2);
                        board[maxI1, maxI2].color = 2;
                        board[maxI1, maxI2].BackColor = Color.Blue;
                        if (IsWinner(2))
                            winText.Text = "Blue player won";
                        turnCounter++;
                    }                  
                }                   
                else if (btn.color == 0)
                {
                    btn.BackColor = Color.Blue;
                    btn.color = 2;
                    if (IsWinner(btn.color))
                    {
                        winText.Text = "Blue player won";
                        IsEnd = true;
                    }                       
                    turnCounter++;

                    // computer move (Red player == 1)
                    if(!IsEnd)
                    {
                        int maxI1 = 0;
                        int maxI2 = 0;
                        int maxV = int.MinValue;
                        int[,] bestMove = new int[boardSize, boardSize];
                        int freeCells = 0;
                        for (int i = 0; i < boardSize; i++)
                        {
                            for (int j = 0; j < boardSize; j++)
                            {
                                bestMove[i, j] = int.MinValue;
                                if (board[i, j].color == 0)
                                    freeCells++;
                            }
                        }
                        if (freeCells > 5)
                        {
                            int[] bestCells = new int[6];
                            int red_player = 1;
                            bestCells = Evaluate(red_player);
                            for (int i = 0; i < 6; i++)
                            {
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 1;
                                bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] = MinMax(1, 2, depth, alpha, beta); 
                                board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                if (bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] > maxV)
                                {
                                    maxV = bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize];
                                    maxI1 = bestCells[i] / boardSize;
                                    maxI2 = bestCells[i] % boardSize;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < boardSize; i++)
                            {
                                for (int j = 0; j < boardSize; j++)
                                {
                                    if (board[i, j].color == 0)
                                    {
                                        board[i, j].color = 1;
                                        bestMove[i, j] = MinMax(1, 2, depth, alpha, beta); 
                                        board[i, j].color = 0;
                                    }
                                }
                            }
                            for (int i = 0; i < boardSize; i++)
                            {
                                for (int j = 0; j < boardSize; j++)
                                {
                                    if (bestMove[i, j] > maxV)
                                    {
                                        maxV = bestMove[i, j];
                                        maxI1 = i;
                                        maxI2 = j;
                                    }
                                }
                            }
                        }                                             
                        board[maxI1, maxI2].color = 1;
                        board[maxI1, maxI2].BackColor = Color.Red;
                        if (IsWinner(1))
                            winText.Text = "Red player won";
                        turnCounter++;
                    }                   
                }                                  
            }
        }

        public void Reset_Button_Click(object sender, EventArgs e)
        {
            winText.Text = "";
            turnCounter = 0;
            for(int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    board[i, j].color = 0;
                    board[i, j].BackColor = Color.Gray;
                }
            }
        }

        public void Start_Button_Click(object sender, EventArgs e)
        {
            if(turnCounter % 2 == 0)
            {
                // computer move
                int maxI1 = 0;
                int maxI2 = 0;
                int maxV = int.MinValue;
                int[,] bestMove = new int[boardSize, boardSize];
                int freeCells = 0;
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        bestMove[i, j] = int.MinValue;
                        if (board[i, j].color == 0)
                            freeCells++;
                    }
                }
                if(freeCells > 5)
                {
                    int[] bestCells = new int[6];
                    int red_player = 1;
                    bestCells = Evaluate(red_player);
                    for (int i = 0; i < 6; i++)
                    {
                        board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 1;
                        bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] = MinMax(1, 2, depth, alpha, beta);
                        board[bestCells[i] / boardSize, bestCells[i] % boardSize].color = 0;
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        if (bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize] > maxV)
                        {
                            maxV = bestMove[bestCells[i] / boardSize, bestCells[i] % boardSize];
                            maxI1 = bestCells[i] / boardSize;
                            maxI2 = bestCells[i] % boardSize;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < boardSize; i++)
                    {
                        for (int j = 0; j < boardSize; j++)
                        {
                            if (board[i, j].color == 0)
                            {
                                board[i, j].color = 1;
                                bestMove[i, j] = MinMax(1, 2, depth, alpha, beta);
                                board[i, j].color = 0;
                            }
                        }
                    }
                    for (int i = 0; i < boardSize; i++)
                    {
                        for (int j = 0; j < boardSize; j++)
                        {
                            if (bestMove[i, j] > maxV)
                            {
                                maxV = bestMove[i, j];
                                maxI1 = i;
                                maxI2 = j;
                            }
                        }
                    }
                }                                            
                board[maxI1, maxI2].color = 1;
                board[maxI1, maxI2].BackColor = Color.Red;
                if (IsWinner(1))
                    winText.Text = "Red player won";
                turnCounter++;
            }            
        }
    }
}
