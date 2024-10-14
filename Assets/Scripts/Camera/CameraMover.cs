using UnityEngine;

namespace Camera {
	public class CameraMover : MonoBehaviour {

		[SerializeField] private UnityEngine.Camera camera;
		
		[SerializeField] private float speed = 15;
		
		private static CameraMover _instance;
		
		private Vector3 _targetPosition;

		private void Awake() {
			_instance = this;
		}

		public static void MoveTo(Vector3 target) {
			_instance._targetPosition = target;
		}

		public static UnityEngine.Camera Camera => _instance.camera;

		private void Update()
		{
			var step = speed * Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, _targetPosition, step);
		}
	}
}