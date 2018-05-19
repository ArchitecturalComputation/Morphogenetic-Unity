using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using static Drawing;

public class Menu : MonoBehaviour
{
    private GameObject _current;
    private Type[] _sketches;

    void Start()
    {
        _sketches = Assembly.GetExecutingAssembly()
          .GetTypes()
          .Where(t => t.IsSubclassOf(typeof(Drawing)))
          .ToArray();
    }

    void OnGUI()
    {
        if (_current == null)
        {
            int m = 30;
            int index = GUI.SelectionGrid(new Rect(m, m, Width - 2 * m, Height - 2 * m), -1, _sketches.Select(s => s.Name).ToArray(), 3);

            if (index != -1)
            {
                var type = _sketches[index];
                _current = new GameObject(type.Name, type);
            }
        }
        else
        {
            if (GUI.Button(new Rect(Width - 70, Height - 40, 50, 20), "Back"))
            {
                Destroy(_current);
                _current = null;
                Defaults();
            }
        }
    }
}
