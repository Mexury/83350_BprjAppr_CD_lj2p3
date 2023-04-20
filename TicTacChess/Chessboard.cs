using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TicTacChess;

namespace TicTacChess
{
    /// <summary>
    /// <c>Chessboard</c> -> A class for keeping track of the gamelogic.
    /// </summary>
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
        public string? placing = null;
        public Canvas? placingImage = null;
        public int whiteWins = 0;
        public int blackWins = 0;

        public Rectangle? placingAllowedSquareWhite = null;
        public Rectangle? placingAllowedSquareBlack = null;

        public Chessboard(Canvas canvas, Border canvasBorder)
        {
            this.canvas = canvas;
            this.canvasBorder = canvasBorder;
            canvas.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// <c>FullReset()</c> -> Clear all chesspieces (references) and redraw the chessboard.
        /// </summary>
        public void FullReset()
        {
            pieces.Clear();
            DrawChessboard();
        }

        /// <summary>
        /// <c>ResetChessboard()</c> -> Clear all canvas children and redraw the chessboard.
        /// </summary>
        public void ResetChessboard()
        {
            canvas.Children.Clear();
        }

        /// <summary>
        /// <c>DrawChessboard()</c> -> Draw the chessboard and selection tiles.
        /// </summary>
        public void DrawChessboard()
        {
            ResetChessboard();

            // Draw the 3x3 tile grid
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

            // If you're currently placing pieces, draw the available pieces to select and place.
            if (gameState == GameState.PLACING_PIECES)
            {
                for (int x = 0; x < 5; x++)
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
                    DrawImage(640, 200, $"{color}king");
                    DrawImage(740, 200, $"{color}wizard");
                }
            }
        }

        /// <summary>
        /// <c>UpdateChessboard()</c> -> Redraws chessboard and draws text and chesspieces.
        /// </summary>
        public void UpdateChessboard()
        {
            ResetChessboard();
            DrawChessboard();
            if (selected != null)
            {
                // If the gamestate is playing, you can move, and if the current turn and the selected colour matches up, draw the moves and selection.
                if (!(selected.color == "black" && gameTurn == GameTurn.WHITE ||
                      selected.color != "black" && gameTurn == GameTurn.BLACK) &&
                      canMove &&
                      gameState == GameState.PLAYING)
                {
                    DrawMoves();
                    DrawSelection();
                }
            }

            // Draw every piece on the board
            foreach (Piece piece in pieces)
            {
                piece.Draw();
            }

            // Draw the information (text) next to the board.
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
            
            
            TextBlock whiteWinsKeyText = new TextBlock();
            TextBlock whiteWinsValueText = new TextBlock();
            whiteWinsKeyText.Text = "White Victories";
            whiteWinsValueText.Text = $"{whiteWins}";
            whiteWinsValueText.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(whiteWinsKeyText, 340);
            Canvas.SetTop(whiteWinsKeyText, 90);
            Canvas.SetLeft(whiteWinsValueText, 340);
            Canvas.SetTop(whiteWinsValueText, 105);


            TextBlock blackWinsKeyText = new TextBlock();
            TextBlock blackWinsValueText = new TextBlock();
            blackWinsKeyText.Text = "Black Victories";
            blackWinsValueText.Text = $"{blackWins}";
            blackWinsValueText.FontWeight = FontWeights.Bold;

            Canvas.SetLeft(blackWinsKeyText, 340);
            Canvas.SetTop(blackWinsKeyText, 125);
            Canvas.SetLeft(blackWinsValueText, 340);
            Canvas.SetTop(blackWinsValueText, 140);


            canvas.Children.Add(turnKeyText);
            canvas.Children.Add(turnValueText);
            canvas.Children.Add(stateKeyText);
            canvas.Children.Add(stateValueText);
            canvas.Children.Add(whiteWinsKeyText);
            canvas.Children.Add(whiteWinsValueText);
            canvas.Children.Add(blackWinsKeyText);
            canvas.Children.Add(blackWinsValueText);
        }

        /// <summary>
        /// <c>DrawSelection()</c> -> Draws a square behind the selected chesspiece.
        /// </summary>
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

        /// <summary>
        /// <c>DrawMoves()</c> -> Draws all available moves.
        /// </summary>
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

        /// <summary>
        /// <c>DrawPlacingMouse()</c> -> Draws the currently placing chesspiece on top of the mouse.
        /// </summary>
        /// <param name="mouse">An object of the type Point that indicates the position of the mouse.</param>
        public void DrawPlacingMouse(Point mouse)
        {
            // Clear the placing image so it can place the new one.
            if (placingImage != null) canvas.Children.Remove(placingImage);

            DrawPlacingAllowedSquare();

            string? character = placing;
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

        /// <summary>
        /// <c>DrawPlacingAllowedSquare()</c> -> Draws all the allowed drop places when selecting a piece.
        /// </summary>
        public void DrawPlacingAllowedSquare()
        {
            // Clear the squares so it can draw new ones.
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

            // Draw the correct placement squares based on the current turn.
            if (gameTurn == GameTurn.WHITE)
            {
                placingAllowedSquareWhite = rect;
                canvas.Children.Add(placingAllowedSquareWhite);
            }
            else
            {
                placingAllowedSquareBlack = rect;
                canvas.Children.Add(placingAllowedSquareBlack);
            }
        }

        /// <summary>
        /// <c>DrawPlacingAllowedSquare()</c> -> Draws all the allowed drop places when selecting a piece.
        /// </summary>
        /// <param name="dx">A variable of the type double that indicates the X position of the click.</param>
        /// <param name="dy">A variable of the type double that indicates the Y position of the click.</param>
        public void CalculateTileCoordinatesFromClick(double dx, double dy)
        {
            int sx = Math.Clamp((int)((dx - 340) / 100) * 100, 0, 400);
            int sy = (int)((dy - 200) / 100) * 100;

            int x = (int)(dx / 100) * 100;
            int y = (int)(dy / 100) * 100;

            // If gamestate is placing, check which piece to select for placing based on coordinates.
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
                        case 300:
                            placing = $"{color}king";
                            break;
                        case 400:
                            placing = $"{color}wizard";
                            break;
                    }
                }

                if (placing != null)
                {
                    // Check if the coordinates are within the board boundaries.
                    if ((x >= 0 && x <= 200) && (y >= 0 && y <= 200))
                    {
                        Piece? occupant = pieces.FirstOrDefault(_ => _.x == x && _.y == y);
                        bool canPlace = ((gameTurn == GameTurn.WHITE && y == 200) || (gameTurn == GameTurn.BLACK && y == 0)) && occupant == null;

                        if (canPlace)
                        {
                            Piece piece = new Piece(this, placing.Replace("black_", ""));
                            pieces.Add(piece);

                            piece.SetPos(x, y);
                            piece.color = gameTurn == GameTurn.WHITE ? "white" : "black";

                            placing = null;
                        }

                        // Check how many pieces are placed.
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

            // Calculate move logic for each chesspiece.
            foreach (Piece piece in pieces)
            {
                if (piece.x == (int)dx &&
                    piece.y == (int)dy)
                {
                    // If the gamestate is playing, you can move, and if the current turn and the selecting colour matches up, select the piece.
                    if (!(piece.color == "black" && gameTurn == GameTurn.WHITE ||
                        piece.color != "black" && gameTurn == GameTurn.BLACK) &&
                        canMove &&
                        gameState == GameState.PLAYING)
                    {
                        bool isWizard = false;
                        if (selected != null)
                        {
                            if (selected.movesLikeWizard)
                            {
                                isWizard = true;

                                Position wizardPos = selected.pos;
                                Position piecePos = piece.pos;

                                piece.SetPos(wizardPos.x, wizardPos.y);
                                selected.SetPos(piecePos.x, piecePos.y);
                                gameTurn = gameTurn == GameTurn.WHITE ? GameTurn.BLACK : GameTurn.WHITE;
                            }
                        }

                        if (isWizard)
                        {
                            selected = null;
                        }
                        else
                        {
                            selected = piece;
                            selected.CalculateMoves();
                        }
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

                            // averages
                            int ax = (old.x + cur.x + 100) / 3;
                            int ay = (old.y + cur.y + 100) / 3;

                            bool isValidMove = true;

                            int xDiff = Math.Abs(old.x - cur.x);
                            int yDiff = Math.Abs(old.y - cur.y);

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
                                isValidMove = selected.CanSkipPieces;
                            }

                            // If the current turn and the selected colour don't match up, it's not a valid move.
                            if ((selected.color == "black" && gameTurn == GameTurn.WHITE ||
                                selected.color != "black" && gameTurn == GameTurn.BLACK))
                            {
                                isValidMove = false;
                            }

                            // If the move is valid, you can move and the gamestate is playing, make the move.
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

        /// <summary>
        /// <c>CheckPieces()</c> -> Checks if all the queries pieces are the same colour
        /// </summary>
        /// <param name="p0">An object of the type Piece</param>
        /// <param name="p1">An object of the type Piece</param>
        /// <param name="p1">An object of the type Piece</param>
        /// <param name="discriminator">A variable of the type string. If set, pieces of that colour can't win in that position.</param>
        private bool CheckPieces(Piece? p0, Piece? p1, Piece? p2, string? discriminator = null)
        {
            bool result = (p0 != null && p1 != null && p2 != null) &&
                          (p0?.color == p1?.color && p0?.color == p2?.color);

            if (discriminator != null) result = result && (p0?.color != discriminator);
            if (result)
            {
                gameTurn = gameTurn == GameTurn.WHITE ? GameTurn.BLACK : GameTurn.WHITE;
                gameState = GameState.WINNER;

                if (gameTurn == GameTurn.WHITE) whiteWins++;
                if (gameTurn == GameTurn.BLACK) blackWins++;
            }

            return result;
        }

        /// <summary>
        /// <c>CheckWinner()</c> -> Checks all possible winning combinations
        /// </summary>
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

        /// <summary>
        /// <c>DrawImage()</c> -> Draws an image at the supplied coordinates.
        /// </summary>
        /// <param name="x">A variable of the type int that indicates the desired X position of the image.</param>
        /// <param name="y">A variable of the type int that indicates the desired Y position of the image.</param>
        /// <param name="character">A variable of the type string that indicates which image should be drawn.</param>
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
}