using System.Drawing;
using Silk.NET.Maths;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace Furball_Tetris;

public class GameplayState {
	public readonly bool[,]  LogicalBoardState;
	public readonly Color[,] BoardColorState;
	public readonly Size     BoardSize;

	public Vector2D<int> FallingPieceLocation = new(4, 0);
	public TetrisPiece?  FallingPiece;
	public TetrisPiece[] FallingPieceArray;
	public int           FallingPieceArrayIndex;

	public TetrisPiece[] NextBox;

	public int LinesCleared {
		get;
		private set;
	}
	public int CurrentLevel {
		get;
		private set;
	}

	public event EventHandler<int>?         OnLevelChange;
	public event EventHandler<TetrisPiece[]>? OnNextBoxChange;

	public GameplayState(int width, int height) {
		this.LogicalBoardState = new bool[width, height];
		this.BoardColorState   = new Color[width, height];
		this.BoardSize         = new Size(width, height);

		this.FallingPiece      = TetrisPiece.S_PIECE[0];
		this.FallingPieceArray = TetrisPiece.S_PIECE;

		this.NextBox = this.PickNewPiece();
		this.OnNextBoxChange?.Invoke(this, this.NextBox);
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

		this.FallingPiece         = null;
		this.FallingPieceLocation = Vector2D<int>.Zero;

		List<int> clearPositions = this.CheckForLineClears();
		if (clearPositions.Count > 0)
			this.DoLineClears(clearPositions);
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

	public ushort RandomNumber = 16748;
	public int GenerateNestrisPrngNumber(ushort value) {
		return ((((value >> 1) & 1) ^ ((value >> 9) & 1)) << 15) | (value >> 1);
	}

	private readonly byte[] _spawnTable = {
		0x02,
		0x07,
		0x08,
		0x0A,
		0x0B,
		0x0E,
		0x12
	};

	private          byte          _spawnId;
	private          byte          _spawnCount;
	public TetrisPiece[] PickNewPiece() {
		unchecked {
			byte newSpawnId;

			this._spawnCount++;

			byte index = (byte)((this.RandomNumber & 0b1111111100000000) >> 8);

			index += this._spawnCount;

			index &= 7;

			if (index == 7)
				goto invalidIndex;

			newSpawnId = this._spawnTable[index];

			if (newSpawnId != this._spawnId)
				goto useNewSpawnId;

		invalidIndex:

			this.RandomNumber = (ushort)this.GenerateNestrisPrngNumber(this.RandomNumber);

			index = (byte)((this.RandomNumber & 0b1111111100000000) >> 8);

			index &= 7;

			index += this._spawnId;

			index %= 7;

			newSpawnId = this._spawnTable[index];

		useNewSpawnId:

			this._spawnId = newSpawnId;

			return this._spawnId switch {
				0x02 => TetrisPiece.T_PIECE,
				0x07 => TetrisPiece.J_PIECE,
				0x08 => TetrisPiece.Z_PIECE,
				0x0A => TetrisPiece.O_PIECE,
				0x0B => TetrisPiece.S_PIECE,
				0x0E => TetrisPiece.L_PIECE,
				0x12 => TetrisPiece.I_PIECE,
				_    => null!
			};
		}
	}

	public void MakePieceFall() {
		this.FallingPieceLocation.Y++;

		if (this.FallingPieceCollides()) {
			this.FallingPieceLocation.Y--;

			this.MakeFallingPiecePermanent();

			this.FallingPieceArray      = this.NextBox;
			this.FallingPiece           = this.FallingPieceArray[0];
			this.FallingPieceArrayIndex = 0;

			this.NextBox = this.PickNewPiece();
			this.OnNextBoxChange?.Invoke(this, this.NextBox);

			this.FallingPieceLocation = new(4, 0);
		}
	}

	public void MakePieceMove(int distance) {
		this.FallingPieceLocation.X += distance;

		if (this.FallingPieceCollides())
			this.FallingPieceLocation.X -= distance;
	}

	public void MakePieceRotateClockwise() {
		int origIndex = this.FallingPieceArrayIndex;

		int newIndex = this.FallingPieceArrayIndex + 1;
		newIndex %= 4;

		this.FallingPiece           = this.FallingPieceArray[newIndex];
		this.FallingPieceArrayIndex = newIndex;

		if (this.FallingPieceCollides()) {
			this.FallingPiece           = this.FallingPieceArray[origIndex];
			this.FallingPieceArrayIndex = origIndex;
		}
	}

	public void MakePieceRotateCounterClockwise() {
		int origIndex = this.FallingPieceArrayIndex;

		int newIndex = this.FallingPieceArrayIndex + 3;
		newIndex %= 4;

		this.FallingPiece           = this.FallingPieceArray[newIndex];
		this.FallingPieceArrayIndex = newIndex;

		if (this.FallingPieceCollides()) {
			this.FallingPiece           = this.FallingPieceArray[origIndex];
			this.FallingPieceArrayIndex = origIndex;
		}
	}

	public List<int> CheckForLineClears() {
		List<int> lineClearPositions = new();

		for (int y = 0; y < this.BoardSize.Height; y++) {
			bool fullLine = true;

			for (int x = 0; x < this.BoardSize.Width; x++)
				if (!this.LogicalBoardState[x, y])
					fullLine = false;

			if (fullLine)
				lineClearPositions.Add(y);
		}

		return lineClearPositions;
	}

	public void DoLineClears(List<int> positions) {
		positions.Sort((y, x) => x.CompareTo(y));

		int clearsDone = 0;
		foreach (int position in positions) {
			for (int y = position; y >= 0; y--) {
				for (int x = 0; x < this.BoardSize.Width; x++) {
					int realY = y + clearsDone;

					// ReSharper disable once SimplifyConditionalTernaryExpression
					bool  newState      = realY == 0 ? false : this.LogicalBoardState[x, realY     - 1];
					Color newColorState = realY == 0 ? Color.Black : this.BoardColorState[x, realY - 1];

					this.LogicalBoardState[x, realY] = newState;
					this.BoardColorState[x, realY]   = newColorState;
				}
			}

			clearsDone++;
		}

		this.LinesCleared += positions.Count;
		this.LevelCheck();
	}

	private void LevelCheck() {
		int shouldLevel = (int)Math.Floor(this.LinesCleared / 10f);

		if (shouldLevel > this.CurrentLevel)
			this.SetLevel(shouldLevel);
	}

	public void SetLevel(int newLevel) {
		this.CurrentLevel = newLevel;

		this.OnLevelChange?.Invoke(this, newLevel);
	}

	public bool FallingPieceCollides() {
		if (this.FallingPiece == null)
			throw new Exception("No piece is falling!");

		for (int x = 0; x < this.FallingPiece.Size.Width; x++) {
			int boardX = this.FallingPieceLocation.X + x;
			for (int y = 0; y < this.FallingPiece.Size.Height; y++) {
				int boardY = this.FallingPieceLocation.Y + y;
				if (!this.FallingPiece.State[x, y])
					continue;

				//Are we at the sides of the screen
				if (boardX >= this.BoardSize.Width || boardX < 0)
					return true;

				//Are we at the bottom of the screen
				if (boardY >= this.BoardSize.Height)
					return true;

				//Are we interacting with the board itself
				if (this.LogicalBoardState[boardX, boardY])
					return true;
			}
		}

		return false;
	}
}
