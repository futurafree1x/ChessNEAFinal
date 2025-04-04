using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessNEA
{
    public partial class Promote : Form
    {
        private Chessboard chessboard;
        public Promote(Chessboard board)
        {
            this.chessboard = board;
            InitializeComponent();
        }

        private void Promote_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ControlBox = false;
            this.BackColor = Color.FromArgb(48, 46, 43);
            if (chessboard.currentTurn == PlayerColour.White)
            {
                queenBtn.Image = Properties.Resources.wQueen;
                rookBtn.Image = Properties.Resources.wRook;
                bishopBtn.Image = Properties.Resources.wBishop;
                btnKnight.Image = Properties.Resources.wKnight;
            }
            else if (chessboard.currentTurn == PlayerColour.Black)
            {
                queenBtn.Image = Properties.Resources.bQueen;
                rookBtn.Image = Properties.Resources.bRook; 
                bishopBtn.Image = Properties.Resources.bBishop;
                btnKnight.Image = Properties.Resources.bKnight;
            }
        }

        private void queenBtn_Click(object sender, EventArgs e)
        {
            if (chessboard.currentTurn == PlayerColour.White)
            {
                chessboard.promotionPiece = Piece.wQueen;
            }
            else
            {
                chessboard.promotionPiece = Piece.bQueen;
            }
            chessboard.PromotionHandler();
            this.Close();
        }

        private void rookBtn_Click(object sender, EventArgs e)
        {
            if (chessboard.currentTurn == PlayerColour.White)
            {
                chessboard.promotionPiece = Piece.wRook;
            }
            else
            {
                chessboard.promotionPiece = Piece.bRook;
            }
            chessboard.PromotionHandler();
            this.Close();

        }

        private void bishopBtn_Click(object sender, EventArgs e)
        {
            if (chessboard.currentTurn == PlayerColour.White)
            {
                chessboard.promotionPiece = Piece.wBishop;
            }
            else
            {
                chessboard.promotionPiece = Piece.bBishop;
            }
            chessboard.PromotionHandler();
            this.Close();

        }

        private void btnKnight_Click(object sender, EventArgs e)
        {
            if (chessboard.currentTurn == PlayerColour.White)
            {
                chessboard.promotionPiece = Piece.wKnight;
            }
            else
            {
                chessboard.promotionPiece = Piece.bKnight;
            }
            chessboard.PromotionHandler();
            this.Close();
        }
    }
}
