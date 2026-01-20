using UnityEngine;

public class ScriptRegistryButtons : MonoBehaviour
{
    public ComputerRegistryUI ui;
    public enum KeyType { Up, Down, Enter }
    public KeyType key;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            if (key == KeyType.Up) ui.MoveSelection(-1);
            if (key == KeyType.Down) ui.MoveSelection(1);
            if (key == KeyType.Enter) ui.InstallSelected();
            
            transform.localPosition -= new Vector3(0, 0.01f, 0);
            Invoke(nameof(ResetButton), 0.1f);
        }
    }

    void ResetButton() => transform.localPosition += new Vector3(0, 0.01f, 0);
}