using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts
{
    public enum SFXEffectType
    {
        Hit
    } 
    
    public class SoundEffect : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> sounds;
        [SerializeField] private AudioSource source;

        [Button]
        public void PlaySound(SFXEffectType type)
        {
            source.PlayOneShot(sounds[(int)type]);
        }
    }
}