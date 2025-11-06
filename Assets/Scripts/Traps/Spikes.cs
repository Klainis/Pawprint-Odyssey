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

    private Vector3 lastPlayerPosition = Vector3.zero;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        safeGroundSaver = player.GetComponent<SafeGroundSaver>();
        if (player != null )
            playerController = player.GetComponent<CharacterController2D>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        playerController.ApplyObjectDamage(damage);
        StartCoroutine(StopFrame());
    }

    public void TeleportPlayer(GameObject player)
    {
        if (safeGroundSaver != null)
        {
            //CalculateLastPosition();
            player.transform.position = safeGroundSaver.SafeGroundLocation;
        }
    }

    //public void CalculateLastPosition()
    //{
    //    if(lastPlatform.lastPosition.x < spikesPostion.position.x)
    //    {
    //        lastPlayerPosition = lastPlatform.lastPosition - new Vector3(0.5f, 0, 0);
    //    }
    //    else if (lastPlatform.lastPosition.x > spikesPostion.position.x)
    //    {
    //        lastPlayerPosition = lastPlatform.lastPosition + new Vector3(0.5f, 0, 0);
    //    }

    //}

    IEnumerator StopFrame()
    {
        //yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        TeleportPlayer(player);
    }
}
