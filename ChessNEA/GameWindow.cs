using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using ChessNEA.Properties;

namespace ChessNEA
{
    public partial class GameWindow : Form
    {
        /*
            1.    Create game window with simple design /
            2.    Definitions for the game board and pieces /
            2.1   Graphical representation of board and pieces /
            3.    Movement for pieces /
            3.1   Basic movement (drag and drop, etc) /
            3.2   Define legal moves /
            3.2.1 special moves 
        - castling / 
        - en passant
        - promotion / 
            3.3   Alternating turns between white and black /
            4.    Determining check and checkmate /

        5. QOL
        5.1. time control /
        5.2. FEN string reader
        5.3. move stack
        5.4. resign button
         */

        private Timer whiteTimer;
        private Timer blackTimer;
        private TimeSpan whiteTime;
        private TimeSpan blackTime;

        private Chessboard chessboard;
        private PictureBox[,] PiecesPb = new PictureBox[8, 8];
        private List<PictureBox> highlightedSquares = new List<PictureBox>();
        public Piece piecePb;
        private SoundPlayer player = new SoundPlayer();

        public GameWindow()
        {
            InitializeComponent();

            chessboard = new Chessboard();

            whiteTime = TimeSpan.FromMinutes(2); 
            blackTime = TimeSpan.FromMinutes(2); 
            whiteTimer = new Timer();
            whiteTimer.Interval = 1000; // 1 second interval
            whiteTimer.Tick += WhiteTimer_Tick;
            blackTimer = new Timer();
            blackTimer.Interval = 1000; // 1 second interval
            blackTimer.Tick += BlackTimer_Tick;
            WhiteTimerText.Text = whiteTime.ToString(@"mm\:ss");
            BlackTimerText.Text = blackTime.ToString(@"mm\:ss");

            
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Size = new Size(1000, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(48, 46, 43);
            DrawBoard();
            StartTimer(PlayerColour.White);
            
        }
        private void StartTimer(PlayerColour playerColour)
        {
            if (playerColour == PlayerColour.White)
            {
                whiteTimer.Start();
                blackTimer.Stop();
            }
            else
            {
                blackTimer.Start();
                whiteTimer.Stop();
            }
        }
        private void WhiteTimer_Tick(object sender, EventArgs e)
        {
            whiteTime = whiteTime.Subtract(TimeSpan.FromSeconds(1));
            WhiteTimerText.Text = whiteTime.ToString(@"mm\:ss");

            if (whiteTime.TotalSeconds <= 0)
            {
                whiteTimer.Stop();
                EndGame("Black");
            }
        }

        private void EndGame(string winningSide)
        {
            whiteTimer.Stop();
            blackTimer.Stop();
            MessageBox.Show($"{winningSide} wins!");
            this.Close();
            
        }

        private void BlackTimer_Tick(object sender, EventArgs e)
        {
            blackTime = blackTime.Subtract(TimeSpan.FromSeconds(1));
            BlackTimerText.Text = blackTime.ToString(@"mm\:ss");

            if (blackTime.TotalSeconds <= 0)
            {
                blackTimer.Stop();
                EndGame("White");
            }
        }

        private void DrawBoard()
        {
            panelBoard.Size = new Size(640, 640); // each cell will be sized 80x80
            for (int row = 0; row < 8; row++) // load each occupied square with a picture box of the relevant piece
            {
                for (int col = 0; col < 8; col++)
                {
                    piecePb = chessboard.Board[row, col];
                    if (piecePb != Piece.None) // create pictureboxes in places where there is a piece in the game logic
                    {
                        Image pieceImage = GetPieceImage(piecePb);
                        if (pieceImage != null)
                        {
                            var individualPiecePb = new PictureBox
                            {
                                Size = new Size(80, 80),
                                Location = new Point(col * 80, row * 80),
                                Image = GetPieceImage(piecePb),
                            };
                            PiecesPb[row, col] = individualPiecePb;
                            panelBoard.Controls.Add(individualPiecePb); // add piece picturebox to the panel
                            Console.WriteLine($"{piecePb} loaded at {row},{col}");
                            individualPiecePb.MouseDown += PieceMouseDown;
                            individualPiecePb.MouseUp += PlacePiece;
                        }
                    }
                }
            }
        }

        private void PieceMouseDown(object sender, MouseEventArgs e)
        {
            PictureBox selectedPiece = (PictureBox)sender;
            if (selectedPiece.Image != null) // only valid if you select a piece
            {
                chessboard.selectedRow = selectedPiece.Location.Y / 80; // as each piece is in intervals of 80 pixels, / by 80 to get the position in the game logic
                chessboard.selectedCol = selectedPiece.Location.X / 80;
                chessboard.SelectPiece();
                Bitmap bmp = (Bitmap)selectedPiece.Image;
                this.Cursor = new Cursor(bmp.GetHicon()); // replace the windows cursor with the piece image for quality of life
                LegalMovesUI(); 
            }
        }

        private void LegalMovesUI() // this fuction takes each legal move for a selected piece and shows the legal moves in UI
        {
            foreach (var moves in chessboard.legalMoves)
            {
                int row = moves.Item1;
                int col = moves.Item2;
                PictureBox validSquares = new PictureBox()
                {
                    Size = new Size(80, 80),
                    Location = new Point(col * 80, row * 80),
                    Image = Resources.moveDot
                };
                highlightedSquares.Add(validSquares);
                panelBoard.Controls.Add(validSquares);
            }
        }
        private void ClearLegalMoves() // helper method to clear old legal squares after mouse button is lifted
        {
            foreach (var square in highlightedSquares)
            {
                panelBoard.Controls.Remove(square);
            }
            highlightedSquares.Clear();
        }

        private void PlacePiece(object sender, MouseEventArgs e)
        {

            ClearLegalMoves();
            Point p = this.PointToClient(Cursor.Position); // when the mouse is released record its position as X Y co-ordinates relative to the game logic
            chessboard.clickRow = p.Y / 80;
            chessboard.clickCol = p.X / 80;
            chessboard.PlacePieceLogic();
            if (!chessboard.moveIsOk) // if the move is illegal then it just leaves the method
            {
                player = new SoundPlayer(Resources.illegal);
                player.Play();
            }
            else if (chessboard.isCastling)
            {
                player = new SoundPlayer(Resources.castle); // castle
                player.Play();
                UpdateUI();
            }
            else if (chessboard.isCaptured && chessboard.moveIsOk) // update UI on capture
            {
                player = new SoundPlayer(Resources.capture);
                player.Play();
                panelBoard.Controls.Remove(PiecesPb[chessboard.clickRow, chessboard.clickCol]);
                UpdateUI();
                chessboard.isCaptured = false;
            }
            else if (chessboard.moveIsOk && !chessboard.isCaptured && chessboard.currentTurn == PlayerColour.White) //if the move is valid update UI
            {
                player = new SoundPlayer(Resources.move_self);
                player.Play();
                UpdateUI();
            }
            else if (chessboard.moveIsOk && !chessboard.isCaptured && chessboard.currentTurn == PlayerColour.Black)
            {
                player = new SoundPlayer(Resources.move_opponent);
                player.Play();
                UpdateUI();
            }
            this.Cursor = default; // after a piece is placed return the cursor to normal

            if (chessboard.gameHasEnded)
            {
                EndGame(chessboard.winningPlayer);

            }

            if (chessboard.moveIsOk)
            {
                StartTimer(chessboard.currentTurn);
            }
        }

        private void UpdateUI() // after a piece is placed in logic the UI updates by removing the picturebox from its old square and placing it on its new square
        {
            lblCurrentTurn.Text = $"Current Turn: {chessboard.currentTurn}";
            piecePb = chessboard.originalPiece;
            if (chessboard.Board[chessboard.selectedRow, chessboard.selectedCol] == Piece.None) // if the piece has moved, its old square is now empty in the game logic so also remove it from the UI
            {
                panelBoard.Controls.Remove(PiecesPb[chessboard.selectedRow, chessboard.selectedCol]);
            }
            var newSquarePb = new PictureBox() // show the pieces new position in UI and add to the panel 
            {
                Size = new Size(80, 80),
                Location = new Point(chessboard.clickCol * 80, chessboard.clickRow * 80),
                Image = GetPieceImage(piecePb),
            };
            PiecesPb[chessboard.clickRow, chessboard.clickCol] = newSquarePb;
            panelBoard.Controls.Add(newSquarePb);
            newSquarePb.MouseDown += PieceMouseDown;
            newSquarePb.MouseUp += PlacePiece;
            if (chessboard.isCastling)
            {
                panelBoard.Controls.Remove(PiecesPb[chessboard.castleFromRow, chessboard.castleFromCol]);
                Piece rookPiece = (chessboard.currentTurn == PlayerColour.Black) ? Piece.wRook : Piece.bRook;
                var rookPb = new PictureBox()
                {
                    Size = new Size(80, 80),
                    Location = new Point(chessboard.castleToCol * 80, chessboard.castleToRow * 80),
                    Image = GetPieceImage(rookPiece),
                };
                PiecesPb[chessboard.castleToRow, chessboard.castleToCol] = rookPb;
                panelBoard.Controls.Add(rookPb);
                rookPb.MouseDown += PieceMouseDown;
                rookPb.MouseUp += PlacePiece;
                chessboard.isCastling = false;
            }
            if ((chessboard.originalPiece == Piece.wPawn && chessboard.clickRow == 0) || (chessboard.originalPiece == Piece.bPawn && chessboard.clickRow == 7)) // promotion
            {
                panelBoard.Controls.Remove(PiecesPb[chessboard.clickRow, chessboard.clickCol]);
                var promotedPiecePb = new PictureBox
                {
                    Size = new Size(80, 80),
                    Location = new Point(chessboard.clickCol * 80, chessboard.clickRow * 80),
                    Image = GetPieceImage(chessboard.Board[chessboard.clickRow, chessboard.clickCol]),
                };

                PiecesPb[chessboard.clickRow, chessboard.clickCol] = promotedPiecePb;
                panelBoard.Controls.Add(promotedPiecePb);
                promotedPiecePb.MouseDown += PieceMouseDown;
                promotedPiecePb.MouseUp += PlacePiece;
            }
        }

        

        private Image GetPieceImage(Piece piecePb) // This method returns the piece image based on its name in the game logic
        {
            switch (piecePb)
            {
                case Piece.wPawn:
                    return Resources.wPawn;
                case Piece.bPawn:
                    return  Resources.bPawn;
                case Piece.wRook:
                    return Resources.wRook;
                case Piece.bRook:
                    return Resources.bRook;
                case Piece.wKnight:
                    return Resources.wKnight;
                case Piece.bKnight:
                    return Resources.bKnight;
                case Piece.wBishop:
                    return Resources.wBishop;
                case Piece.bBishop:
                    return Resources.bBishop;
                case Piece.wQueen:
                    return Resources.wQueen;
                case Piece.bQueen:
                    return Resources.bQueen;
                case Piece.wKing:
                    return Resources.wKing;
                case Piece.bKing:
                    return Resources.bKing;
                default: return null;
            }
        }

        private void button1_Click(object sender, EventArgs e) // resign button
        {
            if (chessboard.currentTurn == PlayerColour.White)
            {
                EndGame("Black");
            }
            else
            {
                EndGame("White");
            }
        }
    }
}
