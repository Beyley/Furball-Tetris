using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;

namespace Furball_Tetris.Drawables; 

public class GameplayDrawable : CompositeDrawable {
	public GameplayState State;

	private RectanglePrimitiveDrawable    _outline;
	private RectanglePrimitiveDrawable[,] _drawables;
	
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
