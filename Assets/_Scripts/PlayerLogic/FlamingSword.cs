﻿using UnityEngine;

public class FlamingSword : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    public ContactFilter2D filter;
    private readonly Collider2D[] hits = new Collider2D[10];
    public float lifeTime = 0.5f;
    private float nowTime;
    private Vector3 originalSize;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        var pos = GameManager.instance.player.transform.position;
        pos.y = GameManager.instance.player.transform.position.y - 0.02f;

        originalSize = GetComponent<Transform>().localScale;
        if (GameManager.instance.player.transform.localScale.x > 0)
        {
            transform.localScale = originalSize;
            pos.x += 0.1f;
        }
        else if (GameManager.instance.player.transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.y);
            pos.x -= 0.1f;
        }

        nowTime = Time.time;
        gameObject.transform.position = pos;
    }

    private void Update()
    {
        if (Time.time - nowTime > lifeTime)
            Destroy(gameObject);

        boxCollider.OverlapCollider(filter, hits);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            OnCollide(hits[i]);

            hits[i] = null;
        }

        transform.position += Vector3.right * Time.deltaTime * transform.localScale.x * 3f;
    }

    private void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            var dmg = new Damage
            {
                damageAmount = GameManager.instance.weapon.damagePoint[GameManager.instance.weapon.weaponLevel] * 2,
                origin = transform.position,
                pushForce = GameManager.instance.weapon.pushForce[GameManager.instance.weapon.weaponLevel] * 2
            };
            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}