using System.Collections;
using System.Collections.Generic;
using Action = System.Action;

using UnityEngine;

using UnitWarfare.Core;

namespace UnitWarfare.Audio
{
    public class AudioManager
    {
        private static AudioManager s_instance;
        public static AudioManager Instance => s_instance;
        public static AudioManager CreateInstance()
        {
            if (s_instance != null)
                return s_instance;
            s_instance = new AudioManager();
            return s_instance;
        }

        private readonly EncapsulatedMonoBehaviour m_emb;
        public EncapsulatedMonoBehaviour EMB => m_emb;

        private AudioManager()
        {
            m_emb = new(new("AUDIO_MANAGER"));
            GameObject.DontDestroyOnLoad(m_emb.gameObject);
        }
    }
}