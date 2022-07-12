using System.Numerics;
using Furball_Tetris.Drawables;
using Furball.Engine.Engine;

namespace Furball_Tetris.Screens; 

public class GameplayScreen : Screen {
	private GameplayDrawable _gameplay;
	
	public override void Initialize() {
		base.Initialize();
		
		this.Manager.Add(this._gameplay = new GameplayDrawable {
			Position = new Vector2(25)
		});
	}
}
