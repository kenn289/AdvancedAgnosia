using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrainCheck
{

	public class MicrophoneHandler : MonoBehaviour
	{
		public SpeechrecognitionOption myOption;
		public string textToConvet;
		string gameObjectName = "UnityReceiveMessage";
		string statusMethodName = "CallbackMethod";

        private void Start()
        {
			RequestMicPermission();
			BrainCheck.SpeechRecognitionBridge.setUnityGameObjectNameAndMethodName(gameObjectName, statusMethodName);
		}

        void OnMouseUp()
		{
			StartCoroutine(BtnAnimation());
		}

		private IEnumerator BtnAnimation()
		{
			Vector3 originalScale = gameObject.transform.localScale;
			gameObject.transform.localScale = 0.9f * gameObject.transform.localScale;
			yield return new WaitForSeconds(0.2f);
			gameObject.transform.localScale = originalScale;
			ButtonAction();
		}


		public void RequestMicPermission()
        {
			BrainCheck.SpeechRecognitionBridge.checkMicPermission();
			BrainCheck.SpeechRecognitionBridge.requestMicPermission();
		}


		public void SpeechToTextInHiddenModeWithSound()
        {
			BrainCheck.SpeechRecognitionBridge.speechToTextInHidenModeWithBeepSound();
			FindObjectOfType<UnityReceiveMessages>().speakIconImage.gameObject.SetActive(true);
		}

		private void ButtonAction()
		{
			BrainCheck.SpeechRecognitionBridge.setUnityGameObjectNameAndMethodName(gameObjectName, statusMethodName);
			switch (myOption)
			{
				case SpeechrecognitionOption.requestMicPermission:
					BrainCheck.SpeechRecognitionBridge.requestMicPermission();
					break;
				case SpeechrecognitionOption.checkMicPermission:
					BrainCheck.SpeechRecognitionBridge.checkMicPermission();
					break;
				case SpeechrecognitionOption.setUpPlugin:
					BrainCheck.SpeechRecognitionBridge.SetupPlugin();
					break;
				case SpeechrecognitionOption.textToSpeech:
					BrainCheck.SpeechRecognitionBridge.textToSpeech(textToConvet, 0);  // 0 is for default locale.
					break;
				case SpeechrecognitionOption.speechToText:
					BrainCheck.SpeechRecognitionBridge.speechToText();
					break;
				case SpeechrecognitionOption.speechToTextSilentMode:
					BrainCheck.SpeechRecognitionBridge.speechToTextInSilentMode();
					break;
				case SpeechrecognitionOption.unmuteSpeakers:
					BrainCheck.SpeechRecognitionBridge.unmuteSpeakers();
					break;
				case SpeechrecognitionOption.speechToTextInHidenModeWithBeepSound:
					BrainCheck.SpeechRecognitionBridge.speechToTextInHidenModeWithBeepSound();
					break;
			}
		}
	}
}