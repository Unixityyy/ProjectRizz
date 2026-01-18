using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ComputerRegistryUI : MonoBehaviour
{
    public ScriptRegistryManager registryManager;
    public TextMeshPro screenText;
    
    private ScriptRegistry _registry;
    private int _selectedIndex = 0;

    void Start() => registryManager.FetchRegistry(r => { _registry = r; UpdateScreen(); });

    public void MoveSelection(int direction)
    {
        if (_registry == null || _registry.scripts.Count == 0) return;
        _selectedIndex = (_selectedIndex + direction + _registry.scripts.Count) % _registry.scripts.Count;
        UpdateScreen();
    }

    public void InstallSelected()
    {
        if (_registry == null) return;
        string file = _registry.scripts[_selectedIndex].filename;
        registryManager.InstallScript(file);
        screenText.text = "\n\n   INSTALLING...\n   PLEASE RESTART GAME";
    }

    void UpdateScreen()
    {
        if (_registry == null) { screenText.text = "Loading Registry..."; return; }

        string display = "--- SCRIPT BROWSER ---\n\n";
        for (int i = 0; i < _registry.scripts.Count; i++)
        {
            if (i == _selectedIndex)
            {
                display += $" > <color=#000000>{_registry.scripts[i].title}</color> <\n";
            }
            else
            {
                display += $"   {_registry.scripts[i].title}\n";
            }
        }
        
        display += $"\n\nSelected: {_registry.scripts[_selectedIndex].description}\n\nPress Enter to install";
        screenText.text = display;
    }
}