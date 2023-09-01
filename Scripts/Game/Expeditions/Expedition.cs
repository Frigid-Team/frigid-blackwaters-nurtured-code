using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using UnityEngine.SceneManagement;

namespace FrigidBlackwaters.Game
{
    public abstract class Expedition : FrigidScriptableObject
    {
        private static Expedition activeExpedition;
        private static Action<Expedition> onExpeditionDeparted;
        private static Action<Expedition> onExpeditionReturned;

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

        static Expedition()
        {
            activeExpedition = null;
        }

        public static bool IsDepartedOnExpedition
        {
            get
            {
                return activeExpedition != null && SceneChanger.Instance.ActiveOrQueuedScene == FrigidScene.Dungeon;
            }
        }

        public static Action<Expedition> OnExpeditionDeparted
        {
            get
            {
                return onExpeditionDeparted;
            }
            set
            {
                onExpeditionDeparted = value;
            }
        }

        public static Action<Expedition> OnExpeditionReturned
        {
            get
            {
                return onExpeditionReturned;
            }
            set
            {
                onExpeditionReturned = value;
            }
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
        
        public List<Expedition> ExpeditionsUnlockedOnComplete
        {
            get
            {
                return this.expeditionsUnlockedOnCompletion;
            }
        }

        public bool Depart(Action onComplete)
        {
            if (SceneChanger.Instance.ActiveOrQueuedScene == FrigidScene.Dungeon || activeExpedition != null)
            {
                return false;
            }

            activeExpedition = this;
            foreach (LevelConfiguration levelConfiguration in this.levelConfigurations)
            {
                levelConfiguration.Depart();
            }
            foreach (MobSpawningConfiguration mobConfiguration in this.mobSpawningConfigurations)
            {
                mobConfiguration.Depart();
            }
            SceneChanger.Instance.ChangeScene(FrigidScene.Dungeon);
            this.Departed(onComplete);
            onExpeditionDeparted?.Invoke(this);

            return true;
        }

        public bool Return()
        {
            if (SceneChanger.Instance.ActiveOrQueuedScene == FrigidScene.PortCity || activeExpedition != this)
            {
                return false;
            }

            activeExpedition = null;
            foreach (LevelConfiguration levelConfiguration in this.levelConfigurations)
            {
                levelConfiguration.Return();
            }
            foreach (MobSpawningConfiguration mobConfiguration in this.mobSpawningConfigurations)
            {
                mobConfiguration.Return();
            }
            SceneChanger.Instance.ChangeScene(FrigidScene.PortCity);
            this.Returned();
            onExpeditionReturned?.Invoke(this);

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