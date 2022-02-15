using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCanvas : MonoBehaviour
{
    public bool menuOpen = false;
    public RectTransform buttonHolder;
    List<Button> buttons;

    public Button waitButton;
    public Button attackButton;

    public void OpenMenu(List<Button> buttons, Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        this.buttons = buttons;
        buttonHolder.position = FindRoom(screenPosition);
        CreateButtons();
        menuOpen = true;
    }

    // Buttons are created starting from the bottom and moving up.
    private void CreateButtons()
    {
        float indent = 0;
        foreach (Button button in buttons)
        {
            Button newButton = Instantiate(button, buttonHolder.transform);
            RectTransform transform = newButton.GetComponent<RectTransform>();
            transform.localPosition = new Vector3(0, indent);
            indent += button.GetComponent<RectTransform>().sizeDelta.y;
        }
    }

    private Vector3 FindRoom(Vector3 position)
    {
        float width = 0;
        float height = 0;

        float xShift = 100;

        foreach(Button button in buttons)
        {
            Vector2 size = button.GetComponent<RectTransform>().sizeDelta;
            if (size.x > width) { width = size.x; }
            height += size.y;
        }

        float newX = position.x + width + xShift < Screen.width ? position.x + xShift : position.x - xShift;

        float spaceY = Screen.height - height;
        float newY = Mathf.Clamp(position.y - (height / 2), height / 2, spaceY - (height / 2));
        return new Vector3(newX, newY, 0);
    }

    public void CloseMenu()
    {
        buttons.Clear();
        for (int i = 0; i < buttonHolder.transform.childCount; i++)
        {
            Destroy(buttonHolder.transform.GetChild(i).gameObject);
        }
        menuOpen = false;
    }
}
