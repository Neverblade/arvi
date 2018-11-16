using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;

public abstract class Mode : MonoBehaviour {

    public static string HIGHLIGHT_COLOR_CODE = "#F3F3A7";
    public abstract void SetupMode();

    public abstract void CleanupMode();

    public void OutputCurrentElement() {
        MM.OutputText(MM.instance.elements[MM.instance.index].name);
    }

    public void OnDoubleTapDefault() {
        MM.instance.elements[MM.instance.index].function();
    }
    
    public void OnSwipeDefault(SwipeData swipeData) {
        SwipeDirection dir = swipeData.Direction;
        if (dir == SwipeDirection.Left || dir == SwipeDirection.Right) {
            SwitchElements(dir);
            OutputCurrentElement();
        }
    }

    public void UnhighlightElement(GameObject gameObject) {
        GameObject ChildGameObject = gameObject.transform.GetChild(MM.instance.index).gameObject;
        Image image = ChildGameObject.GetComponent<Image>();
        image.color = Color.white;
    }

    public void HighlightElement(GameObject gameObject, RectTransform transform = null){
        GameObject ChildGameObject = gameObject.transform.GetChild(MM.instance.index).gameObject;
        Image image = ChildGameObject.GetComponent<Image>();
        Color highlightColor = new Color();
        ColorUtility.TryParseHtmlString(HIGHLIGHT_COLOR_CODE, out highlightColor);
        image.color = highlightColor;

        if(transform != null && transform.childCount!=0){
            GameObject listItemGameObject =transform.GetChild(0).gameObject;
            listItemGameObject.GetComponent<Image>().color = MM.instance.index == 0 ? highlightColor : Color.white;
        }
    } 

    public void SwitchElements(SwipeDirection dir) {
        //unhighlight current object
        UnhighlightElement(MM.instance.currentPanel);

        if (dir == SwipeDirection.Right) {
            MM.instance.index -= 1;
            MM.instance.index += MM.instance.elements.Count;
        } else {
            MM.instance.index += 1;
        }
        MM.instance.index = MM.instance.index % MM.instance.elements.Count;

        //switch highlight to next new focus
        HighlightElement(MM.instance.currentPanel, MM.instance.listTransform);
    }
}
