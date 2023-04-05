using System;
using System.Collections.Generic;
using System.Linq;
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

    public class Position
    {
        public int x = 0;
        public int y = 0;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Piece
    {
        public string color = "white";
        public int x = 0;
        public int y = 0;
        public Position pos = new Position(0,0);

        string type = "rook";
        Chessboard chessboard;

        bool movesHorizontally = false;
        bool movesVertically = false;
        bool movesDiagonally = false;

        bool movesLikeKnight = true;
        bool canSkipPieces = true;

        public Piece(Chessboard chessboard, string type = "rook") {
            this.type = type;
            this.chessboard = chessboard;
            switch (type)
            {
                default:
                case "rook":
                    movesHorizontally = true;
                    movesVertically = true;
                    movesDiagonally = false;
                    movesLikeKnight = false;
                    canSkipPieces = false;
                    break;
                case "queen":
                    movesHorizontally = true;
                    movesVertically = true;
                    movesDiagonally = true;
                    movesLikeKnight = false;
                    canSkipPieces = false;
                    break;
                case "knight":
                    movesHorizontally = false;
                    movesVertically = false;
                    movesDiagonally = false;
                    movesLikeKnight = true;
                    canSkipPieces = true;
                    break;
            }
        }

        public void SetPos(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.pos = new Position(this.x, this.y);
            chessboard.UpdateChessboard();
        }

        public void Draw()
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri($"pack://application:,,,/Resources/{(color == "black" ? "black_" : "") + type}.png"));
            ImageBrush imageBrush = new ImageBrush(bitmapImage);

            Canvas imageCanvas = new Canvas();
            imageCanvas.Height = 80;
            imageCanvas.Width = 80;
            imageCanvas.Background = imageBrush;

            Canvas.SetLeft(imageCanvas, x + 10);
            Canvas.SetTop(imageCanvas, y + 10);

            chessboard.canvas.Children.Add(imageCanvas);
        }

        public void CalculateMoves()
        {
            List<Position> occupied = new();
            List<Position> moves = new();

            foreach(Piece occupant in chessboard.pieces)
            {
                occupied.Add(occupant.pos);
            }

            int x = chessboard.selected.pos.x;
            int y = chessboard.selected.pos.y;
            Piece piece = chessboard.selected;

            if (piece.movesHorizontally)
            {
                if (x == 0)
                {
                    moves.Add(new Position(1, (int)y/100));
                    moves.Add(new Position(2, (int)y / 100));
                }
                if (x == 100)
                {
                    moves.Add(new Position(0, (int)y / 100));
                    moves.Add(new Position(2, (int)y / 100));
                }
                if (x == 200)
                {
                    moves.Add(new Position(0, (int)y / 100));
                    moves.Add(new Position(1, (int)y / 100));
                }
            }

            if (piece.movesVertically)
            {
                if (y == 0)
                {
                    moves.Add(new Position((int)x / 100, 1));
                    moves.Add(new Position((int)x / 100, 2));
                }
                if (y == 100)
                {
                    moves.Add(new Position((int)x / 100, 0));
                    moves.Add(new Position((int)x / 100, 2));
                }
                if (y == 200)
                {
                    moves.Add(new Position((int)x / 100, 0));
                    moves.Add(new Position((int)x / 100, 1));
                }
            }

            if (piece.movesDiagonally)
            {
                List<Position> tempMoves = new();

                tempMoves.Add(new Position(((int)x / 100) - 2, ((int)y / 100) - 2));
                tempMoves.Add(new Position(((int)x / 100) - 1, ((int)y / 100) - 1));

                tempMoves.Add(new Position(((int)x / 100) - 1, ((int)y / 100) + 1));
                tempMoves.Add(new Position(((int)x / 100) - 2, ((int)y / 100) + 2));

                tempMoves.Add(new Position(((int)x / 100) + 1, ((int)y / 100) - 1));
                tempMoves.Add(new Position(((int)x / 100) + 2, ((int)y / 100) - 2));

                tempMoves.Add(new Position(((int)x / 100) + 1, ((int)y / 100) + 1));
                tempMoves.Add(new Position(((int)x / 100) + 2, ((int)y / 100) + 2));

                tempMoves.RemoveAll(m => m.x < 0 || m.x > 2);
                tempMoves.RemoveAll(m => m.y < 0 || m.y > 2);

                moves.AddRange(tempMoves);

            }

            if (piece.movesLikeKnight)
            {
                moves.Clear();

                List<Position> tempMoves = new();

                tempMoves.Add(new Position(((int)x / 100)-2, ((int)y / 100)-1));
                tempMoves.Add(new Position(((int)x / 100)-1, ((int)y / 100)-2));

                tempMoves.Add(new Position(((int)x / 100)+1, ((int)y / 100)-2));
                tempMoves.Add(new Position(((int)x / 100)+2, ((int)y / 100)-1));

                tempMoves.Add(new Position(((int)x / 100)-2, ((int)y / 100)+1));
                tempMoves.Add(new Position(((int)x / 100)-1, ((int)y / 100)+2));

                tempMoves.Add(new Position(((int)x / 100)+1, ((int)y / 100)+2));
                tempMoves.Add(new Position(((int)x / 100)+2, ((int)y / 100)+1));

                tempMoves.RemoveAll(m => m.x < 0 || m.x > 2);
                tempMoves.RemoveAll(m => m.y < 0 || m.y > 2);

                moves.AddRange(tempMoves);
            }

            foreach (Position occupant in occupied)
            {
                moves.RemoveAll(m => m.x * 100 == occupant.x && m.y * 100 == occupant.y);
            }

            chessboard.savedMoves = moves;
        }
    }

    public class Chessboard
    {
        public Canvas canvas;
        Border canvasBorder;
        public Piece? selected;
        public List<Piece> pieces = new();
        public List<Position> savedMoves = new();

        public Chessboard(Canvas canvas, Border canvasBorder)
        {
            this.canvas = canvas;
            this.canvasBorder = canvasBorder;
            canvas.Cursor = Cursors.Hand;
        }

        public void ResetChessboard()
        {
            canvas.Children.Clear();
        }

        public void DrawChessboard()
        {
            ResetChessboard();

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Color color = (x + y) % 2 == 0 ? Colors.Black : Colors.White;

                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(color);
                    rect.Fill = new SolidColorBrush(color);
                    rect.Width = 100;
                    rect.Height = 100;

                    Canvas.SetLeft(rect, x * 100);
                    Canvas.SetTop(rect, y * 100);
                    canvas.Children.Add(rect);
                }
            }
        }

        public void UpdateChessboard()
        {
            ResetChessboard();
            DrawChessboard();
            DrawMoves();
            DrawSelection();

            foreach (Piece piece in pieces)
            {
                piece.Draw();
            }
        }

        public void DrawSelection()
        {
            if (selected == null) return;

            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Color.FromArgb(127, 100, 165, 82));
            rect.Fill = new SolidColorBrush(Color.FromArgb(127, 100, 165, 82));
            rect.Width = 80;
            rect.Height = 80;

            Canvas.SetLeft(rect, (selected.x) + 10);
            Canvas.SetTop(rect, (selected.y) + 10);
            canvas.Children.Add(rect);
        }

        public void DrawMoves()
        {
            foreach(Position move in savedMoves)
            {
                Ellipse rect = new Ellipse();
                rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 165, 82));
                rect.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 165, 82));
                rect.Width = 40;
                rect.Height = 40;

                Canvas.SetLeft(rect, (move.x * 100) + 30);
                Canvas.SetTop(rect, (move.y * 100) + 30);
                canvas.Children.Add(rect);
            }
        }

        public void CalculateTileCoordinatesFromClick(double dx, double dy)          
        {
            dx = Math.Clamp((int)(dx / 100) * 100, 0, 200);
            dy = Math.Clamp((int)(dy / 100) * 100, 0, 200);

            ResetChessboard();
            DrawChessboard();

            foreach (Piece piece in pieces)
            {
                if (piece.x == (int)dx &&
                    piece.y == (int)dy)
                {
                    // selected a piece
                    //piece.SetPos((int)dx, (int)dy + 100);
                    selected = piece;
                    selected.CalculateMoves();
                }
            }


            if (selected != null)
            {
                foreach(Position move in savedMoves)
                {
                    if (move.x * 100 == (int)dx &&
                        move.y * 100 == (int)dy)
                    {
                        // selected a valid move.
                        selected.SetPos((int)dx, (int)dy);
                        selected.CalculateMoves();

                        CheckWinner();
                    }
                }
            }

            UpdateChessboard();
        }

        private bool CheckPieces(Piece? p0, Piece? p1, Piece? p2, string? discriminator = null)
        {
            bool result = (p0 != null && p1 != null && p2 != null) &&
                          (p0?.color == p1?.color && p0?.color == p2?.color);

            if (discriminator != null) result = result && (p0?.color != discriminator);
            if (result) MessageBox.Show($"Winner: {p0?.color}");

            return result;
        }

        public void CheckWinner()
        {
            Piece? p0 = pieces.FirstOrDefault(_ => _.x ==   0 && _.y ==   0);
            Piece? p1 = pieces.FirstOrDefault(_ => _.x == 100 && _.y ==   0);
            Piece? p2 = pieces.FirstOrDefault(_ => _.x == 200 && _.y ==   0);
            Piece? p3 = pieces.FirstOrDefault(_ => _.x ==   0 && _.y == 100);
            Piece? p4 = pieces.FirstOrDefault(_ => _.x == 100 && _.y == 100);
            Piece? p5 = pieces.FirstOrDefault(_ => _.x == 200 && _.y == 100);
            Piece? p6 = pieces.FirstOrDefault(_ => _.x ==   0 && _.y == 200);
            Piece? p7 = pieces.FirstOrDefault(_ => _.x == 100 && _.y == 200);
            Piece? p8 = pieces.FirstOrDefault(_ => _.x == 200 && _.y == 200);

            #region HORIZONTAL
            CheckPieces(p0, p1, p2, "black");
            CheckPieces(p3, p4, p5);
            CheckPieces(p6, p7, p8, "white");
            #endregion

            #region VERTICAL
            CheckPieces(p0, p3, p6);
            CheckPieces(p1, p4, p7);
            CheckPieces(p2, p5, p8);
            #endregion

            #region DIAGONAL
            CheckPieces(p0, p4, p8);
            CheckPieces(p2, p4, p6);
            #endregion
        }

        public void DrawImage(int x, int y, string character)
        {
            Color color = (x + y) % 2 == 0 ? Colors.Black : Colors.White;

            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/" + character + ".png"));
            ImageBrush imageBrush = new ImageBrush(bitmapImage);

            Canvas imageCanvas = new Canvas();
            imageCanvas.Height = 80;
            imageCanvas.Width = 80;
            imageCanvas.Background = imageBrush;

            Canvas.SetLeft(imageCanvas, x + 10);
            Canvas.SetTop(imageCanvas, y + 10);

            canvas.Children.Add(imageCanvas);
        }
    }

    public partial class MainWindow : Window
    {
        Chessboard chessboard;

        public MainWindow()
        {
            InitializeComponent();

            chessboard = new (MainCanvas, CanvasBorder);
            chessboard.UpdateChessboard();

            Piece queen = new Piece(chessboard, "queen");
            Piece rook = new Piece(chessboard, "rook");
            Piece knight = new Piece(chessboard, "knight");
            
            Piece dark_queen = new Piece(chessboard, "queen");
            Piece dark_rook = new Piece(chessboard, "rook");
            Piece dark_knight = new Piece(chessboard, "knight");

            chessboard.pieces.Add(queen);
            chessboard.pieces.Add(rook);
            chessboard.pieces.Add(knight);

            chessboard.pieces.Add(dark_queen);
            chessboard.pieces.Add(dark_rook);
            chessboard.pieces.Add(dark_knight);

            dark_queen.color = "black";
            dark_rook.color = "black";
            dark_knight.color = "black";
            
            dark_queen.SetPos(0, 0);
            dark_rook.SetPos(100, 0);
            dark_knight.SetPos(200, 0);

            queen.SetPos(0, 200);
            rook.SetPos(100, 200);
            knight.SetPos(200, 200);

            MainButton.Click += MainButton_Click;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Point position = Mouse.GetPosition((Button)sender);

            chessboard.CalculateTileCoordinatesFromClick(position.X, position.Y);
        }
    }
}
