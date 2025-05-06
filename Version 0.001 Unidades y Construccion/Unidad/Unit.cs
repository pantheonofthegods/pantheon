using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Atributos Basicos")]
    [SerializeField] private float unitHealth;
    public float unitMaxHealth;
    [Range(0, 100)] public float shieldPercentage = 20f; // % de reducción de daño

    [Header("Sistema de Experiencia")]
    public int currentXP = 0;
    public int[] xpToNextLevel = { 100, 200, 400 }; // XP requerida para niveles

    [Header("Referencias UI")]
    public HealthTracker healthTracker;
    public ShieldDisplay shieldDisplay;

    void Start()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitsList.Add(gameObject);
        }

        unitHealth = unitMaxHealth;
        UpdateUI();
    }

    void OnDestroy()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        // Aplicar reducción de daño por escudo
        float finalDamage = damage * (1f - shieldPercentage / 100f);
        unitHealth -= finalDamage;

        UpdateUI();

        if (unitHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AddXP(int xp)
    {
        currentXP += xp;
        // Aquí puedes agregar lógica de subida de nivel si lo necesitas después
    }

    private void UpdateUI()
    {
        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        }

        if (shieldDisplay != null)
        {
            shieldDisplay.UpdateDisplay(shieldPercentage);
        }
    }
}