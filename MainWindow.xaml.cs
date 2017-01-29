using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>

    [Serializable]
    public partial class MainWindow : Window, INotifyPropertyChanged, ISerializable
    {
        public OthelloBoard myBoard;
        public bool isWhite;
        private Stack<MemoryStream> undoList;
        private Rectangle[,] gridRects;

        public event PropertyChangedEventHandler PropertyChanged;

        private String scoreWhite;
        private String scoreBlack;

        //timer
        private DispatcherTimer myTimer;
        

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainWindow()
        {
            // init window (its in another function for deserialize constructor to use)
            doSetup();
            
            // Board object
            myBoard = new OthelloBoard();
            
            // update the board gui
            updateBoard();

            // black always start
            isWhite = false;

            //start timer
            myTimer.Start();

        }

        /// <summary>
        /// Function to initialize the UI components
        /// </summary>
        private void doSetup()
        {
            InitializeComponent();

            gridRects = new Rectangle[8, 8];


            undoList = new Stack<MemoryStream>();

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
        

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// Update time labels
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void TimerEventProcessor(Object myObject,EventArgs myEventArgs)
        {
            //white playtime
            TimeSpan t = myBoard.elapsedWatch1();
            time1.Content = String.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);

            //black playtime
            TimeSpan t2 = myBoard.elapsedWatch2();
            time2.Content = String.Format("{0:00}:{1:00}", t2.Minutes, t2.Seconds);
        }

        /// <summary>
        /// Click on a rectangle
        /// </summary>
        /// <param name="sender">Rectangle who was triggered</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void onClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle curRect = (Rectangle)sender;
            //MessageBox.Show("col(x)= "+ Grid.GetColumn(curRect)+ " row(y)="+ Grid.GetRow(curRect));
            int posX = Grid.GetColumn(curRect);
            int posY = Grid.GetRow(curRect);
            
            // Save board state before trying to play a move
            BinaryFormatter binaryFmt = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            binaryFmt.Serialize(ms, myBoard);

            //play
            if (myBoard.playMove(posX, posY, isWhite))
            {
                //other turn
                isWhite = !isWhite;

                // add board to undo list
                undoList.Push(ms);
            }                

            //Update board colors
            updateBoard();
 
        }

        /// <summary>
        /// Update board GUI
        /// </summary>
        private void updateBoard()
        {
            //Change grid background color based on scores
            int greenValue = 128;
            greenValue -= myBoard.getBlackScore() - myBoard.getWhiteScore();
            Brush background_brush = new SolidColorBrush(Color.FromRgb(0, (byte)greenValue, 0));
            theGrid.Background = background_brush;

            // Board state
            tileState[,] state= myBoard.getState();

            // Update the UI scores
            updateScores();
            
            //To draw image in tiles
            ImageSource srcWhite = new BitmapImage(new Uri("../../Ressources/white.png", UriKind.Relative));
            ImageSource srcBlack = new BitmapImage(new Uri("../../Ressources/black.png", UriKind.Relative));
            ImageBrush brsh;

            for (int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    //apply correct image
                    if(state[i,j]==tileState.BLACK)
                    {
                        brsh = new ImageBrush(srcBlack);
                        brsh.Stretch = Stretch.Fill;
                        gridRects[i, j].Fill = brsh;
                    }else if(state[i, j] == tileState.WHITE)
                    {
                        brsh = new ImageBrush(srcWhite);
                        brsh.Stretch = Stretch.Fill;
                        gridRects[i, j].Fill = brsh;
                    }else 
                    
                    //apply "playable" color or null (to see the grid background color)
                    if(myBoard.isPlayable(i,j,isWhite))
                    {
                        gridRects[i, j].Fill = new SolidColorBrush(Colors.LightGreen);

                    }
                    else if(state[i,j]==tileState.EMPTY)
                    {
                        gridRects[i, j].Fill = null;
                    }
                }
            }            
            
            //get image            
            if (isWhite)
            {
                brsh = new ImageBrush(srcWhite);
            }
            else
            {
                brsh = new ImageBrush(srcBlack);
            }

            //set image
            brsh.Stretch = Stretch.Uniform;
            playerTurn.Fill = brsh;

            // If actual player cant play
            if(myBoard.getCanMove().Count == 0)
            {              
                // Calculate moves for next player
                myBoard.possibleMoves(!isWhite);

                // Both players cant play = end of the game
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
       // Deserialize
        public MainWindow(SerializationInfo info, StreamingContext context)
        {
            doSetup();
            myBoard = (OthelloBoard)info.GetValue("othelloboard", typeof(OthelloBoard));
            isWhite = (bool)info.GetValue("iswhite", typeof(bool));
            undoList = (Stack<MemoryStream>)info.GetValue("undoList", typeof(Stack<MemoryStream>));

        }

        //Serialize
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("othelloboard", myBoard, typeof(OthelloBoard));
            info.AddValue("iswhite", isWhite, typeof(bool));
            info.AddValue("undoList", undoList, typeof(Stack<MemoryStream>));
        }


       /*-------------------------------------------------------
       * Menu Actions
       -------------------------------------------------------- */
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            // reset objects
            myBoard = new OthelloBoard();

            undoList = new Stack<MemoryStream>();

            //black always start
            isWhite = false;

            //update the board gui
            updateBoard();


            //start timer
            myTimer.Start();
        }
        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            // Dialog to open a file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Load game";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                //Deserialize
                BinaryFormatter binaryFmt = new BinaryFormatter();
                FileStream fs = new FileStream
                    (openFileDialog1.FileName, FileMode.OpenOrCreate);
                MainWindow old_window = (MainWindow)binaryFmt.Deserialize(fs);
                fs.Close();

                myBoard = old_window.myBoard;
                isWhite = old_window.isWhite;
                undoList = old_window.undoList;
                old_window.Close();
                updateBoard();
            }
        }
        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            // Dialog to save
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save game";

            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                //Serialize
                BinaryFormatter binaryFmt = new BinaryFormatter();
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                binaryFmt.Serialize(fs, this);

                fs.Close();
            }

        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if(undoList.Count > 0)
            {
                // Deserialize from memorybuffer (contains the old board state)
                BinaryFormatter binaryFmt = new BinaryFormatter();
                MemoryStream ms = undoList.Pop();
                ms.Position = 0;
                OthelloBoard old_board = (OthelloBoard) binaryFmt.Deserialize(ms);
                myBoard = old_board;
                // Change turn
                isWhite = !isWhite;

                //Update UI
                updateBoard();
            }

        }


        /*-------------------------------------------------------
        * Keyboard Shortcuts
        -------------------------------------------------------- */
        private void MyOthello_KeyDown(object sender, KeyEventArgs e)
        {
            // CTRL + Z
            if (e.Key == Key.Z && (Keyboard.Modifiers  == ModifierKeys.Control))
            {
                Undo_Click(null, null);
            }
        }
    }
}
