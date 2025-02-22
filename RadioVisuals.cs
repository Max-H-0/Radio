using Core.Sounds;
using MelonLoader;
using Ship.Interface.Model.Parts.State;
using Ship.Interface.Model.Parts.StateTypes;
using Ship.Network.Model;
using Ship.Parts.Common;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Radio
{
    public partial class RadioVisuals : DefaultShipPartVisuals
    {
        struct RadioAnimations
        {
            public const string OpenAntenna = "Open Antenna";
            public const string CloseAntenna = "Close Antenna";

            public const string OpenHandle = "Open Handle";
            public const string CloseHandle = "Close Handle";

            public const string EnableRockerSwitch = "Enable Rocker Switch";
            public const string DisableRockerSwitch = "Disable Rocker Switch";
        }


        public LeverState State { get; private set; }
        public bool IsRadioEnabled { get; private set; }

        private bool _isInitializing = true;


        private void Update()
        {
            if (IsRadioEnabled)
            {
                AnimateSpeaker();
            }
        }


        public override void SetNetworkState(IStatefulPartState state)
        {
            State = (LeverState)state;
            IsRadioEnabled = State.IsRadioEnabled();

            if (_isInitializing)
            {
                _isInitializing = false;
                if(!IsRadioEnabled) return;
            }

            if (IsRadioEnabled)
            {
                EnableRadio();
            }
            else
            {
                DisableRadio();
            }
        }

        public override void ReactToNetworkStateChange(StatefulPartStateChangeEvent change)
        {
            var oldState = (LeverState)change.PreviousState;
            var newState = (LeverState)change.NewState;

            State = newState;
            IsRadioEnabled = State.IsRadioEnabled();

            if (_isInitializing)
            {
                _isInitializing = false;
                if (!IsRadioEnabled) return;
            }

            if (!oldState.IsRadioEnabled() && newState.IsRadioEnabled())
            {
                EnableRadio();
            }

            if (oldState.IsRadioEnabled() && !newState.IsRadioEnabled())
            {
                DisableRadio();
            }
        }


        private void EnableRadio()
        {
            StopAllCoroutines();
            StartCoroutine(Enable());
        }

        private void DisableRadio()
        {
            StopAllCoroutines();
            StartCoroutine(Disable());
        }


        private IEnumerator Enable()
        {
            StartCoroutine(EnableAnimation());

            yield return new WaitForSeconds(0.3f);

            PlayFlickSound();

            yield return new WaitForSeconds(0.2f);

            PlayMusic();
        }

        private IEnumerator Disable()
        {
            StartCoroutine(DisableAnimation());
            PlayFlickSound();
            StopMusic();

            yield return null;
        }
    }



    public partial class RadioVisuals : DefaultShipPartVisuals
    {
        private IEnumerator EnableAnimation()
        {
            PlayAnimation(RadioAnimations.OpenHandle);

            yield return new WaitForSeconds(0.15f);

            PlayAnimation(RadioAnimations.OpenAntenna);

            yield return new WaitForSeconds(0.15f);

            PlayAnimation(RadioAnimations.EnableRockerSwitch);
        }

        private IEnumerator DisableAnimation()
        {
            PlayAnimation(RadioAnimations.DisableRockerSwitch);

            yield return new WaitForSeconds(0.15f);

            PlayAnimation(RadioAnimations.CloseAntenna);

            yield return new WaitForSeconds(0.15f);

            PlayAnimation(RadioAnimations.CloseHandle);
        }


        private void PlayAnimation(string state)
        {
            GetComponentInChildren<Animator>().Play(state);
        }
    }



    public partial class RadioVisuals : DefaultShipPartVisuals
    {
        private void PlayMusic()
        {
            PlaySound(RadioMod.MusicParams.First());
        }
        private void StopMusic()
        {
            StopSound(RadioMod.MusicParams.First());
        }


        private void PlayFlickSound()
        {
            PlaySound(RadioMod.FlickSoundParams);
        }


        private void PlaySound(SoundParams sound)
        {
            var source = GetComponentInChildren<SoundSource>();

            source.Play(sound);
        }

        private void StopSound(SoundParams sound)
        {
            var source = GetComponentInChildren<SoundSource>();

            source.Stop(sound);
        }
    }


    public partial class RadioVisuals : DefaultShipPartVisuals
    {
        private float _intensity = 5;
        private Vector3 _minPosition = new(-0.0015f, 0.00125f, 0);
        private Vector3 _maxPosition = new(-0.0015f, 0.001f,   0);
        private float[] _samples = new float[256];


        private void AnimateSpeaker()
        {
            MelonLogger.Msg(1);
            var musicSource = GetComponentsInChildren<AudioSource>().FirstOrDefault(s => s.clip == RadioMod.MusicParams.First().audioClip);

            MelonLogger.Msg(2);
            if (musicSource is null) return;

            MelonLogger.Msg(3);
            musicSource.GetOutputData(_samples, 0);

            MelonLogger.Msg(4);
            MelonLogger.Msg(transform.Find("Radio/Case/Panels/Front Panel/Speaker/Speaker 1") == null);

            MelonLogger.Msg(5);
            transform.Find("Radio/Case/Panels/Front Panel/Speaker/Speaker 1").localPosition = Vector3.Lerp(_minPosition, _maxPosition, _samples.Average() * _intensity);
        }
    }
}
