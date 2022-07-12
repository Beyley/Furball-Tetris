using System.Drawing;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace Furball_Tetris;

public class GameplayState {
	public readonly bool[,]  LogicalBoardState;
	public readonly Color[,] BoardColorState;
	public readonly Size     BoardSize;

	public GameplayState(int width, int height) {
		this.LogicalBoardState = new bool[width, height];
		this.BoardColorState   = new Color[width, height];
		this.BoardSize         = new Size(width, height);

		int a = 0;
		for (int index0 = 0; index0 < this.LogicalBoardState.GetLength(0); index0++)
			for (int index1 = 0; index1 < this.LogicalBoardState.GetLength(1); index1++) {
				bool b = this.LogicalBoardState[index0, index1];

				this.LogicalBoardState[index0, index1] = (a % 3) == 0;
				this.BoardColorState[index0, index1]   = (a % 6) == 0 ? Color.Blue : Color.Red;
				
				a++;
			}
	}
}
