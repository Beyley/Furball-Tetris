using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;

namespace Furball_Tetris.Drawables; 

public class GameplayDrawable : CompositeDrawable {
	public GameplayState State;

	private          RectanglePrimitiveDrawable    _outline;
	private readonly RectanglePrimitiveDrawable[,] _drawables;

	private readonly FixedTimeStepMethod _pieceFallLoop;

	public GameplayDrawable() {
		this.State = new GameplayState(10, 20);

		this._drawables = new RectanglePrimitiveDrawable[this.State.BoardSize.Width, this.State.BoardSize.Height];

		const float boxSize = 25;

		this.Drawables.Add(this._outline = new RectanglePrimitiveDrawable(-Vector2.One, new(this.State.BoardSize.Width * boxSize + 2, this.State.BoardSize.Height * boxSize + 2), 1, false));
		
		for (int x = 0; x < this.State.BoardSize.Width; x++) {
			for (int y = 0; y < this.State.BoardSize.Height; y++) {
				this._drawables[x, y] = new RectanglePrimitiveDrawable(new Vector2(x * boxSize, y * boxSize), new(boxSize), 1, false) {
					ColorOverride = Color.White,
					Visible       = false,
					Filled        = true
				};
				
				this.Drawables.Add(this._drawables[x, y]);
			}
		}
		
		this.FullStateRedraw();
		
		FurballGame.TimeStepMethods.Add(this._pieceFallLoop = new FixedTimeStepMethod(1000d / 60d, this.OnPieceFallLoop));
		
		this.State.OnLevelChange += this.OnLevelChange;
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

	private int fallDelta = 0;
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
