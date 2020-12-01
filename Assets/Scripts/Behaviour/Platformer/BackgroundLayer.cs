using UnityEngine;
using UnityEngine.UI;

namespace SmtProject.Behaviour.Platformer {
	[RequireComponent(typeof(RawImage))]
	public sealed class BackgroundLayer : MonoBehaviour {
		public Camera        Camera;
		public RectTransform RectTransform;
		public float         MoveSpeed;
		public Sprite        Sprite;
		public bool          Stretch;
		public bool          PreserveSize;

		RawImage _rawImage;
		Vector2  _uvRectSize;
		float    _yOffset;

		void Start() {
			_rawImage = GetComponent<RawImage>();

			var height = Camera.orthographicSize * 2f;
			var size   = new Vector2(height * Camera.aspect, height);
			if ( Stretch ) {
				RectTransform.sizeDelta = size;
			}

			_rawImage.texture = Sprite.texture;
			var spriteSize    = Sprite.rect.size;
			var pixelsPerUnit = Sprite.pixelsPerUnit;
			if ( PreserveSize ) {
				_uvRectSize = _rawImage.uvRect.size;
			} else {
				_uvRectSize = new Vector2(size.x / (spriteSize.x / pixelsPerUnit),
					size.y / (spriteSize.y / pixelsPerUnit));
				_rawImage.uvRect = new Rect(Vector2.zero, _uvRectSize);
			}
			_yOffset = _rawImage.uvRect.position.y;
		}

		void Update() {
			var cameraPos = Camera.transform.position;
			var prevY     = transform.position.y;
			transform.position = new Vector3(cameraPos.x, prevY, 0);
			_rawImage.uvRect   = new Rect(new Vector2(cameraPos.x * MoveSpeed, _yOffset), _uvRectSize);
		}
	}
}
