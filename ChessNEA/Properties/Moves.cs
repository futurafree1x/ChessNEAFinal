using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessNEA
{
    internal class Moves
    {
        private Chessboard chessboard;

        public int kingRow = -1;
        public int kingCol = -1;
        private int[] rowSlideOffsets = { 1, -1, 0, 0 };
        private int[] colSlideOffsets = { 0, 0, 1, -1 };
        private int[] rowDiagOffsets = { 1, 1, -1, -1 };
        private int[] colDiagOffsets = { 1, -1, 1, -1 };
        private int[] rowKnightOffsets = { -1, -1, 1, 1, -2, 2, -2, 2 };
        private int[] colKnightOffsets = { 2, -2, -2, 2, 1, -1, -1, 1 };
        private int[] kingRowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private int[] kingColOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

        //castling flags 
        public bool wKingMoved = false;
        public bool bKingMoved = false;
        public bool wRookLongMoved = false; // this is the rook that the king castles with queenside
        public bool wRookShortMoved = false; // ditto but kingside
        public bool bRookLongMoved = false; // same idea as before...
        public bool bRookShortMoved = false;
       
        private List<(int, int)> checkPath = new List<(int, int)>();
        private List<(int, int)> checkingPieces = new List<(int, int)>();

        public Moves(Chessboard chessboard)
        {
            this.chessboard = chessboard;
        }

        // after a piece is place check all king directions that can result in check /
        //this will be the two pawn capture squares /
        //the 8 knight moves /
        //all horizontal and vertical squares from the king /
        // all diagonal squares from the king /

        /* determining checks
         0.1 king cannot move to any square that is under attack / 
         1. after a move get the kings position /
         2. check LOS on the king for all pieces / 
        2.1 if there is a pin between the king and the enemy piece then the piece which is pinned cannot move / 
         3. if the king is in any piece LOS then initiate check /
         4. check for legal king moves / 
         4.1 if a friendly piece can block the check /
       
        consider double checks as well - in this case only the king is allowed to move /

        fix: 
        pawn out of bounds error / 
        
         
         */

        private bool IsWithinBounds(int row, int col) // useful for preventing index out of bounds errors
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        #region move generation
        public void GetPawnMoves(int selectedRow, int selectedCol, PlayerColour playerColour)
        {
            if (IsPinned(selectedRow, selectedCol, playerColour))
            {
                return; // the piece is pinned and cannot move
            }
            string yourPiece = chessboard.originalPiece.ToString();
            int pawnDirection = playerColour == PlayerColour.White ? -1 : 1;
            int startRow = playerColour == PlayerColour.White ? 6 : 1;

            if (IsWithinBounds(selectedRow + pawnDirection, selectedCol) && chessboard.Board[selectedRow + pawnDirection, selectedCol] == Piece.None) // move forward 1 square
            {
                chessboard.legalMoves.Add((selectedRow + pawnDirection, selectedCol));
            }
            if (IsWithinBounds(selectedRow + 2 * pawnDirection, selectedCol) && chessboard.Board[selectedRow + 2 * pawnDirection, selectedCol] == Piece.None && selectedRow == startRow) // move two square if pawn hasnt moved yet
            {
                chessboard.legalMoves.Add((selectedRow + 2 * pawnDirection, selectedCol));
            }
            //captures - pawns capture diagonally 1 square in front of them

            if (IsWithinBounds(selectedRow + pawnDirection, selectedCol + 1) &&
                chessboard.Board[selectedRow + pawnDirection, selectedCol + 1] != Piece.None &&
                Convert.ToString(chessboard.Board[selectedRow + pawnDirection, selectedCol + 1])[0] != yourPiece[0])
            {
                chessboard.legalMoves.Add((chessboard.selectedRow + pawnDirection, selectedCol + 1));
            }

            if (IsWithinBounds(selectedRow + pawnDirection, selectedCol - 1) &&
               chessboard.Board[selectedRow + pawnDirection, selectedCol - 1] != Piece.None &&
               Convert.ToString(chessboard.Board[selectedRow + pawnDirection, selectedCol - 1])[0] != yourPiece[0])
            {
                chessboard.legalMoves.Add((chessboard.selectedRow + pawnDirection, selectedCol - 1));
            }
        }

        public void GetKnightMoves(int selectedRow, int selectedCol, PlayerColour playerColour)
        {
            if (IsPinned(selectedRow, selectedCol, playerColour))
            {
                return; // the piece is pinned and cannot move
            }
            string knightColour = chessboard.originalPiece.ToString();
            for (int i = 0; i < 8; i++)
            {
                int newRow = selectedRow + rowKnightOffsets[i]; // knight offsets are already defined to reduce repetition of code - similar for sliding pieces rook queen and bishop
                int newCol = selectedCol + colKnightOffsets[i];
                if (IsWithinBounds(newRow, newCol))
                {
                    char targetPiece = chessboard.Board[newRow, newCol].ToString()[0];
                    if ((targetPiece != 'w' && knightColour == "wKnight") ||
                        (targetPiece != 'b' && knightColour == "bKnight"))
                    {
                        chessboard.legalMoves.Add((newRow, newCol));
                    }
                }
            }
        }

        public void GetDiagonalMoves(int selectedRow, int selectedCol, PlayerColour playerColour)
        {
           
            for (int i = 0; i < 4; i++)
            {
                if (IsPinned(selectedRow, selectedCol, playerColour))
                {
                    return; // the piece is pinned and cannot move
                }
                int row = selectedRow + rowDiagOffsets[i];
                int col = selectedCol + colDiagOffsets[i];

                while (IsWithinBounds(row, col))
                {
                    if (chessboard.Board[row, col] == Piece.None)
                    {
                        chessboard.legalMoves.Add((row, col));
                    }
                    else if ((Convert.ToString(chessboard.Board[row, col])[0] != 'w' && chessboard.originalPiece.ToString()[0] == 'w') ||
                              (Convert.ToString(chessboard.Board[row, col])[0] != 'b' && chessboard.originalPiece.ToString()[0] == 'b'))
                    {
                        chessboard.legalMoves.Add((row, col));
                        break;
                    }
                    else
                    {
                        break;
                    }
                    row += rowDiagOffsets[i];
                    col += colDiagOffsets[i];
                }
            }
        }

        public void GetHorVMoves(int selectedRow, int selectedCol, PlayerColour playerColour)
        {
            if (IsPinned(selectedRow, selectedCol, playerColour))
            {
                return; // the piece is pinned and cannot move
            }
            for (int i = 0; i < 4; i++)
            {
                int row = selectedRow + rowSlideOffsets[i];
                int col = selectedCol + colSlideOffsets[i];
                while (IsWithinBounds(row, col))
                {
                    if (chessboard.Board[row, col] == Piece.None)
                    {
                        chessboard.legalMoves.Add((row, col));
                    }
                    else if ((Convert.ToString(chessboard.Board[row, col])[0] != 'w' && chessboard.originalPiece.ToString()[0] == 'w') ||
                              (Convert.ToString(chessboard.Board[row, col])[0] != 'b' && chessboard.originalPiece.ToString()[0] == 'b'))
                    {
                        chessboard.legalMoves.Add((row, col));
                        break;
                    }
                    else
                    {
                        break;
                    }
                    row += rowSlideOffsets[i];
                    col += colSlideOffsets[i];
                }
            }
        }

        public void GetKingMoves(int selectedRow, int selectedCol, PlayerColour playerColour)
        {
           
            playerColour = chessboard.currentTurn;
            string yourPiece = chessboard.originalPiece.ToString();

            for (int i = 0; i < 8; i++) // move generation for king
            {
                int row = selectedRow + kingRowOffsets[i];
                int col = selectedCol + kingColOffsets[i];

                if (IsWithinBounds(row, col))
                {
                    if (MoveResultsInCheck(row, col, playerColour)) // if a potential move results in check, just restart the loop and it wont be added to the legal moves list
                    {
                        continue;
                    }
                    else if (!MoveResultsInCheck(row, col, playerColour) && chessboard.Board[row, col] == Piece.None || Convert.ToString(chessboard.Board[row, col])[0] != yourPiece[0])
                    {
                        chessboard.legalMoves.Add((row, col));
                    }

                }
            }

            if (!wKingMoved && playerColour == PlayerColour.White) // castling moves
            {
                if (!wRookLongMoved && chessboard.Board[7, 1] == Piece.None && chessboard.Board[7, 2] == Piece.None && chessboard.Board[7, 3] == Piece.None && chessboard.Board[7, 0] == Piece.wRook && !MoveResultsInCheck(7, 3, playerColour) && !MoveResultsInCheck(7, 2, playerColour))
                {
                    chessboard.legalMoves.Add((7, 2));
                }
                if (!wRookShortMoved && chessboard.Board[7, 6] == Piece.None && chessboard.Board[7, 5] == Piece.None && chessboard.Board[7, 7] == Piece.wRook && !MoveResultsInCheck(7, 5, playerColour) && !MoveResultsInCheck(7, 6, playerColour))
                {
                    chessboard.legalMoves.Add((7, 6));
                }
            }
            else if (!bKingMoved && playerColour == PlayerColour.Black)
            {

                if (!bRookShortMoved && chessboard.Board[0, 5] == Piece.None && chessboard.Board[0, 6] == Piece.None && chessboard.Board[0, 7] == Piece.bRook && !MoveResultsInCheck(0, 5, playerColour) && !MoveResultsInCheck(0, 6, playerColour))
                {
                    chessboard.legalMoves.Add((0, 6));
                }


                if (!bRookLongMoved && chessboard.Board[0, 1] == Piece.None && chessboard.Board[0, 2] == Piece.None && chessboard.Board[0, 3] == Piece.None && chessboard.Board[0, 0] == Piece.bRook && !MoveResultsInCheck(0, 3, playerColour) && !MoveResultsInCheck(0, 2, playerColour))
                {
                    chessboard.legalMoves.Add((0, 2));
                }
            }
        }
        #endregion


        private bool MoveResultsInCheck(int row, int col, PlayerColour playerColour)
        {
            playerColour = chessboard.currentTurn;

            for (int i = 0; i < 8; i++) // check if knight is attacking a potential king move
            {
                int newRow = row + rowKnightOffsets[i];
                int newCol = col + colKnightOffsets[i];

                if (IsWithinBounds(newRow, newCol) &&
            ((chessboard.Board[newRow, newCol] == Piece.wKnight && playerColour == PlayerColour.Black) ||
             (chessboard.Board[newRow, newCol] == Piece.bKnight && playerColour == PlayerColour.White)))
                {
                    return true;
                }
            }

            for (int i = 0; i < 4; i++) // diagonal pieces that will result in check
            {
                int newRow = row + rowDiagOffsets[i];
                int newCol = col + colDiagOffsets[i];

                while (IsWithinBounds(newRow, newCol)) // if the loop reaches an end square move to the next direction
                {
                    if ((chessboard.Board[newRow, newCol] == Piece.wBishop || chessboard.Board[newRow, newCol] == Piece.wQueen) && playerColour == PlayerColour.Black ||
                        (chessboard.Board[newRow, newCol] == Piece.bBishop || chessboard.Board[newRow, newCol] == Piece.bQueen) && playerColour == PlayerColour.White)
                    {
                        return true;
                    }
                    else if (chessboard.Board[newRow, newCol] != Piece.None) // if a piece is blocking line of sight move to the next direction
                    {
                        break;
                    }

                    newRow += rowDiagOffsets[i]; // if neither condition of the if statement is met check the next square in direction
                    newCol += colDiagOffsets[i];
                }
            }

            for (int i = 0; i < 4; i++) // horizontal vertical pieces that will result in check
            {
                int newRow = row + rowSlideOffsets[i];
                int newCol = col + colSlideOffsets[i];

                while (IsWithinBounds(newRow, newCol)) // if the loop reaches an end square move to the next direction
                {
                    if ((chessboard.Board[newRow, newCol] == Piece.wRook || chessboard.Board[newRow, newCol] == Piece.wQueen) && playerColour == PlayerColour.Black ||
                        (chessboard.Board[newRow, newCol] == Piece.bRook || chessboard.Board[newRow, newCol] == Piece.bQueen) && playerColour == PlayerColour.White)
                    {
                        return true;
                    }
                    else if (chessboard.Board[newRow, newCol] != Piece.None) // if a piece is blocking line of sight move to the next direction
                    {
                        break;
                    }

                    newRow += rowSlideOffsets[i]; // if neither condition of the if statement is met check the next square in direction
                    newCol += colSlideOffsets[i];
                }
            }

            if (playerColour == PlayerColour.White) // pawn attacks
            {
                if ((IsWithinBounds(row - 1, col - 1) && chessboard.Board[row - 1, col - 1] == Piece.bPawn) ||
                    (IsWithinBounds(row - 1, col + 1) && chessboard.Board[row - 1, col + 1] == Piece.bPawn))
                {
                    return true;
                }
            }
            else
            {
                if ((IsWithinBounds(row + 1, col - 1) && chessboard.Board[row + 1, col - 1] == Piece.wPawn) ||
                    (IsWithinBounds(row + 1, col + 1) && chessboard.Board[row + 1, col + 1] == Piece.wPawn))
                {
                    return true;
                }
            }

            for (int i = 0; i < 8; i++) // kings cant be adjacent
            {
                int newRow = row + kingRowOffsets[i];
                int newCol = col + kingColOffsets[i];

                if (IsWithinBounds(newRow, newCol) && chessboard.Board[newRow, newCol] == Piece.wKing && playerColour == PlayerColour.Black)
                {
                    return true;
                }
                else if (IsWithinBounds(newRow, newCol) && chessboard.Board[newRow, newCol] == Piece.bKing && playerColour == PlayerColour.White)
                {
                    return true;
                }
            }

            return false;
        }

        public void GetKingPos(PlayerColour playerColour) // get the kings position to help with check detection
        {
            chessboard.kingInCheck = false;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessboard.Board[row, col] == Piece.wKing && playerColour == PlayerColour.White)
                    {
                        kingRow = row;
                        kingCol = col;
                        Console.WriteLine($"white king is on square {kingRow} , {kingCol}");
                        break;

                    }
                    else if (chessboard.Board[row, col] == Piece.bKing && playerColour == PlayerColour.Black)
                    {
                        kingRow = row;
                        kingCol = col;
                        Console.WriteLine($"black king is on square {kingRow} , {kingCol}");
                        break;

                    }
                }
            }
        }

        public void IsKingInCheck(PlayerColour playerColour)
        {
            checkPath.Clear();
            checkingPieces.Clear();
            chessboard.kingInCheck = false;
            chessboard.doubleCheck = false;

            for (int i = 0; i < 8; i++) // knight checks
            {
                int newRow = kingRow + rowKnightOffsets[i];
                int newCol = kingCol + colKnightOffsets[i];

                if (IsWithinBounds(newRow, newCol) &&
            ((chessboard.Board[newRow, newCol] == Piece.wKnight && playerColour == PlayerColour.Black) ||
             (chessboard.Board[newRow, newCol] == Piece.bKnight && playerColour == PlayerColour.White)))
                {
                    chessboard.kingInCheck = true;
                    checkingPieces.Add((newRow, newCol));
                }
            }

            for (int i = 0; i < 4; i++) // diagonal checks
            {
                int newRow = kingRow + rowDiagOffsets[i];
                int newCol = kingCol + colDiagOffsets[i];
                List<(int, int)> currentPath = new List<(int, int)>();

                while (IsWithinBounds(newRow, newCol)) 
                {
                    currentPath.Add((newRow, newCol));
                    if ((chessboard.Board[newRow, newCol] == Piece.wBishop || chessboard.Board[newRow, newCol] == Piece.wQueen) && playerColour == PlayerColour.Black ||
                        (chessboard.Board[newRow, newCol] == Piece.bBishop || chessboard.Board[newRow, newCol] == Piece.bQueen) && playerColour == PlayerColour.White)
                    {
                        chessboard.kingInCheck = true;
                        checkingPieces.Add((newRow, newCol));
                        checkPath.AddRange(currentPath);
                        break;
                    }
                    else if (chessboard.Board[newRow, newCol] != Piece.None) 
                    {
                        break;
                    }

                    newRow += rowDiagOffsets[i]; 
                    newCol += colDiagOffsets[i];
                }
            }

            for (int i = 0; i < 4; i++) // horizontal vertical checks
            {
                int newRow = kingRow + rowSlideOffsets[i];
                int newCol = kingCol + colSlideOffsets[i];
                List<(int, int)> currentPath = new List<(int, int)>();

                while (IsWithinBounds(newRow, newCol))
                {
                    currentPath.Add((newRow, newCol));
                    if ((chessboard.Board[newRow, newCol] == Piece.wRook || chessboard.Board[newRow, newCol] == Piece.wQueen) && playerColour == PlayerColour.Black ||
                        (chessboard.Board[newRow, newCol] == Piece.bRook || chessboard.Board[newRow, newCol] == Piece.bQueen) && playerColour == PlayerColour.White)
                    {
                        chessboard.kingInCheck = true;
                        checkingPieces.Add((newRow, newCol));
                        checkPath.AddRange(currentPath);
                        break;
                    }
                    else if (chessboard.Board[newRow, newCol] != Piece.None)
                    {
                        break;
                    }
                    newRow += rowSlideOffsets[i];
                    newCol += colSlideOffsets[i];
                }
            }

            if (playerColour == PlayerColour.White) // pawn checks
            {
                if (IsWithinBounds(kingRow - 1, kingCol - 1) && chessboard.Board[kingRow - 1, kingCol - 1] == Piece.bPawn)
                {
                    chessboard.kingInCheck = true;
                    checkingPieces.Add((kingRow - 1, kingCol - 1));
                }
                if (IsWithinBounds(kingRow - 1, kingCol + 1) && chessboard.Board[kingRow - 1, kingCol + 1] == Piece.bPawn)
                {
                    chessboard.kingInCheck = true;
                    checkingPieces.Add((kingRow - 1, kingCol + 1));
                }
            }
            else
            {
                if (IsWithinBounds(kingRow + 1, kingCol - 1) && chessboard.Board[kingRow + 1, kingCol - 1] == Piece.wPawn)
                {
                    chessboard.kingInCheck = true;
                    checkingPieces.Add((kingRow + 1, kingCol - 1));
                }
                if (IsWithinBounds(kingRow + 1, kingCol + 1) && chessboard.Board[kingRow + 1, kingCol + 1] == Piece.wPawn)
                {
                    chessboard.kingInCheck = true;
                    checkingPieces.Add((kingRow + 1, kingCol + 1));
                }
            }
            if (checkingPieces.Count > 1) // needed as in the event of a double check only the king can move
            {
                chessboard.doubleCheck = true;
            }

        }
        public void GetBlockingPieces(int row, int col, PlayerColour playerColour, Piece originalPiece)
        {
            if (chessboard.doubleCheck)
            {
                return; // Only king can move in double check
            }
            List<(int, int)> copyLegalMoves = new List<(int, int)>();

            switch (originalPiece) // get all legal moves
            {
                case Piece.wPawn:
                case Piece.bPawn:
                    GetPawnMoves(row, col, playerColour);
                    break;
                case Piece.wKnight:
                case Piece.bKnight:
                    GetKnightMoves(row, col, playerColour);
                    break;
                case Piece.wBishop:
                case Piece.bBishop:
                    GetDiagonalMoves(row, col, playerColour);
                    break;
                case Piece.wRook:
                case Piece.bRook:
                    GetHorVMoves(row, col, playerColour);
                    break;
                case Piece.wQueen:
                case Piece.bQueen:
                    GetDiagonalMoves(row, col, playerColour);
                    GetHorVMoves(row, col, playerColour);
                    break;
                case Piece.wKing:
                case Piece.bKing:
                    GetKingMoves(row, col, playerColour);
                    break;
                default:
                    break;
            }

            copyLegalMoves.AddRange(chessboard.legalMoves);
            chessboard.legalMoves.Clear(); // legal moves are copied into a new list

            foreach (var move in copyLegalMoves) // legal moves are filtered so that only the moves that block the check are added to the legal moves list
            {
                if (CanBlockCheck(row, col, move.Item1, move.Item2))
                {
                    chessboard.legalMoves.Add(move);
                }
            }
        }
        public bool CanBlockCheck(int fromRow, int fromCol, int row, int col)
        {
            if (chessboard.doubleCheck) // if there's a double check return early as no other friendly piece should be able to move
            {
                return false;
            }
            
            foreach (var checkingPiece in checkingPieces) // if the move captures the checking piece
            {
                if (row == checkingPiece.Item1 && col == checkingPiece.Item2)
                {
                    return true;
                }
            }
          
            foreach (var square in checkPath) // if the move blocks the check path
            {
                if (row == square.Item1 && col == square.Item2)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCheckmate(PlayerColour playerColour)
        {
            if (!chessboard.kingInCheck) // no checkmate if a move the opponent has played isnt a checking move
            {
                return false;
            }
            int saveKingRow = kingRow;
            int saveKingCol = kingCol;
            int saveOrigRow = chessboard.selectedRow;
            int saveOrigCol = chessboard.selectedCol;
            Piece savePiece = chessboard.originalPiece;
            chessboard.selectedRow = kingRow;
            chessboard.selectedCol = kingCol;

            chessboard.originalPiece = Piece.wKing;
            List<(int, int)> kingMoves = new List<(int, int)>();
            chessboard.legalMoves.Clear();
            GetKingMoves(kingRow, kingCol, playerColour);
            kingMoves = new List<(int, int)>(chessboard.legalMoves);

            if (kingMoves.Count > 0) // not checkmate if the king has a legal move
            {
                chessboard.selectedRow = saveOrigRow;
                chessboard.selectedCol = saveOrigCol;
                chessboard.originalPiece = savePiece;
                return false; // restore boards original state now
            }

            chessboard.selectedRow = saveOrigRow;
            chessboard.selectedCol = saveOrigCol;
            chessboard.originalPiece = savePiece;

            return true;
        }

        private bool IsPinned(int pieceRow, int pieceCol, PlayerColour playerColour)
        {
            int kingRow = this.kingRow;
            int kingCol = this.kingCol;

            if (pieceRow == kingRow || pieceCol == kingCol || Math.Abs(pieceRow - kingRow) == Math.Abs(pieceCol - kingCol)) // check if a piece is in the same x y or diagonal
            {
                Piece originalPiece = chessboard.Board[pieceRow, pieceCol]; // temporarily store the original piece
                chessboard.Board[pieceRow, pieceCol] = Piece.None;

                bool kingInCheck = MoveResultsInCheck(kingRow, kingCol, playerColour); // see if the king is in check after the pinned piece moves

                chessboard.Board[pieceRow, pieceCol] = originalPiece; // restore board state

                return kingInCheck;
            }

            return false;
        }

    }
}


