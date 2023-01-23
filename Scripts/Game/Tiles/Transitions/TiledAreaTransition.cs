using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaTransition", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaTransition")]
    public class TiledAreaTransition : FrigidScriptableObject
    {
        [SerializeField]
        private RuntimeAnimatorController animatorController;
        [SerializeField]
        private Sprite openSprite;
        [SerializeField]
        private Sprite closeSprite;
        [SerializeField]
        private AudioClip transitionAwayAudioClip;
        [SerializeField]
        private AudioClip transitionToAudioClip;
        [SerializeField]
        private float baseDuration;

        public RuntimeAnimatorController AnimatorController
        {
            get
            {
                return this.animatorController;
            }
        }

        public Sprite OpenSprite
        {
            get
            {
                return this.openSprite;
            }
        }

        public Sprite CloseSprite
        {
            get
            {
                return this.closeSprite;
            }
        }

        public AudioClip TransitionAwayAudioClip
        {
            get
            {
                return this.transitionAwayAudioClip;
            }
        }

        public AudioClip TransitionToAudioClip
        {
            get
            {
                return this.transitionToAudioClip;
            }
        }

        public float BaseDuration
        {
            get
            {
                return this.baseDuration;
            }
        }
    }
}
