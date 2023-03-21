using UnityEngine;

namespace FrigidBlackwaters
{
    public class FrigidMonoBehaviourWithPhysics : FrigidMonoBehaviour
    {
        protected virtual void FixedUpdate() { }

        protected virtual void OnTriggerEnter2D(Collider2D collision) { }

        protected virtual void OnTriggerStay2D(Collider2D collision) { }

        protected virtual void OnTriggerExit2D(Collider2D collision) { }

        protected virtual void OnCollisionEnter2D(Collision2D collision) { }

        protected virtual void OnCollisionStay2D(Collision2D collision) { }

        protected virtual void OnCollisionExit2D(Collision2D collision) { }
    }
}
