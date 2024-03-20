using UnityEngine;

public enum Team
{
    Blue,
    Yellow,
    White
}

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects/TeamData", order = 1)]
public class TeamData : ScriptableObject
{
    public Team team;
    public Color teamColor;
}
