using Script.GameFramework.GamePlay;
using Script.GameFramework.GamePlay.InteractiveSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.LFE.GamePlay
{
    [RequireComponent(typeof(InteractiveItem))]
    public class RedirectRestartPlace : MonoBehaviour
    {
        [SerializeField] private int _restartPointIndex = 0;
        [SerializeField] private float _deathTime = 0.0f;

        void Start()
        {
            gameObject.GetComponent<InteractiveItem>().InteractiveAction.AddListener(() =>
            {
                RestartPlayerAtRestartPoint(_restartPointIndex, _deathTime);
            });
        }

        private void RestartPlayerAtRestartPoint(int index = 0, float time = 0f)
        {
            string targetName = $"Restart{index}";
            GameObject[] restartPoints = GameObject.FindGameObjectsWithTag("Restart");
            foreach (var point in restartPoints)
            {
                if (point.name == targetName)
                {
                    MyGameMode.Instance?.RestartPlayerAt(point.transform.position, time);
                }
            }
        }
    }
}