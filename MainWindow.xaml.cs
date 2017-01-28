using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //TODO: binder les grids avec des objets/fonctions pour qu'elles changent de couleurs (ou qu'une image s'affiche) lors d'un click
        //et selon les "etats" possibles

        private OthelloBoard myBoard;
        private bool isWhite;
        private Rectangle[,] gridRects;

        //timer
        private DispatcherTimer myTimer;
        


        public MainWindow()
        {
            InitializeComponent();

            gridRects = new Rectangle[8,8];

            curentColor = new SolidColorBrush(Colors.Red);

            //timer
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = new TimeSpan(0, 0, 1);
            

            //Add rectangles to grid/xaml
            for (int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Green);
                    rect.Stroke = new SolidColorBrush(Colors.Black);

                    //add to grid
                    theGrid.Children.Add(rect);
                                    
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    rect.MouseLeftButtonDown += new MouseButtonEventHandler(onClick);
                    rect.MouseDown += new MouseButtonEventHandler(updateScores);
                    //add in rectsArray
                    gridRects[i,j] = rect;
                }
                   
            }

            //TEST OthelloBoard
            myBoard = new OthelloBoard();

            //update the board gui
            updateBoard();

            //boolean Black/White => turn to turn
            isWhite = true;

            //start timer
            myTimer.Start();

        }

        //Handler for dataBinding
        public event PropertyChangedEventHandler PropertyChanged;
        private Brush curentColor;

        private String scoreWhite;
        private String scoreBlack;



        public Brush ReColor
        {
            get { return curentColor; }
            set
            {
                curentColor = value;
                NotifyPropertyChanged("ReColor");
            }
        }
        public String updateScoreBlack
        {
            get { return scoreBlack; }
            set
            {
                scoreBlack = value;
                NotifyPropertyChanged("updateScoreBlack");
            }
        }
        public String updateScoreWhite
        {
            get { return scoreWhite; }
            set
            {
                scoreWhite = value;
                NotifyPropertyChanged("updateScoreWhite");
            }
        }
        //databinding
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        // --databinding
        public void doColor(object sender)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

        //update time labels
        private void TimerEventProcessor(Object myObject,EventArgs myEventArgs)
        {
            //white playtime
            TimeSpan t = myBoard.elapsedWatch1();
            time1.Content = String.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);

            //black playtime
            TimeSpan t2 = myBoard.elapsedWatch2();
            time2.Content = String.Format("{0:00}:{1:00}", t2.Minutes, t2.Seconds);
        }

        //Event called by all rectangles
        private void onClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle curRect = (Rectangle)sender;
            //MessageBox.Show("col(x)= "+ Grid.GetColumn(curRect)+ " row(y)="+ Grid.GetRow(curRect));
            int posX = Grid.GetColumn(curRect);
            int posY = Grid.GetRow(curRect);

            //play
            myBoard.playMove(posX, posY, isWhite);
                
            //other turn
            isWhite = !isWhite;

            //Update board colors
            updateBoard();

            


            //TODO: Databinding to display score and who's turn to play
            
        }
        private void updateBoard()
        {
           tileState[,] state= myBoard.getState();

            //test for when one color can't be played before end of game
            int playableCount = 0;

            for(int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    //apply correct color
                    if(state[i,j]==tileState.BLACK)
                    {
                        gridRects[i,j].Fill=new SolidColorBrush(Colors.Black);
                    }else if(state[i, j] == tileState.WHITE)
                    {
                        gridRects[i, j].Fill = new SolidColorBrush(Colors.WhiteSmoke);
                    }else 
                    
                    //apply "playable" color or background color
                    if(myBoard.isPlayable(i,j,isWhite))
                    {
                        gridRects[i, j].Fill = new SolidColorBrush(Colors.LightGreen);
                        
                        //imcrement counter
                        playableCount++;

                    }else if(state[i,j]==tileState.EMPTY)
                    {
                        gridRects[i, j].Fill = new SolidColorBrush(Colors.Green);
                    }
                }
            }


            if(myBoard.getCanMove().Count == 0)
            {
                myBoard.passTurn();
                // Both players cant play
                if(myBoard.getCanMove().Count == 0)
                {
                    string winner = "";
                    if (myBoard.getBlackScore() < myBoard.getWhiteScore())
                    {
                        winner = ("White win!");
                    }
                    else
                    {
                        winner = ("Black win!");
                    }
                    MessageBox.Show("--- END OF GAME --- \n" + scoreBlack + "\n" + scoreWhite + "\n" + winner);

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Can't play, pass turn");
                    updateBoard();
                }
            }
        }

        private void updateScores(object sender, MouseButtonEventArgs e)
        {
            updateScoreBlack = "Score black "+myBoard.getBlackScore().ToString();
            updateScoreWhite = "Score white "+myBoard.getWhiteScore().ToString();
        }
    }
    public class TestOthello : IPlayable
    {
        public int getBlackScore()
        {
            throw new NotImplementedException();
        }

        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int getWhiteScore()
        {
            throw new NotImplementedException();
        }

        public bool isPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }
    }
}
