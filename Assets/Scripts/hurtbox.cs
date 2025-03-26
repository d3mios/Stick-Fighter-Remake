using UnityEngine;
using Characters;

namespace Hurtbox {
    public class hurtbox : MonoBehaviour {
        private character user;
        private Vector3 offset;
        public void Initialize(Bounds bounds_, character user_, Vector3 initialPos) {
            user = user_;
            this.gameObject.AddComponent<SpriteRenderer>();
            GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
			var texture = (Texture2D)Resources.Load("Sprites/solidWhite");
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, 1, 1), new Vector2(0.5f, 0.5f));
			GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
            transform.localScale = new Vector3(bounds_.size.x, bounds_.size.y, 1);
            transform.position = initialPos;
            offset = bounds_.center;
        }

        public Vector3 getPosition() { return transform.position; }
        public Bounds getBounds() {
            return GetComponent<SpriteRenderer>().bounds;
        }

        public Vector3 getOffset() {
            return new Vector3(offset.x * user.getFacing(), offset.y, offset.z);
        }

        void Update() {
            //transform.position = user.transform.position;
        }
        public void updatePosition(Vector3 newPosition) {
            transform.position = newPosition + offset;
        }

        public void updateSize(Bounds newBounds) {
          transform.localScale = new Vector3(newBounds.size.x, newBounds.size.y, 1);
          offset = newBounds.center;
        }
    }
}