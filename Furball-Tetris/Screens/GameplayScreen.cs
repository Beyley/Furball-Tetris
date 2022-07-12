using System.Numerics;
using Furball_Tetris.Drawables;
using Furball.Engine;
using Furball.Engine.Engine;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Furball_Tetris.Screens; 

public class GameplayScreen : Screen {
	private GameplayDrawable _gameplay;
	
	public override void Initialize() {
		base.Initialize();
		
		this.Manager.Add(this._gameplay = new GameplayDrawable {
			Position = new Vector2(25)
		});
		
		FurballGame.InputManager.OnKeyDown += delegate(object? sender, Key key) {
			if (key == Key.Space) {
				this._gameplay.State.MakeFallingPiecePermanent();
				
				this._gameplay.State.FallingPiece = TetrisPiece.L_PIECE[1];
				this._gameplay.State.FallingPieceLocation = Vector2D<int>.Zero;
				this._gameplay.FullStateRedraw();
			}
			if (key == Key.Right) {
				this._gameplay.State.FallingPieceLocation.X++;
				this._gameplay.FullStateRedraw();
			}
			if (key == Key.Left) {
				this._gameplay.State.FallingPieceLocation.X--;
				this._gameplay.FullStateRedraw();
			}
		};
		
		FurballGame.TimeStepMethods.Add(new FixedTimeStepMethod(1000d / 20d, delegate {
			this._gameplay.State.MakePieceFall();
			this._gameplay.FullStateRedraw();
		}));
	}
}
