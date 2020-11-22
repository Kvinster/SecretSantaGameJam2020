using UnityEngine;

namespace SmtProject {
    public class PlayerController : MonoBehaviour {
        public Rigidbody2D Rigidbody2D;
        public float       Speed;
        public Shapes.Disc Disk;
        public float       InputMult;
        public float       LinearDrag;

        Vector2 _input;

        float _prevAngle;

        void Reset() {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Start() {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update() {
            _input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * InputMult;
            if ( _input != Vector2.zero ) {
                var angle = -Vector2.SignedAngle(_input, Vector2.right);
                angle                = Mathf.LerpAngle(_prevAngle, angle, 0.1f);
                Disk.AngRadiansStart = Mathf.Deg2Rad * (angle + 330);
                Disk.AngRadiansEnd   = Mathf.Deg2Rad * (angle + 30);
                _prevAngle           = angle;
            }
        }

        void FixedUpdate() {
            if ( _input == Vector2.zero ) {
                Rigidbody2D.velocity = Vector2.Lerp(Rigidbody2D.velocity, Vector2.zero, LinearDrag);
            } else {
                Rigidbody2D.velocity += _input * Speed;
                _input               =  Vector2.zero;
            }
        }
    }
}