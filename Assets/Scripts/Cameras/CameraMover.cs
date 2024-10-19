using UnityEngine;
using UnityEngine.Serialization;

namespace Cameras {
	public class CameraMover : MonoBehaviour {

		[SerializeField] private Camera cam;
		
		[SerializeField] private float speed = 15;
		
		private static CameraMover _instance;
		
		private Vector3 _targetPosition;

		private void Awake() {
			_instance = this;
		}

		public static void MoveTo(Vector3 target) {
			_instance._targetPosition = target;
		}

		public static void SetPos(Vector3 target) {
			_instance._targetPosition = target;
			_instance.transform.position = target;
		}

		public static Camera Cam => _instance.cam;

		private void Update()
		{
			var step = speed * Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, _targetPosition, step);
		}
	}
}