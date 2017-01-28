using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    
    [Serializable]
    public partial class MainWindow : Window, INotifyPropertyChanged, ISerializable
    {
        //TODO: binder les grids avec des objets/fonctions pour qu'elles changent de couleurs (ou qu'une image s'affiche) lors d'un click
        //et selon les "etats" possibles

        public OthelloBoard myBoard;
        public bool isWhite;
        private Rectangle[,] gridRects;

        //timer
        private DispatcherTimer myTimer;
        


        public MainWindow()
        {
            doSetup();

            //TEST OthelloBoard
            myBoard = new OthelloBoard();
            

            //update the board gui
            updateBoard();

            //boolean Black/White => turn to turn
            isWhite = false;

            //start timer
            myTimer.Start();

        }

        private void doSetup()
        {
            InitializeComponent();

            gridRects = new Rectangle[8, 8];

            curentColor = new SolidColorBrush(Colors.Red);

            //timer
            myTimer = new DispatcherTimer();
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);


            //Add rectangles to grid/xaml
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Green);
                    rect.Stroke = new SolidColorBrush(Colors.Black);

                    //add to grid
                    theGrid.Children.Add(rect);

                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    rect.MouseLeftButtonDown += new MouseButtonEventHandler(onClick);
                    //add in rectsArray
                    gridRects[i, j] = rect;
                }

            }
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
            if(myBoard.playMove(posX, posY, isWhite))
            {
                //other turn
                isWhite = !isWhite;
            }
                

            //Update board colors
            updateBoard();

            


            //TODO: Databinding to display score and who's turn to play
            
        }
        private void updateBoard()
        {
            tileState[,] state= myBoard.getState();
            updateScores();
            //test for when one color can't be played before end of game

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

                    }else if(state[i,j]==tileState.EMPTY)
                    {
                        gridRects[i, j].Fill = new SolidColorBrush(Colors.Green);
                    }
                }
            }

            //update playerTurn display
            ImageSource src;
            
            //get image            
            if (isWhite)
            {
                src = new BitmapImage(new Uri("../../Ressources/white.png", UriKind.Relative));

            }
            else
            {
                src = new BitmapImage(new Uri("../../Ressources/black.png",UriKind.Relative));
            }
            //set image
            ImageBrush brsh = new ImageBrush(src);
            brsh.Stretch = Stretch.Uniform;
            playerTurn.Fill = brsh;


            if(myBoard.getCanMove().Count == 0)
            {
                
                myBoard.possibleMoves(!isWhite);
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
                }
                else
                {
                    MessageBox.Show("Can't play, pass turn");
                    isWhite = !isWhite;
                    updateBoard();
                }
            }
        }

        private void updateScores()
        {
            updateScoreBlack = "Score black " + myBoard.getBlackScore().ToString();
            updateScoreWhite = "Score white " + myBoard.getWhiteScore().ToString();
        }


        /*-------------------------------------------------------
       * ISerializable functions
       -------------------------------------------------------- */
        public MainWindow(SerializationInfo info, StreamingContext context)
        {
            doSetup();
            myBoard = (OthelloBoard)info.GetValue("othelloboard", typeof(OthelloBoard));
            isWhite = (bool)info.GetValue("iswhite", typeof(bool));

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("othelloboard", myBoard, typeof(OthelloBoard));
            info.AddValue("iswhite", isWhite, typeof(bool));
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            myBoard = new OthelloBoard();


            //update the board gui
            updateBoard();

            //boolean Black/White => turn to turn
            isWhite = false;

            //start timer
            myTimer.Start();
        }
        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {

            //Deserialize
            BinaryFormatter binaryFmt = new BinaryFormatter();
            FileStream fs = new FileStream
                ("game.xml", FileMode.OpenOrCreate);
            MainWindow old_window = (MainWindow)binaryFmt.Deserialize(fs);
            fs.Close();

            myBoard = old_window.myBoard;
            isWhite = old_window.isWhite;
            updateBoard();
        }
        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            //Serialize
            BinaryFormatter binaryFmt = new BinaryFormatter();
            FileStream fs = new FileStream("game.xml", FileMode.Create);
            binaryFmt.Serialize(fs, this);
            fs.Close();

        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
