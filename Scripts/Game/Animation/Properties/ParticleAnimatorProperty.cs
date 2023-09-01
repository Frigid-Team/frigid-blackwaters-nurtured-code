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
        private Nested2DList<bool> playedThisFrames;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<bool> onlyPlayOnFirstCycles;

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

        public bool GetPlayThisFrame(int animationIndex, int frameIndex)
        {
            return this.playedThisFrames[animationIndex][frameIndex];
        }

        public void SetPlayThisFrame(int animationIndex, int frameIndex, bool playThisFrame)
        {
            if (this.playedThisFrames[animationIndex][frameIndex] != playThisFrame)
            {
                FrigidEdit.RecordChanges(this);
                this.playedThisFrames[animationIndex][frameIndex] = playThisFrame;
            }
        }

        public bool GetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex)
        {
            return this.onlyPlayOnFirstCycles[animationIndex][frameIndex];
        }

        public void SetOnlyPlayOnFirstCycle(int animationIndex, int frameIndex, bool onlyPlayOnFirstCycle)
        {
            if (this.onlyPlayOnFirstCycles[animationIndex][frameIndex] != onlyPlayOnFirstCycle)
            {
                FrigidEdit.RecordChanges(this);
                this.onlyPlayOnFirstCycles[animationIndex][frameIndex] = onlyPlayOnFirstCycle;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.particleSystem = FrigidEdit.AddComponent<ParticleSystem>(this.gameObject);
            this.particleSystemRenderer = this.gameObject.GetComponent<ParticleSystemRenderer>();
            this.playedThisFrames = new Nested2DList<bool>();
            this.onlyPlayOnFirstCycles = new Nested2DList<bool>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.playedThisFrames.Add(new Nested1DList<bool>());
                this.onlyPlayOnFirstCycles.Add(new Nested1DList<bool>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.playedThisFrames[animationIndex].Add(false);
                    this.onlyPlayOnFirstCycles[animationIndex].Add(false);
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames.Insert(animationIndex, new Nested1DList<bool>());
            this.onlyPlayOnFirstCycles.Insert(animationIndex, new Nested1DList<bool>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.playedThisFrames[animationIndex].Add(false);
                this.onlyPlayOnFirstCycles[animationIndex].Add(false);
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames.RemoveAt(animationIndex);
            this.onlyPlayOnFirstCycles.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames[animationIndex].Insert(frameIndex, false);
            this.onlyPlayOnFirstCycles[animationIndex].Insert(frameIndex, false);
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.playedThisFrames[animationIndex].RemoveAt(frameIndex);
            this.onlyPlayOnFirstCycles[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void CopyPasteToAnotherFrame(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex)
        {
            ParticleAnimatorProperty otherParticleProperty = otherProperty as ParticleAnimatorProperty;
            if (otherParticleProperty)
            {
                otherParticleProperty.SetPlayThisFrame(toAnimationIndex, toFrameIndex, this.GetPlayThisFrame(fromAnimationIndex, fromFrameIndex));
                otherParticleProperty.SetOnlyPlayOnFirstCycle(toAnimationIndex, toFrameIndex, this.GetOnlyPlayOnFirstCycle(fromAnimationIndex, fromFrameIndex));
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
            if (!this.Body.Previewing)
            {
                if (!this.Loop && this.GetPlayThisFrame(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex) && (this.Body.CurrentCycleIndex == 0 || !this.GetOnlyPlayOnFirstCycle(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex)))
                {
                    if (this.particleSystem.isPlaying)
                    {
                        this.particleSystem.Stop();
                    }
                    this.particleSystem.Play();
                }
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
    }
}
