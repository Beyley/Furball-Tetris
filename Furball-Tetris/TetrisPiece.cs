using System.Drawing;

namespace Furball_Tetris; 

public class TetrisPiece {
	public bool[,]       State;
	public Size          Size;

	public TetrisPiece(int width, int height) {
		this.State = new bool[width, height];
		this.Size  = new Size(width, height);
	}

	static TetrisPiece() {
		I_PIECE[0] = new(1, 4) {
			State = {
				/*
				 * 0
				 * 0
				 * 0
				 * 0
				 */
				[0, 0] = true,
				[0, 1] = true,
				[0, 2] = true,
				[0, 3] = true
			}
		};
		I_PIECE[1] = new(4, 1) {
			State = {
				/*
				 * 0000
				 */
				[0, 0] = true, [1, 0] = true, [2, 0] = true, [3, 0] = true
			}
		}; 
		//180 rotation = 0 rotation
		I_PIECE[2] = I_PIECE[0];
		//270 rotation = 90 rotation
		I_PIECE[3] = I_PIECE[1];
		
		O_PIECE[0] = new(2, 2) {
			State = {
				/*
				 * 00
				 * 00
				 */
				[0, 0] = true, [1, 1] = true,
				[0, 1] = true, [1, 0] = true
			}
		};
		O_PIECE[1] = O_PIECE[0];
		O_PIECE[2] = O_PIECE[0];
		O_PIECE[3] = O_PIECE[0];

		S_PIECE[0] = new(3, 2) {
			State = {
				/*
				 *  00
				 * 00
				 */
				[0, 0] = false, [1, 0] = true, [2, 0] = true,
				[0, 1] = true,  [1, 1] = true, [2, 1] = false
			}
		};
		S_PIECE[1] = new(2, 3) {
			State = {
				/*
				 * 0
				 * 00
				 *  0
				 */
				[0, 0] = true, [1, 0]  = false,
				[0, 1] = true, [1, 1]  = true,
				[0, 2] = false, [1, 2] = true
			}
		};
		//180 rotation = 0 rotation
		S_PIECE[2] = S_PIECE[0];
		//270 rotation = 90 rotation
		S_PIECE[3] = S_PIECE[1];
		
		Z_PIECE[0] = new(3, 2) {
			State = {
				/*
				 * 00
				 *  00
				 */
				[0, 0] = true,  [1, 0] = true, [2, 0] = false,
				[0, 1] = false, [1, 1] = true, [2, 1] = true
			}
		};
		Z_PIECE[1] = new(2, 3) {
			State = {
				/*
				 *  0
				 * 00
				 * 0 
				 */
				[0, 0] = false, [1, 0] = true,
				[0, 1] = true, [1, 1] = true,
				[0, 2] = true, [1, 2] = false,
			}
		};
		//180 rotation = 0 rotation
		Z_PIECE[2] = Z_PIECE[0];
		//270 rotation = 0 rotation
		Z_PIECE[3] = Z_PIECE[1];

		T_PIECE[0] = new(3, 2) {
			State = {
				/*
				 *  0
				 * 000
				 */
				[0, 0] = false, [1, 0] = true, [2, 0] = false,
				[0, 1] = true, [1, 1]  = true, [2, 1] = true
			}
		};
		T_PIECE[1] = new(2, 3) {
			State = {
				/*
				 * 0
				 * 00
				 * 0
				 */
				[0, 0] = true, [1, 0] = false,
				[0, 1] = true, [1, 1] = true,
				[0, 2] = true, [1, 2] = false,
			}
		};
		T_PIECE[2] = new(3, 2) {
			State = {
				/*
				 * 000
				 *  0
				 */
				[0, 0] = true, [1, 0]  = true, [2, 0] = true,
				[0, 1] = false, [1, 1] = true, [2, 1] = false
			}
		};
		T_PIECE[3] = new(2, 3) {
			State = {
				/*
				 *  0
				 * 00
				 *  0
				 */
				[0, 0] = false, [1, 0] = true,
				[0, 1] = true, [1, 1]  = true,
				[0, 2] = false, [1, 2] = true,
			}
		};
		
		L_PIECE[0] = new(2, 3) {
			State = {
				/*
				 * 0
				 * 0
				 * 00
				 */
				[0, 0] = true, [1, 0] = false,
				[0, 1] = true, [1, 1] = false,
				[0, 2] = true, [1, 2] = true
			}
		};
		L_PIECE[1] = new(3, 2) {
			State = {
				/*
				 * 000
				 * 0
				 */
				[0, 0] = true, [1, 0] = true, [2, 0]  = true,
				[0, 1] = true, [1, 1] = false, [2, 1] = false,
			}
		};
		L_PIECE[2] = new(2, 3) {
			State = {
				/*
				 * 00
				 *  0
				 *  0
				 */
				[0, 0] = true, [1, 0]  = true,
				[0, 1] = false, [1, 1] = true,
				[0, 2] = false, [1, 2] = true
			}
		};
		L_PIECE[3] = new(3, 2) {
			State = {
				/*
				 *    0
				 *  000
				 */
				[0, 0] = false, [1, 0] = false, [2, 0] = true,
				[0, 1] = true, [1, 1]  = true, [2, 1]  = true,
			}
		};
		
		J_PIECE[0] = new(2, 3) {
			State = {
				/*
				 *  0
				 *  0
				 * 00
				 */
				[0, 0] = false, [1, 0] = true,
				[0, 1] = false, [1, 1] = true,
				[0, 2] = true,  [1, 2] = true
			}
		};
		J_PIECE[1] = new(3, 2) {
			State = {
				/*
				 * 0
				 * 000
				 */
				[0, 0] = true, [1, 0] = false, [2, 0] = false,
				[0, 1] = true, [1, 1] = true, [2, 1]  = true,
			}
		};
		J_PIECE[2] = new(2, 3) {
			State = {
				/*
				 * 00
				 * 0 
				 * 0 
				 */
				[0, 0] = true, [1, 0] = true,
				[0, 1] = true, [1, 1] = false,
				[0, 2] = true, [1, 2] = false
			}
		};
		J_PIECE[3] = new(3, 2) {
			State = {
				/*
				 * 000
				 *   0
				 */
				[0, 0] = true, [1, 0]  = true, [2, 0]  = true,
				[0, 1] = false, [1, 1] = false, [2, 1] = true,
			}
		};
	}
	
	public static readonly TetrisPiece[] I_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] O_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] S_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] Z_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] T_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] L_PIECE = new TetrisPiece[4];
	public static readonly TetrisPiece[] J_PIECE = new TetrisPiece[4];
}
