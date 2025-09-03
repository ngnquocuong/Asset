using _Main.Scripts.Utils;
using UnityEngine;

namespace _Ball
{
    public class SoundHelper : Singleton<SoundHelper>
    {
        public AudioClip m_Item1;
        public AudioClip m_Item2;
        public AudioClip m_SizeUp;
        public AudioSource m_SizeUpSource;

        public void PlayLevelUp()
        {
            m_SizeUpSource.PlayOneShot(m_SizeUp);
        }
    }
}