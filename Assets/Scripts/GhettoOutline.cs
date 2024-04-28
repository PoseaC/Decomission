using UnityEngine;
using System.Collections;
//the name "ghetto" comes from the fact that I couldn't get the desired effect using the shader graph so I made my own script for it
public class GhettoOutline : BeatListener
{
    public Material outlineMaterial; //a material made in unity's shader graph that just renders the back faces of the mesh
    public float thickness = .1f;

    PlayerMovementManager inputManager;
    GameObject outlineObj;

    private void Start()
    {
        //we duplicate the current object, parent it to this one and remove the components we don't need, leaving only the mesh renderer
        inputManager = FindObjectOfType<PlayerMovementManager>();
        outlineObj = Instantiate(gameObject, transform);
        try
        {
            Destroy(outlineObj.GetComponent<GhettoOutline>());
            Destroy(outlineObj.GetComponent<MovingPlatform>());
            Destroy(outlineObj.GetComponent<BoxCollider>());
            Destroy(outlineObj.GetComponent<MeshCollider>());
        }
        finally
        {
            if (!outlineObj.GetComponent<MeshRenderer>().enabled)
                outlineObj.GetComponent<MeshRenderer>().enabled = true;

            outlineObj.GetComponent<MeshRenderer>().material = outlineMaterial;
            outlineObj.transform.position = transform.position;
            outlineObj.transform.rotation = transform.rotation;
            outlineObj.transform.localScale = (1 + thickness) * Vector3.one;
        }
        animatedValueStart = 1 + thickness;
        AudioManager.instance.AddListener(this);
    }

    private void Update()
    {
        animatedValueEnd = 1.015f + (thickness * inputManager.speed * 2);
        outlineObj.transform.localScale = Vector3.one * animatedValue;
    }
}
