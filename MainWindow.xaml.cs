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
        public MainWindow()
        {
            InitializeComponent();

            curentColor = new SolidColorBrush(Colors.Red);

            for(int i=0;i<8;i++)
            {
                for(int j=0;j<8;j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.AliceBlue);
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    theGrid.Children.Add(rect);
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    rect.MouseLeftButtonDown += new MouseButtonEventHandler(onClick);
                }
                   
            }
       
        }

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
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public void doColor(object sender)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

        private void onClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle curRect = (Rectangle)sender;

            MessageBox.Show("col(x)= "+ Grid.GetColumn(curRect)+ " row(y)="+ Grid.GetRow(curRect));
        }

        private void rect00_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

        private void rect10_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReColor = new SolidColorBrush(Colors.Red);
        }

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
