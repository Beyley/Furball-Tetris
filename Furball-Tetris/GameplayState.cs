using System.Drawing;
using Silk.NET.Maths;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace Furball_Tetris;

public class GameplayState {
	public readonly bool[,]  LogicalBoardState;
	public readonly Color[,] BoardColorState;
	public readonly Size     BoardSize;

	public Vector2D<int> FallingPieceLocation = new Vector2D<int>(4, 0);
	public TetrisPiece?  FallingPiece         = null;

	public GameplayState(int width, int height) {
		this.LogicalBoardState = new bool[width, height];
		this.BoardColorState   = new Color[width, height];
		this.BoardSize         = new Size(width, height);

		this.FallingPiece = TetrisPiece.S_PIECE[0];

		int a = 0;
		for (int x = 0; x < this.LogicalBoardState.GetLength(0); x++)
			for (int y = 15; y < this.LogicalBoardState.GetLength(1); y++) {
				this.LogicalBoardState[x, y] = (a % 3) == 0;
				this.BoardColorState[x, y]   = (a % 6) == 0 ? Color.Blue : Color.Red;
				
				a++;
			}
	}

	public void MakeFallingPiecePermanent() {
		if (this.FallingPiece == null)
			throw new Exception("No piece is falling!");
		
		for (int x = 0; x < this.FallingPiece.Size.Width; x++) {
			for (int y = 0; y < this.FallingPiece.Size.Height; y++) {
				if (!this.FallingPiece.State[x, y])
					continue;
				
				this.LogicalBoardState[x + this.FallingPieceLocation.X, y + this.FallingPieceLocation.Y] = true;
				this.BoardColorState[x   + this.FallingPieceLocation.X, y + this.FallingPieceLocation.Y] = Color.Blue;
			}
		}

		this.FallingPiece = null;
		this.FallingPieceLocation = Vector2D<int>.Zero;
	}
	
	public bool GetPixel(int x, int y) {
		bool b = this.LogicalBoardState[x, y];

		if (x >= this.FallingPieceLocation.X && y >= this.FallingPieceLocation.Y 
												&& x < this.FallingPieceLocation.X + this.FallingPiece.Size.Width && y < this.FallingPieceLocation.Y + this.FallingPiece.Size.Height) {
			bool fallingPieceState = this.FallingPiece.State[x - this.FallingPieceLocation.X, y - this.FallingPieceLocation.Y];

			if (fallingPieceState) {
				this.BoardColorState[x, y] = Color.Red;
				b                          = true;
			}
		}

		return b;
	}
	public void MakePieceFall() {
		this.FallingPieceLocation.Y++;
		
		if (this.FallingPieceOverlaps()) {
			this.FallingPieceLocation.Y--;
			
			this.MakeFallingPiecePermanent();

			this.FallingPiece         = TetrisPiece.O_PIECE[0];
			this.FallingPieceLocation = new(5, 0);
		}
	}
	public bool FallingPieceOverlaps() {
		if(this.FallingPiece == null)
			throw new Exception("No piece is falling!");

		for (int x = 0; x < this.FallingPiece.Size.Width; x++) {
			for (int y = 0; y < this.FallingPiece.Size.Height; y++) {
				if (!this.FallingPiece.State[x, y])
					continue;

				if (this.LogicalBoardState[x + this.FallingPieceLocation.X, y + this.FallingPieceLocation.Y])
					return true;
			}
		}

		return false;
	}
}
