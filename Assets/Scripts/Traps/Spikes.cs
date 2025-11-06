using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Spikes : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private CharacterController2D playerController;
    [SerializeField] private SafeGroundSaver safeGroundSaver;
    [SerializeField] private Transform spikesPostion;

    private int damage = 1;

    private Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        safeGroundSaver = player.GetComponent<SafeGroundSaver>();
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController2D>();
            rb = player.GetComponent <Rigidbody2D>();
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerController.ApplyObjectDamage(damage);
            StartCoroutine(StopFrame());
        }
    }

    public void TeleportPlayer(GameObject player)
    {
        if (safeGroundSaver != null)
        {
            rb.linearVelocity = Vector2.zero;
            player.transform.position = safeGroundSaver.SafeGroundLocation;
        }
    }

    IEnumerator StopFrame()
    {
        //yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        TeleportPlayer(player);
    }
}
