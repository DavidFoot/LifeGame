using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventBAsedAlternative.Runtime
{
    public class CellBehaviour : MonoBehaviour
    {
        //public EventBaseAlternative m_eventBaseAlternative;
        //void OnEnable()
        //{
        //    if (m_eventBaseAlternative != null)
        //    {
        //        Debug.Log("Register to Event");
        //        m_eventBaseAlternative.OnNewGeneration += OnNewGeneration;
        //    }
        //}

        //void OnDisable()
        //{
        //    if (m_eventBaseAlternative != null)
        //    {
        //        m_eventBaseAlternative.OnNewGeneration -= OnNewGeneration;
        //    }
        //}

        private void OnMouseUp()
        {
            SetCellState(!(GetCellState()));
        }


        public bool GetCellState()
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<SpriteRenderer>().GetPropertyBlock(propertyBlock);
            if (propertyBlock.GetColor("_Color") == Color.white) return true;
            return false;
        }
        public void SetCellState(bool newState)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            if (newState) propertyBlock.SetColor("_Color", Color.white);
            else propertyBlock.SetColor("_Color", Color.black);
            GetComponent<SpriteRenderer>().SetPropertyBlock(propertyBlock);
        }
        
        public int CountAliveNeighbour()
        {
            Vector2[] neighborOffsets = new Vector2[]
            {
            new Vector2(-1, 1),  // Haut-gauche
            new Vector2(0, 1),   // Haut
            new Vector2(1, 1),   // Haut-droit
            new Vector2(-1, 0),  // Gauche
            new Vector2(1, 0),   // Droite
            new Vector2(-1, -1), // Bas-gauche
            new Vector2(0, -1),  // Bas
            new Vector2(1, -1)   // Bas-droit
            };
            Vector2 cellPosition = transform.position;
            int cellAliveAround = 0;
            foreach (Vector2 offset in neighborOffsets)
            {
                Vector2 neighborPosition = cellPosition + offset ;
                Collider2D collider = Physics2D.OverlapPoint(neighborPosition);
                if (collider != null)
                {
                    GameObject neighbor = collider.gameObject;
                    if (collider.gameObject.GetComponent<CellBehaviour>().GetCellState()) cellAliveAround++;
                }
            }
            return cellAliveAround;
        }

        public bool NextState()
        {
            var aliveCellAround = CountAliveNeighbour();
            if (aliveCellAround == 3) return true;
            if (aliveCellAround == 2) return GetCellState();
            if (aliveCellAround <= 1 || aliveCellAround >= 4) return false;
            return GetCellState(); // ???
        }
        public void OnNewGeneration()
        {
            SetCellState(NextState());
        }

        
    }
}

