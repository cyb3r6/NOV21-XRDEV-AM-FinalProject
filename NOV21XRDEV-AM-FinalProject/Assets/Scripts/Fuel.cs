using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    public string fuelType = "Default";
    public GameObject plugObject;
    public ParticleSystem liquidParticleSystem;
    public float fillAmount = 0.8f;
    public MeshRenderer meshRenderer;
    private bool plugIn = true;
    private MaterialPropertyBlock materialPropertyBlock;
    private Rigidbody plugRigidbody;
    private Rigidbody fuelRigidbody;
    private float startingFillAmount;
    
    void Start()
    {
        liquidParticleSystem.Stop();

        materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);


        plugRigidbody = plugObject.GetComponent<Rigidbody>();
        fuelRigidbody = GetComponent<Rigidbody>();

        startingFillAmount = fillAmount;
    }

    
    void Update()
    {
        if(Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0 && plugIn == false)
        {
            if (liquidParticleSystem.isStopped)
            {
                liquidParticleSystem.Play();
            }

            fillAmount -= 0.1f * Time.deltaTime;
            float fillRatio = fillAmount / startingFillAmount;

            // detecting pour collisions
            RaycastHit hit;
            if (Physics.Raycast(liquidParticleSystem.transform.position, Vector3.down, out hit, 50.0f, ~0, QueryTriggerInteraction.Collide))
            {
                FuelReceiver receiver = hit.collider.GetComponent<FuelReceiver>();
                if (receiver)
                {
                    receiver.RecieveFuel(fuelType);
                }
            }

        }
        else
        {
            liquidParticleSystem.Stop();
        }

        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void PlugOff()
    {
        if (plugIn)
        {
            plugIn = false;
            plugObject.transform.SetParent(null);
            plugRigidbody.isKinematic = false;
            plugRigidbody.AddRelativeForce(new Vector3(0, 0, 120));
            plugObject.transform.parent = null;

        }
    }

    public void SetPlug()
    {
        plugObject.transform.SetParent(null);
        plugRigidbody.isKinematic = false;
        plugRigidbody.AddRelativeForce(new Vector3(0, 0, 120));
    }
}
