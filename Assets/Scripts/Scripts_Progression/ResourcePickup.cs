
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public enum RewardDestination
    {
        BankedImmediately,
        CurrentRun
    }

    [Header("Reward")]
    public ResourceType resourceType = ResourceType.Gold;
    [Min(1)] public int amount = 1;
    public RewardDestination destination = RewardDestination.CurrentRun;

    [Header("Collection")]
    public string playerTag = "Player";
    public bool destroyOnCollect = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (destination == RewardDestination.BankedImmediately)
        {
            if (GameProgression.Instance == null)
                return;

            GameProgression.Instance.AddResource(resourceType, amount);
        }
        else
        {
            RunResourceWallet wallet =
                other.GetComponent<RunResourceWallet>();

            if (wallet == null)
                wallet = other.GetComponentInParent<RunResourceWallet>();

            if (wallet == null)
                return;

            wallet.AddResource(resourceType, amount);
        }

        if (destroyOnCollect)
            Destroy(gameObject);
    }
}
