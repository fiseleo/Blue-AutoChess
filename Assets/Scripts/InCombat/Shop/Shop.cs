using GameEnum;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<Button> ShopButtons = new List<Button>();
    private List<Image> images = new List<Image>();
    private List<GameObject> Characters = new List<GameObject>();
    public BenchManager benchManager;
    public RoundProbabilityData roundProbabilityData;

    private List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();

    public void Start()
    {
        for (int i = 0; i < ShopButtons.Count; i++)
        {
            images.Add(ShopButtons[i].GetComponent<Image>());
            Characters.Add(null);
            buttonTexts.Add(ShopButtons[i].GetComponentInChildren<TextMeshProUGUI>());
            int indx = i;
            ShopButtons[indx].onClick.AddListener(() => SpawnCharacter(indx));
        }
    }

    public void Refresh()
    {
        GetCharacter();
    }

    public void SpawnCharacter(int index)
    {
        if (!benchManager.IsBenchFull())
        {
            bool added = benchManager.AddToBench(Characters[index]);
            if (added)
            {
                images[index].sprite = null;
                images[index].color = new Color(1, 1, 1, 0);
                buttonTexts[index].text = "";
                ShopButtons[index].interactable = false;
                UpdateShopUI();
            }
        }
        else
        {
            // �B�z�ƾԮu�w�������p�A��p��ܤ@�Ӯ��������a
        }
    }

    public void GetCharacter()
    {
        int currentRound = 5; // ���]���Ĥ���
        RoundProbability currentProbability = roundProbabilityData.roundProbabilities[currentRound];
        for (int i = 0; i < ShopButtons.Count; i++)
        {
            float rand = Random.Range(0, 100);
            int selectedCharacterId = -1;
            if (rand < currentProbability.OneCostProbability)
            {
                selectedCharacterId = GetRandomCharacterId(ResourcePool.Instance.OneCostCharacter);
            }
            else if (rand < currentProbability.OneCostProbability + currentProbability.TwoCostProbability)
            {
                selectedCharacterId = GetRandomCharacterId(ResourcePool.Instance.TwoCostCharacter);
            }
            else
            {
                selectedCharacterId = GetRandomCharacterId(ResourcePool.Instance.ThreeCostCharacter);
            }

            Character character = ResourcePool.Instance.GetCharacterByID(selectedCharacterId);
            if (character != null)
            {
                images[i].sprite = character.Sprite;
                images[i].color = new Color(1, 1, 1, 1);
                Characters[i] = character.Model;
                string traitsText = GetCharacterTraitsText(character);
                buttonTexts[i].text = $"Cost: {character.Level}\n{traitsText}";
                ShopButtons[i].interactable = true;
            }
        }

        UpdateShopUI();
    }
    public void UpdateShopUI()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            // �T�O�ӫ��s���Ϥ��M���ⳣ����
            if (ShopButtons[i].interactable && images[i].sprite != null)
            {
                CharacterCTRL characterCTRL = Characters[i].GetComponent<CharacterCTRL>();
                int characterId = ResourcePool.Instance.GetCharacterByID(characterCTRL.characterStats.CharacterId).CharacterId;

                // �p����W�M�ƾԮu���Ө���
                int owned1StarCount = CountOwnedCharactersWithSameStar(characterId, 1);  // �u�p��@�P����
                int owned2StarCount = CountOwnedCharactersWithSameStar(characterId, 2);
                // �ھھ֦��ƶq�վ㰪�G�C��
                if (owned1StarCount == 0 &&owned2StarCount == 0)
                {
                    images[i].color = new Color(1, 1, 1, 1); // ���]���q�{�C��
                }
                else if (owned1StarCount == 2)  
                {
                    images[i].color = new Color(1f, 0.84f, 0f, 1); // �����Ⱚ�G�A�Ȧb����Ӥ@�P�����
                }
                else if (owned1StarCount == 1 || owned2StarCount == 1)
                {
                    images[i].color = new Color(0.7f, 0.7f, 0.7f, 1); // �ܷt���
                }

            }
        }
    }

    // �p��ۦP����ID�B�P�Ŭ����w�P�Ū�����ƶq
    private int CountOwnedCharactersWithSameStar(int characterId, int starLevel)
    {
        int count = 0;
        foreach (var character in benchManager.characterParent.childCharacters)
        {
            CharacterCTRL ctrl = character.GetComponent<CharacterCTRL>();
            if (ctrl.characterStats.CharacterId == characterId && ctrl.star == starLevel)
            {
                count++;
            }
        }
        return count;
    }
    private int GetRandomCharacterId(List<Character> characterList)
    {
        if (characterList.Count > 0)
        {
            int randIndex = Random.Range(0, characterList.Count);
            return characterList[randIndex].CharacterId;
        }
        return -1;
    }

    private string GetCharacterTraitsText(Character character)
    {
        return string.Join("\n", character.Traits);
    }
}