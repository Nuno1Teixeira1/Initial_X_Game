using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
	public Text Score_text;

	void Start(){
		Score_text.text = PlayerPrefs.GetInt ("HighScore").ToString ();
	}

	public void Back(){
		Application.LoadLevel ("menu");
	}
}
