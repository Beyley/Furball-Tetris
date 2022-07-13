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

		this.Manager.Add(this._ui = new TextDrawable(new(0), FurballGame.DEFAULT_FONT, "", 30));
	}

	public override void Update(double gameTime) {
		base.Update(gameTime);

		this._ui.Text = $"Level: {this._gameplay.State.CurrentLevel} LinesCleared: {this._gameplay.State.LinesCleared}";
	}
}
