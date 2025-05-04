using System;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class PlayerHose : MonoBehaviour
{
    public Transform ropeStart, ropeEnd;
    public Transform ropeViz;
    public bool isVisible;
    private GameState gameState;
    public Transform target;

    void Start()
    {
        gameState = FindFirstObjectByType<GameState>();
        gameState.onHoseStateChanged.AddListener(() =>
        {
            if (gameState.isCarryingHose)
            {
                EquipHose();
            }
            else
            {
                UnequipHose();
            }
        });

        UnequipHose();
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
            ropeStart.transform.position = target.transform.position;
    }

    public void EquipHose()
    {
        isVisible = true;
        ropeViz.gameObject.SetActive(true);
        target = gameState.hoseStartBeacon.myNode.transform;
        ropeStart.transform.position = target.transform.position;
        ropeViz.GetComponent<Rope>().RecalculateRope();
    }

    public void UnequipHose()
    {
        isVisible = false;
        ropeViz.gameObject.SetActive(false);
    }
}