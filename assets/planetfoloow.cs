using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class planetfoloow : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    public string nombre;
    Transform planet ;
    Image img;
    Camera cam;
    TextMeshProUGUI tmp;
    bool creado = false;
    Selectable selectable;
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (!creado)
        {
            if (nombre != "")
            {

                planet = GameObject.Find(nombre).GetComponent<Transform>();
                img = GetComponent<Image>();
                cam = GameObject.Find("Camera").GetComponent<Camera>();
                tmp = this.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = planet.name;
                selectable = GetComponent<Selectable>();
                creado = true;
            }
            return;
        }
        if (img != null && planet != null)
        {
            Vector3 pos = planet.position;
            Vector3 pos2 = cam.WorldToScreenPoint(pos);
            if (pos2.z < 0)
            {
                img.enabled = false;
                tmp.enabled = false;
            } else
            {

                img.enabled = true;
                tmp.enabled = true;
                pos2.z = 0;
            }
            img.rectTransform.position = pos2;
        }
    }
    void OnMouseOver()
    {
        img.color = new Color(0, 1, 0, 1);
    }
    void OnMouseExit()
    {
        img.color = new Color(1, 0, 0, 1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("Camera").GetComponent<CustomCamerino>().planet = planet.name;

    }
}
