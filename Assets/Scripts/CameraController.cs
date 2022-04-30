using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public int size = 10;
	// Update is called once per frame
	void Update () {
        Vector3 PLayerPOS = GameObject.Find("Green Car").transform.transform.position;
        GameObject.Find("Main Camera").transform.position = new Vector3(PLayerPOS.x, PLayerPOS.y, PLayerPOS.z - size);
        /*GameObject GreenCarObj = GameObject.Find ("Green Car");
		if (GreenCarObj != null) {
			Vector3 PLayerPOS = GreenCarObj.transform.transform.position;
			transform.position = new Vector3 (PLayerPOS.x, PLayerPOS.y, PLayerPOS.z - 3);*/
		}
	}
