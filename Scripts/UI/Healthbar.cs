using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider HealthBarSlider;
    [SerializeField] private PlayerStats PlayerStats;
    [SerializeField] private Image Fill;
    [SerializeField] private Color OrangeColor;
    [SerializeField] private EnemyObject BossObject;
    [SerializeField] private Enemies Boss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HealthBarSlider = GetComponent<Slider>();

        if (PlayerStats != null)
        {
            HealthBarSlider.value = PlayerStats.MaxHealth;
        }

        else
        {
            HealthBarSlider.value = BossObject.MaxHealth;
        }

        Fill.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerStats != null)
        {
            HealthBarSlider.value = PlayerStats.CurrentHealth;

            switch (PlayerStats.CurrentHealth)
            {
                case 1:
                    Fill.color = Color.red;
                    break;
                case 2:
                    Fill.color = OrangeColor;
                    break;
                case 3:
                    Fill.color = Color.yellow;
                    break;
                case 4:
                    Fill.color = Color.green;
                    break;
                case 5:
                    Fill.color = Color.green;
                    break;
            }
        }

        else
        {
            HealthBarSlider.value = Boss.CurrentHealth;

            switch (Boss.CurrentHealth)
            {
                case 1:
                    Fill.color = Color.red;
                    break;
                case 2:
                    Fill.color = Color.red;
                    break;
                case 3:
                    Fill.color = Color.yellow;
                    break;
                case 4:
                    Fill.color = Color.yellow; ;
                    break;
                case 5:
                    Fill.color = Color.yellow;
                    break;
                case 6:
                    Fill.color = OrangeColor;
                    break;
                case 7:
                    Fill.color = OrangeColor;
                    break;
                case 8:
                    Fill.color = OrangeColor;
                    break;
                case 9:
                    Fill.color = Color.green;
                    break;
                case 10:
                    Fill.color = Color.green;
                    break;
            }
        }
    }
}
