using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class Expedition : FrigidScriptableObject
    {
        private static Expedition activeExpedition;
        private static BoonLoadout activeExpeditionBoonLoadout;

        [SerializeField]
        private List<LevelConfiguration> levelConfigurations;
        [SerializeField]
        private List<MobSpawningConfiguration> mobSpawningConfigurations;

        [Header("Info")]
        [SerializeField]
        private string expeditionName;
        [SerializeField]
        private string flairDescription;
        [SerializeField]
        private string dungeonInfoDescription;
        [SerializeField]
        private string expeditionTypeDescription;
        [SerializeField]
        private string compensationFieldDescription;

        [Header("Progression")]
        [SerializeField]
        private IntSerializedReference numberStampsAwarded;
        [SerializeField]
        private List<Expedition> expeditionsUnlockedOnCompletion;
        [SerializeField]
        private List<Boon> boonsUnlockedOnCompletion;

        static Expedition()
        {
            activeExpedition = null;
        }

        public string ExpeditionName
        {
            get
            {
                return this.expeditionName;
            }
        }

        public string FlairDescription
        {
            get
            {
                return this.flairDescription;
            }
        }

        public string DungeonInfoDescription
        {
            get
            {
                return this.dungeonInfoDescription;
            }
        }

        public string ExpeditionTypeDescription
        {
            get
            {
                return this.expeditionTypeDescription;
            }
        }

        public string CompensationFieldDescription
        {
            get
            {
                return this.compensationFieldDescription;
            }
        }

        public int NumberAwardedStamps
        {
            get
            {
                return this.numberStampsAwarded.ImmutableValue;
            }
        }
        
        public List<Expedition> ExpeditionsUnlockedOnCompletion
        {
            get
            {
                return this.expeditionsUnlockedOnCompletion;
            }
        }

        public List<Boon> BoonsUnlockedOnCompletion
        {
            get
            {
                return this.boonsUnlockedOnCompletion;
            }
        }

        public bool Depart(BoonLoadout boonLoadout, Action onComplete)
        {
            if (activeExpedition != null)
            {
                return false;
            }

            activeExpedition = this;
            activeExpeditionBoonLoadout = new BoonLoadout(boonLoadout);
            foreach (LevelConfiguration levelConfiguration in this.levelConfigurations)
            {
                levelConfiguration.Depart();
            }
            foreach (MobSpawningConfiguration mobConfiguration in this.mobSpawningConfigurations)
            {
                mobConfiguration.Depart();
            }
            activeExpeditionBoonLoadout.Activate();

            SceneChanger.Instance.ChangeScene(FrigidScene.Dungeon);
            this.Departed(onComplete);

            return true;
        }

        public bool Return()
        {
            if (activeExpedition != this)
            {
                return false;
            }

            activeExpeditionBoonLoadout.Deactivate();
            foreach (LevelConfiguration levelConfiguration in this.levelConfigurations)
            {
                levelConfiguration.Return();
            }
            foreach (MobSpawningConfiguration mobConfiguration in this.mobSpawningConfigurations)
            {
                mobConfiguration.Return();
            }
            activeExpedition = null;
            activeExpeditionBoonLoadout = null;

            SceneChanger.Instance.ChangeScene(FrigidScene.PortCity);
            this.Returned();

            return true;
        }

        protected abstract void Departed(Action onComplete);

        protected abstract void Returned();

        [Serializable]
        private class LevelConfiguration
        {
            [SerializeField]
            private TiledLevelPlannerScriptableVariable tiledLevelPlannerVariable;
            [SerializeField]
            private RelativeWeightPool<TiledLevelPlannerSerializedReference> tiledLevelPlanners;

            public void Depart()
            {
                this.tiledLevelPlannerVariable.Value = this.tiledLevelPlanners.Retrieve().MutableValue;
            }

            public void Return()
            {
                this.tiledLevelPlannerVariable.Value = this.tiledLevelPlannerVariable.InitialValue;
            }
        }

        [Serializable]
        private class MobSpawningConfiguration
        {
            [SerializeField]
            private TiledAreaMobSpawnerScriptableVariable tiledAreaMobSpawnerVariable;
            [SerializeField]
            private RelativeWeightPool<TiledAreaMobSpawnerSerializedReference> tiledAreaMobSpawners;

            public void Depart()
            {
                this.tiledAreaMobSpawnerVariable.Value = this.tiledAreaMobSpawners.Retrieve().MutableValue;
            }

            public void Return()
            {
                this.tiledAreaMobSpawnerVariable.Value = this.tiledAreaMobSpawnerVariable.InitialValue;
            }
        }
    }
}