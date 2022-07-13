using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;
using Silk.NET.Input;

namespace Furball_Tetris.Drawables; 

public class GameplayDrawable : CompositeDrawable {
	public GameplayState State;

	private          RectanglePrimitiveDrawable    _outline;
	private readonly RectanglePrimitiveDrawable[,] _drawables;

	private readonly RectanglePrimitiveDrawable    _nextBoxOutline;
	private readonly RectanglePrimitiveDrawable[,] _nextBoxDrawables;
	
	private readonly FixedTimeStepMethod _pieceFallLoop;
	private readonly FixedTimeStepMethod _inputLoop;

	public GameplayDrawable() {
		this.State = new GameplayState(10, 20);

		this._drawables = new RectanglePrimitiveDrawable[this.State.BoardSize.Width, this.State.BoardSize.Height];

		const float boxSize = 25;

		this.Drawables.Add(this._outline = new RectanglePrimitiveDrawable(-Vector2.One, new(this.State.BoardSize.Width * boxSize + 2, this.State.BoardSize.Height * boxSize + 2), 1, false));
		
		for (int x = 0; x < this.State.BoardSize.Width; x++) {
			for (int y = 0; y < this.State.BoardSize.Height; y++) {
				this._drawables[x, y] = new RectanglePrimitiveDrawable(new Vector2(x * boxSize, y * boxSize), new(boxSize), 1, true) {
					ColorOverride = Color.White,
					Visible       = false
				};
				
				this.Drawables.Add(this._drawables[x, y]);
			}
		}

		Vector2 nextBoxPosition = new(this._outline.Size.X + 10, 0);
		this.Drawables.Add(this._nextBoxOutline = new RectanglePrimitiveDrawable(nextBoxPosition - Vector2.One, new Vector2(boxSize) * 4f + new Vector2(2), 1, false));

		this._nextBoxDrawables = new RectanglePrimitiveDrawable[4, 4];
		
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				this.Drawables.Add(this._nextBoxDrawables[x, y] = new RectanglePrimitiveDrawable(new Vector2(x * boxSize, y * boxSize) + nextBoxPosition, new(boxSize), 1, true) {
					ColorOverride = Color.Red,
					Visible = false
				});
			}
		}
		
		this.FullStateRedraw();
		
		FurballGame.TimeStepMethods.Add(this._pieceFallLoop = new FixedTimeStepMethod(1000d / 60d, this.OnPieceFallLoop));
		FurballGame.TimeStepMethods.Add(this._inputLoop     = new FixedTimeStepMethod(1000d / 60d, this.OnInputLoop));
		
		this.State.OnLevelChange   += this.OnLevelChange;
		this.State.OnNextBoxChange += this.OnNextBoxChange;
		this.OnNextBoxChange(null, this.State.NextBox);
	}

	private bool _lastLeftState;
	private bool _lastRightState;
	private bool _lastDownState;

	private bool _lastRotateClockwiseState;
	private bool _lastRotateCounterClockwiseState;
	private void OnInputLoop() {
		List<Key> heldKeys = FurballGame.InputManager.HeldKeys;

		bool leftState  = false;
		bool rightState = false;
		bool downState  = false;

		bool rotateClockwiseState        = false;
		bool rotateCounterClockwiseState = false;
		foreach (Key heldKey in heldKeys) {
			switch (heldKey) {
				case Key.Left:
					leftState = true;
					break;
				case Key.Right:
					rightState = true;
					break;
				case Key.Down:
					downState = true;
					break;
				case Key.Z:
					rotateCounterClockwiseState = true;
					break;
				case Key.X:
					rotateClockwiseState = true;
					break;
			}
		}

		if (leftState && !this._lastLeftState) {
			this.State.MakePieceMove(-1);
			this.FullStateRedraw();
		}
		if (rightState && !this._lastRightState) {
			this.State.MakePieceMove(1);
			this.FullStateRedraw();
		}
		if (downState && !this._lastDownState) {
			this.State.MakePieceFall();
			this.FullStateRedraw();
		}
		if (rotateClockwiseState && !this._lastRotateClockwiseState) {
			this.State.MakePieceRotateClockwise();
			this.FullStateRedraw();
		}
		if (rotateCounterClockwiseState && !this._lastRotateCounterClockwiseState) {
			this.State.MakePieceRotateCounterClockwise();
			this.FullStateRedraw();
		}
		
		this._lastLeftState  = leftState;
		this._lastRightState = rightState;
		this._lastDownState  = downState;

		this._lastRotateClockwiseState        = rotateClockwiseState;
		this._lastRotateCounterClockwiseState = rotateCounterClockwiseState;
	}
	private void OnNextBoxChange(object? sender, TetrisPiece[] piece) {
		foreach (RectanglePrimitiveDrawable drawable in this._nextBoxDrawables) {
			drawable.Visible = false;
		}

		TetrisPiece e = piece[0];
		
		for (int x = 0; x < e.State.GetLength(0); x++)
			for (int y = 0; y < e.State.GetLength(1); y++) {
				this._nextBoxDrawables[x, y].Visible = e.State[x, y];
			}
	}
	private void OnLevelChange(object? sender, int newLevel) {
		newLevel = Math.Clamp(newLevel, 0, 29);

		this.FramesPerFall = FrameDropsPerLevel[newLevel];
	}

	public static readonly int[] FrameDropsPerLevel = {
		0x30, 0x2B, 0x26, 0x21, 0x1C, 0x17, 0x12, 0x0D,
		0x08, 0x06, 0x05, 0x05, 0x05, 0x04, 0x04, 0x04,
		0x03, 0x03, 0x03, 0x02, 0x02, 0x02, 0x02, 0x02,
		0x02, 0x02, 0x02, 0x02, 0x02, 0x01
	};

	private int FramesPerFall = FrameDropsPerLevel[0];

	private          int                 fallDelta;
	private void OnPieceFallLoop() {
		this.fallDelta++;
		
		if(this.fallDelta > this.FramesPerFall) {
			this.State.MakePieceFall();
			this.FullStateRedraw();
			this.fallDelta = 0;
		}

		this.State.RandomNumber = (ushort)this.State.GenerateNestrisPrngNumber(this.State.RandomNumber);
	}

	public override void Dispose() {
		FurballGame.TimeStepMethods.Remove(this._pieceFallLoop);
		
		base.Dispose();
	}

	public void FullStateRedraw() {
		for (int x = 0; x < this.State.BoardSize.Width; x++) {
			for (int y = 0; y < this.State.BoardSize.Height; y++) {
				if (this.State.GetPixel(x, y)) {
					this._drawables[x, y].ColorOverride = this.State.BoardColorState[x, y];
					this._drawables[x, y].Visible = true;
				}
				else {
					this._drawables[x, y].Visible = false;
				}
			}
		}
	}
}
