using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;
	
    private float _hor, _ver;

    private Rigidbody _rb;

    private void Awake() {
	    _rb = GetComponent<Rigidbody>();
    }

    public bool IsMoving() {
	    return _hor != 0 || _ver != 0;
    }

    private void Update()
    {
        _hor = Input.GetAxisRaw("Horizontal");
        _ver = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() {
	    var dir = new Vector3(_hor, 0, _ver);
	    dir.Normalize();
	    var velocity = moveSpeed * dir;
	    _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
    }
}
