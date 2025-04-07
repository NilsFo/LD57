using System.Collections.Generic;
using UnityEngine;

public class MusicScheduler : MonoBehaviour
{

    private MusicManager musicManager;
    private GameState gameState;

    public List<int> stingerIndicesUnlocked;
    public List<int> stingerQueue;

    public float stingerTimer = 30;
    public float stingerTimerCurrent = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        musicManager = FindFirstObjectByType<MusicManager>();

        stingerIndicesUnlocked.Clear();
        UnlockSong(1);
        UnlockSong(2);
        UnlockSong(3);
        UnlockSong(4);
        UnlockSong(5);

        stingerTimerCurrent = stingerTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicManager.IsPlayingMusic())
        {
            stingerTimerCurrent = stingerTimer;
        }

        if (gameState.gameState == GameState.GAME_STATE.MAIN_MENU || gameState.gameState == GameState.GAME_STATE.PAUSED)
        {
            musicManager.SkipFade();
        }

        if (gameState.gameState == GameState.GAME_STATE.MAIN_MENU)
        {
            musicManager.Play(0);
            return;
        }

        if (gameState.gameState == GameState.GAME_STATE.ERROR)
        {
            musicManager.Stop();
            musicManager.SkipFade();
            return;
        }

        if (gameState.gameState == GameState.GAME_STATE.PLAYING)
        {
            stingerTimerCurrent -= Time.deltaTime;

            if (stingerQueue.Count > 0)
            {
                stingerTimerCurrent -= Time.deltaTime * 5;
            }

            if (stingerTimerCurrent < 0)
            {
                stingerTimerCurrent = stingerTimer;
                PlayNextStinger();
            }
        }
    }

    public void PlayNextStinger()
    {
        print("Scheduler: Playing next stinger!");

        if (stingerIndicesUnlocked.Count == 0 && stingerQueue.Count == 0)
        {
            print("No stingers unlocked. :(");
            return;
        }

        int choice = Random.Range(0, stingerIndicesUnlocked.Count);
        int songIndex = stingerIndicesUnlocked[choice];

        if (stingerQueue.Count > 0)
        {
            songIndex = stingerQueue[0];
            stingerQueue.RemoveAt(0);
        }

        musicManager.Play(songIndex);
    }

    public void AddToStringerQueue(int index)
    {
        if (!stingerQueue.Contains(index))
        {
            stingerQueue.Add(index);
        }
    }

    public void UnlockSong(int index)
    {
        if (!stingerQueue.Contains(index))
        {
            stingerQueue.Add(index);
        }
        stingerIndicesUnlocked.Add(index);
    }

    public void OnMusicStopped()
    {
    }

}
