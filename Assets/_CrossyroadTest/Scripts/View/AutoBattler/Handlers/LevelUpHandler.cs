using Assets._CrossyroadTest.Scripts.Games.AutoBattler.Entities.Player.Model;
using Assets._CrossyroadTest.Scripts.View.AutoBattler.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._CrossyroadTest.Scripts.View.AutoBattler.Handlers
{
    internal class LevelUpHandler : MonoBehaviour, IAsyncRequestHandler<LevelUpRequest, LevelUpResponse>
    {
        [SerializeField] private List<TextMeshProUGUI> _textClass = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> _textDescription = new List<TextMeshProUGUI>();
        [SerializeField] private List<Button> _buttons = new List<Button>();
        [SerializeField] private GameObject _panel;
        private CharacterClass _selectedClass;

        public async UniTask<LevelUpResponse> InvokeAsync(LevelUpRequest request, CancellationToken cancellationToken = default)
        {
            _selectedClass = CharacterClass.None;
            _panel.SetActive(true);
            for (int i = 0; i < _textClass.Count; i++)
            {
                _textClass[i].text = request.Variants[i].Class switch
                {
                    CharacterClass.Barbarian => "Варвар",
                    CharacterClass.Warrior => "Воин",
                    CharacterClass.Reaver => "Разбойник",
                };
                _textDescription[i].text = request.Variants[i].Bonus switch
                {
                    LevelBonus.HidenAttack => "Скрытая атака:\r\n+1 к урону если \r\nловкость персонажа \r\nвыше ловкости цели",
                    LevelBonus.ImpulsesToAction => "Порыв к действию:\r\nВ первый ход наносит \r\nдвойной урон \r\nоружием",
                    LevelBonus.PoisonAttack => "Яд:\r\nНаносит \r\nдополнительные +1 \r\nурона на втором \r\nходу, +2 на третьем \r\nи так далее.",
                    LevelBonus.Rage => "Ярость:\r\n+2 к урону в первые 3 \r\nхода, потом -1 к урону",
                    LevelBonus.Shield => "Щит:\r\n-3 к получаемому \r\nурону если сила \r\nперсонажа выше \r\nсилы атакующего",
                    LevelBonus.StoneSkin => "Каменная кожа:\r\nПолучаемый урон \r\nснижается на \r\nзначение \r\nвыносливости",
                    LevelBonus.Strength => "Сила +1",
                    LevelBonus.Dexterity => "Ловкость +1",
                    LevelBonus.Endurance => "Выносливость +1",
                };
                var index = i;
                _buttons[i].onClick.AddListener(() => OnSelectClass(request.Variants[index].Class));
            }
            await UniTask.WaitWhile(() => _selectedClass == CharacterClass.None);
            return new LevelUpResponse() { SelectedClass = _selectedClass };
        }

        private void OnSelectClass(CharacterClass characterClass)
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
            }
            _selectedClass = characterClass;
            _panel.SetActive(false);
        }
    }
}
