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

    public enum GameState
    {
        NOT_STARTED,
        PLAYING,
        PLACING_PIECES,
        WINNER
    }

    public enum GameTurn
    {
        WHITE,
        BLACK
    }

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
        public Position pos = new Position(0, 0);

        string type = "rook";
        Chessboard chessboard;

        bool movesHorizontally = false;
        bool movesVertically = false;
        bool movesDiagonally = false;

        bool movesLikeKnight = true;
        bool canSkipPieces = true;

        public bool CanSkipPieces
        {
            get { return canSkipPieces; }
        }


        public Piece(Chessboard chessboard, string type = "rook")
        {
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

            foreach (Piece occupant in chessboard.pieces)
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
                    moves.Add(new Position(1, (int)y / 100));
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

                tempMoves.Add(new Position(((int)x / 100) - 2, ((int)y / 100) - 1));
                tempMoves.Add(new Position(((int)x / 100) - 1, ((int)y / 100) - 2));

                tempMoves.Add(new Position(((int)x / 100) + 1, ((int)y / 100) - 2));
                tempMoves.Add(new Position(((int)x / 100) + 2, ((int)y / 100) - 1));

                tempMoves.Add(new Position(((int)x / 100) - 2, ((int)y / 100) + 1));
                tempMoves.Add(new Position(((int)x / 100) - 1, ((int)y / 100) + 2));

                tempMoves.Add(new Position(((int)x / 100) + 1, ((int)y / 100) + 2));
                tempMoves.Add(new Position(((int)x / 100) + 2, ((int)y / 100) + 1));

                tempMoves.RemoveAll(m => m.x < 0 || m.x > 2);
                tempMoves.RemoveAll(m => m.y < 0 || m.y > 2);

                moves.AddRange(tempMoves);
            }

            foreach (Position occupant in occupied)
            {
                moves.RemoveAll(m => m.x * 100 == occupant.x && m.y * 100 == occupant.y);
            }

            chessboard.savedMoves = moves;
            chessboard.occupiedMoves = occupied;
        }
    }

    public class Chessboard
    {
        public Canvas canvas;
        Border canvasBorder;
        public Piece? selected;
        public List<Piece> pieces = new();
        public List<Position> savedMoves = new();
        public List<Position> occupiedMoves = new();
        public bool canMove = true;
        public GameState gameState = GameState.NOT_STARTED;
        public GameTurn gameTurn = GameTurn.WHITE;
        public string placing = null;
        public Canvas placingImage = null;

        public Rectangle placingAllowedSquareWhite = null;
        public Rectangle placingAllowedSquareBlack = null;

        public Chessboard(Canvas canvas, Border canvasBorder)
        {
            this.canvas = canvas;
            this.canvasBorder = canvasBorder;
            canvas.Cursor = Cursors.Hand;
        }

        public void FullReset()
        {
            pieces.Clear();
            DrawChessboard();
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

            if (gameState == GameState.PLACING_PIECES)
            {
                for (int x = 0; x < 3; x++)
                {
                    Color background = Color.FromRgb(232, 237, 237);
                    Color border = Color.FromRgb(200, 204, 204);

                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(border);
                    rect.StrokeThickness = 1;
                    rect.Fill = new SolidColorBrush(background);
                    rect.Width = 100;
                    rect.Height = 100;

                    Canvas.SetLeft(rect, (x * 100) + 340);
                    Canvas.SetTop(rect, 200);
                    canvas.Children.Add(rect);

                    string color = (gameTurn == GameTurn.BLACK) ? "black_" : "";

                    DrawImage(340, 200, $"{color}queen");
                    DrawImage(440, 200, $"{color}rook");
                    DrawImage(540, 200, $"{color}knight");
                }
            }
        }

        public void UpdateChessboard()
        {
            ResetChessboard();
            DrawChessboard();
            if (selected != null)
            {
                if (!(selected.color == "black" && gameTurn == GameTurn.WHITE ||     // piece is black, but turn is for white
                      selected.color != "black" && gameTurn == GameTurn.BLACK) &&
                      canMove &&
                      gameState == GameState.PLAYING)      // piece is white, but turn is for black
                {
                    DrawMoves();
                    DrawSelection();
                }
            }

            foreach (Piece piece in pieces)
            {
                piece.Draw();
            }

            TextBlock turnKeyText = new TextBlock();
            TextBlock turnValueText = new TextBlock();
            string turnColor = gameTurn.ToString().ToUpper();
            turnKeyText.Text = "Current turn";
            turnValueText.Text = $"{turnColor}";
            turnValueText.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(turnKeyText, 340);
            Canvas.SetTop(turnKeyText, 20);
            Canvas.SetLeft(turnValueText, 340);
            Canvas.SetTop(turnValueText, 35);

            TextBlock stateKeyText = new TextBlock();
            TextBlock stateValueText = new TextBlock();
            stateKeyText.Text = "State";
            stateValueText.Text = $"{gameState.ToString().ToUpper()}";
            if (gameState == GameState.NOT_STARTED) stateValueText.Text += $" Click the board to start!";
            if (gameState == GameState.WINNER) stateValueText.Text += $" ({gameTurn.ToString().ToUpper()}) Click the board twice to start!";
            stateValueText.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(stateKeyText, 340);
            Canvas.SetTop(stateKeyText, 55);

            Canvas.SetLeft(stateValueText, 340);
            Canvas.SetTop(stateValueText, 70);

            canvas.Children.Add(turnKeyText);
            canvas.Children.Add(turnValueText);
            canvas.Children.Add(stateKeyText);
            canvas.Children.Add(stateValueText);
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
            foreach (Position move in savedMoves)
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

        public void DrawPlacingMouse(Point mouse)
        {
            if (placingImage != null) canvas.Children.Remove(placingImage);

            DrawPlacingAllowedSquare();

            string character = placing;
            int x = ((int)mouse.X) - 100;
            int y = ((int)mouse.Y) - 100;

            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/" + character + ".png"));
            ImageBrush imageBrush = new ImageBrush(bitmapImage);

            Canvas imageCanvas = new Canvas();
            imageCanvas.Height = 80;
            imageCanvas.Width = 80;
            imageCanvas.Background = imageBrush;
            imageCanvas.IsHitTestVisible = false;

            placingImage = imageCanvas;

            Canvas.SetLeft(placingImage, x + 10);
            Canvas.SetTop(placingImage, y + 10);

            canvas.Children.Add(placingImage);
        }

        public void DrawPlacingAllowedSquare()
        {
            if (placingAllowedSquareWhite != null) canvas.Children.Remove(placingAllowedSquareWhite);
            if (placingAllowedSquareBlack != null) canvas.Children.Remove(placingAllowedSquareBlack);

            int x = 0;
            int y = 200;

            if (gameTurn == GameTurn.BLACK)
            {
                y = 0;
            }

            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Color.FromArgb(127, 100, 165, 82));
            rect.Fill = new SolidColorBrush(Color.FromArgb(127, 100, 165, 82));
            rect.Width = 280;
            rect.Height = 80;
            rect.IsHitTestVisible = false;

            Canvas.SetLeft(rect, x + 10);
            Canvas.SetTop(rect, y + 10);

            if (gameTurn == GameTurn.WHITE)
            {
                placingAllowedSquareWhite = rect;
                canvas.Children.Add(placingAllowedSquareWhite);
            } else
            {
                placingAllowedSquareBlack = rect;
                canvas.Children.Add(placingAllowedSquareBlack);
            }
            
            //canvas.Children.Add(placingAllowedSquareBlack);
        }

        public void CalculateTileCoordinatesFromClick(double dx, double dy)
        {
            int sx = Math.Clamp((int)((dx - 340) / 100) * 100, 0, 200);
            int sy = (int)((dy - 200) / 100) * 100;

            int x = (int)(dx / 100) * 100;
            int y = (int)(dy / 100) * 100;

            Debug.WriteLine($"-----------------");
            Debug.WriteLine($"{x} {y}");

            if (gameState == GameState.PLACING_PIECES)
            {
                if (dx >= 340 && dy >= 200)
                {
                    string color = (gameTurn == GameTurn.BLACK) ? "black_" : "";

                    switch (sx)
                    {
                        case 0:
                            placing = $"{color}queen";
                            break;
                        case 100:
                            placing = $"{color}rook";
                            break;
                        case 200:
                            placing = $"{color}knight";
                            break;
                    }
                }

                if (placing != null)
                {
                    if ((x >= 0 && x <= 200) && (y >= 0 && y <= 200))
                    {
                        Piece? occupant = pieces.FirstOrDefault(_ => _.x == x && _.y == y);
                        bool canPlace = ((gameTurn == GameTurn.WHITE && y == 200) || (gameTurn == GameTurn.BLACK && y == 0)) && occupant == null;
                        
                        if (canPlace) {
                            Piece piece = new Piece(this, placing.Replace("black_", ""));
                            pieces.Add(piece);

                            piece.SetPos(x, y);
                            piece.color = gameTurn == GameTurn.WHITE ? "white" : "black";

                            placing = null;
                        }

                        switch (pieces.Count())
                        {
                            case 3:
                                gameTurn = GameTurn.BLACK;
                                break;
                            case 6:
                                gameTurn = GameTurn.WHITE;
                                gameState = GameState.PLAYING;
                                break;
                        }

                    }
                }
            }

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
                    if (!(piece.color == "black" && gameTurn == GameTurn.WHITE ||     // piece is black, but turn is for white
                        piece.color != "black" && gameTurn == GameTurn.BLACK) &&
                        canMove &&
                        gameState == GameState.PLAYING)      // piece is white, but turn is for black
                    {
                        selected = piece;
                        selected.CalculateMoves();
                    }
                }
            }


            if (selected != null)
            {
                foreach (Position move in savedMoves)
                {
                    if (move.x * 100 == (int)dx &&
                        move.y * 100 == (int)dy)
                    {
                        // selected a valid move.

                        if (selected != null)
                        {
                            Position old = selected.pos;
                            Position cur = new Position((int)dx, (int)dy);
                            Debug.WriteLine($"x{old.x}, y{old.y} -> x{cur.x}, y{cur.y}");

                            // averages
                            int ax = (old.x + cur.x + 100) / 3;
                            int ay = (old.y + cur.y + 100) / 3;

                            bool isValidMove = true;
                            Debug.WriteLine($"x{ax}, y{ay}");

                            int xDiff = Math.Abs(old.x - cur.x);
                            int yDiff = Math.Abs(old.y - cur.y);

                            Debug.WriteLine($"DIFF: x{xDiff}, y{yDiff}");
                            Position? occupant = null;

                            if (xDiff >= 200)
                            {
                                occupant = (yDiff >= 200) ? occupiedMoves.FirstOrDefault(_ => _.x == ax && _.y == ay) : occupiedMoves.FirstOrDefault(_ => _.x == ax && _.y == cur.y);
                            }
                            else if (yDiff >= 200)
                            {
                                occupant = occupiedMoves.FirstOrDefault(_ => _.x == cur.x && _.y == ay);
                            }

                            if (occupant != null)
                            {
                                Debug.WriteLine($"OCCUPANT: x{occupant.x}, {occupant.y}");
                                isValidMove = selected.CanSkipPieces;
                            }

                            if ((selected.color == "black" && gameTurn == GameTurn.WHITE ||     // piece is black, but turn is for white
                                selected.color != "black" && gameTurn == GameTurn.BLACK))      // piece is white, but turn is for black
                            {
                                isValidMove = false;
                            }

                            if (isValidMove && canMove && gameState == GameState.PLAYING)
                            {
                                selected.SetPos((int)dx, (int)dy);
                                selected.CalculateMoves();

                                // Switch turn
                                gameTurn = gameTurn == GameTurn.WHITE ? GameTurn.BLACK : GameTurn.WHITE;

                                CheckWinner();
                            }
                        }
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
            if (result)
            {
                gameTurn = gameTurn == GameTurn.WHITE ? GameTurn.BLACK : GameTurn.WHITE;
                gameState = GameState.WINNER;
            }

            return result;
        }

        public void CheckWinner()
        {
            Piece? p0 = pieces.FirstOrDefault(_ => _.x == 0 && _.y == 0);
            Piece? p1 = pieces.FirstOrDefault(_ => _.x == 100 && _.y == 0);
            Piece? p2 = pieces.FirstOrDefault(_ => _.x == 200 && _.y == 0);
            Piece? p3 = pieces.FirstOrDefault(_ => _.x == 0 && _.y == 100);
            Piece? p4 = pieces.FirstOrDefault(_ => _.x == 100 && _.y == 100);
            Piece? p5 = pieces.FirstOrDefault(_ => _.x == 200 && _.y == 100);
            Piece? p6 = pieces.FirstOrDefault(_ => _.x == 0 && _.y == 200);
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

            chessboard = new(MainCanvas, CanvasBorder);
            chessboard.UpdateChessboard();

            //Piece queen = new Piece(chessboard, "queen");
            //Piece rook = new Piece(chessboard, "rook");
            //Piece knight = new Piece(chessboard, "knight");

            //Piece dark_queen = new Piece(chessboard, "queen");
            //Piece dark_rook = new Piece(chessboard, "rook");
            //Piece dark_knight = new Piece(chessboard, "knight");

            // ------------------------------------------

            //chessboard.pieces.Add(queen);
            //queen.SetPos(0, 200);

            //chessboard.pieces.Add(rook);
            //rook.SetPos(100, 200);

            //chessboard.pieces.Add(knight);
            //knight.SetPos(200, 200);

            // ------------------------------------------

            //chessboard.pieces.Add(dark_queen);
            //dark_queen.color = "black";
            //dark_queen.SetPos(0, 0);

            //chessboard.pieces.Add(dark_rook);
            //dark_rook.color = "black";
            //dark_rook.SetPos(100, 0);

            //chessboard.pieces.Add(dark_knight);
            //dark_knight.color = "black";
            //dark_knight.SetPos(200, 0);

            // ------------------------------------------

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
            // Do something with the current mouse position, such as displaying it in a label or updating a model property.
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
