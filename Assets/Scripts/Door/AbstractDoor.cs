using System;
using System.Collections;
using Interaction;
using UnityEngine;

namespace Door {
	public abstract class AbstractDoor : AbstractInteractable {
		private static readonly int Open = Animator.StringToHash("TryOpen");

		/*[SerializeField] private DoorCollider doorCollider;
		[SerializeField] private Transform hingeTransform;

		[SerializeField] private ConfigurableJoint configurableJoint;
		[SerializeField] private Rigidbody doorRigidbody;

		private bool _targetOpened;

		private float _targetAngle;

		private bool _move;

		private bool _greater;

		private const float ClosedAngle = 0;
		private const float OpenedAngle = 89;
		public abstract void PerformAction(IActor actor);
		
		protected bool IsClosed { get; private set; } = true;

		private void Awake() {
			doorCollider.OnCrash.AddListener(OnCrash);
		}

		protected void ChangeState(IActor actor) {
			StopAllCoroutines();
			_targetOpened = !_targetOpened;
			StartCoroutine(_targetOpened ? Open(actor) : Close());
		}

		private void SetLimits(float left, float right) {
			var xLowLimit = configurableJoint.lowAngularXLimit;
			xLowLimit.limit = left;
			configurableJoint.lowAngularXLimit = xLowLimit;
			
			var xHighLimit = configurableJoint.highAngularXLimit;
			xHighLimit.limit = right;
			configurableJoint.highAngularXLimit = xHighLimit;
		}

		private IEnumerator Open(IActor actor) {
			_move = true;
			var fromLeft = Vector3.Dot(actor.GetPosition() - transform.position, transform.right) > 0;
			var left = fromLeft ? 0 : -OpenedAngle;
			var right = fromLeft ? OpenedAngle : 0;
			
			SetLimits(left, right);
			
			//doorRigidbody.angularVelocity = Vector3.up * (200 * (fromLeft ? 1 : -1));
			_targetAngle = OpenedAngle * (fromLeft ? 1 : -1);
			_greater = _targetAngle > 0;
			doorRigidbody.isKinematic = false;
			
			yield break;
			/*IsClosed = false;
			var fromLeft = Vector3.Dot(actor.GetPosition() - transform.position, transform.right) > 0;
			doorCollider.enabled = false;
			yield return ChangeAngle(OpenedAngle * (fromLeft ? 1 : -1));
			doorCollider.enabled = true;#1#
		}

		private IEnumerator Close() {
			_move = true;
			//SetLimits(0, 0);
			_targetAngle = 0;
			_greater = doorRigidbody.rotation.eulerAngles.y < 0;
			doorRigidbody.isKinematic = false;
			
			yield break;
			/*doorCollider.enabled = false;
			yield return ChangeAngle(ClosedAngle);
			doorCollider.enabled = true;
			IsClosed = true;#1#
		}

		private void FixedUpdate() {
			if ( !_move ) {
				doorRigidbody.MoveRotation(Quaternion.Euler(Vector3.up * _targetAngle));
				doorRigidbody.isKinematic = true;
				return;
			}
			var currentAngle = doorRigidbody.rotation.eulerAngles.y;
			var delta = Mathf.DeltaAngle(currentAngle, _targetAngle);
			if ( Mathf.Abs(delta) < 1f && (_greater ? delta > 0 : delta < 0) ) {
				_move = false;
				doorRigidbody.rotation = Quaternion.Euler(Vector3.up * _targetAngle);
				doorRigidbody.isKinematic = true;
				return;
			}
			var sign = Mathf.Sign(delta);
			doorRigidbody.angularVelocity = Vector3.up * (200 * sign);
		}

		private IEnumerator ChangeAngle(float endAngle) {
			var startAngle = hingeTransform.localEulerAngles.y;
			var deltaAngle = Mathf.DeltaAngle(startAngle, endAngle);
			endAngle = startAngle + deltaAngle;
			const float speed = 300;
			var dt = 0.01f;
			var n = (int)(Mathf.Abs(endAngle - startAngle) / speed / dt);
			dt = Mathf.Abs(endAngle - startAngle) / speed / n;
			for ( var i = 0; i <= n; i++ ) {
				var t = (float)i / n;
				hingeTransform.localEulerAngles = Vector3.up * Mathf.Lerp( startAngle, endAngle, t);
				yield return new WaitForSeconds(dt);
			}
		}
		
		private void OnCrash()
		{
			_move = false;
			doorRigidbody.MoveRotation(Quaternion.Euler(Vector3.up * _targetAngle));
			doorRigidbody.isKinematic = true;
		}*/
		[SerializeField] private Transform hingeTransform;
		
		[SerializeField] private Animator animator;

		private bool _opened;

		private const float ClosedAngle = 0;
		private const float OpenedAngle = 90;
		
		protected void ChangeState(Vector3 characterPosition) {
			_opened = !_opened;
			var angle = ClosedAngle;
			if ( _opened ) {
				var toCharacter = characterPosition - transform.position;
				var fromLeft = Vector3.Dot(toCharacter, transform.right) > 0;
				angle = OpenedAngle * (fromLeft ? 1 : -1);
			}
			hingeTransform.localRotation = Quaternion.Euler(0, angle, 0);
		}

		protected void TryOpen() {
			animator.SetTrigger(Open);
		}
	}
}