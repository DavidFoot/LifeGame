using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace EventBAsedAlternative.Runtime
{
    public class EventBaseAlternative : MonoBehaviour
    {

        public UnityEvent OnNewGeneration;
        public UnityEvent OnDisplayGeneration;
        private void Awake()
        {
            gridCellCount = _gridDimensions.x * _gridDimensions.y;
            GenerateGRidCell2D();
        }
        private void Update()
        {
            OnNewGeneration.Invoke();
        }

        public void RaiseNewGeneration()
        {
            OnNewGeneration.Invoke();
        }
        private void GenerateGRidCell2D()
        {
            for (int i = 0; i < _gridDimensions.x; i++)
            {
                for (int j = 0; j < _gridDimensions.y; j++)
                {
                    var cellObject = Instantiate(_cellPrefab, new Vector2(i, j), Quaternion.identity, transform);
                    cellObject.name = $"Cell({i},{j})";
                    OnNewGeneration.AddListener(cellObject.GetComponent<CellBehaviour>().OnNewGeneration);
                    OnNewGeneration.AddListener(cellObject.GetComponent<CellBehaviour>().OnDisplayGeneration);
                    cellObject.GetComponent<CellBehaviour>().OnCompleteStateChange.AddListener(OnCompleteStateChange);
                    if (UnityEngine.Random.value < _probabilityOfLifeAtInit) cellObject.GetComponent<CellBehaviour>().SetCellState(true);
                    else cellObject.GetComponent<CellBehaviour>().SetCellState(false);
                }
            }
        }
        public void OnCompleteStateChange()
        {
            completeChangeState++;
            if (completeChangeState >= gridCellCount)
            {
                OnDisplayGeneration.Invoke();
                completeChangeState = 0;
            }
        }



        int gridCellCount;
        int completeChangeState = 0;
        #region Privates & Protected

        [SerializeField] Vector2Int _gridDimensions;
        [SerializeField] GameObject _cellPrefab;
        [Range(0f, 1f), SerializeField] float _probabilityOfLifeAtInit;

        #endregion
    }
}
