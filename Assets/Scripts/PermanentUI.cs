using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PermanentUI : MonoBehaviour
{
    private const int HEALTH_POINTS = 100;

    [SerializeField] private Text healthText;
    private int healthPoints = HEALTH_POINTS;
}
