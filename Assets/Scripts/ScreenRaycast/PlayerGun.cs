using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : RaycastFromScreenCentre
{
    private DamageSource damageSource;
    private GunEffect effect;
    Camera mainCamera => Camera.main;
    public Vector3 hitPoint;
    protected override void Start()
    {
        //base. refers to the parent script
        //base.Start() will run the parent's Start() on this script
        base.Start();
        damageSource = GetComponent<DamageSource>();
        effect = GetComponentInChildren<GunEffect>();
    }
    public void Shoot()
    {
        //if we hit something, hit.collider will have a value, else, hit.collider will be null
        //RaycastHit hit = TryToHit();
        RaycastHit hit;
        //if we did hit something
        Vector3 rayDir = mainCamera.transform.forward;
        Vector3 rayOrigin = mainCamera.transform.position;
        if (Physics.Raycast(rayOrigin, rayDir, out hit, maxDistance, hitLayer))
        {
            //if the thing we hit has a damagable component...
            Damageable damageable;
            if(hit.collider.TryGetComponent<Damageable>(out damageable))
            {
                damageable.TakeDamage(damageSource.GetDamage());
            }
        }
        //Debug.LogError($"hit.collider={hit.collider}");

        effect.Play(rayOrigin, hit.point);
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(hitPoint, 0.5f);
    }
}
