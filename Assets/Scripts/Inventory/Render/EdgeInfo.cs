namespace Inventory.Render {
	public struct EdgeInfo {
		public readonly bool Left, Right, Top, Bottom;

		public EdgeInfo(bool left, bool right, bool top, bool bottom) {
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}
	}
}