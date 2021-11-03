using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    [SerializeField] int pointsForCoinPickup = 10;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Player"))) {
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
            Destroy(gameObject);
        }
    }
}
