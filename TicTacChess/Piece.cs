using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TicTacChess
{
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
}