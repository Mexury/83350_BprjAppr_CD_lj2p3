using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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

namespace TicTacChess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        Chessboard chessboard;

        public MainWindow()
        {
            InitializeComponent();

            chessboard = new(MainCanvas, CanvasBorder);
            chessboard.UpdateChessboard();

            MainButton.Click += MainButton_Click;
        }

        private void MyControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(this);
            //Debug.WriteLine($"{mouse.X} {mouse.Y}");

            if (chessboard.gameState == GameState.PLACING_PIECES &&
                chessboard.placing != null)
            {
                // has placing

                //chessboard.UpdateChessboard();
                chessboard.DrawPlacingMouse(mouse);
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Point position = Mouse.GetPosition((Button)sender);

            if (chessboard.gameState == GameState.NOT_STARTED)
            {
                chessboard.gameState = GameState.PLACING_PIECES;
                chessboard.gameTurn = GameTurn.WHITE;
            }

            if (chessboard.gameState == GameState.WINNER)
            {
                chessboard.gameState = GameState.NOT_STARTED;
                chessboard.gameTurn = GameTurn.WHITE;
                chessboard.FullReset();
            }

            chessboard.CalculateTileCoordinatesFromClick(position.X, position.Y);
        }
    }
}
