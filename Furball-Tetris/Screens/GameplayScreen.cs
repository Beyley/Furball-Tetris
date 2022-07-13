using System.Numerics;
using Furball_Tetris.Drawables;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Furball_Tetris.Screens; 

public class GameplayScreen : Screen {
	private GameplayDrawable _gameplay;
	private TextDrawable     _ui;

	public override void Initialize() {
		base.Initialize();
		
		this.Manager.Add(this._gameplay = new GameplayDrawable {
			Position = new Vector2(25)
		});

		FurballGame.InputManager.OnKeyDown += delegate(object? _, Key key) {
			switch (key) {
				case Key.Space:
					this._gameplay.State.MakeFallingPiecePermanent();

					this._gameplay.State.FallingPiece           = TetrisPiece.L_PIECE[1];
					this._gameplay.State.FallingPieceArray      = TetrisPiece.L_PIECE;
					this._gameplay.State.FallingPieceArrayIndex = 1;

					this._gameplay.State.FallingPieceLocation = Vector2D<int>.Zero;
					this._gameplay.FullStateRedraw();
					break;
				case Key.Right:
					this._gameplay.State.MakePieceMove(1);
					this._gameplay.FullStateRedraw();
					break;
				case Key.Left:
					this._gameplay.State.MakePieceMove(-1);
					this._gameplay.FullStateRedraw();
					break;
				case Key.Down:
					this._gameplay.State.MakePieceFall();
					this._gameplay.FullStateRedraw();
					break;
				case Key.Z:
					this._gameplay.State.MakePieceRotateCounterClockwise();
					this._gameplay.FullStateRedraw();
					break;
				case Key.X:
					this._gameplay.State.MakePieceRotateClockwise();
					this._gameplay.FullStateRedraw();
					break;
			}
		};
		
		this.Manager.Add(this._ui = new TextDrawable(new(0), FurballGame.DEFAULT_FONT, "", 30));
	}

	public override void Update(double gameTime) {
		base.Update(gameTime);

		this._ui.Text = $"Level: {this._gameplay.State.CurrentLevel} LinesCleared: {this._gameplay.State.LinesCleared}";
	}
}
