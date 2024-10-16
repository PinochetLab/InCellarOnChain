using UnityEngine;

namespace InputSystem {
	public enum Mode {Game, UI}
	
	public static class GameInputManager {
		private static Mode _mode = Mode.Game;

		public static void SetMode(Mode mode) {
			_mode = mode;
		}

		public static float GetAxisRaw(string axisName) {
			if ( _mode != Mode.Game ) return 0;
			return Input.GetAxisRaw(axisName);
		}
		
		public static bool GetButtonDown(string buttonName) {
			return _mode == Mode.Game && Input.GetButtonDown(buttonName);
		}
	}
}