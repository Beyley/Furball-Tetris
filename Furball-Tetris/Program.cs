namespace Furball_Tetris; 

public static class Program {
	public static void Main(string[] args) {
		using TetrisGame game = new TetrisGame();
		
		game.Run();
	} 
}