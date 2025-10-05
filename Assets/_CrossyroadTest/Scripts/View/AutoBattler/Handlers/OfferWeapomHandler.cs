using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler.Handlers
{
    internal class OfferWeapomHandler : MonoBehaviour, IAsyncRequestHandler<OfferWeaponRequest, OfferWeaponResponse>
    {
        [SerializeField] private Button _buttonTake;
        [SerializeField] private Button _buttonThrow;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private GameObject _panel;
        private bool _isHaveResult;
        private bool _isPickUpWeapon;

        public async UniTask<OfferWeaponResponse> InvokeAsync(OfferWeaponRequest request, CancellationToken cancellationToken = default)
        {
            _panel.SetActive(true);
            _isHaveResult = false;
            _textMeshProUGUI.text = request.WeaponType switch
            {
                Weapon.Dagger => "Кинжал",
                Weapon.Club => "Дубина",
                Weapon.Spear => "Копье",
                Weapon.Sword => "Меч",
                Weapon.Axe => "Топор",
                Weapon.LegendarySword => "Легендарный меч",
            };
            _buttonTake.onClick.AddListener(() => SetResult(true));
            _buttonThrow.onClick.AddListener(() => SetResult(false));
            await UniTask.WaitWhile(() => _isHaveResult == false);
            return new OfferWeaponResponse() { IsPickUpWeapon = _isPickUpWeapon };
        }

        private void SetResult(bool result)
        {
            _isPickUpWeapon = result;
            _buttonTake.onClick.RemoveAllListeners();
            _buttonThrow.onClick.RemoveAllListeners();
            _isHaveResult = true;
            _panel.SetActive(false);
        }
    }
}
