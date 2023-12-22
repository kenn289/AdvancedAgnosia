using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using TMPro;

namespace BrainCheck {


	public class UnityReceiveMessages : MonoBehaviour {
		public static UnityReceiveMessages Instance;
		public TextMesh tMesh;
		public TextMesh tMesh1;

		public StroopTestBase stroopTestBase;
		public TextMeshProUGUI speechText;
		public Image speakIconImage;

		void Awake(){
			Instance = this;
		}

		// Use this for initialization
		void Start () {
			stroopTestBase = FindObjectOfType<StroopTestBase>();
		}

		// Update is called once per frame
		void Update () {
		 
		}
		public void CallbackMethod(string messages){
			if (messages.Equals("SpeechRecognitionFinished")) {
					tMesh1.text = messages;

				if (speechText)
					speechText.text = tMesh.text;

				if(stroopTestBase)
                {
					stroopTestBase.ValidateAnswer(StroopTestBase.presentSelectedItem, tMesh.text);
					//Debug.Log("Answer Given: " + tMesh.text);
                }

				} else {
					tMesh.text = messages;
					tMesh1.text = "";
				}

			speakIconImage.gameObject.SetActive(false);
		}


	}
}
