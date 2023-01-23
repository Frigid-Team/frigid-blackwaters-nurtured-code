using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

        private const float LOG_TRANSLATION_VALUE = 1.05925372518f;
        private const string PLAYER_PREF_VOLUME_SETTING = "VolumeSetting";
        private const string PLAYER_PREF_QUALITY_SETTING = "QualityLevel";

        protected override void Opened()
        {
            this.pauseContentTransform.gameObject.SetActive(true);
        }

        protected override void Closed()
        {
            this.pauseContentTransform.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();
            this.exitButton.onClick.AddListener(ExitGame);
            for (int i = 0; i < this.audioSliders.Count; i++)
            {
                int sliderIndex = i;
                this.audioSliders[sliderIndex].onValueChanged.AddListener((float slider01) => UpdateVolume(sliderIndex, slider01));
            }

            this.pauseContentTransform.gameObject.SetActive(false);

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                options.Add(new Dropdown.OptionData(QualitySettings.names[i]));
            }
            this.qualityDropdown.AddOptions(options);
            this.qualityDropdown.onValueChanged.AddListener(UpdateQuality);
        }

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < this.audioSliders.Count; i++)
            {
                if (PlayerPrefs.HasKey(PLAYER_PREF_VOLUME_SETTING + i))
                {
                    this.audioSliders[i].value = PlayerPrefs.GetFloat(PLAYER_PREF_VOLUME_SETTING + i);
                }
                UpdateVolume(i, this.audioSliders[i].value);
            }

            if (PlayerPrefs.HasKey(PLAYER_PREF_QUALITY_SETTING))
            {
                int qualityLevel = PlayerPrefs.GetInt(PLAYER_PREF_QUALITY_SETTING);
                UpdateQuality(qualityLevel);
                this.qualityDropdown.SetValueWithoutNotify(qualityLevel);
            }
            else
            {
                this.qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
            }
        }

        private void UpdateVolume(int sliderIndex, float slider01)
        {
            this.audioMixer.SetFloat(PLAYER_PREF_VOLUME_SETTING + sliderIndex, Mathf.Log(slider01, LOG_TRANSLATION_VALUE) - 80);
            PlayerPrefs.SetFloat(PLAYER_PREF_VOLUME_SETTING + sliderIndex, slider01);
        }

        private void UpdateQuality(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
            PlayerPrefs.SetInt(PLAYER_PREF_QUALITY_SETTING, qualityLevel);
        }

        private void ExitGame()
        {
            Debug.Log("Quitting!");
            Application.Quit();
        }
    }
}