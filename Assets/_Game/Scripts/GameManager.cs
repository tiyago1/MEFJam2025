using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private SpawnerController spawner;
        [Inject] private SignalBus signalBus;
        
        private void Start()
        {
            signalBus.Subscribe<GameSignals.OnPlayButtonPressed>(StartGame);
        }

        private void StartGame()
        {
            spawner.Initialize();
        }
    }
}