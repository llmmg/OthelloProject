using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            InitializeComponent();

            gridRects = new Rectangle[8,8];

            curentColor = new SolidColorBrush(Colors.Red);

            //Add rectangles to grid/xaml
            for(int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Green);
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    //preset pieces colors
                    if((i==3 && j==3) ||(i==4 && j==4))
                    {
                        rect.Fill = new SolidColorBrush(Colors.WhiteSmoke);

                    }else if((i ==4  && j == 3) || (i == 3 && j == 4))
                    {
                        rect.Fill = new SolidColorBrush(Colors.Black);
                    }
                    theGrid.Children.Add(rect);

                    

                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    rect.MouseLeftButtonDown += new MouseButtonEventHandler(onClick);
                
                    //add in rectsArray
                    gridRects[i,j] = rect;
                }
                   
            }

            //TEST OthelloBoard
            myBoard = new OthelloBoard();
            //update for playables areas
            updateBoard();

            //boolean Black/White => turn to turn
            isWhite = true;
 
        }

        //Handler for dataBinding
        public event PropertyChangedEventHandler PropertyChanged;

        private Brush curentColor;

        public Brush ReColor
        {
            get { return curentColor; }
            set
            {
                curentColor = value;
                NotifyPropertyChanged("ReColor");
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

        //Event called by all rectangles
        private void onClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle curRect = (Rectangle)sender;
            //MessageBox.Show("col(x)= "+ Grid.GetColumn(curRect)+ " row(y)="+ Grid.GetRow(curRect));
            int posX = Grid.GetColumn(curRect);
            int posY = Grid.GetRow(curRect);

            if(myBoard.playMove(posX, posY, isWhite))
            {
                if(isWhite)
                {
                    curRect.Fill = new SolidColorBrush(Colors.WhiteSmoke);
                }else
                {
                    curRect.Fill = new SolidColorBrush(Colors.Black);
                }
            }
                
            //other turn
            isWhite = !isWhite;

            //Update board colors
            updateBoard();

            //TODO: Databinding to display score and who's turn to play
            
        }
        private void updateBoard()
        {
           tileState[,] state= myBoard.getState();

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
        }

        //DEPRECATED - used for/by databinding
        private void rect00_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

        //DEPRECATED - used for/by databinding
        private void rect10_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

        //DEPRECATED - used for/by databinding
        private void rect20_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReColor = new SolidColorBrush(Colors.Red);
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
