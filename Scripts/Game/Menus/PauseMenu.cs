using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class PauseMenu : Menu
    {
        [Header("Pause Menu")]
        [SerializeField]
        private Transform pauseContentTransform;
        [SerializeField]
        private Button exitButton;

        [Header("Audio Settings")]
        [SerializeField]
        private List<Slider> audioSliders;
        [SerializeField]
        private AudioMixer audioMixer;

        [Header("Quality Settings")]
        [SerializeField]
        private Dropdown qualityDropdown;

        private const string VolumeSettingKey = "VolumeSetting";
        private const string QualitySettingKey = "QualityLevel";

        public override bool WantsToOpen()
        {
            return InterfaceInput.ReturnPerformedThisFrame;
        }

        public override bool WantsToClose()
        {
            return InterfaceInput.ReturnPerformedThisFrame;
        }

        public override void Opened()
        {
            base.Opened();
            this.pauseContentTransform.gameObject.SetActive(true);
        }

        public override void Closed()
        {
            base.Closed();
            this.pauseContentTransform.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();
            this.exitButton.onClick.AddListener(this.ExitGame);
            for (int i = 0; i < this.audioSliders.Count; i++)
            {
                int sliderIndex = i;
                this.audioSliders[sliderIndex].onValueChanged.AddListener((float slider01) => this.UpdateVolume(sliderIndex, slider01));
            }

            this.pauseContentTransform.gameObject.SetActive(false);

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                options.Add(new Dropdown.OptionData(QualitySettings.names[i]));
            }
            this.qualityDropdown.AddOptions(options);
            this.qualityDropdown.onValueChanged.AddListener(this.UpdateQuality);
        }

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < this.audioSliders.Count; i++)
            {
                if (PlayerPrefs.HasKey(VolumeSettingKey + i))
                {
                    this.audioSliders[i].value = PlayerPrefs.GetFloat(VolumeSettingKey + i);
                }
                this.UpdateVolume(i, this.audioSliders[i].value);
            }

            if (PlayerPrefs.HasKey(QualitySettingKey))
            {
                int qualityLevel = PlayerPrefs.GetInt(QualitySettingKey);
                this.UpdateQuality(qualityLevel);
                this.qualityDropdown.SetValueWithoutNotify(qualityLevel);
            }
            else
            {
                this.qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
            }
        }

        private void UpdateVolume(int sliderIndex, float slider01)
        {
            const float LogTranslationValue = 1.05925372518f;
            this.audioMixer.SetFloat(VolumeSettingKey + sliderIndex, Mathf.Log(slider01, LogTranslationValue) - 80);
            PlayerPrefs.SetFloat(VolumeSettingKey + sliderIndex, slider01);
        }

        private void UpdateQuality(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
            PlayerPrefs.SetInt(QualitySettingKey, qualityLevel);
        }

        private void ExitGame()
        {
            Debug.Log("Quitting!");
            Application.Quit();
        }
    }
}