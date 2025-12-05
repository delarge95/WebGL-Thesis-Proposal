using UnityEngine;

namespace WebGL.Core.Managers
{
    public static class SaveSystem
    {
        private const string VolumeKey = "MasterVolume";
        private const string QualityKey = "QualityLevel";

        public static void SaveVolume(float volume)
        {
            PlayerPrefs.SetFloat(VolumeKey, volume);
            PlayerPrefs.Save();
        }

        public static float LoadVolume()
        {
            return PlayerPrefs.GetFloat(VolumeKey, 1.0f);
        }

        public static void SaveQuality(int qualityIndex)
        {
            PlayerPrefs.SetInt(QualityKey, qualityIndex);
            PlayerPrefs.Save();
        }

        public static int LoadQuality()
        {
            return PlayerPrefs.GetInt(QualityKey, 2); // Default to High (2)
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
