using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessNEA;

namespace ChessNEA
{
    public class Chessboard
    {
        #region globals
        Moves moves;

        public Piece[,] Board = new Piece[8, 8];
        public Piece originalPiece = new Piece();
        public Piece promotionPiece = new Piece();
        public PlayerColour currentTurn = PlayerColour.White;

        public int selectedRow = 0;
        public int selectedCol = 0;
        public int clickRow = 0;
        public int clickCol = 0;

        public List<(int, int)> legalMoves = new List<(int, int)>();

        //flags for legal moves
        public bool isCaptured = false;
        public bool moveIsOk = false;
        public bool kingInCheck = false;
        public bool doubleCheck = false;
        public bool isCheckmate = false;

        public bool isCastling = false;
        public int castleFromRow = 0;
        public int castleFromCol = 0;
        public int castleToRow = 0;
        public int castleToCol = 0;

        public string winningPlayer = "";
        public bool gameHasEnded = false;


        #endregion


        public Chessboard()
        {
            moves = new Moves(this);

            SetupBoard();
        }

        private void SetupBoard() // load the array Board with the default position for each chess piece
        {
            for (int i = 0; i < 8; i++)
            {
                Board[1, i] = Piece.bPawn;
                Board[6, i] = Piece.wPawn;
            }
            Board[0, 0] = Piece.bRook;
            Board[0, 7] = Piece.bRook;
            Board[7, 0] = Piece.wRook;
            Board[7, 7] = Piece.wRook;

            Board[0, 1] = Piece.bKnight;
            Board[0, 6] = Piece.bKnight;
            Board[7, 1] = Piece.wKnight;
            Board[7, 6] = Piece.wKnight;

            Board[0, 2] = Piece.bBishop;
            Board[0, 5] = Piece.bBishop;
            Board[7, 2] = Piece.wBishop;
            Board[7, 5] = Piece.wBishop; // you can mess around with these numbers to change the initial position for debug

            Board[0, 3] = Piece.bQueen;
            Board[7, 3] = Piece.wQueen;

            Board[0, 4] = Piece.bKing;
            Board[7, 4] = Piece.wKing;
        }
        public void SelectPiece() // If a piece is selected in UI go to this method
        {
            originalPiece = Board[selectedRow, selectedCol];
            Console.WriteLine($"Selected {originalPiece} at {selectedRow} , {selectedCol}");
            GetLegalMoves(); // function to get all legal moves for selected piece and temporarily store moves in a list

        }
        public void GetLegalMoves()
        {
            legalMoves.Clear();
            if (kingInCheck) // if the king is in check only allow pieces to block if possible and for the king to move to a safe sqaure
            {
                if (originalPiece == Piece.wKing || originalPiece == Piece.bKing)
                {
                    moves.GetKingMoves(selectedRow, selectedCol, currentTurn);
                }
                else if ((currentTurn == PlayerColour.White && originalPiece.ToString()[0] == 'w') ||
                 (currentTurn == PlayerColour.Black && originalPiece.ToString()[0] == 'b'))
                {
                    moves.GetBlockingPieces(selectedRow, selectedCol, currentTurn, originalPiece);
                }
            }
            else
            {
                switch (originalPiece)
            {
                case Piece.wPawn:
                case Piece.bPawn:
                    moves.GetPawnMoves(selectedRow, selectedCol, currentTurn);
                    break;
                case Piece.wKnight:
                case Piece.bKnight:
                    moves.GetKnightMoves(selectedRow, selectedCol, currentTurn);
                    break;
                case Piece.wBishop:
                case Piece.bBishop:
                    moves.GetDiagonalMoves(selectedRow, selectedCol, currentTurn);
                    break;
                case Piece.wRook:
                case Piece.bRook:
                    moves.GetHorVMoves(selectedRow, selectedCol, currentTurn);
                    break;
                case Piece.wQueen:
                case Piece.bQueen:
                    moves.GetDiagonalMoves(selectedRow, selectedCol, currentTurn);
                    moves.GetHorVMoves(selectedRow, selectedCol, currentTurn);
                    break;
                case Piece.wKing:
                case Piece.bKing:
                    moves.GetKingMoves(selectedRow, selectedCol, currentTurn);
                    break;
                default:
                    break;
            }
            }
            foreach (var moves in legalMoves)
            {
                Console.WriteLine($"Can move to {moves.Item1} , {moves.Item2}");
            }
        }

        public void PlacePieceLogic()
        {
            CheckLegalMove(); // before anything check that the move is in the list legalMoves
            string checkPiece = originalPiece.ToString();
            bool isWhitePiece = checkPiece.StartsWith("w");
            bool isBlackPiece = checkPiece.StartsWith("b");
            if (currentTurn == PlayerColour.White && isBlackPiece || currentTurn == PlayerColour.Black && isWhitePiece) // attempts to move piece that isn't their own
            {
                moveIsOk = false;
                return;
            }
            else if (moveIsOk)
            {
                if ((currentTurn == PlayerColour.White && clickRow == 0 & originalPiece == Piece.wPawn) ||
                    (currentTurn == PlayerColour.Black && clickRow == 7 & originalPiece == Piece.bPawn))
                {
                    Promote promote = new Promote(this);
                    promote.ShowDialog();
                    if (promotionPiece != Piece.None)
                    {
                        PromotionHandler();
                    }
                }
                else if (Board[clickRow, clickCol] == Piece.None) // empty square
                {
                    Console.WriteLine($"{originalPiece} moved to {clickRow} , {clickCol}");
                    UpdateBoard();
                    Console.WriteLine($"{selectedRow} , {selectedCol} is now {Board[selectedRow, selectedCol]}");
                }
                else if (Board[clickRow, clickCol] != Piece.None) // square is occupied
                {
                    string pieceDestination = Board[clickRow, clickCol].ToString();
                    string yourPiece = originalPiece.ToString();
                    if (yourPiece[0] != pieceDestination[0]) // checks if an enemy piece by using the first character in the piece name (will be either 'b' or 'w')  
                    {
                        isCaptured = true;
                        CapturePiece();
                    }
                }
                currentTurn = (currentTurn == PlayerColour.White) ? PlayerColour.Black : PlayerColour.White; // after a legal move switch player turns
                moves.GetKingPos(currentTurn); // get king position and see if its in check
                moves.IsKingInCheck(currentTurn);
                if (moves.IsCheckmate(currentTurn))
                {
                    Console.WriteLine("checkmate");
                    GameEnd();
                }
                else if (kingInCheck) 
                {
                    Console.WriteLine("check");

                }
            }
            else // if move is not ok then it will just leave the function
            {
                Console.WriteLine("illegal move");
            }
        }


        private void CheckLegalMove()
        {
            var attemptedMove = (clickRow, clickCol);
            moveIsOk = legalMoves.Contains(attemptedMove);
            if (moveIsOk && Board[clickRow, clickCol] != Piece.None)
            {
                string destinationPiece = Board[clickRow, clickCol].ToString();
                string currentPiece = originalPiece.ToString();

                if (destinationPiece[0] != currentPiece[0])
                {
                    isCaptured = true;
                }
                else
                {
                    moveIsOk = false;
                }
            }
        }

        public void PromotionHandler()
        {
            if (currentTurn == PlayerColour.White)
            {
                if (promotionPiece == Piece.wQueen)
                {
                    Board[clickRow, clickCol] = Piece.wQueen;
                }
                else if (promotionPiece == Piece.wRook)
                {
                    Board[clickRow, clickCol] = Piece.wRook;
                }
                else if (promotionPiece == Piece.wBishop)
                {
                    Board[clickRow, clickCol] = Piece.wBishop;
                }
                else if (promotionPiece == Piece.wKnight)
                {
                    Board[clickRow, clickCol] = Piece.wKnight;
                }
            }
            else
            {
                if (promotionPiece == Piece.bQueen)
                {
                    Board[clickRow, clickCol] = Piece.bQueen;
                }
                else if (promotionPiece == Piece.bRook)
                {
                    Board[clickRow, clickCol] = Piece.bRook;
                }
                else if (promotionPiece == Piece.bBishop)
                {
                    Board[clickRow, clickCol] = Piece.bBishop;
                }
                else if (promotionPiece == Piece.bKnight)
                {
                    Board[clickRow, clickCol] = Piece.bKnight;
                }
            }

            moves.GetKingPos(currentTurn);
            moves.IsKingInCheck(currentTurn);

            Board[selectedRow, selectedCol] = Piece.None;

        }
        private void UpdateBoard() // after a legal move update the game logic
        {
            if (originalPiece == Piece.wKing) // handling castles
            {
                moves.wKingMoved = true;
                if (selectedRow == 7 && selectedCol == 4)
                {

                    if (clickRow == 7 && clickCol == 6)
                    {
                        Board[7, 5] = Piece.wRook;
                        Board[7, 7] = Piece.None;
                        isCastling = true;
                        castleFromRow = 7;
                        castleFromCol = 7;
                        castleToRow = 7;
                        castleToCol = 5;
                    }

                    else if (clickRow == 7 && clickCol == 2)
                    {
                        Board[7, 3] = Piece.wRook;
                        Board[7, 0] = Piece.None;
                        isCastling = true;
                        castleFromRow = 7;
                        castleFromCol = 0;
                        castleToRow = 7;
                        castleToCol = 3;
                    }
                }
            }

            else if (originalPiece == Piece.wRook)
            {
                if (selectedRow == 7 && selectedCol == 0)
                    moves.wRookLongMoved = true;
                else if (selectedRow == 7 && selectedCol == 7)
                    moves.wRookShortMoved = true;
            }

            else if (originalPiece == Piece.bKing)
            {
                moves.bKingMoved = true;
                if (selectedRow == 0 && selectedCol == 4)
                {

                    if (clickRow == 0 && clickCol == 6)
                    {

                        Board[0, 5] = Piece.bRook;
                        Board[0, 7] = Piece.None;
                        isCastling = true;
                        castleFromRow = 0;
                        castleFromCol = 7;
                        castleToRow = 0;
                        castleToCol = 5;
                    }

                    else if (clickRow == 0 && clickCol == 2)
                    {

                        Board[0, 3] = Piece.bRook;
                        Board[0, 0] = Piece.None;
                        isCastling = true;
                        castleFromRow = 0;
                        castleFromCol = 0;
                        castleToRow = 0;
                        castleToCol = 3;

                    }
                }
            }

            else if (originalPiece == Piece.bRook)
            {
                if (selectedRow == 0 && selectedCol == 0)
                    moves.bRookLongMoved = true;
                else if (selectedRow == 0 && selectedCol == 7)
                    moves.bRookShortMoved = true;
            }

            Board[clickRow, clickCol] = originalPiece;
            Board[selectedRow, selectedCol] = Piece.None;
        }

        public void CapturePiece()
        {
            Console.WriteLine("piece captured");
            Board[clickRow, clickCol] = originalPiece;
            Board[selectedRow, selectedCol] = Piece.None;
        }

        internal void GameEnd()
        {
            winningPlayer = currentTurn == PlayerColour.White ? "Black" : "White";
            gameHasEnded = true;
        }
    }
}

