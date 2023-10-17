using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Json 데이터 파싱
public class JsonReader
{
    public static void ItemReader()
    {
        ItemManager manager = ItemManager.GetInstance;

        // 파일 로드
        string filePath = Path.Combine(Application.streamingAssetsPath, "Items.json");

        if (!File.Exists(filePath)) return;

        // Json 데이터 추출
        string jsonString = File.ReadAllText(filePath);
        ItemList ItemArr = JsonUtility.FromJson<ItemList>(jsonString);

        // Sprite 로드
        foreach (ItemData data in ItemArr.data)
        {
            string texturePath = $"Sprites/{data.Texture}";
            Sprite sprite = Resources.Load<Sprite>(texturePath); // 텍스쳐 경로 string으로 리소스 로드

            // Json 데이터가 object 타입을 지원하지 않는 관계로 int[]로 받아온 후 object[]로 변환
            object[] objs = null;
            if (data.Parameters != null)
            {
                objs = new object[data.Parameters.Length];
                for (int i = 0; i < data.Parameters.Length; ++i)
                {
                    objs[i] = data.Parameters[i];
                }
            }

            Item item = new Item(data.Code, data.Name, sprite, data.Text, data.Type, data.Grade, data.MaxStack, data.Function, objs);

            manager.m_ItemList.Add(data.Code, item);
        }
    }

    // Json으로 저장된 슬롯 정보 불러오기
    public static void SlotLoad()
    {
        ItemManager manager = ItemManager.GetInstance;
        UI_Manager uimanager = UI_Manager.GetInstance;

        string filePath = Path.Combine(Application.streamingAssetsPath, "EquipmentSlots.json");
        string jsonString;
        if (File.Exists(filePath))
        {
            jsonString = File.ReadAllText(filePath);
            SlotList list = JsonUtility.FromJson<SlotList>(jsonString);

            foreach (SlotData data in list.data)
            {
                UI_Slot slot = uimanager.m_EquipmentSlots[data.Index];

                slot.m_Item = manager.m_ItemList[data.Code];
                slot.m_Stack = data.Amount;
                slot.UIUpdate();
            }
        }
        uimanager.SetEquipStats();

        filePath = Path.Combine(Application.streamingAssetsPath, "QuickSlots.json");
        if (File.Exists(filePath))
        {
            jsonString = File.ReadAllText(filePath);
            SlotList list = JsonUtility.FromJson<SlotList>(jsonString);

            foreach (SlotData data in list.data)
            {
                UI_Slot slot = uimanager.m_QuickSlots[data.Index];

                slot.m_Item = manager.m_ItemList[data.Code];
                slot.m_Stack = data.Amount;
                slot.UIUpdate();
            }
        }

        filePath = Path.Combine(Application.streamingAssetsPath, "InventorySlots.json");
        if (File.Exists(filePath))
        {
            jsonString = File.ReadAllText(filePath);
            SlotList list = JsonUtility.FromJson<SlotList>(jsonString);

            foreach (SlotData data in list.data)
            {
                UI_Slot slot = uimanager.m_InventorySlots[data.Index];

                slot.m_Item = manager.m_ItemList[data.Code];
                slot.m_Stack = data.Amount;
                slot.UIUpdate();
            }
        }
    }

    // 모든 슬롯에 저장되어 있는 아이템 정보 저장
    public static void SlotSave()
    {
        UI_Manager uimanager = UI_Manager.GetInstance;

        SlotList list = new SlotList();
        list.data = new List<SlotData>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "EquipmentSlots.json");

        for (int i = 0; i < uimanager.m_EquipmentSlots.Count; ++i)
        {
            UI_Slot slot = uimanager.m_EquipmentSlots[i];
            if (slot.m_Item == null) continue;

            Item item = slot.m_Item;
            SlotData data = new SlotData();
            data.Code = item.Code;
            data.Amount = slot.m_Stack;
            data.Index = i;

            list.data.Add(data);
        }
        File.WriteAllText(filePath, JsonUtility.ToJson(list));

        list.data.Clear();
        filePath = Path.Combine(Application.streamingAssetsPath, "QuickSlots.json");

        for (int i = 0; i < uimanager.m_QuickSlots.Count; ++i)
        {
            UI_Slot slot = uimanager.m_QuickSlots[i];
            if (slot.m_Item == null) continue;

            Item item = slot.m_Item;
            SlotData data = new SlotData();
            data.Code = item.Code;
            data.Amount = slot.m_Stack;
            data.Index = i;

            list.data.Add(data);
        }
        File.WriteAllText(filePath, JsonUtility.ToJson(list));

        list.data.Clear();
        filePath = Path.Combine(Application.streamingAssetsPath, "InventorySlots.json");

        for (int i = 0; i < uimanager.m_InventorySlots.Count; ++i)
        {
            UI_Slot slot = uimanager.m_InventorySlots[i];
            if (slot.m_Item == null) continue;

            Item item = slot.m_Item;
            SlotData data = new SlotData();
            data.Code = item.Code;
            data.Amount = slot.m_Stack;
            data.Index = i;

            list.data.Add(data);
        }
        File.WriteAllText(filePath, JsonUtility.ToJson(list));
    }

    public static PlayerData PlayerInfoLoad()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "PlayerInfo.json");

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(jsonString);

            return data;
        }

        return null;
    }

    public static void PlayerInfoSave()
    {
        Info info = Info.PlayerInfo;

        PlayerData data = new PlayerData();
        string filePath = Path.Combine(Application.streamingAssetsPath, "PlayerInfo.json");

        data.Level = info.m_Level;
        data.EXP = info.m_EXP;
        data.Attack = info.m_Attack;
        data.Defense = info.m_Defense;
        data.Currency = info.m_Currency;

        File.WriteAllText(filePath, JsonUtility.ToJson(data));
    }

    public static void SkillTreeLoad()
    {
        UI_SkillTree skills = UI_Manager.GetInstance.m_SkillTreeUI;

        string filePath = Path.Combine(Application.streamingAssetsPath, "SkillTree.json");

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            SkillDataPack data = JsonUtility.FromJson<SkillDataPack>(jsonString);

            skills.m_SP = data.data.SP;

            for (int i = 0; i < skills.m_SkillInfos.Count; ++i)
            {
                skills.m_SkillInfos[i].m_SkillLevel = data.data.Levels[i];
            }
        }
    }

    public static void SkillTreeSave()
    {
        UI_SkillTree skills = UI_Manager.GetInstance.m_SkillTreeUI;

        SkillDataPack pack = new SkillDataPack();
        SkillData data = new SkillData();
        string filePath = Path.Combine(Application.streamingAssetsPath, "SkillTree.json");

        data.SP = skills.m_SP;
        data.Levels = new List<int>();

        foreach (UI_SkillInfo info in skills.m_SkillInfos)
        {
            data.Levels.Add(info.m_SkillLevel);
        }
        pack.data = data;

        File.WriteAllText(filePath, JsonUtility.ToJson(pack));
    }
}

[System.Serializable]
public class ItemData
{
    public int Code;
    public string Name;
    public string Texture;
    public string Text;
    public int MaxStack;
    public Type Type;
    public Grade Grade;
    public string Function;
    public int[] Parameters;
}

[System.Serializable]
public class ItemList
{
    public List<ItemData> data;
}

[System.Serializable]
public class PlayerData
{
    public int Level;
    public int EXP;
    public int Currency;
    public int Attack;
    public int Defense;
}

[System.Serializable]
public class SlotData
{
    public int Code;
    public int Amount;
    public int Index;
}

[System.Serializable]
public class SlotList
{
    public List<SlotData> data;
}

[System.Serializable]
public class SkillData
{
    public int SP;
    public List<int> Levels;
}

[System.Serializable]
public class SkillDataPack
{
    public SkillData data;
}