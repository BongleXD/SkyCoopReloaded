﻿using Il2Cpp;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SkyCoop;

// Honorably stolen from Mod Settings
public static class UIPrefabs
{
    public static bool isInitialized;

    internal static void Initialize(Panel_OptionsMenu optionsPanel)
    {
        if (isInitialized)
            return;

        var firstSection = InterfaceManager.LoadPanel<Panel_CustomXPSetup>().m_ScrollPanelOffsetTransform.GetChild(0);

        HeaderLabelPrefab = Object.Instantiate(firstSection.Find("Header").gameObject);
        HeaderLabelPrefab.SetActive(false);

        ComboBoxPrefab = Object.Instantiate(InterfaceManager.LoadPanel<Panel_CustomXPSetup>()
            .m_AllowInteriorSpawnPopupList.gameObject);
        ComboBoxPrefab.SetActive(false);

        CustomComboBoxPrefab = MakeCustomComboBoxPrefab();
        CustomComboBoxPrefab.SetActive(false);

        DisplayPrefab = MakeDisplayPrefab();
        DisplayPrefab.SetActive(false);

        EmptyPrefab = MakeEmptyPrefab();
        EmptyPrefab.SetActive(false);

        KeyEntryPrefab = MakeKeyEntryPrefab(optionsPanel);
        KeyEntryPrefab.SetActive(false);

        TextEntryPrefab = MakeTextEntryPrefab();
        TextEntryPrefab.SetActive(false);

        Object.DestroyImmediate(optionsPanel.m_FieldOfViewSlider.m_SliderObject.GetComponent<GenericSliderSpawner>());
        SliderPrefab = Object.Instantiate(optionsPanel.m_FieldOfViewSlider.gameObject);
        SliderPrefab.SetActive(false);
        SliderPrefab.transform.Find("Label_FOV").localPosition = new Vector3(-10, 0, -1);

        // Fix slider hitbox
        var collider = SliderPrefab.GetComponentInChildren<BoxCollider>();
        collider.center = new Vector3(150, 0);
        collider.size = new Vector3(900, 30);

        isInitialized = true;
    }

    private static GameObject MakeCustomComboBoxPrefab()
    {
        var result = Object.Instantiate(ComboBoxPrefab);
        Object.DestroyImmediate(result.GetComponent<ConsoleComboBox>());
        return result;
    }

    private static GameObject MakeDisplayPrefab()
    {
        var result = Object.Instantiate(ComboBoxPrefab);

        Object.DestroyImmediate(result.GetComponent<ConsoleComboBox>());
        result.DestroyChild("Button_Decrease");
        result.DestroyChild("Button_Increase");

        return result;
    }

    private static GameObject MakeEmptyPrefab()
    {
        var result = Object.Instantiate(ComboBoxPrefab);

        Object.DestroyImmediate(result.GetComponent<ConsoleComboBox>());
        result.DestroyChild("Button_Decrease");
        result.DestroyChild("Button_Increase");
        result.DestroyChild("Label_Value");

        return result;
    }

    private static GameObject MakeKeyEntryPrefab(Panel_OptionsMenu optionsPanel)
    {
        var result = Object.Instantiate(ComboBoxPrefab);

        var rebindingTab = optionsPanel.m_RebindingTab.transform;
        var originalButton = rebindingTab.FindChild("GameObject").FindChild("LeftSide").FindChild("Button_Rebinding")
            .gameObject;
        var keybindingButton = Object.Instantiate(originalButton);

        keybindingButton.transform.position = result.transform.FindChild("Label_Value").position;
        keybindingButton.transform.parent = result.transform;
        keybindingButton.name = "Keybinding_Button";

        Object.DestroyImmediate(result.GetComponent<ConsoleComboBox>());
        result.DestroyChild("Button_Decrease");
        result.DestroyChild("Button_Increase");
        result.DestroyChild("Label_Value");

        keybindingButton.DestroyChild("Label_Name");

        return result;
    }

    private static GameObject MakeTextEntryPrefab()
    {
        var result = Object.Instantiate(ComboBoxPrefab);

        var originalTextBox = InterfaceManager.LoadPanel<Panel_Confirmation>().m_GenericMessageGroup.m_InputField
            .gameObject;
        var newTextBox = Object.Instantiate(originalTextBox);

        newTextBox.transform.position = result.transform.FindChild("Label_Value").position;
        newTextBox.transform.parent = result.transform;
        newTextBox.name = "Text_Box";

        var textInputField = newTextBox.GetComponent<TextInputField>();
        textInputField.m_MaxLength = 25;

        Object.DestroyImmediate(result.GetComponent<ConsoleComboBox>());
        result.DestroyChild("Button_Decrease");
        result.DestroyChild("Button_Increase");
        result.DestroyChild("Label_Value");

        newTextBox.DestroyChild("bg");
        newTextBox.DestroyChild("glow");

        result.AddComponent<UIButton>();

        return result;
    }

    private static GameObject GetChild(this GameObject parent, string childName)
    {
        return parent.transform.FindChild(childName).gameObject;
    }

    private static void DestroyChild(this GameObject parent, string childName)
    {
        var child = parent?.transform?.FindChild(childName)?.gameObject;
        if (child != null)
            Object.DestroyImmediate(child);
    }
#nullable disable
    internal static GameObject ComboBoxPrefab { get; private set; }
    internal static GameObject CustomComboBoxPrefab { get; private set; }
    internal static GameObject DisplayPrefab { get; private set; }
    internal static GameObject EmptyPrefab { get; private set; }
    internal static GameObject HeaderLabelPrefab { get; private set; }
    internal static GameObject KeyEntryPrefab { get; private set; }
    internal static GameObject SliderPrefab { get; private set; }
    internal static GameObject TextEntryPrefab { get; private set; }
}