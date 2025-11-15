using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Spikes : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SafeGroundSaver safeGroundSaver;
    [SerializeField] private Transform spikesPostion;

    private GameObject enemy;
    private Rigidbody2D rigidBody;
    private PlayerView playerView;

    private int damage = 1;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        safeGroundSaver = player.GetComponent<SafeGroundSaver>();
        if (player != null)
        {
            playerView = player.GetComponent<PlayerView>();
            rigidBody = player.GetComponent <Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerView.ApplyObjectDamage(damage);
            StartCoroutine(StopFrame());
        }
        else if (collider.gameObject.CompareTag("Enemy"))
        {
            enemy = collider.gameObject;
            enemy.SendMessage("ApplyDamage", 9999);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        if (safeGroundSaver != null)
        {
            rigidBody.linearVelocity = Vector2.zero;
            player.transform.position = safeGroundSaver.SafeGroundLocation;
        }
    }

    private IEnumerator StopFrame()
    {
        //yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        TeleportPlayer(player);
    }
}
