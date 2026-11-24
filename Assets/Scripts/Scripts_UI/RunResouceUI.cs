using System;
using System.Text;
using TMPro;
using UnityEngine;

public class RunResourceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RunResourceWallet resourceWallet;

    [SerializeField] private TMP_Text resourceText;

    [Header("Display Settings")]
    [SerializeField] private bool showZeroResources = true;

    private void OnEnable()
    {
        SubscribeToWallet();
        RefreshDisplay();
    }

    private void Start()
    {
        // Refresh again after scene initialization.
        SubscribeToWallet();
        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (resourceWallet != null)
        {
            resourceWallet.OnRunResourcesChanged -= RefreshDisplay;
        }
    }

    private void SubscribeToWallet()
    {
        if (resourceWallet == null)
            return;

        resourceWallet.OnRunResourcesChanged -= RefreshDisplay;
        resourceWallet.OnRunResourcesChanged += RefreshDisplay;
    }

    public void RefreshDisplay()
    {
        if (resourceText == null || resourceWallet == null)
            return;

        StringBuilder display = new StringBuilder();

        foreach (ResourceType type in
                 (ResourceType[])Enum.GetValues(typeof(ResourceType)))
        {
            int amount = resourceWallet.GetResource(type);

            if (!showZeroResources && amount <= 0)
                continue;

            display.AppendLine(type + ": " + amount);
        }

        resourceText.text = display.ToString();
    }
}
