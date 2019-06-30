using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_nothrow : MonoBehaviour
{

    public AudioSource sound1;
    public bool soundplay = false;
    public bool experience = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (experience == false)
        {
            if (soundplay == true)
            {
                sound1.Play();
                soundplay = false;
                experience = true;

            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Robot")
        {
            soundplay = true;
        }
        
    }

}
