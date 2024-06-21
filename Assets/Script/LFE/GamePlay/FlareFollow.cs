/*
 * File: FlareFollow.cs
 * Description: 令火炬组跟随玩家
 * Author: tianlan
 * Last update at 24/6/21
 */

using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class FlareFollow : MonoBehaviour
    {
        private GameObject _player;

        // Start is called before the first frame update
        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
        }
        
        private void FixedUpdate()
        {
            if (_player.activeSelf)
            {
                transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, 0);
            }
        }
    }
}