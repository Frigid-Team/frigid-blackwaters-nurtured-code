using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class ParticleAnimatorProperty : RendererAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private ParticleSystem particleSystem;
        [SerializeField]
        [ReadOnly]
        private ParticleSystemRenderer particleSystemRenderer;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<PlayBehaviour> playBehaviours;

        public bool Loop
        {
            get
            {
                return this.particleSystem.main.loop;
            }
            set
            {
                if (this.particleSystem.main.loop != value)
                {
                    FrigidEdit.RecordChanges(this.particleSystem);
                    ParticleSystem.MainModule main = this.particleSystem.main;
                    main.loop = value;
                }
            }
        }

        public PlayBehaviour GetPlayBehaviour(int animationIndex, int frameIndex)
        {
            return this.playBehaviours[animationIndex][frameIndex];
        }

        public void SetPlayBehaviour(int animationIndex, int frameIndex, PlayBehaviour playBehaviour)
        {
            if (this.playBehaviours[animationIndex][frameIndex] != playBehaviour)
            {
                FrigidEdit.RecordChanges(this);
                this.playBehaviours[animationIndex][frameIndex] = playBehaviour;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.particleSystem = FrigidEdit.AddComponent<ParticleSystem>(this.gameObject);
            this.particleSystemRenderer = this.gameObject.GetComponent<ParticleSystemRenderer>();
            this.playBehaviours = new Nested2DList<PlayBehaviour>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.playBehaviours.Add(new Nested1DList<PlayBehaviour>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playBehaviours[animationIndex].Add(PlayBehaviour.NoPlay);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours.Insert(animationIndex, new Nested1DList<PlayBehaviour>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playBehaviours[animationIndex].Add(PlayBehaviour.NoPlay);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours[animationIndex].Insert(frameIndex, PlayBehaviour.NoPlay);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playBehaviours[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            ParticleAnimatorProperty otherParticleProperty = otherProperty as ParticleAnimatorProperty;
            if (otherParticleProperty)
            {
                otherParticleProperty.SetPlayBehaviour(toAnimationIndex, toFrameIndex, this.GetPlayBehaviour(fromAnimationIndex, fromFrameIndex));
            }
            base.CopyPasteToAnotherFrame(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Body.OnTimeScaleChanged +=
                () =>
                {
                    ParticleSystem.MainModule main = this.particleSystem.main;
                    main.simulationSpeed = this.Body.TimeScale;
                };
        }

        public override void AnimationEnter()
        {
            if (!this.Body.Previewing)
            {
                if (this.Loop)
                {
                    this.particleSystem.Play();
                }
            }
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (!this.Body.Previewing)
            {
                if (this.Loop)
                {
                    this.particleSystem.Stop();
                }
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            if (!this.Loop && !this.Body.Previewing)
            {
                switch (this.GetPlayBehaviour(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex))
                {
                    case PlayBehaviour.NoPlay:
                        goto skipPlay;
                    case PlayBehaviour.PlayEveryCycle:
                        break;
                    case PlayBehaviour.PlayOnFirstCycle:
                        if (this.Body.CycleIndex == 0)
                        {
                            break;
                        }
                        goto skipPlay;
                }

                if (this.particleSystem.isPlaying)
                {
                    this.particleSystem.Stop();
                }
                this.particleSystem.Play();

            skipPlay:;
            }
            base.FrameEnter();
        }

        protected override Renderer Renderer
        {
            get
            {
                return this.particleSystemRenderer;
            }
        }

        public enum PlayBehaviour
        {
            NoPlay,
            PlayEveryCycle,
            PlayOnFirstCycle
        }
    }
}
