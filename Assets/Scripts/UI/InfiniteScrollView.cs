using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollView : MonoBehaviour
{
    public Transform content;
    public ScrollRect scrollRect;

    private List<GameObject> items = new List<GameObject>();

    public Action<GameObject> RecycleButton;
    public Func<GameObject, GameObject> GetScrollElement;
    public Func<BuildingType, GameObject> GetBuildingButton;

    private const int MAX_ACTIVE_ITEMS = 5;
    private const float ITEM_HEIGHT = 100f;
    private int currentIndex = 0;

    void Start()
    {
        for (int i = 0; i < MAX_ACTIVE_ITEMS; i++)
        {
            AddItem(GetNextBuildingButton());
        }

        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    void AddItem(GameObject item)
    {
        items.Add(item);
        item.transform.SetParent(content, false);
        item.SetActive(true);
    }

    void OnScroll(Vector2 scrollPosition)
    {
        if (scrollRect.verticalNormalizedPosition >= 0.9f)
        {
            AddToTop();
        }
        else if (scrollRect.verticalNormalizedPosition <= 0.1f)
        {
            AddToBottom();
        }
    }

    void AddToTop()
    {
        if (items.Count > 0)
        {
            GameObject item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);

            GameObject newItem = GetScrollElement.Invoke(item);
            newItem.transform.SetParent(content, false);
            items.Insert(0, newItem);

            DeactivateExcessItems();
        }
    }

    void AddToBottom()
    {
        if (items.Count > 0)
        {
            GameObject item = items[0];
            items.RemoveAt(0);
            GameObject newItem = GetScrollElement.Invoke(item);
            newItem.transform.SetParent(content, false);
            items.Add(newItem);

            DeactivateExcessItems();
        }
    }


    void DeactivateExcessItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(i < MAX_ACTIVE_ITEMS);
        }
    }

    GameObject GetNextBuildingButton()
    {
        BuildingType type = currentIndex % 2 == 0 ? BuildingType.Barrack : BuildingType.PowerPlant;
        currentIndex++;
        return GetBuildingButton.Invoke(type);
    }
}
