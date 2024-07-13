using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer[] masterMixers;
    [SerializeField] private AudioMixer[] musicMixers;
    [SerializeField] private AudioMixer[] charactersMixers;
    [SerializeField] private AudioMixer[] backgroundMixers;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider charactersSlider;
    [SerializeField] private Slider backgroundSlider;
    public float MasterSlider { get => masterSlider.value; }
    public float MusicSlider { get => musicSlider.value; }
    public float CharactersSlider { get => charactersSlider.value; }
    public float BackgroundSlider { get => backgroundSlider.value; }

    private SaveFileHandler saveFileHandler;

    private void Start()
    {
        saveFileHandler = FindObjectOfType<SaveFileHandler>();
        saveFileHandler.LoadSettingsData();

        gameObject.SetActive(false);
    }

    public void LoadSliderValues(float master, float music, float characters, float background)
    {
        masterSlider.value = master;
        musicSlider.value = music;
        charactersSlider.value = characters;
        backgroundSlider.value = background;

        UpdateMixerVolumes();
    }

    public void SaveSliderValues()
    {
        UpdateMixerVolumes();
        saveFileHandler.SaveSettingsData();
    }

    private void UpdateMixerVolumes()
    {
        foreach (AudioMixer am in masterMixers)
            am.SetFloat("MasterVolume", masterSlider.value);

        foreach (AudioMixer am in musicMixers)
            am.SetFloat("MusicVolume", musicSlider.value);

        foreach (AudioMixer am in charactersMixers)
            am.SetFloat("CharacterVolume", charactersSlider.value);

        foreach (AudioMixer am in backgroundMixers)
            am.SetFloat("BackgroundVolume", backgroundSlider.value);
    }
}
