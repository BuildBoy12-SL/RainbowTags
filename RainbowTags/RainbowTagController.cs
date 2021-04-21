using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace RainbowTags.Components
{
    public sealed class RainbowTagController : MonoBehaviour
    {
        private Player _player;
        private int _position;
        private int _intervalInFrames;
        private string[] _colors;
        private CoroutineHandle _coroutineHandle;

        private void Awake()
        {
            _player = Player.Get(gameObject);
            // TODO: make configurable from the component
            _intervalInFrames = Mathf.CeilToInt(RainbowTagMod.Instance.Config.TagInterval) * 50;
            _coroutineHandle = Timing.RunCoroutine(UpdateColor().CancelWith(_player.GameObject));
        }

        private void OnDestroy()
        {
            Timing.KillCoroutines(_coroutineHandle);
        }

        public void SetColors(string[] colors)
        {
            _colors = colors ?? Array.Empty<string>();
            _position = 0;
        }

        private string RollNext()
        {
            if (++_position >= _colors.Length)
                _position = 0;

            return _colors.Length != 0 ? _colors[_position] : string.Empty;
        }
        
        private IEnumerator<float> UpdateColor()
        {
            while (true)
            {
                for (var z = 0; z < _intervalInFrames; z++)
                    yield return 0f;

                var nextColor = RollNext();
                if (string.IsNullOrEmpty(nextColor))
                {
                    Destroy(this);
                    break;
                }

                _player.RankColor = nextColor;
            }
        }
    }
}