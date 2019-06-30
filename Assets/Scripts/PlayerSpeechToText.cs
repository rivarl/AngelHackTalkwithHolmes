using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.DataTypes;

public class PlayerSpeechToText : MonoBehaviour
{

    public InputField inputFieldObject;
    

    #region Watson SpeechToTextの設定
    [Space(10)]
    [Header("Watson SpeechToText Config")]
    [Tooltip("SpeechToText service URL")]
    [SerializeField]
    private string sttServiceUrl = "";
    [Tooltip("The authentication api key.")]
    [SerializeField]
    private string sttApiKey = "";
    #endregion

    

    #region STT用変数
    // SpeechToText
    private SpeechToText sttService;

    private int recordingRoutine = 0;
    private string microphoneID = null;
    private AudioClip recording = null;
    private int recordingBufferSize = 1;
    private int recordingHZ = 22050;

    private string recognizeText = null;
    #endregion

    public GameObject testCube;

    void Start()
    {

        // デフォルトのマイクを取得
        microphoneID = Microphone.devices[0];

        // SpeechToText初期化
        StartCoroutine(SetSttToken());

        //testCube.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        // Sを押したらレコード開始
        
        if (OVRInput.Get(OVRInput.Button.One))
        {
            testCube.SetActive(false);
            Active = true;
            Debug.Log("Sが押されました");
            StartRecording();
            
        }
        if (recognizeText != null)
        {
            var text = recognizeText;
            recognizeText = null;
            // StartCoroutine(Chat(text));
        }
    }

    void OnGUI()
    {
        
        GUI.Box(new Rect(Screen.width - 110, 10, 100, 60), "雑談");
        if (GUI.Button(new Rect(Screen.width - 100, 40, 80, 20), "音声"))
        {
            Active = true;
            StartRecording();
        }

    }

    private IEnumerator SetSttToken(){
        TokenOptions iamTokenOptions = new TokenOptions()
        {
            IamApiKey = sttApiKey
        };

        //  Create credentials using the IAM token options
        var credentials = new Credentials(iamTokenOptions, sttServiceUrl);
        while (!credentials.HasIamTokenData())
            yield return null;

        sttService = new SpeechToText(credentials)
        {
            StreamMultipart = true
        };
        Debug.Log("STT Service set.");
    }

    #region SpeechToText
    public bool Active
    {
        get { return sttService.IsListening; }
        set
        {
            if (value && !sttService.IsListening)
            {
                sttService.RecognizeModel = "ja-JP_BroadbandModel";
                sttService.DetectSilence = true;
                sttService.EnableWordConfidence = true;
                sttService.EnableTimestamps = true;
                sttService.SilenceThreshold = 0.01f;
                sttService.MaxAlternatives = 0;
                sttService.EnableInterimResults = true;
                sttService.OnError = SttOnError;
                sttService.InactivityTimeout = -1;
                sttService.ProfanityFilter = false;
                sttService.SmartFormatting = true;
                sttService.SpeakerLabels = false;
                sttService.WordAlternativesThreshold = null;
                sttService.StartListening(OnRecognize);
            }
            else if (!value && sttService.IsListening)
            {
                sttService.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (recordingRoutine != 0)
        {
            Microphone.End(microphoneID);
            Runnable.Stop(recordingRoutine);
            recordingRoutine = 0;
        }
    }

    private void SttOnError(string error)
    {
        Active = false;
        Debug.LogFormat("SpeechToText Recording Error. {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Debug.LogFormat("Start recording. devices: {0}", microphoneID);
        recording = Microphone.Start(microphoneID, true, recordingBufferSize, recordingHZ);
        yield return null;

        if (recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = recording.samples / 2;
        float[] samples = null;

        while (recordingRoutine != 0 && recording != null)
        {
            int writePos = Microphone.GetPosition(microphoneID);
            if (writePos > recording.samples || !Microphone.IsRecording(microphoneID))
            {
                Debug.LogErrorFormat("Recording Error. Microphone disconnected.");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
              || (!bFirstBlock && writePos < midPoint))
            {
                samples = new float[midPoint];
                recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, recording.channels, recordingHZ, false);
                record.Clip.SetData(samples, 0);

                sttService.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                int remaining = bFirstBlock ? (midPoint - writePos) : (recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }

    private void OnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    var reply = alt.transcript;
                    if (res.final)
                    {
                        Debug.Log("[音声]" + reply);
                        inputFieldObject.text = reply;
                        Active = false;
                        StopRecording();
                        recognizeText = reply;
                        return;
                    }
                }
            }
        }
    }

    #endregion

}