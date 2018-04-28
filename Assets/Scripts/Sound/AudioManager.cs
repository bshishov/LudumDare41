using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class AudioManager : Singleton<AudioManager>
    {
        public const string MusicVolumeKey = "MusicVolume";
        public const string SoundVolumeKey = "SoundVolumeKey";

        public AudioClip Ost;
        public float Proximity = 10f;

        private AudioSource _musicAudioSource;
        private AudioSource _soundsAudioSource;

        public float MusicVolume
        {
            get { return _musicAudioSource.volume; }
            set
            {
                _musicAudioSource.volume = value;
                PlayerPrefs.SetFloat(MusicVolumeKey, value);
            }
        }

        public float SoundsVolume
        {
            get { return _soundsAudioSource.volume; }
            set
            {
                _soundsAudioSource.volume = value;
                PlayerPrefs.SetFloat(SoundVolumeKey, value);
            }
        }

        void Start ()
        {
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
            _musicAudioSource.volume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.1f);

            _soundsAudioSource = gameObject.AddComponent<AudioSource>();
            _soundsAudioSource.volume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f);

            _musicAudioSource.clip = Ost;
            _musicAudioSource.loop = true;
            _musicAudioSource.Play();
        }
        
        void Update ()
        {
        }

        public void PlayClip(AudioClip clip, float volume)
        {
            _soundsAudioSource.PlayOneShot(clip, volume);
        }

        public void PlayClip(AudioClip clip)
        {
            _soundsAudioSource.PlayOneShot(clip);
        }

        public void PlayClip(AudioClipWithVolume clip)
        {
            if (clip.RandomizePitch)
                _soundsAudioSource.pitch = Random.Range(0.9f, 1.1f);

            _soundsAudioSource.PlayOneShot(clip.Clip, clip.VolumeModifier);
        }

        public void PlayClip(Vector3 position, AudioClip clip, float volume)
        {
            var vol = DistanceVolumeMod(position);
            if (vol < 0.1)
                return;

            _soundsAudioSource.PlayOneShot(clip, volume * vol);
        }

        public void PlayClip(Vector3 position, AudioClip clip)
        {
            var vol = DistanceVolumeMod(position);
            if (vol < 0.1)
                return;

            _soundsAudioSource.PlayOneShot(clip, vol);
        }

        public void PlayClip(Vector3 position, AudioClipWithVolume clip)
        {
            var vol = DistanceVolumeMod(position);
            if(vol < 0.1)
                return;

            if (clip.RandomizePitch)
                _soundsAudioSource.pitch = Random.Range(0.9f, 1.1f);

            _soundsAudioSource.PlayOneShot(clip.Clip, clip.VolumeModifier * vol);
        }

        private float DistanceVolumeMod(Vector3 p)
        {
            return 1f - Mathf.Clamp01(Vector3.Distance(p, transform.position) / Proximity);
        }
    }
}
