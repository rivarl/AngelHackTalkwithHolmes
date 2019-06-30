using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//シーンマネジメントを有

public class Scene_Manage : MonoBehaviour
{
    public string this_scene_name;
    public string next_scene_name;
    public string previous_scene_name;
    public string first_scene_name;

    public string Additive_scene_name;

    public float fadetime = 5.0f;
    public bool trig = false;

    [SerializeField]
    public Color fadeColor = Color.black;
    [SerializeField]
    public float vive_fadeTime = 5.0f;

    // Use this for initialization
    void Start()
    {
        SceneManager.LoadScene(Additive_scene_name, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Delete))
        {
            Change_this(); //同じシーンを再開
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.RightArrow))
        {
            Change_next(); //次のシーンへスキップ
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftArrow))
        {
            Change_back();//前のシーンに戻る
        }
        else if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.KeypadEnter))
        {
            Change_start();//最初のシーンに戻る
        }
    }

    public void Change_this()
    {
        trig = true;
        //SceneManager.LoadScene(this_scene_name);
        //FadeManager.Instance.LoadScene(this_scene_name, fadetime); //PC上ではこっち      

    }

    public void Change_next()
    {
        trig = true;
        SceneManager.LoadScene(next_scene_name);
        //FadeManager.Instance.LoadScene(next_scene_name, fadetime); //PC上ではこっち      

    }

    public void Change_back()
    {
        trig = true;
        //SceneManager.LoadScene(previous_scene_name);
        //FadeManager.Instance.LoadScene(previous_scene_name, fadetime); //PC上ではこっち      

    }

    public void Change_start()
    {
        trig = true;
        //SceneManager.LoadScene(first_scene_name);
        //FadeManager.Instance.LoadScene(first_scene_name, fadetime); //PC上ではこっち      

    }

    private void OnTriggerEnter(Collider other)
    {
        Change_next();
    }
}
