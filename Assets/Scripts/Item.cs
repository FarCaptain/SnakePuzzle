using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector2Int m_Coord;
    public CollectableType m_Type;
    private MapLocator m_MapLocator;

    public Vector2Int m_CoordProperty
    {
        get
        {
            return m_Coord;
        }
        set
        {
            //Vector2Int oldCoord = m_Coord;
            m_Coord = value;
            OnCoordChange();
        }
    }

    private void Start()
    {
        m_MapLocator = MapLocator.instance;

        // find its coordinate
        m_CoordProperty = m_MapLocator.GuessCoordination(transform.position);

        if(!m_MapLocator.FindItem(m_CoordProperty))
            m_MapLocator.AddItem(m_CoordProperty, this);
    }

    private void OnCoordChange()
    {
        if (m_MapLocator == null)
            m_MapLocator = MapLocator.instance;
        m_Coord.x %= m_MapLocator.m_MapSize.x;
        m_Coord.y %= m_MapLocator.m_MapSize.y;
        transform.position = m_MapLocator.GetCoordinatePosition(m_Coord.x, m_Coord.y);

        // update recorded coordinate
    }

}

public enum CollectableType
{
    EXPAND,
    SHRINK,
    ABYSS,
    WATER
}