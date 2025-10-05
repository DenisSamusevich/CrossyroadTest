using Assets._CrossyroadTest.Scripts.Games.AutoBattler;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler
{
    internal class GameMessageView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _button;
        private IAsyncSubscriber<EndGameMessage> _endGameSubscriber;
        private IAsyncSubscriber<WinGameMessage> _winGameSubscriber;
        private IDisposable _disposable;
        private bool _isClosePanel = false;

        [Inject]
        public void Initialize(IAsyncSubscriber<EndGameMessage> endGameSubscriber,
            IAsyncSubscriber<WinGameMessage> winGameSubscriber)
        {
            _endGameSubscriber = endGameSubscriber;
            _winGameSubscriber = winGameSubscriber;
            var bag = DisposableBag.CreateBuilder();
            _endGameSubscriber.Subscribe(EndGame).AddTo(bag);
            _winGameSubscriber.Subscribe(WinGame).AddTo(bag);
            _disposable = bag.Build();
        }

        public void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private async UniTask EndGame(EndGameMessage message, CancellationToken token)
        {
            await ShowMessage("Поражение");
        }

        private async UniTask WinGame(WinGameMessage message, CancellationToken token)
        {
            await ShowMessage("Победа");
        }

        private async UniTask ShowMessage(string message)
        {
            _panel.SetActive(true);
            _isClosePanel = false;
            _text.text = message;
            _button.onClick.AddListener(() => { _panel.SetActive(false); _isClosePanel = true; _button.onClick.RemoveAllListeners(); });
            await UniTask.WaitWhile(() => _isClosePanel == false);
        }
    }
}